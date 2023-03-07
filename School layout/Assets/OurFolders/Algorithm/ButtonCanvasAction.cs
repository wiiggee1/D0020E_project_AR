using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public class ButtonCanvasAction : MonoBehaviour
{
    public Text positionTextFull;
    public Algorithm algorithm;

    public void OnItemSelect()
    {
        var position_data = algorithm.GetPositionData();
        positionTextFull.text = "Position data: " + position_data["x_position"].ToString()+", "+ position_data["y_position"].ToString() + ", "+ position_data["z_position"].ToString();
        Debug.Log(positionTextFull.text);
    }  
}
