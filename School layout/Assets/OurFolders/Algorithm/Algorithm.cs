using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR;
using UnityEngine.InputSystem.XR;
using UnityEngine.Android;
using UnityEditor;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.XR.OpenXR.Input;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using Unity.XR.CoreUtils;
//using Accord.Math;

public class Algorithm : MonoBehaviour
{
    private ARCameraManager arCameraObject; //ARCore device gameobject
    //private PosPers posPers;
    private ARPointCloud _pointCloud; // The AR Foundation PointCloud script
    private ARSessionOrigin _arSessionOrigin;
    private ARSession _arSession;
    private ARPoseDriver _arPoseDriver;

    private UnityEngine.Vector2 deviceLocation;
    private float device_lat; //device geodata
    private float device_long; //device geodata
    public Vector3 arCameraPosition;
    public UnityEngine.Quaternion arCameraRotation;
    private float arHorizontal;
    private float arVertical;
    private UnityEngine.Matrix4x4 pointCloudMatrix;

    float[] xPos = new float[] {};
    float[] yPos = new float[] {};
    float[] zPos = new float[] {};
    float[] verticalPos = new float[] {};
    float[] horizontalPos = new float[] {};
    IDictionary<string, float[]> positionData = new Dictionary<string, float[]>();

    void Awake()
    {
        // initialize the AR Session Origin component as reference during awake state of application. 
        _arSessionOrigin = GetComponent<ARSessionOrigin>();
        _pointCloud = GetComponent<ARPointCloud>();
        _arPoseDriver = GetComponent<ARPoseDriver>();

}

// Start is called before the first frame update
void Start()
    {

        if (ARSession.state == ARSessionState.Unsupported)
        {
            Debug.Log(ARSession.CheckAvailability());
        }
        else
        {
            // Start the AR session
            _arSession.enabled = true;
        }
                  

        // Request permission to access the device's location (ANDROID)
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        else
        {
            Input.location.Start(); //get's the android device current location.
        }
        
        ARSession.stateChanged += ARSessionStateChanged;
        
    }

    // Update is called once per frame
    void Update()
    {
        mapCameraLocationData();
        var showpos = ("x: " + arCameraPosition.x + ", y: " + arCameraPosition.y + ", z: " + arCameraPosition.z);
        
    }

    private void mapCameraLocationData()
    {
        // The transform object is attached to the gameobject 

        //Vector3 camPositionCloud = _pointCloud.transform.position;
        arCameraPosition = _arSessionOrigin.camera.transform.position;
        //arCameraPosition = _arPoseDriver.gameObject.transform.position;

        //UnityEngine.Quaternion catRotation = _pointCloud.transform.rotation;
        arCameraRotation = _arSessionOrigin.camera.transform.rotation;
        //arCameraRotation = _arPoseDriver.gameObject.transform.rotation;


        // gets the euler angle for the horizontal and vertical movment in float
        Vector3 arEulerAngles = arCameraRotation.eulerAngles;
        arHorizontal = arEulerAngles.y;
        arVertical = arEulerAngles.x;

        //This will set the AR session space to world space in unity
        //_arPoseDriver.gameObject.transform.SetPositionAndRotation(arCameraPosition, arCameraRotation);
        Vector3 deviceToWorldLocation = _arSessionOrigin.camera.transform.localToWorldMatrix.GetPosition();
        //_arPoseDriver.gameObject.transform.position = deviceToWorldLocation;

        // Call the setCurrentPosPers method...
        setCurrentPosPers(arCameraPosition.x, arCameraPosition.y, arCameraPosition.z, arVertical, arHorizontal);
    }


    private Vector3[] generateAlgorithmLocation(Vector3[] currentFramePoints, UnityEngine.Matrix4x4 pointCloudMatrix)
    {
        // SLAM and pointCloud matching, for finding the "iterative closest point (ICP) & NDT algorithms
        // data structure backend for comparing features/detected landmarks (previously measured positions)
        // PointCloud for map construction, generally movment is estimated sequentially by matching the point clouds. 

        // prior frame pointcloud compares to the current frame pointcloud with its confidence????
        Vector3[] mapped_points = new Vector3[currentFramePoints.Length];
        int i = 0; 
        while(i < currentFramePoints.Length)
        {
            mapped_points[i] = pointCloudMatrix.MultiplyPoint3x4(currentFramePoints[i]);
            i++;
        }
        //Accord.Math.Matrix.ArgMin;
        return mapped_points;
    }

    private void ARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        if (args.state == ARSessionState.SessionTracking)
        {
            // Wait until AR is fully initialized
            ARSession.stateChanged -= ARSessionStateChanged;

            // Using the android device's current geolocation
            device_lat = Input.location.lastData.latitude;
            device_long = Input.location.lastData.longitude;
            deviceLocation = new UnityEngine.Vector2(device_lat, device_long);

            Input.location.Stop(); // stops the location service updates only need reference point

            // Set the ARPoseDriver's target transform to the ARReferencePoint
            var map_long_lat = UnityEngine.Quaternion.AngleAxis(device_long, -Vector3.up) * UnityEngine.Quaternion.AngleAxis(device_lat, -Vector3.right) * new Vector3(0, 0, 1);
            _arPoseDriver.transform.TransformPoint(map_long_lat);       
            _arPoseDriver.enabled = true;
        }
    }

    // Should be of type PosPers instead of double! 
    public void setCurrentPosPers(float x, float y, float z, float vertical, float horizontal)
    {
        xPos = xPos.Append(x).ToArray();
        yPos = yPos.Append(y).ToArray();
        zPos = zPos.Append(z).ToArray();

        verticalPos = verticalPos.Append(vertical).ToArray();
        horizontalPos = horizontalPos.Append(horizontal).ToArray();
    }

    // Should be of type PosPers instead of double!
    public float startPosPers()
    {
        //QRScanner position should have a method for returning its coordinates and if QR is scanned? (readQR???)
        float init_x;
        float init_y;
        float init_z;
        float init_horizontal;
        float init_vertical;
        
        //init_x = _pointCloud.transform.position.x;
        //init_y = _pointCloud.transform.position.y;
        //init_z = _pointCloud.transform.position.z;
        //init_horizontal = Input.GetAxis("Horizontal");
        //init_vertical = Input.GetAxis("Vertical");
        //transform.SetParent(_pointCloud.transform); // sets the parent location


        return 0;
    }

    public ARSession arSession
    {
        get { return _arSession; }
        set { _arSession = value; }
    }

    public void ResetButtonTrigger()
    {
        _arSession.Reset();
    }

    public bool safeZoneReached()
    {
        bool isReached = false;
        return isReached;
    }

}
