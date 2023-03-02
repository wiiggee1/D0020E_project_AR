using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class UIPositionCanvas : MonoBehaviour
{
    public Text PositionText;
    public ARPointCloud _pointCloud;

    void OnEnable()
    {
        _pointCloud = GetComponent<ARPointCloud>();
        _pointCloud.updated += OnPointCloudChanged;
    }

    private void OnPointCloudChanged(ARPointCloudUpdatedEventArgs eventArgs)
    {
        var positions = _pointCloud.positions.Value;
        var positionOutput = "x: " + positions[0].x.ToString() + ", " + "y: " + positions[0].y.ToString() + ", " + "z: " + positions[0].z.ToString();
        PositionText.text = positionOutput;

    }
}
