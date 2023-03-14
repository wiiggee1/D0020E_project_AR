<<<<<<< HEAD
=======
<<<<<<< HEAD
using System.Collections;
>>>>>>> origin/Eddie
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Android;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEngine.XR.ARCore;

public class Algorithm : MonoBehaviour
{
    public static Algorithm algo_instance;
    private ARCameraManager arCameraObject; //ARCore device gameobject
    private ARPointCloud _pointCloud; // The AR Foundation PointCloud script
    private ARSessionOrigin _arSessionOrigin;
    private ARSession _arSession;
    private ARPoseDriver _arPoseDriver;

    //Reference to scripts and gameobjects
    [SerializeField] GameObject timerObject;
    Timer _timer;
    [SerializeField] GameObject safeZoneReachedObject;
    WinLose _winLose;
    DataPopupCanvas _popupCanvas;
    [SerializeField] GameObject mainAR;
    static public string requestData;
    static public Dictionary<string, float[]> requestDataDictionary = new Dictionary<string, float[]>();

    // STATE VARIABLES DURING GAMEPLAY
    private float timeCycle;
    private float device_lat; //device geodata
    private float device_long; //device geodata
    public Vector2 deviceLocation = new Vector2();
    public Vector3 arCameraPosition = new Vector3();
    public Vector3 arEulerAngles= new Vector3();
    public Quaternion arCameraRotation = new Quaternion();
    private float arHorizontal;
    private float arVertical;

    // DATASTRUCTURE FOR SAVING GAMEDATA
    float[] xPos = new float[100];
    float[] yPos = new float[100];
    float[] zPos = new float[100];
    float[] verticalPos = new float[100];
    float[] horizontalPos = new float[100];
    float[] timeStampData = new float[100];
    Dictionary<string, float[]> positionData = new Dictionary<string, float[]>();


