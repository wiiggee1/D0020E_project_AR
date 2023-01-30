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

public class Algorithm : MonoBehaviour
{
    public ARCameraManager arCameraObject; //ARCore device gameobject
    private PosPers posPers;
    private float[] startPosition;
    private ARPointCloud _pointCloud; // The AR Foundation PointCloud script
    private ARSessionOrigin _arSessionOrigin;
    private ARTrackable _arTrackable;
    private ARSession _arSession;

    [Obsolete]
    public Algorithm()
    {
        Console.WriteLine(ARSession.CheckAvailability());
        //var pointCloudPos = _pointCloud.positions;
        PosPers posPers = startPosPers();     
                            
    }

    // Start is called before the first frame update
    void Start()
    {
        // Request permission to access the device's location (ANDROID)
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        // inital state and start positioning
        float start_x = posPers.getPos().x;
        float start_y = posPers.getPos().y;
        float start_z = posPers.getPos().z;
        float start_vertical = posPers.getPos().upDown;
        float start_horizontal = posPers.getPos().leftRight;

        float[] startPosition = {start_x,start_y,start_z,start_vertical,start_horizontal};
        
    }

    // Update is called once per frame
    void Update()
    {
        // The transform object is attached to the gameobject 

        //Vector3 camPosition = arCameraObject.gameObject.transform.position;
        Vector3 camPosition = _pointCloud.transform.position;

        //UnityEngine.Quaternion catRotation = arCameraObject.transform.rotation;
        UnityEngine.Quaternion catRotation = _pointCloud.transform.rotation;


        //This will set the local space from the camera position vector
        transform.localPosition = camPosition;
        transform.localRotation = catRotation;

        //This transform from local to world space
        transform.TransformPoint(camPosition);

        transform.Translate(Vector3.up * Input.GetAxis("Vertical"));
        transform.Translate(Vector3.right * Input.GetAxis("Horizontal"));
        
        // Call the setPosPers method in the PosPers class
        posPers.setPosPers(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z, Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

    }

    // Should be of type PosPers instead of double! 
    public float[] getCurrentPosPers()
    {
        var x = posPers.getPos().x;
        var y = posPers.getPos().y;
        var z = posPers.getPos().z;
        var vertical = posPers.getPos().upDown;
        var horizontal = posPers.getPos().leftRight;
        float[] currentPosition = {x, y, z, vertical, horizontal};
        return currentPosition;
    }

    // Should be of type PosPers instead of double!
    public PosPers startPosPers()
    {
        //QRScanner position should have a method for returning its coordinates and if QR is scanned? (readQR???)
        float init_x = _pointCloud.transform.position.x;
        float init_y = _pointCloud.transform.position.y;
        float init_z = _pointCloud.transform.position.z;
        float init_horizontal = Input.GetAxis("Horizontal");
        float init_vertical = Input.GetAxis("Vertical");
        
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
