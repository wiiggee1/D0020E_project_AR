using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMe : MonoBehaviour
{
    void Start(){
        DontDestroyOnLoad(gameObject);
    }
}
