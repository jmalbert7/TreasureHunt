using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GetPrevClueScript : MonoBehaviour
{
    public InputField riddleInput;
    public string riddle;
    public string location;
//    public int clueId;

    readonly string getUrl = "https://functionapplicationgroupx.azurewebsites.net/api/clues/?clueid=";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public class Clue
    {
        public int ClueId { get; set; }
        public int HuntId { get; set; }
        public bool FirstFlag { get; set; }
        public bool LastFlag { get; set; }
        public int LastClueId { get; set; }
        public string Location { get; set; }
        public string Riddle { get; set; }
    }

    public void OnButtonCallAzureFunction()
    {
        StartCoroutine(AzureGetRequest());
    }

    IEnumerator AzureGetRequest()
    {
        string azureUrl;

        azureUrl = getUrl + AddClueScript.lastClueId;
        
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
            //            var responseBody = JObject.Parse(www.downloadHandler.text);
            string responseBody = www.downloadHandler.text;
            responseBody = responseBody.Remove(responseBody.IndexOf("["), 1);
            responseBody = responseBody.Remove(responseBody.IndexOf("]"), 1);
            var clue = JsonConvert.DeserializeObject<Clue>(responseBody);
            AddClueScript.clueId = AddClueScript.lastClueId;
            Debug.Log(clue.Location);
            Debug.Log(clue.Riddle);
            AddClueScript.firstFlag = clue.FirstFlag ? 1 : 0;
            AddClueScript.lastFlag = clue.LastFlag ? 1 : 0;
            AddClueScript.lastClueId = clue.LastClueId;
            AddClueScript.location = clue.Location;
            AddClueScript.riddle = clue.Riddle;

        }
    }
}
