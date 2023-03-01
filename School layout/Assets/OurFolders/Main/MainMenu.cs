using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    int spawn;
    int course;

    public void playGame () 
    {
        Obstacle.spawn = spawn;
        Obstacle.course = course;
        SceneManager.LoadScene("GameScene");
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void setSpawn(int inSpawn)
    {
        spawn = inSpawn;
    }

    public void setCourse(int inCourse)
    {
        course = inCourse;
    }
}