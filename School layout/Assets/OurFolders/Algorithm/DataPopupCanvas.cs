using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.XR.ARSubsystems.XRCpuImage;

public class DataPopupCanvas : MonoBehaviour
{
    public Canvas dataPopupCanvas;
    public Button buttonDisplay;
    public Text popupDisplayText;
    public string jsonDataToSend;

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
        StartCoroutine(SqlWebrequest(Algorithm.requestData));
        dataPopupCanvas.gameObject.SetActive(value: true);

    }
    public IEnumerator SqlWebrequest(string inputDataToSend)
    {
        string jsonStringData = inputDataToSend;
        var dataDict = Algorithm.requestDataDictionary;

        string server_url = "http://130.240.202.127/server_repo/gameSqlHandler.php";
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<string, float[]> posData in dataDict)
        {
            string key = posData.Key;
            float[] values = posData.Value;
            for (int i = 0; i < values.Length; i++)
            {
                form.AddField(key + "[]", values[i].ToString());
            }
        }
        //form.AddField("data", jsonStringData);
        UnityWebRequest www = UnityWebRequest.Post(server_url, form);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (Algorithm.requestData == "")
            {
                SetPopupText("Trying to send empty data string!");
            }
            else
            {
                SetPopupText("Data sent successfully!");
                StopAllCoroutines();
            }
        }
        else
        {
            SetPopupText("Error sending data: " + www.error);
        }
    }


}



