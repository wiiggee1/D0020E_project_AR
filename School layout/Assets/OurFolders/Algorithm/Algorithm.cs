using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Android;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class Algorithm : MonoBehaviour
{
    public ARCameraManager arCameraObject; //ARCore device gameobject
    public ARPointCloud _pointCloud; // The AR Foundation PointCloud script
    public ARSessionOrigin _arSessionOrigin;
    public ARSession _arSession;
    public ARPoseDriver _arPoseDriver;

    //Reference to scripts and gameobjects
    [SerializeField] GameObject timerObject;
    Timer _timer;
    [SerializeField] GameObject safeZoneReachedObject;
    WinLose _winLose;
    DataPopupCanvas _popupCanvas;

    // STATE VARIABLES DURING GAMEPLAY
    private float timeCycle;
    private float device_lat; //device geodata
    private float device_long; //device geodata
    public Vector2 deviceLocation;
    public Vector3 arCameraPosition;
    public Quaternion arCameraRotation;
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


    public void InvokeGameObjectReferences()
    {
        ARSessionOrigin[] sessionOrigins = FindObjectsOfType<ARSessionOrigin>();
        ARSession[] sessionAr = FindObjectsOfType<ARSession>();

        foreach (ARSessionOrigin activeSessionOrigin in sessionOrigins)
        {
            if (activeSessionOrigin.isActiveAndEnabled)
            {
                // Do something with this AR session origin
                _arSessionOrigin = activeSessionOrigin.GetComponent<ARSessionOrigin>();
                _arSessionOrigin.camera = activeSessionOrigin.camera;
                _arSessionOrigin.camera.transform.position = activeSessionOrigin.transform.position;
                activeSessionOrigin.gameObject.SetActive(false);
            }
        }

        foreach (ARSession activeSession in sessionAr)
        {
            if (activeSession.isActiveAndEnabled)
            {
                // Do something with this AR session origin
                _arSession = activeSession.GetComponent<ARSession>();
                activeSession.gameObject.SetActive(true);

                //session.enabled = false;

            }
        }
        
        // REFRESH ACTIVE STATE OF GAME OBJECT 
        _arSession.gameObject.SetActive(true);
        _arSessionOrigin.gameObject.SetActive(true); // activates new main AR session origin

        _timer = timerObject.GetComponent<Timer>();
        _winLose = safeZoneReachedObject.GetComponent<WinLose>();

        // set inital starting data
        var start_x = _arSessionOrigin.camera.transform.position.x;
        var start_y = _arSessionOrigin.camera.transform.position.y;
        var start_z = _arSessionOrigin.camera.transform.position.z;
        Vector3 arRotation = _arSessionOrigin.camera.transform.eulerAngles;
        var start_horizontal = arRotation.y;
        var start_vertical = arRotation.x;
        var start_time = _timer.getTimer();

        SetPosPersAndTime(start_x, start_y, start_z, start_vertical, start_horizontal, start_time);
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
        //_arSessionOrigin = GetComponent<ARSessionOrigin>();
        _pointCloud = GetComponent<ARPointCloud>();
        _arPoseDriver = GetComponent<ARPoseDriver>();
        //arCameraObject = GetComponent<ARCameraManager>();

        //arCameraObject.frameReceived += OnFrameReceived;
        ARSession.stateChanged += ARSessionStateChanged;        

        InvokeGameObjectReferences();
    }

        
    // Update is called once per frame
    public void Update()
    {
        if (!Timer.gameEnded)
        {
            Invoke(nameof(FetchLocationTimeData),1f);
        }
 
        IfSafeZoneReachedSqlAction();
    }

    public void FetchLocationTimeData()
    {
        //timeCycle += _timer.getTimer();
           
        //yield return new WaitForSeconds(1.0f);

        timeCycle = Timer.currentTime;

        //Vector3 camPositionCloud = _pointCloud.transform.position;
        arCameraPosition = _arSessionOrigin.camera.transform.position;

        //UnityEngine.Quaternion catRotation = _pointCloud.transform.rotation;
        arCameraRotation = _arSessionOrigin.camera.transform.rotation;

        // gets the euler angle for the horizontal and vertical movment in float
        Vector3 arEulerAngles = arCameraRotation.eulerAngles;
        arHorizontal = arEulerAngles.y;
        arVertical = arEulerAngles.x;

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
            DataPopupCanvas.SetPopupText(MapDataToJsonString());
            //_popupCanvas.SetPopupText(MapDataToJsonString());
            DataPopupCanvas.SetDataToSend(MapDataToJsonString());

            //_popupCanvas.SetDataToSend(MapDataToJsonString());
            //ResetPositionDataState();
            CancelInvoke(nameof(FetchLocationTimeData));
        }

    }

    public string MapDataToJsonString()
    {
        string jsonStringData = JsonConvert.SerializeObject(GetPositionData());
        return jsonStringData;
    }
        
}
