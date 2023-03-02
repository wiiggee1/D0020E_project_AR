using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Timer : MonoBehaviour
{
    static public float currentTime;
    [SerializeField] TextMeshProUGUI time;
    static public bool gameEnded = false;

    void Start()
    {
    }

    void Update()
    {
        if (gameEnded != true){
            currentTime -= 1 * Time.deltaTime;
            time.text = currentTime.ToString("00.00");
            if(currentTime < 40)
            {
                time.color = new Color(1f, 1f, 0f, 1f);
            }
            if (currentTime < 25)
            {
                time.color = new Color(1f, 0f, 0f, 1f);
            }
            if (currentTime < 0){
                SceneManager.LoadScene("YouDied");
            }
        }
    }
    public float getTimer()
    {
        return currentTime;
    }
    public void collisionTimer(float collision){
        currentTime -= collision;
    }
}