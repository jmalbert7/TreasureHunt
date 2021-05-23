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
    public static int clueId = 1;

    public TextMeshProUGUI riddleText;
    public GameObject finishHuntScreen;
    public GameObject playGameScreen;

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
        riddleText = GameObject.Find("RiddleText").GetComponent<TextMeshProUGUI>();
    }


    public void CheckGameStatus()
    {
        if (lastFlag == 0)
        {
            OnButtonCallAzureFunction();
            //Debug.Log("not last");
        }
        else
        {
            Debug.Log("last");
            finishHuntScreen.SetActive(true);
            playGameScreen.SetActive(false);

        }
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
            List<CurClue> clue = JsonConvert.DeserializeObject<List<CurClue>>(responseBody);
            clueId = clue[0].ClueId;
            Debug.Log(clueId);
            firstFlag = clue[0].FirstFlag ? 1 : 0;
            lastFlag = clue[0].LastFlag ? 1 : 0;
            lastClueId = clue[0].LastClueId;
            location = clue[0].Location;
            riddle = clue[0].Riddle;
            riddleText.text = riddle;
        }
    }
}