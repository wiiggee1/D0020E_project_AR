using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DataPopupCanvas : MonoBehaviour
{
    static public Canvas dataPopupCanvas;
    public Button buttonDisplay;
    static public Text popupDisplayText;
    static public string jsonDataToSend;

    // Start is called before the first frame update
    void Start()
    {
        //dataPopupCanvas.gameObject.SetActive(false);
        buttonDisplay = dataPopupCanvas.GetComponentInChildren<Button>();
        popupDisplayText = dataPopupCanvas.GetComponentInChildren<Text>();
        //buttonDisplay.onClick.AddListener(OnButtonClickDisplayData);
    }

    public static void SetPopupText(string popupText)
    {
        popupDisplayText.text = popupText;
    }

    public static void SetDataToSend(string dataToSend)
    {
        jsonDataToSend = dataToSend;
    }

    public void OnButtonClickDisplayData()
    {
        if (jsonDataToSend == "")
        {
            popupDisplayText.text = "No data received or stored!";
        }
        else
        { 
            popupDisplayText.text = jsonDataToSend;     
        }

        //dataPopupCanvas.gameObject.SetActive(value: true);
        popupDisplayText.gameObject.SetActive(!popupDisplayText.gameObject.activeSelf);
    }

    public void OnUploadButton()
    {
        StartCoroutine(SqlWebrequest());
        dataPopupCanvas.gameObject.SetActive(value: true);

    }
    public IEnumerator SqlWebrequest()
    {
        string jsonStringData = JsonUtility.ToJson(jsonDataToSend);
        string server_url = "http://130.240.202.127/server_repo/gameSqlHandler.php";
        WWWForm form = new WWWForm();
        form.AddField("data", jsonStringData);
        UnityWebRequest www = UnityWebRequest.Post(server_url, form);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            SetPopupText("Data sent successfully!");
        }
        else
        {
            SetPopupText("Error sending data: " + www.error);
        }
    }


}



