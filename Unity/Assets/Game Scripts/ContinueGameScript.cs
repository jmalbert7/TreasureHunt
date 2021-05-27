using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;

public class ContinueGameScript : MonoBehaviour
{
    readonly string getUrl = "https://functionapplicationgroupx.azurewebsites.net/api/clues/?clueid=";

    public GameObject mainMenu;
    public GameObject playGameScreen;
    public GameObject noGameScreen;

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
        if (SolveClueScript.clueId == 0)
        {
            noGameScreen.SetActive(true);
            mainMenu.SetActive(false);
        }
        else
        {
            StartCoroutine(AzureGetRequest());
        }
    }

    IEnumerator AzureGetRequest()
    {
        string azureUrl;

        azureUrl = getUrl + SolveClueScript.clueId;
        
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
            mainMenu.SetActive(false);
        }
    }
}