    public void Awake()
    {
        if (algo_instance == null)
        {
            algo_instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    public void Start()
    {

        // Request permission to access the device's location (ANDROID)
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        else
        {
            Input.location.Start(); //get's the android device current location.
        }

        // initialize the AR component of application. 
        _pointCloud = GetComponent<ARPointCloud>();
        _arPoseDriver = GetComponent<ARPoseDriver>();
        _arSession = GetComponent<ARSession>();
        _arSessionOrigin = GetComponent<ARSessionOrigin>();
        //arCameraObject = GetComponent<ARCameraManager>();

        //ARSession.stateChanged += ARSessionStateChanged;

        // SET INITIAL POSITION to 0
        SetPosPersAndTime(0, 0, 0, 0, 0, 0);

        // INITIALIZE OTHER GAMEOBJECTS
        _timer = timerObject.GetComponent<Timer>();
        _winLose = safeZoneReachedObject.GetComponent<WinLose>();
    }

        
    // Update is called once per frame
    public void Update()
    {
        InvokeGameObjectReferences(); // get the current state from the active gameobjects
        Invoke(nameof(FetchLocationTimeData),2.0f);
        //FetchLocationTimeData();
        IfSafeZoneReachedSqlAction();       
    }

    public void InvokeGameObjectReferences()
    {
        ARSessionOrigin activeSessionOrigin = FindObjectOfType<ARSessionOrigin>();
        ARSession activeSession = FindObjectOfType<ARSession>();

        if (activeSession.isActiveAndEnabled | activeSessionOrigin.isActiveAndEnabled)
        {
            // Do something with this AR session origin
            _arSession = activeSession.GetComponent<ARSession>();
            _arSessionOrigin = activeSession.GetComponent<ARSessionOrigin>();

            _arSessionOrigin = activeSessionOrigin.GetComponent<ARSessionOrigin>();
            _arSessionOrigin.camera = activeSessionOrigin.camera;
            //_arSessionOrigin.camera.transform.position = activeSessionOrigin.transform.position;
        }

        //SWITCH OBJECT STATES
        /*
        _arSessionOrigin.MakeContentAppearAt(activeSessionOrigin.transform, Vector3.zero);
        mainAR.SetActive(true);
        activeSession.gameObject.SetActive(false);
        
        _arSession.enabled = true;
        */

    }

    public void FetchLocationTimeData()
    {
        //yield return new WaitForSeconds(1.0f);

        timeCycle = Timer.currentTime;

        //Vector3 camPositionCloud = _pointCloud.transform.position;
        arCameraPosition = _arSessionOrigin.camera.transform.position;

        //UnityEngine.Quaternion catRotation = _pointCloud.transform.rotation;
        arCameraRotation = _arSessionOrigin.camera.transform.rotation;

        // gets the euler angle for the horizontal and vertical movment in float
        arEulerAngles = arCameraRotation.eulerAngles;
       
        var arHorizontal = arEulerAngles.y;
        var arVertical = arEulerAngles.x;

        // Call the setCurrentPosPers method...
        SetPosPersAndTime(arCameraPosition.x, arCameraPosition.y, arCameraPosition.z, arVertical, arHorizontal, timeCycle);
    }

    private void ARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        if (args.state == ARSessionState.SessionTracking)
        {
            // Wait until AR is fully initialized
            ARSession.stateChanged -= ARSessionStateChanged;
 
            // Using the android device's current geolocation
            device_lat = Input.location.lastData.latitude;
            device_long = Input.location.lastData.longitude;
            deviceLocation = new Vector2(device_lat, device_long);

            Input.location.Stop(); // stops the location service updates only need reference point
        }
    }
   
    // Should be of type PosPers instead of double! 
    public void SetPosPersAndTime(float x, float y, float z, float vertical, float horizontal, float time)
    {      
        xPos = xPos.Append(x).ToArray();
        yPos = yPos.Append(y).ToArray();
        zPos = zPos.Append(z).ToArray();
        verticalPos = verticalPos.Append(vertical).ToArray();
        horizontalPos = horizontalPos.Append(horizontal).ToArray();
        timeStampData = timeStampData.Append(time).ToArray();    
     
    }

    // Should be of type PosPers instead of double!
    public Dictionary<string, float[]> GetPositionData()
    {

        positionData.Add("position_x", xPos);
        positionData.Add("position_y", yPos);
        positionData.Add("position_z", zPos);
        positionData.Add("vertical_position", verticalPos);
        positionData.Add("horizontal_position", horizontalPos);
        positionData.Add("timestamp_frame", timeStampData);
        
        return positionData;
    }

    public void ResetPositionDataState()
    {
        positionData = positionData = new Dictionary<string, float[]>();
        xPos = new float[] { };
        yPos = new float[] { };
        zPos = new float[] { };
        verticalPos = new float[] { };
        horizontalPos = new float[] { };
        timeStampData = new float[] { };
    }

    public void IfSafeZoneReachedSqlAction()
    {
        if (_winLose.endGame == true | SceneManager.GetSceneByName("YouDied").isLoaded | SceneManager.GetSceneByName("Victory").isLoaded)
        {
            //DataPopupCanvas.SetPopupText(MapDataToJsonString());
            //_popupCanvas.SetPopupText(MapDataToJsonString());
            requestData = MapDataToJsonString();
            requestDataDictionary = GetPositionData();
            //DataPopupCanvas.SetDataToSend(MapDataToJsonString());

            //_popupCanvas.SetDataToSend(MapDataToJsonString());
            //ResetPositionDataState();
            //CancelInvoke(nameof(FetchLocationTimeData));
        }

    } 

    public string getRequestStringData()
    {
        return requestData;
    }

    public string MapDataToJsonString()
    {
        string jsonStringData = JsonConvert.SerializeObject(GetPositionData());
        return jsonStringData;
    }
        
}
=======
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR;
using UnityEngine.InputSystem.XR;
using UnityEngine.Android;
using UnityEditor;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.XR.OpenXR.Input;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARSubsystems;

public class Algorithm : MonoBehaviour
{
    private ARCameraManager arCameraObject; //ARCore device gameobject
    private PosPers posPers;
    private ARPointCloud _pointCloud; // The AR Foundation PointCloud script
    private ARSessionOrigin _arSessionOrigin;
    private ARSession _arSession;
    private ARAnchor _arPoseDriver;

    private UnityEngine.Vector2 deviceLocation;
    private float device_lat; //device geodata
    private float device_long; //device geodata
    private Vector3 arCameraPosition;
    private UnityEngine.Quaternion arCameraRotation;
    private float arHorizontal;
    private float arVertical;

    void Awake()
    {
        // initialize the AR Session Origin component as reference during awake state of application. 
        _arSessionOrigin = GetComponent<ARSessionOrigin>();
        _pointCloud = GetComponent<ARPointCloud>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (ARSession.state == ARSessionState.Unsupported)
        {
            Debug.Log(ARSession.CheckAvailability());
        }
        else
        {
            // Start the AR session
            _arSession.enabled = true;
        }
                  

        // Request permission to access the device's location (ANDROID)
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        else
        {
            Input.location.Start(); //get's the android device current location.
        }
        
        ARSession.stateChanged += ARSessionStateChanged;
        
    }

