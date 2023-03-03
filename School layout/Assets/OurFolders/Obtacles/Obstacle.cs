using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Obstacle : MonoBehaviour
{
    public static int spawn;
    public static int course;

    public int courseID;

    [SerializeField] GameObject spawnPoint1;
    [SerializeField] GameObject spawnPoint2;
    [SerializeField] GameObject spawnPoint3;
    [SerializeField] GameObject spawnPoint4;
    [SerializeField] GameObject spawnPoint5;

    [SerializeField] GameObject safeZone1;
    [SerializeField] GameObject safeZone2;
    [SerializeField] GameObject safeZone3;

    [SerializeField] GameObject obstacle1;
    [SerializeField] GameObject obstacle2;
    [SerializeField] GameObject obstacle3;
    [SerializeField] GameObject obstacle4;
    [SerializeField] GameObject obstacle5;
    [SerializeField] GameObject obstacle6;
    [SerializeField] GameObject obstacle7;
    [SerializeField] GameObject obstacle8;
    [SerializeField] GameObject obstacle9;
    [SerializeField] GameObject obstacle10;
    [SerializeField] GameObject obstacle11;
    [SerializeField] GameObject obstacle12;
    [SerializeField] GameObject obstacle13;
    [SerializeField] GameObject obstacle14;
    [SerializeField] GameObject obstacle15;

    [SerializeField] GameObject smoke1;
    [SerializeField] GameObject smoke2;
    [SerializeField] GameObject smoke3;
    [SerializeField] GameObject smoke4;
    [SerializeField] GameObject smoke5;
    [SerializeField] GameObject smoke6;
    [SerializeField] GameObject smoke7;
    [SerializeField] GameObject smoke8;
    [SerializeField] GameObject smoke9;
    [SerializeField] GameObject smoke10;
    [SerializeField] GameObject smoke11;
    [SerializeField] GameObject smoke12;
    [SerializeField] GameObject smoke13;
    [SerializeField] GameObject smoke14;
    [SerializeField] GameObject smoke15;
    [SerializeField] GameObject smoke16;

    [SerializeField] GameObject fire1;
    [SerializeField] GameObject fire2;
    [SerializeField] GameObject fire3;
    [SerializeField] GameObject fire4;
    [SerializeField] GameObject fire5;
    [SerializeField] GameObject fire6;
    [SerializeField] GameObject fire7;
    [SerializeField] GameObject fire8;
    [SerializeField] GameObject fire9;
    [SerializeField] GameObject fire10;
    [SerializeField] GameObject fire11;
    [SerializeField] GameObject fire12;
    [SerializeField] GameObject fire13;
    [SerializeField] GameObject fire14;
    [SerializeField] GameObject fire15;
    [SerializeField] GameObject fire16;

    [SerializeField] GameObject alarm;
    [SerializeField] GameObject timer;

    void Start(){
        fireAndSmoke13AlarmAndTimer();
        if (spawn == 1){
            spawnPoint1.SetActive(true);
            if (course == 1){
                courseID = 1;
                safeZone1.SetActive(true);
                obstacle1.SetActive(true);
                obstacle4.SetActive(true);
                obstacle6.SetActive(true);
                obstacle7.SetActive(true);
                obstacle8.SetActive(true);
                obstacle9.SetActive(true);
                obstacle10.SetActive(true);
                obstacle11.SetActive(true);
                obstacle12.SetActive(true);
                obstacle14.SetActive(true);
                fireAndSmoke2();
                fireAndSmoke6();
                fireAndSmoke7();
                fireAndSmoke9();
                fireAndSmoke11();
            }
            else if (course == 2){
                courseID = 2;
                safeZone2.SetActive(true);
                obstacle1.SetActive(true);
                obstacle2.SetActive(true);
                obstacle3.SetActive(true);
                obstacle4.SetActive(true);
                obstacle5.SetActive(true);
                obstacle7.SetActive(true);
                obstacle9.SetActive(true);
                obstacle10.SetActive(true);
                obstacle12.SetActive(true);
                fireAndSmoke1();
                fireAndSmoke2();
                fireAndSmoke6();
                fireAndSmoke7();
                fireAndSmoke8();
                fireAndSmoke10();
            }
            else if (course == 3){
                courseID = 3;
                safeZone3.SetActive(true);
                obstacle1.SetActive(true);
                obstacle5.SetActive(true);
                obstacle7.SetActive(true);
                obstacle12.SetActive(true);
                obstacle13.SetActive(true);
                obstacle15.SetActive(true);
                fireAndSmoke2();
                fireAndSmoke4();
                fireAndSmoke5();
                fireAndSmoke7();
                fireAndSmoke8();
                fireAndSmoke10();
                fireAndSmoke11();
            }
        }
        if (spawn == 2){
            spawnPoint2.SetActive(true);
            if (course == 1){
                courseID = 4;
                safeZone1.SetActive(true);
                obstacle4.SetActive(true);
                obstacle7.SetActive(true);
                obstacle8.SetActive(true);
                obstacle9.SetActive(true);
                obstacle10.SetActive(true);
                obstacle11.SetActive(true);
                obstacle12.SetActive(true);
                obstacle14.SetActive(true);
                fireAndSmoke2();
                fireAndSmoke6();
                fireAndSmoke9();
            }
            else if (course == 2){
                courseID = 5;
                safeZone2.SetActive(true);
                obstacle1.SetActive(true);
                obstacle2.SetActive(true);
                obstacle3.SetActive(true);
                obstacle4.SetActive(true);
                obstacle7.SetActive(true);
                obstacle8.SetActive(true);
                obstacle9.SetActive(true);
                obstacle10.SetActive(true);
                obstacle11.SetActive(true);
                obstacle12.SetActive(true);
                obstacle14.SetActive(true);
                fireAndSmoke2();
                fireAndSmoke6();
                fireAndSmoke9();
                fireAndSmoke10();
            }
            else if (course == 3){
                courseID = 6;
                safeZone2.SetActive(true);
                obstacle2.SetActive(true);
                obstacle3.SetActive(true);
                obstacle4.SetActive(true);
                obstacle7.SetActive(true);
                obstacle8.SetActive(true);
                obstacle9.SetActive(true);
                obstacle10.SetActive(true);
                obstacle11.SetActive(true);
                obstacle12.SetActive(true);
                obstacle13.SetActive(true);
                obstacle14.SetActive(true);
                fireAndSmoke2();
                fireAndSmoke6();
                fireAndSmoke8();
                fireAndSmoke10();
                fireAndSmoke12();
                fireAndSmoke14();
            }
        }
        if (spawn == 3){
            spawnPoint3.SetActive(true);
            if (course == 1){
                courseID = 7;
                safeZone3.SetActive(true);
                obstacle4.SetActive(true);
                obstacle7.SetActive(true);
                obstacle8.SetActive(true);
                obstacle9.SetActive(true);
                obstacle10.SetActive(true);
                obstacle11.SetActive(true);
                obstacle12.SetActive(true);
                obstacle13.SetActive(true);
                fireAndSmoke2();
                fireAndSmoke8();
            }
            else if (course == 2){
                courseID = 8;
                safeZone3.SetActive(true);
                obstacle2.SetActive(true);
                obstacle3.SetActive(true);
                obstacle4.SetActive(true);
                obstacle7.SetActive(true);
                obstacle8.SetActive(true);
                obstacle9.SetActive(true);
                obstacle10.SetActive(true);
                obstacle11.SetActive(true);
                obstacle12.SetActive(true);
                obstacle14.SetActive(true);
                fireAndSmoke1();
                fireAndSmoke2();
                fireAndSmoke7();
                fireAndSmoke8();
            }
            else if (course == 3){
                courseID = 9;
                safeZone2.SetActive(true);
                obstacle2.SetActive(true);
                obstacle3.SetActive(true);
                obstacle4.SetActive(true);
                obstacle7.SetActive(true);
                obstacle8.SetActive(true);
                obstacle9.SetActive(true);
                obstacle10.SetActive(true);
                obstacle11.SetActive(true);
                obstacle12.SetActive(true);
                fireAndSmoke2();
                fireAndSmoke4();
                fireAndSmoke6();
                fireAndSmoke7();
                fireAndSmoke8();
                fireAndSmoke16();
            }
        }
        if (spawn == 4){
            spawnPoint4.SetActive(true);
            if (course == 1){
                courseID = 10;
                safeZone3.SetActive(true);
                obstacle2.SetActive(true);
                obstacle3.SetActive(true);
                obstacle4.SetActive(true);
                obstacle7.SetActive(true);
                obstacle8.SetActive(true);
                obstacle9.SetActive(true);
                obstacle10.SetActive(true);
                obstacle11.SetActive(true);
                obstacle12.SetActive(true);
                obstacle14.SetActive(true);
                fireAndSmoke2();
                fireAndSmoke8();
            }
            else if (course == 2){
                courseID = 11;
                safeZone3.SetActive(true);
                obstacle2.SetActive(true);
                obstacle3.SetActive(true);
                obstacle4.SetActive(true);
                obstacle7.SetActive(true);
                obstacle8.SetActive(true);
                obstacle9.SetActive(true);
                obstacle10.SetActive(true);
                obstacle11.SetActive(true);
                obstacle12.SetActive(true);
                obstacle13.SetActive(true);
                obstacle14.SetActive(true);
                fireAndSmoke1();
                fireAndSmoke2();
                fireAndSmoke6();
                fireAndSmoke7();
                fireAndSmoke8();
                fireAndSmoke10();
            }
            else if (course == 3){
                courseID = 12;
                safeZone3.SetActive(true);
                obstacle2.SetActive(true);
                obstacle3.SetActive(true);
                obstacle4.SetActive(true);
                obstacle7.SetActive(true);
                obstacle8.SetActive(true);
                obstacle9.SetActive(true);
                obstacle10.SetActive(true);
                obstacle11.SetActive(true);
                obstacle12.SetActive(true);
                obstacle13.SetActive(true);
                obstacle14.SetActive(true);
                fireAndSmoke2();
                fireAndSmoke3();
                fireAndSmoke7();
                fireAndSmoke8();
                fireAndSmoke12();
            }
        }
        if (spawn == 5){
            spawnPoint5.SetActive(true);
            if (course == 1){
                courseID = 13;
                safeZone3.SetActive(true);
                obstacle2.SetActive(true);
                obstacle3.SetActive(true);
                obstacle4.SetActive(true);
                obstacle7.SetActive(true);
                obstacle8.SetActive(true);
                obstacle9.SetActive(true);
                obstacle10.SetActive(true);
                obstacle11.SetActive(true);
                obstacle12.SetActive(true);
                obstacle14.SetActive(true);
                fireAndSmoke2();
                fireAndSmoke8();
                fireAndSmoke10();
                fireAndSmoke11();
                fireAndSmoke12();
                fireAndSmoke15();
            }
            else if (course == 2){
                courseID = 14;
                safeZone2.SetActive(true);
                obstacle2.SetActive(true);
                obstacle3.SetActive(true);
                obstacle4.SetActive(true);
                obstacle7.SetActive(true);
                obstacle8.SetActive(true);
                obstacle9.SetActive(true);
                obstacle10.SetActive(true);
                obstacle11.SetActive(true);
                obstacle12.SetActive(true);
                fireAndSmoke2();
                fireAndSmoke6();
                fireAndSmoke7();
                fireAndSmoke8();
                fireAndSmoke14();
            }
            else if (course == 3){
                courseID = 15;
                safeZone1.SetActive(true);
                obstacle2.SetActive(true);
                obstacle3.SetActive(true);
                obstacle4.SetActive(true);
                obstacle7.SetActive(true);
                obstacle8.SetActive(true);
                obstacle9.SetActive(true);
                obstacle10.SetActive(true);
                obstacle11.SetActive(true);
                obstacle13.SetActive(true);
                obstacle14.SetActive(true);
                fireAndSmoke2();
                fireAndSmoke6();
                fireAndSmoke7();
                fireAndSmoke9();
                fireAndSmoke11();
                fireAndSmoke15();
            }
        }
    }

    private void fireAndSmoke13AlarmAndTimer(){
        fire13.SetActive(true);
        smoke13.SetActive(true);
        alarm.SetActive(true);
        timer.SetActive(true);
    }

    private void fireAndSmoke1(){
        fire1.SetActive(true);
        smoke1.SetActive(true);
    }

    private void fireAndSmoke2(){
        fire2.SetActive(true);
        smoke2.SetActive(true);
    }

    private void fireAndSmoke3(){
        fire3.SetActive(true);
        smoke3.SetActive(true);
    }

    private void fireAndSmoke4(){
        fire4.SetActive(true);
        smoke4.SetActive(true);
    }

    private void fireAndSmoke5(){
        fire5.SetActive(true);
        smoke5.SetActive(true);
    }

    private void fireAndSmoke6(){
        fire6.SetActive(true);
        smoke6.SetActive(true);
    }

    private void fireAndSmoke7(){
        fire7.SetActive(true);
        smoke7.SetActive(true);
    }

    private void fireAndSmoke8(){
        fire8.SetActive(true);
        smoke8.SetActive(true);
    }

    private void fireAndSmoke9(){
        fire9.SetActive(true);
        smoke9.SetActive(true);
    }

    private void fireAndSmoke10(){
        fire10.SetActive(true);
        smoke10.SetActive(true);
    }

    private void fireAndSmoke11(){
        fire11.SetActive(true);
        smoke11.SetActive(true);
    }

    private void fireAndSmoke12(){
        fire12.SetActive(true);
        smoke12.SetActive(true);
    }

    private void fireAndSmoke14(){
        fire14.SetActive(true);
        smoke14.SetActive(true);
    }

    private void fireAndSmoke15(){
        fire15.SetActive(true);
        smoke15.SetActive(true);
    }

    private void fireAndSmoke16(){
        fire16.SetActive(true);
        smoke16.SetActive(true);
    }
}
