using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.IO;
using System;

// Inherits the parent base class of the "AlgorithmClass"
public class SaveData : Algorithm
{
    // Should be an array of type PosPers instead of double! 
    private Dictionary<string, List<float>> _posPers = new Dictionary<string, List<float>>();

    [Obsolete]
    public SaveData()
    {
        _posPers.Add("lat", new List<float>());
        _posPers.Add("long", new List<float>());
        //_posPers.Add("safeZone_lat", );
        //_posPers.Add("safeZone_long", sz_long);
    }

    public void SavePosPersAndTime()
    {
        //setter and getter for saving the location attrributes in the dictionary!
        //_posPers["lat"].append();
        //_posPers["long"].append();
    }

    public Dictionary<string, List<float>> PosPers
    {
        get { return _posPers; }
        set { _posPers = value; }
    }

    public void saveToFile()
    {
        // Should save the data of the run into a local file (csv)

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Should call the SaveCurrentRunData method continiously! 
    }
}