    // Update is called once per frame
    void Update()
    {
        mapCameraLocationData();
        Debug.Log("x: "+_arSessionOrigin.transform.position.x+", y: "+_arSessionOrigin.transform.position.y + ", z: " + _arSessionOrigin.transform.position.z);
    }

    private void mapCameraLocationData()
    {
        // The transform object is attached to the gameobject 

        //Vector3 camPositionCloud = _pointCloud.transform.position;
        arCameraPosition = _arSessionOrigin.camera.transform.position;

        //UnityEngine.Quaternion catRotation = _pointCloud.transform.rotation;
        arCameraRotation = _arSessionOrigin.camera.transform.rotation;

        // gets the euler angle for the horizontal and vertical movment in float
        Vector3  arEulerAngles = arCameraRotation.eulerAngles;
        arHorizontal = arEulerAngles.y;
        arVertical = arEulerAngles.x;

        //This will set the AR session space to world space in unity
        _arSessionOrigin.camera.transform.SetPositionAndRotation(arCameraPosition, arCameraRotation);
        Vector3 deviceToWorldLocation = _arSessionOrigin.camera.transform.localToWorldMatrix.GetPosition();
        _arSessionOrigin.transform.position = deviceToWorldLocation;

        // Call the setPosPers method in the PosPers class
        posPers.setPosPers(arCameraPosition.x, arCameraPosition.y, arCameraPosition.z, arVertical, arHorizontal);
    }

    private void generateAlgorithmLocation()
    {
        // SLAM and pointCloud??????
    }

    private void ARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        if (args.state == ARSessionState.SessionTracking)
        {
            // Wait until AR is fully initialized
            ARSession.stateChanged -= ARSessionStateChanged;

            // Using the android device's current geolocation
            device_lat = Input.location.lastData.latitude;
            device_long = Input.location.lastData.longitude;
            deviceLocation = new UnityEngine.Vector2(device_lat, device_long);

            Input.location.Stop(); // stops the location service updates only need reference point

            // Set the ARPoseDriver's target transform to the ARReferencePoint
            var map_long_lat = UnityEngine.Quaternion.AngleAxis(device_long, -Vector3.up) * UnityEngine.Quaternion.AngleAxis(device_lat, -Vector3.right) * new Vector3(0, 0, 1);
            _arPoseDriver.transform.TransformPoint(map_long_lat);       
            _arPoseDriver.enabled = true;
        }
    }

    // Should be of type PosPers instead of double! 
    public float[] getCurrentPosPers()
    {
        var x = this.posPers.getPos().x;
        var y = this.posPers.getPos().y;
        var z = this.posPers.getPos().z;
        var vertical = this.posPers.getPos().upDown;
        var horizontal = this.posPers.getPos().leftRight;
        float[] currentPosition = {x, y, z, vertical, horizontal};
      
        return currentPosition;
    }

    // Should be of type PosPers instead of double!
    public PosPers startPosPers()
    {
        //QRScanner position should have a method for returning its coordinates and if QR is scanned? (readQR???)
        float init_x;
        float init_y;
        float init_z;
        float init_horizontal;
        float init_vertical;
        
        try
        {
            _arSession.enabled = true; //will start the AR session        
        }
        catch 
        {
            if(ARSession.state == ARSessionState.Unsupported)
            {
                Console.WriteLine("NOT SUPPORTED!");
            }
        }

        init_x = _pointCloud.transform.position.x;
        init_y = _pointCloud.transform.position.y;
        init_z = _pointCloud.transform.position.z;
        init_horizontal = Input.GetAxis("Horizontal");
        init_vertical = Input.GetAxis("Vertical");
        transform.SetParent(_pointCloud.transform); // sets the parent location

        // Initilize a new PosPers object that will be called in the constructor as reference
        posPers = new PosPers(init_x, init_y, init_z, init_vertical, init_horizontal); // Sets the initial 

        return posPers;
    }

    public ARSession arSession
    {
        get { return _arSession; }
        set { _arSession = value; }
    }

    public void ResetButtonTrigger()
    {
        _arSession.Reset();
    }

    public bool safeZoneReached()
    {
        bool isReached = false;
        return isReached;
    }

}
>>>>>>> origin/Viktor
