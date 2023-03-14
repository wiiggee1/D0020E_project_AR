using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLose : MonoBehaviour
{
    public bool endGame = false;

    public void winLevel() {
        if (!endGame){
            endGame = true;
            Timer.gameEnded = true;
            SceneManager.LoadScene("Victory");
        }
    }

    public void loseLevel() {
        if (!endGame){
            endGame = true;
            SceneManager.LoadScene("YouDied");
        }
    }
}
