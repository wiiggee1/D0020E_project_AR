using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public Obstacles obstacles;
    public PosPers posPers;
    public Timer timer;
    public boolean QRscanned;
    public boolean checkForQR;
    public int obstacleModel;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //handler of issue #16 succesfull scan of QR
    void QRscan(int obsModel) {
            QRscanned = true;
            obstacleModel = obsModel;
    }

    //handler of issue #17
    void QRfail(int obsModel) {
                Debug.Log("Not a valid QR code");
        }

    void initGame() {
        if (QRscanned == true) {
        obstacles.placeObstacles(int obstacleModel);
        posPers.startPosPers();
        timer.startTimer();
        }
    }
}