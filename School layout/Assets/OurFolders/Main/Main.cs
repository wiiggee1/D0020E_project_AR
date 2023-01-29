using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public Obstacles obstacles;
    public PosPers posPers;
    public Timer timer;
    public bool QRscanned;
    public bool checkForQR;
    

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    { 
    }

    //When player pressed button to try and scan QR
    void QRscan() {
        checkForQR = true;
    }

    //handler of issue #16 succesfull scan of QR
    void QRdetected() {
            QRscanned = true;
            checkForQR = false;
    }

    //handler of issue #17
    void QRfail() {
                Debug.Log("Not a valid QR code");
        }

    void initGame() {
        if (QRscanned == true) {
            Debug.Log("Methods initialize game");
        //obstacles.placeObstacles();
        //posPers.startPosPers();
        //timer.startTimer();
        }
    }
}