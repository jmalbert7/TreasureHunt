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
    private double latitude = UseCurrentButton.latitude;
    private double longitude = UseCurrentButton.longitude;
    

    readonly string getUrl = "https://functionapplicationgroupx.azurewebsites.net/api/clues/?huntid=";
    readonly string postUrl = "https://functionapplicationgroupx.azurewebsites.net/api/clues/";


    public string riddle;
    public int firstflag = 1;
    //public bool firstflag = true;
    //public int lastflag = 1; 
    public bool lastflag = true;
    public int lastClueId = 2;
    private int huntId = 1;     // Using huntId 1 for now 
  

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

    public void IncrementFirstFlag()
    {
        firstflag += 1;
    }

    public void ClearInputField()
    {
        riddleInput.text = "";
    }

    public void OnButtonCallAzureFunction()
    {
        StartCoroutine(AzureGetRequest());
    }


    IEnumerator AzureGetRequest()
    {
        string location;
        if(UseCurrentButton.usingCurrentLocation == true)
        {
            location = UseCurrentButton.latitude.ToString() + "," + UseCurrentButton.longitude.ToString();
        }
        else
        {
            location = ReverseGeocodeOnClick.latitude.ToString() + "," + ReverseGeocodeOnClick.longitude.ToString();
        }
        string azureUrl;
        string firstFlagParam = "&firstflag=";
        string lastClueIdParam = "&lastclueid=";
        string lastFlagParam = "&lastflag=";
        string locationParam = "&location=";
        string riddleParam = "&riddle=";

        // Check if on first clue or not
        if (firstflag == 1)
        {
            azureUrl = getUrl + huntId + locationParam + location + riddleParam + riddle + firstFlagParam + firstflag;
        }
        else
        {
            azureUrl = getUrl + huntId + locationParam + location + riddleParam + riddle;
        }
        

        Debug.Log(location);
        Debug.Log(azureUrl);
        Debug.Log(firstflag);
        UnityWebRequest www = UnityWebRequest.Get(azureUrl);
    

        //Form to make post request
        //List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
        //wwwForm.Add(new MultipartFormDataSection("huntid", "1"));
       // wwwForm.Add(new MultipartFormDataSection("firstflag", "1"));
        //wwwForm.Add(new MultipartFormDataSection("lastflag", "0"));
       // wwwForm.Add(new MultipartFormDataSection("lastclueid", "1"));
       // wwwForm.Add(new MultipartFormDataSection("location", "coord"));
        //wwwForm.Add(new MultipartFormDataSection("riddle", "riddle"));

        // Try this form type
        //WWWForm form = new WWWForm();
        //form.AddField("huntid", 1);
        //form.AddField("firstflag", 1);
        //form.AddField("lastflag", 0);
        //form.AddField("lastclueid", "1");
        //form.AddField("location", "coord");
        //form.AddField("riddle", "riddle");
        //UnityWebRequest www = UnityWebRequest.Post(postUrl, form);

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