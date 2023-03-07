<<<<<<< HEAD
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Android;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Algorithm : MonoBehaviour
{
    public ARCameraManager arCameraObject; //ARCore device gameobject
    public ARPointCloud _pointCloud; // The AR Foundation PointCloud script
    public ARSessionOrigin _arSessionOrigin;
    public ARSession _arSession;
    public ARPoseDriver _arPoseDriver;

    //Reference to scripts and gameobjects
    public GameObject timerObject;
    private static Timer _timer;
    public GameObject safeZoneReachedObject;
    private static WinLose _winLose;
    private float timeCycle = 0f; 
    private static DataPopupCanvas _popupCanvas;

    // STATE VARIABLES DURING GAMEPLAY
    public Text positionTextNew;
    private float device_lat; //device geodata
    private float device_long; //device geodata
    public Vector2 deviceLocation;
    public Vector3 arCameraPosition;
    public Quaternion arCameraRotation;
    private float arHorizontal;
    private float arVertical;

    // DATASTRUCTURE FOR SAVING GAMEDATA
    float[] xPos;
    float[] yPos;
    float[] zPos;
    float[] verticalPos;
    float[] horizontalPos;
    public long? timeStampOutput;
    float[] timeStampData;
    Dictionary<string, float[]> positionData;


    public void invokeGameObjectReferences()
    {
        ARSessionOrigin[] sessionOrigins = FindObjectsOfType<ARSessionOrigin>();

        foreach (ARSessionOrigin sessionOrigin in sessionOrigins)
        {
            if (sessionOrigin.isActiveAndEnabled)
            {
                // Do something with this AR session origin
                _arSessionOrigin = sessionOrigin.GetComponent<ARSessionOrigin>();
                _arSessionOrigin.camera.transform.position = sessionOrigin.transform.position;
                sessionOrigin.gameObject.SetActive(false); // deactivates the old session origin 

            }
        }

        ARSession[] sessionAr = FindObjectsOfType<ARSession>();

        foreach (ARSession session in sessionAr)
        {
            if (session.isActiveAndEnabled)
            {
                // Do something with this AR session origin
                _arSession = session.GetComponent<ARSession>();

            }
        }

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

=======
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algorithm : MonoBehaviour
{
>>>>>>> 86df386b5967b3f8bb264b37ae56f8ee4737711e
    // Start is called before the first frame update
    void Start()
    {
<<<<<<< HEAD

        // Request permission to access the device's location (ANDROID)
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        else
        {
            Input.location.Start(); //get's the android device current location.
        }
        invokeGameObjectReferences();

        // initialize the AR component of application. 
        _pointCloud = GetComponent<ARPointCloud>();
        _arPoseDriver = GetComponent<ARPoseDriver>();
        arCameraObject = GetComponent<ARCameraManager>();

        //arCameraObject.frameReceived += OnFrameReceived;
        ARSession.stateChanged += ARSessionStateChanged;

        // INITIALIZE DATASTRUCTURE FOR SAVING
        positionData = new Dictionary<string, float[]>();
        timeStampData = new float[] { };
        xPos = new float[] { };
        yPos = new float[] { };
        zPos = new float[] { };
        verticalPos = new float[] { };
        horizontalPos = new float[] { };
    }

    /*
    void OnFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        var timeStampOutputFrame = eventArgs.timestampNs;
        timeStampOutput = timeStampOutputFrame;

        var positionOutput = "Position data: x: " + arCameraPosition.x + ", y: "
            + arCameraPosition.y + ", z: " + arCameraPosition.z;

        positionTextNew.text = positionOutput + ", timestamp: " + timeStampOutputFrame.ToString();

    }*/
=======
        
    }
>>>>>>> 86df386b5967b3f8bb264b37ae56f8ee4737711e

    // Update is called once per frame
    void Update()
    {
<<<<<<< HEAD
        FetchLocationTimeData();

        var positionOutput = "Position data: x: " + arCameraPosition.x + ", y: "
            + arCameraPosition.y + ", z: " + arCameraPosition.z;

        positionTextNew.text = positionOutput;

        IfSafeZoneReachedSqlAction();
    }

    private void FetchLocationTimeData()
    {
        timeCycle += _timer.getTimer();

        if (timeCycle >= 1f)
        {
            var currentTime = _timer.getTimer();

            //Vector3 camPositionCloud = _pointCloud.transform.position;
            arCameraPosition = _arSessionOrigin.camera.transform.position;

            //UnityEngine.Quaternion catRotation = _pointCloud.transform.rotation;
            arCameraRotation = _arSessionOrigin.camera.transform.rotation;

            // gets the euler angle for the horizontal and vertical movment in float
            Vector3 arEulerAngles = arCameraRotation.eulerAngles;
            arHorizontal = arEulerAngles.y;
            arVertical = arEulerAngles.x;

            // Call the setCurrentPosPers method...
            SetPosPersAndTime(arCameraPosition.x, arCameraPosition.y, arCameraPosition.z, arVertical, arHorizontal, currentTime);

            timeCycle = 0f;
        }
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

    public void resetPositionDataState()
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
            _popupCanvas.DisplayPositionDataInCanvas(GetPositionData());
            _popupCanvas.setDataToSend(MapDataToJsonString());
            //StartCoroutine(webclientSqlHandler(getPositionData()));
            //resetPositionDataState();
        }

    }

    public string MapDataToJsonString()
    {
        string jsonStringData = JsonUtility.ToJson(GetPositionData());
        return jsonStringData;
    }
=======
        
    }
>>>>>>> 86df386b5967b3f8bb264b37ae56f8ee4737711e
}
