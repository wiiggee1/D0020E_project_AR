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

public class Algorithm : MonoBehaviour
{
    public ARCameraManager arCameraObject; //ARCore device gameobject
    private PosPers posPers;
    private float[] startPosition;
    private ARPointCloud _pointCloud; // The AR Foundation PointCloud script
    private ARSessionOrigin _arSessionOrigin;
    private ARTrackable _arTrackable;
    private ARSession _arSession;
    private ARAnchor _arPoseDriver;

    private float device_lat; //device geodata
    private float device_long; //device geodata

    [Obsolete]
    public Algorithm()
    {
        Console.WriteLine(ARSession.CheckAvailability());
        //var pointCloudPos = _pointCloud.positions;
        posPers = startPosPers();     
                            
    }

    // Start is called before the first frame update
    void Start()
    {
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
        generateAlgorithmLocation();

    }

    private void generateAlgorithmLocation()
    {
        // The transform object is attached to the gameobject 

        //Vector3 camPosition = arCameraObject.gameObject.transform.position;
        Vector3 camPosition = _pointCloud.transform.position;

        //UnityEngine.Quaternion catRotation = arCameraObject.transform.rotation;
        UnityEngine.Quaternion catRotation = _pointCloud.transform.rotation;


        //This will set the local space from the camera position vector
        _pointCloud.transform.SetLocalPositionAndRotation(camPosition,catRotation);
        transform.localPosition = camPosition;
        transform.localRotation = catRotation;

        //This transform from local to world space
        transform.TransformPoint(camPosition);

        // Call the setPosPers method in the PosPers class
        posPers.setPosPers(transform.position.x, transform.position.y, transform.position.z, Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
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
            Input.location.Stop(); // stops the location service updates only need reference point

            // Set the ARPoseDriver's target transform to the ARReferencePoint
            var map_long_lat = UnityEngine.Quaternion.AngleAxis(device_long, -Vector3.up) * UnityEngine.Quaternion.AngleAxis(device_lat, -Vector3.right) * new Vector3(0, 0, 1);
            _arPoseDriver.transform.TransformPoint(map_long_lat);       
            _arPoseDriver.enabled = true;
        }
    }

    // Should be of type PosPers instead of double! 
    public float[] getCurrentPosPers()
    {
        var x = this.posPers.getPos().x;
        var y = this.posPers.getPos().y;
        var z = this.posPers.getPos().z;
        var vertical = this.posPers.getPos().upDown;
        var horizontal = this.posPers.getPos().leftRight;
        float[] currentPosition = {x, y, z, vertical, horizontal};
      
        return currentPosition;
    }

    // Should be of type PosPers instead of double!
    public PosPers startPosPers()
    {
        //QRScanner position should have a method for returning its coordinates and if QR is scanned? (readQR???)
        float init_x;
        float init_y;
        float init_z;
        float init_horizontal;
        float init_vertical;
        
        try
        {
            _arSession.enabled = true; //will start the AR session
            
        }
        catch 
        {
            if(ARSession.state == ARSessionState.Unsupported)
            {
                Console.WriteLine("NOT SUPPORTED!");
            }
        }

        init_x = _pointCloud.transform.position.x;
        init_y = _pointCloud.transform.position.y;
        init_z = _pointCloud.transform.position.z;
        init_horizontal = Input.GetAxis("Horizontal");
        init_vertical = Input.GetAxis("Vertical");
        transform.SetParent(_pointCloud.transform); // sets the parent location

        // Initilize a new PosPers object that will be called in the constructor as reference
        posPers = new PosPers(init_x, init_y, init_z, init_vertical, init_horizontal); // Sets the initial 

        return posPers;

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
