using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class UIObject : MonoBehaviour
{
    public Text positionText;
    public ARPointCloud _pointCloud;
    //public Algorithm algoObj;
    //public RectTransform targetReactTransform;
    //public RectTransform canvasReactTransform;
    //public GameObject positionUI;

    void OnEnable()
    {
        
        _pointCloud = GetComponent<ARPointCloud>();
        _pointCloud.updated += OnPointCloudChanged;
    }

    public void OnPointCloudChanged(ARPointCloudUpdatedEventArgs eventArgs)
    {
        //Vector3 cameraPosition = algoObj.arCameraPosition;
        //Quaternion cameraRotation = algoObj.arCameraRotation;
        //#################################################

        var positions = _pointCloud.positions.Value;
        var positionOutput = "x: " + positions[0].x.ToString() + ", " + "y: " + positions[0].y.ToString() + ", " + "z: " + positions[0].z.ToString();
        //var timeStampOutput =  algoObj.timestampData.ToString();
        positionText.text = positionOutput;
        //positionText.transform.position = cameraPosition;
        //positionText.transform.rotation = cameraRotation;

        //##########################################
        //positionUI = GameObject.Find("CanvasPosition");
        //positionText = positionUI.GetComponentInChildren<Text>();
        /* 
        var positionOutput2 = "x: " + cameraPosition.x.ToString() + ", " + "y: " + cameraPosition.y.ToString() + ", " + "z: " + cameraPosition.z.ToString();
        var timeStampOutput2 = algoObj.timestampData.ToString();
        
        positionText.text = positionOutput2;
        positionUI.transform.position = cameraPosition;
        positionUI.transform.rotation = cameraRotation;
        */
    }

}

