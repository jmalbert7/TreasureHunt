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

    readonly string getUrl = "https://functionapplicationgroupx.azurewebsites.net/api/clues/create/?huntid=";
//    readonly string postUrl = "https://functionapplicationgroupx.azurewebsites.net/api/clues/create/";

    public string riddle;
    public int firstFlag;
    public int lastFlag; 
    public int lastClueId;
    private int huntId = 1;

    private void Start()
    {
        riddleInput.onEndEdit.AddListener(GetRiddle);
        firstFlag = 1;
        lastFlag = 0;
    }

    private void GetRiddle(string input)
    {
       riddle = input;
    }

    public void ClearFirstFlag()
    {
        firstFlag = 0;
    }
    public void SetLastFlag()
    {
        lastFlag = 1;
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
//        huntId = AddHuntScript.huntId;
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

        if (firstFlag == 1 && lastFlag == 1)    // single clue hunt
        {
            azureUrl = getUrl + huntId + locationParam + location + riddleParam + riddle + firstFlagParam + firstFlag + lastFlagParam + lastFlag;
        }
        else if (firstFlag == 1)    // first clue in multi-clue hunt
        {
            azureUrl = getUrl + huntId + locationParam + location + riddleParam + riddle + firstFlagParam + firstFlag;
        }
        else if (lastFlag == 1)     // last clue in multi-clue hunt
        {
            azureUrl = getUrl + huntId + locationParam + location + riddleParam + riddle + lastFlagParam + lastFlag + lastClueIdParam + lastClueId;
        }
        else                        // middle clue in multi-clue hunt
        {
            azureUrl = getUrl + huntId + locationParam + location + riddleParam + riddle + lastClueIdParam + lastClueId;
        }
        

        Debug.Log(location);
        Debug.Log(azureUrl);
        Debug.Log(firstFlag);
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
            lastClueId = Convert.ToInt32(www.downloadHandler.text);
            Debug.Log(www.downloadHandler.text);
        }
    }
}