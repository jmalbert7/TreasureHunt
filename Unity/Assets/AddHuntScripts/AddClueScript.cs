using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class AddClueScript : MonoBehaviour
{
    public InputField riddleInput;
    

    readonly string getUrl = "https://functionapplicationgroupx.azurewebsites.net/api/clues/?huntid=";
    //readonly string getUrl = "https://treasurehuntgroupx.azurewebsites.net/api/hunts/?userid=";

    public string riddle;
    public int firstflag = 1;
    public int lastflag = 0;
    public int lastClueId = 0;
    public string myLocation = "Great place";
    private int huntId = 1;     // Using user id 2 (Louis) for now 
  

    private void Start()
    {
        riddleInput.onEndEdit.AddListener(GetRiddle);
    }

    private void GetRiddle(string input)
    {
       riddle = input;
    }



    public void DisplayInfo()
    {
       
    }

    public void OnButtonCallAzureFunction()
    {
        StartCoroutine(AzureGetRequest());
    }


    IEnumerator AzureGetRequest()
    {
        string firstFlagParam = "&firstflag=";
        string lastClueIdParam = "&lastclueid=";
        string lastFlagParam = "&lastflag=";
        string locationParam = "&location=";
        string riddleParam = "&riddle=";

        string azureUrl = getUrl + huntId + firstFlagParam + firstflag + lastClueIdParam + lastClueId + lastFlagParam + lastflag + locationParam + myLocation + riddleParam + riddle;
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