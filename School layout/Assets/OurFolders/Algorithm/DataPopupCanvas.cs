using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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
        buttonDisplay = dataPopupCanvas.GetComponentInChildren<Button>();
        popupDisplayText = dataPopupCanvas.GetComponentInChildren<Text>();
        //buttonDisplay.onClick.AddListener(OnButtonClickDisplayData);
    }

    public void setPopupText(string popupText)
    {
        this.popupDisplayText.text = popupText;
    }

    public void setDataToSend(string dataToSend)
    {
        this.jsonDataToSend = dataToSend;
    }

    public void DisplayPositionDataInCanvas<T>(Dictionary<string, T> positionDataDict)
    {
        popupDisplayText.text = "";
        foreach (KeyValuePair<string, T> pair in positionDataDict)
        {
            popupDisplayText.text += pair.Key + ": " + pair.Value + "\n";
        }
        dataPopupCanvas.gameObject.SetActive(true);
    }

    public void OnButtonClickDisplayData()
    {
        popupDisplayText.gameObject.SetActive(!popupDisplayText.gameObject.activeSelf);
        
    }

    public void OnUploadButton()
    {
        StartCoroutine(SqlWebrequest());
        dataPopupCanvas.gameObject.SetActive(true);

    }
    public IEnumerator SqlWebrequest()
    {
        WebClient webClient = new WebClient();
        WWWForm form = new WWWForm();

        //Dictionary<string, float[]> gameData = getPositionData();
        string jsonStringData = JsonUtility.ToJson(jsonDataToSend);
        string server_url = "http://130.240.202.127/server_repo/gameSqlHandler.php";

        form.AddField("data", jsonStringData);

        UnityWebRequest www = UnityWebRequest.Post(server_url, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            setPopupText("Data sent successfully!");
        }
        else
        {
            setPopupText("Error sending data: " + www.error);
        }
    }


}



