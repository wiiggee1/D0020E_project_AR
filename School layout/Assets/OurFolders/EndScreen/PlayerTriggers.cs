using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggers : MonoBehaviour
{
    public WinLose winLoseScript;
    // Update is called once per frame
    void Update()
    {
        
    public Obstacles obstacles;
    public PosPers posPers;
    public Timer timer;
    public bool QRscanned;
    public bool checkForQR;

    //When player pressed button to try and scan QR
    void QRscan() {
        checkForQR = true;
    }

    //handler of issue #16 succesfull scan of QR
    void QRdetected() {
            QRscanned = true;
            checkForQR = false;
            initGame();
    }

    //handler of issue #17
    void QRfail() {
                Debug.Log("Not a valid QR code");
        }

    void initGame() {
        if (QRscanned == true) {
            Debug.Log("Methods initialize game");
        //SceneManager.LoadScene("GameScene");
        //obstacles.placeObstacles();
        //timer.startTimer();
        }
    }
}