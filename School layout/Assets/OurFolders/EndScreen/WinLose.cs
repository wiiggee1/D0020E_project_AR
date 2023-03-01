using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLose : MonoBehaviour
{
    public bool safeZoneReached = false;

    public void winLevel() {
        if (!safeZoneReached){
            safeZoneReached = true;
            SceneManager.LoadScene("Victory");
        }
    }

    public void loseLevel() {
        if (!safeZoneReached){
            safeZoneReached = true;
            SceneManager.LoadScene("YouDied");
        }
    }
}
