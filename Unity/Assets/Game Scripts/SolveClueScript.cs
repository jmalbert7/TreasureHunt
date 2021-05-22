using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class SolveClueScript : MonoBehaviour
{
    readonly string getNextClueUrl = "https://functionapplicationgroupx.azurewebsites.net/api/clues/next/?lastclueid=";


    public static string riddle;
    public static string location;
    public static int firstFlag;
    public static int lastFlag;
    public static int lastClueId;
    public static int clueId = 85;

    public class CurClue
    {
        public int ClueId { get; set; }
        public int HuntId { get; set; }
        public bool FirstFlag { get; set; }
        public bool LastFlag { get; set; }
        public int LastClueId { get; set; }
        public string Location { get; set; }
        public string Riddle { get; set; }
    }

    private void Start()
    {

    }



    public void OnButtonCallAzureFunction()
    {
        StartCoroutine(AzureGetNextClueByIdRequest());
    }


    IEnumerator AzureGetNextClueByIdRequest()
    {
        string azureUrl;

        azureUrl = getNextClueUrl + clueId;

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
            string responseBody = www.downloadHandler.text;
            responseBody = responseBody.Remove(responseBody.IndexOf("["), 1);
            responseBody = responseBody.Remove(responseBody.IndexOf("]"), 1);
            CurClue clue = JsonConvert.DeserializeObject<CurClue>(responseBody);
            clueId = clue.ClueId;
            Debug.Log(clue.Riddle);
            firstFlag = clue.FirstFlag ? 1 : 0;
            lastFlag = clue.LastFlag ? 1 : 0;
            lastClueId = clue.LastClueId;
            location = clue.Location;
            riddle = clue.Riddle;

        }
    }
}