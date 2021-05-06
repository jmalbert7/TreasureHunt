using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class CallAzure : MonoBehaviour
{
    //public Text messageText;
    public TextMeshProUGUI messageText;
    public GameObject inputField;

    readonly string getUrl = "https://functionapplicationgroupx.azurewebsites.net/api/v1/dummyfunction?name=";

    private string myName;

    private void Start()
    {
        messageText = GameObject.Find("Canvas/ScreenElements/Message").GetComponent<TextMeshProUGUI>();
        messageText.text = "Type name and click CALL for special message";
        string starter = "Press button to interact with Azure function";
        Debug.Log(starter);
    }

    public void ReadNameInput()
    {
        myName = inputField.GetComponent<Text>().text;
        Debug.Log(myName);
    }

    public void OnButtonCallAzureFunction()
    {
        //messageText.text = "Caliing Azure function";
        Debug.Log("Calling Azure function");
        StartCoroutine(AzureGetRequest());
    }

    IEnumerator AzureGetRequest()
    {
        string azureUrl = getUrl + myName;
        UnityWebRequest www = UnityWebRequest.Get(azureUrl);

        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            messageText.text = www.downloadHandler.text;
            Debug.Log(www.downloadHandler.text);
        }
    }


}
