using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AlgorithmClass : MonoBehaviour
{
    public GameObject arDeviceObject; //ARCore device gameobject
    private bool camAvailable; //bool used for seeing if rendering with camera is possible
    private WebCamTexture backCam; //used to obtain video from device camera
    private Texture defaultBackground;

    public AlgorithmClass()
    {
        Console.WriteLine(ARSession.CheckAvailability());
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Should be of type PosPers instead of double! 
    public double getCurrentPosPers()
    {
      
        return 10;
    }

    // Should be of type PosPers instead of double!
    public double startPosPers()
    {
        //
        return 10;
    }


    public bool safeZoneReached()
    {

        bool isReached = false;
        return isReached;
    }

}