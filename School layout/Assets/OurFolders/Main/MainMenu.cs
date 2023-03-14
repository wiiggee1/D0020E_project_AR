using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    int spawn;
    int course;
    int difficulty;

    public void playGame () 
    {
        Obstacle.spawn = spawn;
        Obstacle.course = course;
        if (difficulty == 1){
            Timer.currentTime = 120f;
        }
        else if (difficulty == 2){
            Timer.currentTime = 90f;
        }
        else{
            Timer.currentTime = 60f;
        }
        
        SceneManager.LoadScene("GameScene");
    }

    public void dataManagementScene()
    {
        SceneManager.LoadScene("ShowDataMenu");
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

    public void setDifficulty(int inDifficulty){
        difficulty = inDifficulty;
    }
}