using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class GetFirstClueScript : MonoBehaviour
{
    public static int huntId;

    readonly string getUrl = "https://functionapplicationgroupx.azurewebsites.net/api/clues/first/?huntid=";

    public GameObject chooseHuntScreen;
    public GameObject playGameScreen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHuntId(Button b)
    {
        huntId = GetHuntsScript.huntIds[Convert.ToInt32(b.tag)];
        Debug.Log("What is the huntid for this button?");
        Debug.Log(huntId);
    }

    public void OnButtonCallAzureFunction()
    {
        StartCoroutine(AzureGetRequest());
    }

    IEnumerator AzureGetRequest()
    {
        string azureUrl;

        azureUrl = getUrl + huntId;
        
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
            var clue = JsonConvert.DeserializeObject<SolveClueScript.CurClue>(responseBody);
            SolveClueScript.clueId = clue.ClueId;
            Debug.Log(clue.Riddle);
            SolveClueScript.huntId = clue.HuntId;
            SolveClueScript.firstFlag = clue.FirstFlag ? 1 : 0;
            SolveClueScript.lastFlag = clue.LastFlag ? 1 : 0;
            SolveClueScript.lastClueId = clue.LastClueId;
            SolveClueScript.location = clue.Location;
            SolveClueScript.riddle = clue.Riddle;
//            SolveClueScript.riddleText.text = clue.Riddle;

            playGameScreen.SetActive(true);
            chooseHuntScreen.SetActive(false);
        }
    }

}
