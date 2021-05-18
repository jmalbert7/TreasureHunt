using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class GetPrevClueScript : MonoBehaviour
{
    public InputField riddleInput;
    public string riddle;
    public int lastClueId;

    readonly string getUrl = "https://functionapplicationgroupx.azurewebsites.net/api/clues/?lastclueid=";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnButtonCallAzureFunction()
    {
        StartCoroutine(AzureGetRequest());
    }


    IEnumerator AzureGetRequest()
    {
        string azureUrl;

        azureUrl = getUrl + lastClueId;
        
        Debug.Log(azureUrl);
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
