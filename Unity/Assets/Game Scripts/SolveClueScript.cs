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
    public TextMeshProUGUI riddleText;
    public GameObject finishHuntScreen;
    public GameObject playGameScreen;

    readonly string getNextClueUrl = "https://functionapplicationgroupx.azurewebsites.net/api/clues/next/?lastclueid=";
    readonly string updateGameUrl = "https://functionapplicationgroupx.azurewebsites.net/api/games/update/?userid=";

    public static int clueId;
    public static int huntId;
    public static int firstFlag;
    public static int lastFlag;
    public static int lastClueId;
    public static string location;
    public static string riddle;

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

    void Start()
    {
        riddleText.text = riddle;
//        riddleText = GameObject.Find("RiddleText").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        riddleText.text = riddle;
    }


    public void CheckGameStatus()
    {
        if (lastFlag == 0)
        {
            OnButtonCallAzureFunction();
//            OnButtonUpdateGame();
            //Debug.Log("not last");
        }
        else
        {
            Debug.Log("last");
            clueId = 0;
            OnButtonUpdateGame();
            finishHuntScreen.SetActive(true);
            playGameScreen.SetActive(false);

        }
    }

    public void OnButtonCallAzureFunction()
    {
        StartCoroutine(AzureGetNextClueByIdRequest());
    }

    public void OnButtonUpdateGame()
    {
        StartCoroutine(UpdateGameRequest());
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
            //Debug.Log(clueId);
            firstFlag = clue[0].FirstFlag ? 1 : 0;
            lastFlag = clue[0].LastFlag ? 1 : 0;
            lastClueId = clue[0].LastClueId;
            location = clue[0].Location;
            riddle = clue[0].Riddle;
            riddleText.text = riddle;

            OnButtonUpdateGame();
        }
    }

    IEnumerator UpdateGameRequest()
    {
        Debug.Log("What is userId and clueId?");
        Debug.Log(LoginScript.userId);
        Debug.Log(clueId);
        string azureUrl;
        string clueIdParam = "&clueid=";

        azureUrl = updateGameUrl + LoginScript.userId + clueIdParam + clueId;

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