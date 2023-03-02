using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Android;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.UI;
using System.Net;

public class Algorithm : MonoBehaviour
{
    private ARCameraManager arCameraObject; //ARCore device gameobject
    public ARPointCloud _pointCloud; // The AR Foundation PointCloud script
    public ARSessionOrigin _arSessionOrigin;
    public ARSession _arSession;
    private ARPoseDriver _arPoseDriver;

    //Reference to scripts and gameobjects
    public GameObject timerObject;
    private Timer _timer;
    public GameObject safeZoneReachedObject;
    private WinLose _winLose;
    private float timeCycle = 0f; 


    public Text positionTextNew;
    private float device_lat; //device geodata
    private float device_long; //device geodata
    public Vector2 deviceLocation;
    public Vector3 arCameraPosition;
    public Quaternion arCameraRotation;
    private float arHorizontal;
    private float arVertical;

    float[] xPos = new float[] { };
    float[] yPos = new float[] { };
    float[] zPos = new float[] { };
    float[] verticalPos = new float[] { };
    float[] horizontalPos = new float[] { };
    public long? timeStampOutput;
    float[] timeStampData = new float[] { };
    Dictionary<string, float[]> positionData = new Dictionary<string, float[]>();


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

        setPosPersAndTime(start_x, start_y, start_z, start_vertical, start_horizontal, start_time);
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
        invokeGameObjectReferences();

        // initialize the AR component of application. 
        _pointCloud = GetComponent<ARPointCloud>();
        _arPoseDriver = GetComponent<ARPoseDriver>();
        arCameraObject = GetComponent<ARCameraManager>();

        arCameraObject.frameReceived += OnFrameReceived;
        ARSession.stateChanged += ARSessionStateChanged;

    }

    void OnFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        var timeStampOutputFrame = eventArgs.timestampNs;
        timeStampOutput = timeStampOutputFrame;

        var positionOutput = "Position data: x: " + arCameraPosition.x + ", y: "
            + arCameraPosition.y + ", z: " + arCameraPosition.z;

        positionTextNew.text = positionOutput + ", timestamp: " + timeStampOutputFrame.ToString();

    }

    // Update is called once per frame
    public void Update()
    {
        fetchLocationTimeData();

        var positionOutput = "Position data: x: " + arCameraPosition.x + ", y: "
            + arCameraPosition.y + ", z: " + arCameraPosition.z;

        positionTextNew.text = positionOutput;

        ifSafeZoneReachedSqlAction();
    }

    private void fetchLocationTimeData()
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
            setPosPersAndTime(arCameraPosition.x, arCameraPosition.y, arCameraPosition.z, arVertical, arHorizontal, currentTime);

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
    public void setPosPersAndTime(float x, float y, float z, float vertical, float horizontal, float time)
    {
        xPos = xPos.Append(x).ToArray();
        yPos = yPos.Append(y).ToArray();
        zPos = zPos.Append(z).ToArray();

        verticalPos = verticalPos.Append(vertical).ToArray();
        horizontalPos = horizontalPos.Append(horizontal).ToArray();

        timeStampData = timeStampData.Append(time).ToArray();
    }

    // Should be of type PosPers instead of double!
    public Dictionary<string, float[]> getPositionData()
    {
        positionData.Add("x_position", xPos);
        positionData.Add("y_position", yPos);
        positionData.Add("z_position", zPos);
        positionData.Add("vertical_position", verticalPos);
        positionData.Add("horizontal_position", horizontalPos);
        positionData.Add("timestamp", timeStampData);

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

    public void ifSafeZoneReachedSqlAction()
    {
        // if the query was successfully executed reset/restore positionData dictionary!
        if (_winLose.safeZoneReached == true)
        {
            webclientSqlHandler();
            resetPositionDataState();
        }
    }

    public void webclientSqlHandler()
    {
        // add getter method here for fetching data from other classes such as:
        //1. starting point... 
        //2. obstacle_course...
        var startingPoint = Obstacle.spawn;
        var obstacle_course = Obstacle.course;

        var gameData = getPositionData();
        string toJson = Newtonsoft.Json.JsonConvert.SerializeObject(gameData);
        string server_url = "http://130.240.202.127/server_repo/gameSqlHandler.php";
        WebClient webClient = new WebClient();

        webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
        _ = webClient.UploadString(server_url, "POST", toJson);
    }

}
