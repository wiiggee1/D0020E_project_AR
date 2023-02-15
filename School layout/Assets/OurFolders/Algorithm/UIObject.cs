using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIObject : MonoBehaviour
{
    public TextMeshProUGUI PositionText;

    public void Initialize(Algorithm posData)
    {
        
        var x = posData.arCameraPosition.x;
        var y = posData.arCameraPosition.y;
        var z = posData.arCameraPosition.z;
        this.PositionText.text = "x: "+x.ToString() + ", " + "y: "+ y.ToString() + ", "+"z: "+z.ToString();
        
    }
}

