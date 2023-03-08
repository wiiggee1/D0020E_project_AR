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
