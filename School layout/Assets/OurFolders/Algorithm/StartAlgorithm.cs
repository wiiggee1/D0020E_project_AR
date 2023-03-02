using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAlgorithm : MonoBehaviour
{
    public Algorithm algorithm;

    // Start is called before the first frame update
    public void Start()
    {
        algorithm.Start();
    }

    // Update is called once per frame
    public void Update()
    {
        algorithm.Update();
    }
}
