using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class AddHuntScript : MonoBehaviour
{
   
    public InputField huntName;
    public InputField locationName;

    readonly string getUrl = "https://functionapplicationgroupx.azurewebsites.net/api/hunts/?userid=";
//    readonly string getUrl = "https://treasurehuntgroupx.azurewebsites.net/api/hunts/?userid=";

    private string myName;
    private int myUserId = 2;     // Using user id 2 (Louis) for now 
    private string myHunt;
    private string myLocation;



    private void Start()
    {
        huntName.onEndEdit.AddListener(GetHuntName);
        locationName.onEndEdit.AddListener(GetHuntLocation);
    }

    private void GetHuntName(string input)
    {
        myHunt = input;
    }

    private void GetHuntLocation(string input)
    {
        myLocation = input;
    }

    public void DisplayInfo()
    {
        Debug.Log(myHunt);
        Debug.Log(myLocation);
    }

    public void OnButtonCallAzureFunction()
    {
        StartCoroutine(AzureGetRequest());
    }


    IEnumerator AzureGetRequest()
    {
        string huntValue = "&name=";
        string locationValue = "&location=";
        string azureUrl = getUrl + myUserId + huntValue + myHunt + locationValue + myLocation;
        Debug.Log(azureUrl);
        UnityWebRequest www = UnityWebRequest.Get(azureUrl);

        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

        }
    }
}
