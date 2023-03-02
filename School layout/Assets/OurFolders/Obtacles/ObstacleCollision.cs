using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ObstacleCollision : MonoBehaviour
{
    public Timer timerScript;
    [SerializeField] private Animator lostTime;
    [SerializeField] private GameObject loseTimeText;

    private void OnTriggerEnter(Collider other){
        loseTimeText.SetActive(true);
        lostTime.SetBool("walkIntoObstacle", true);
        timerScript.collisionTimer(10f);
    }

    private void OnTriggerExit(Collider other){
        lostTime.SetBool("walkIntoObstacle", false);
    }
}
