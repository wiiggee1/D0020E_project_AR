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
using UnityEngine.UI;
using Unity.XR.CoreUtils;
using System.Threading.Tasks;
using UnityEditor.VersionControl;
using System.Xml.Linq;
using MySqlConnector;

public class Algorithm : MonoBehaviour
{
    private ARCameraManager arCameraObject; //ARCore device gameobject
    private ARPointCloud _pointCloud; // The AR Foundation PointCloud script
    private ARSessionOrigin _arSessionOrigin;
    private ARSession _arSession;
    private ARPoseDriver _arPoseDriver;

    //public event Action<ARCameraFrameEventArgs> frameReceived;
    private bool trackingflag = false;
    public Text positionTextNew;
    public InputField passwordInput;
    private UnityEngine.Vector2 deviceLocation;
    private float device_lat; //device geodata
    private float device_long; //device geodata
    public Vector3 previousArCameraPosition;
    public Vector3 arCameraPosition;
    public UnityEngine.Quaternion arCameraRotation;
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



    // Start is called before the first frame update
    void Start()
    {

        if (ARSession.state == ARSessionState.Unsupported)
        {
            _arSession.enabled = false;
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

        // initialize the AR Session Origin component as reference during awake state of application. 
        _arSessionOrigin = GetComponent<ARSessionOrigin>();
        _arSession = GetComponent<ARSession>();
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
    void Update()
    {
        mapCameraLocationData();

        var positionOutput = "Position data: x: " + arCameraPosition.x + ", y: "
            + arCameraPosition.y + ", z: " + arCameraPosition.z;

        positionTextNew.text = positionOutput;

        ifSafeZoneReachedSqlAction();
    }

    private void mapCameraLocationData()
    {
        // The transform object is attached to the gameobject 

        //Vector3 camPositionCloud = _pointCloud.transform.position;
        arCameraPosition = _arSessionOrigin.camera.transform.position;

        //UnityEngine.Quaternion catRotation = _pointCloud.transform.rotation;
        arCameraRotation = _arSessionOrigin.camera.transform.rotation;

        // gets the euler angle for the horizontal and vertical movment in float
        Vector3 arEulerAngles = arCameraRotation.eulerAngles;
        arHorizontal = arEulerAngles.y;
        arVertical = arEulerAngles.x;

        if (!trackingflag)
        {
            trackingflag = true;
            previousArCameraPosition = _arSessionOrigin.camera.transform.position;
        }

        // delta position (position difference)
        Vector3 deltaArPosition = arCameraPosition - previousArCameraPosition;
        previousArCameraPosition = arCameraPosition;

        //This will set the AR session space to world space in unity
        //Vector3 deviceToWorldLocation = _arSessionOrigin.camera.transform.localToWorldMatrix.GetPosition();

        // Call the setCurrentPosPers method...
        setCurrentPosPers(arCameraPosition.x, arCameraPosition.y, arCameraPosition.z, arVertical, arHorizontal);
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
    public void setCurrentPosPers(float x, float y, float z, float vertical, float horizontal)
    {
        xPos = xPos.Append(x).ToArray();
        yPos = yPos.Append(y).ToArray();
        zPos = zPos.Append(z).ToArray();

        verticalPos = verticalPos.Append(vertical).ToArray();
        horizontalPos = horizontalPos.Append(horizontal).ToArray();

        timeStampData = timeStampData.Append((float)timeStampOutput).ToArray();
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
        bool safeZoneReached = true; 
        if (safeZoneReached)
        {
            passwordInput.contentType = InputField.ContentType.Password;
            _ = sqlHandlerAsync(passwordInput.contentType);
            resetPositionDataState();
        }
    }


    public async System.Threading.Tasks.Task sqlHandlerAsync(InputField.ContentType passwrd)
    {
        
        var password = passwrd;
        var server = "130.240.202.127";
        var userID = "projectmember";
        var dbName = "d0020e";
        var connectionString = "server=" + server+";uid="+userID+";pwd="+password+";database="+dbName;

        try
        {

            var connection = new MySqlConnection();
            connection.ConnectionString = connectionString;
            await connection.OpenAsync();
                                        
            var x_val = getPositionData()["x_position"];
            var y_val = getPositionData()["y_position"];
            var z_val = getPositionData()["z_position"];
            var vertical_val = getPositionData()["vertical_position"];
            var horizontal_val = getPositionData()["horizontal_position"];
            var timestamp_val = getPositionData()["timestamp"];

            var row_count = timestamp_val.Length;
            var col_names = positionData.Keys.ToArray();
            string col_name_join = string.Join(",", col_names);
            
            for (int i = 0 ; i<=row_count ; i++)
            {
                object[] row_values = { x_val.ElementAt(i), y_val.ElementAt(i), z_val.ElementAt(i), vertical_val.ElementAt(i), horizontal_val.ElementAt(i), timestamp_val.ElementAt(i) };
                string row_val_join = string.Join(",", row_values); 
                var query = new MySqlCommand("INSERT INTO d0020e ("+col_name_join+") VALUES (+"+row_val_join+");", connection);
                var query_output = await query.ExecuteNonQueryAsync();
            }
            
            await connection.CloseAsync();
        }
        catch (MySqlException ex)
        {
            Debug.LogError(ex.Message);
        }

    }

}
