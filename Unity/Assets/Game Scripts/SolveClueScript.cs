using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;

public class SolveClueScript : MonoBehaviour
{
    public TextMeshProUGUI riddleText;
    public GameObject finishHuntScreen;
    public GameObject playGameScreen;
    public GameObject wrongLocationScreen;
    public GameObject rightLocationScreen;

    readonly string getNextClueUrl = "https://functionapplicationgroupx.azurewebsites.net/api/clues/next/?lastclueid=";
    readonly string updateGameUrl = "https://functionapplicationgroupx.azurewebsites.net/api/games/update/?userid=";

    public static int clueId;
    public static int huntId;
    public static int firstFlag;
    public static int lastFlag;
    public static int lastClueId;
    public static string location;
    public static string riddle;
    private bool correctLocation;
    private double correctLatitude;
    private double correctLongitude;
    private double latitudeDiff;
    private double longitudeDiff;
    private string[] coordinates;

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

    public bool VerifyLocation()
    {
        coordinates = location.Split(',');
        correctLatitude = Convert.ToDouble(coordinates[0]);
        correctLongitude = Convert.ToDouble(coordinates[1]);
        Debug.Log("What is the correctLatitude and correctLongitude?");
        Debug.Log(correctLatitude);
        Debug.Log(correctLongitude);
        if (UseCurrentButton.usingCurLocInGame == true)
        {
            latitudeDiff = Math.Abs(UseCurrentButton.latitude - correctLatitude);
            longitudeDiff = Math.Abs(UseCurrentButton.longitude - correctLongitude);
        }
        else
        {
            latitudeDiff = Math.Abs(ReverseGeocodeOnClick.latitude - correctLatitude);
            longitudeDiff = Math.Abs(ReverseGeocodeOnClick.longitude - correctLongitude);
        }
                                                            // at the equator...
        if (latitudeDiff <= 0.01 && longitudeDiff <= 0.01)  // .01 deg = 1.11 km
        {                                                   // .001 deg = 111 m
            return true;                                    // .0001 deg = 11.1 m
        }                                                   // moving north or south a degree of longitude
        return false;                                       // is a shorter distance
    }                                                       // https://en.wikipedia.org/wiki/Decimal_degrees

    public void CheckGameStatus()
    {
        correctLocation = VerifyLocation();
        if (correctLocation)
        {
            if (lastFlag == 0)
            {
                OnButtonCallAzureFunction();
                // OnButtonUpdateGame() is called after we get the next clue
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
        else
        {
            wrongLocationScreen.SetActive(true);
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
            rightLocationScreen.SetActive(true);
            playGameScreen.SetActive(false);
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