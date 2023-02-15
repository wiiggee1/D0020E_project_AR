using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Scan () {
        //Debug.Log("Pressed Scan QR");
        SceneManager.LoadScene("ScanScene");
    }

        public void PlayGame () {
        //Debug.Log("Pressed Scan QR");
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame () {
        Debug.Log("Pressed Quit");
        Application.Quit();

    }
}
