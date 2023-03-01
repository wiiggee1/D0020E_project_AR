using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Timer : MonoBehaviour
{
    float currentTime = 0f;
    [SerializeField] TextMeshProUGUI time;

    void Start()
    {
    }

    void Update()
    {
        currentTime += 1 * Time.deltaTime;
        time.text = currentTime.ToString("00.00");
        if(currentTime > 25)
        {
            time.color = new Color(1f, 1f, 0f, 1f);
        }
        if (currentTime > 40)
        {
            time.color = new Color(1f, 0f, 0f, 1f);
        }
        if (currentTime > 60){
            SceneManager.LoadScene("YouDied");
        }

    }
    public float getTimer()
    {
        return currentTime;
    }
}