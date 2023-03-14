using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public WinLose winLoseScript;

    private void OnTriggerEnter(Collider other){
        winLoseScript.winLevel();
    }
}
