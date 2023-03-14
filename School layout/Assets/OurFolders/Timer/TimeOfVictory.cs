using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeOfVictory : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI time;
    
    // Start is called before the first frame update
    void Update()
    {
        time.text = Timer.currentTime.ToString("00.00");
    }
}
