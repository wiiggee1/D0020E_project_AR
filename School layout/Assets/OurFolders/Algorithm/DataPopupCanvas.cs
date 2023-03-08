using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;
using UnityEditor.MemoryProfiler;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class DataPopupCanvas : MonoBehaviour
{
    public Canvas dataPopupCanvas;
    public Button buttonDisplay;
    public Text popupDisplayText;
    public string jsonDataToSend;

    private MySqlConnection connection;
    private string connectionString = "server=130.240.202.127;user=root;database=d0020e;port=3306;password=456;";

    // Start is called before the first frame update
    void Start()
    {
        //dataPopupCanvas.gameObject.SetActive(false);
        //buttonDisplay = dataPopupCanvas.GetComponentInChildren<Button>();
        //popupDisplayText = dataPopupCanvas.GetComponentInChildren<Text>();
        //buttonDisplay.onClick.AddListener(OnButtonClickDisplayData);
    }

    public void SetPopupText(string popupText)
    {
        popupDisplayText.text = popupText;
    }

    public void SetDataToSend(string dataToSend)
    {
        jsonDataToSend = dataToSend;
    }

    public void OnButtonClickDisplayData()
    {
        if (Algorithm.requestData == "")
        {
            popupDisplayText.text = "No data received or stored!";
        }
        else
        { 
            popupDisplayText.text = Algorithm.requestData;     
        }

        dataPopupCanvas.gameObject.SetActive(value: true);
        popupDisplayText.gameObject.SetActive(!popupDisplayText.gameObject.activeSelf);
    }

    public void OnUploadButton()
    {
        InsertPositionData(Algorithm.requestDataDictionary);
        //StartCoroutine(SqlWebrequest(Algorithm.requestData));
        dataPopupCanvas.gameObject.SetActive(value: true);

    }
    public IEnumerator SqlWebrequest(string dataRequest)
    {
        string jsonStringData = dataRequest;
        var dataDict = Algorithm.requestDataDictionary;

        string server_url = "http://130.240.202.127/server_repo/gameSqlHandler.php";

        var request = new UnityWebRequest(server_url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStringData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        /*
        WWWForm form = new WWWForm();

        form.AddField("data", jsonStringData);

        foreach (KeyValuePair<string, float[]> posData in dataDict)
        {
            string key = posData.Key;
            float[] values = posData.Value;
            for (int i = 0; i < values.Length; i++)
            {
                form.AddField(key +"[]", values[i].ToString());
            }
        }

        UnityWebRequest www = UnityWebRequest.Put(server_url, form);

        yield return www.SendWebRequest(); */

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (Algorithm.requestData == "")
            {
                SetPopupText("Trying to send empty data string!");
            }
            else
            {
                SetPopupText("Data sent successfully!");
                //StopAllCoroutines();
            }
        }
        else
        {
            SetPopupText("Error sending data: " + request.error);
        }
    }

    private void InsertPositionData(Dictionary<string, float[]> positionData)
    {
        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();

            // Build the query string
            string query = "INSERT INTO position_data (position_x, position_y, position_z, vertical_position, horizontal_position, timestamp_frame) VALUES ";
            query += "(@position_x, @position_y, @position_z, @vertical_position, @horizontal_position, @timestamp_frame)";

            int arrayLength = positionData["position_x"].Length;
            for (int i = 0; i < arrayLength; i++)
            {
                // Add the parameters to the command
                command.Parameters.AddWithValue("@position_x", positionData["position_x"][i]);
                command.Parameters.AddWithValue("@position_y", positionData["position_y"][i]);
                command.Parameters.AddWithValue("@position_z", positionData["position_z"][i]);
                command.Parameters.AddWithValue("@vertical_position", positionData["vertical_position"][i]);
                command.Parameters.AddWithValue("@horizontal_position", positionData["horizontal_position"][i]);
                command.Parameters.AddWithValue("@timestamp_frame", positionData["timestamp_frame"][i]);

                // Execute the query
                command.CommandText = query;
                command.ExecuteNonQuery();

                // Clear the parameters for the next iteration
                command.Parameters.Clear();
            }
            connection.Close();
            SetPopupText("Query was successfully executed!");
        }
        catch (MySqlException ex)
        {
            SetPopupText("Error sending data: " + ex.Message);
        }
           
    }

}



