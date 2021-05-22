using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;

public class GetHuntsScript : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject chooseHuntScreen;
    public TextMeshProUGUI[] huntsArray;

    readonly string getUrl = "https://functionapplicationgroupx.azurewebsites.net/api";

    // Start is called before the first frame update
    void Start()
    {
        huntsArray = new TextMeshProUGUI[12];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public class Hunt
    {
        public int HuntId { get; set; }
        public int UserId { get; set; }
        public string HuntName { get; set; }
        public string GeneralLocation { get; set; }
    }

    public void OnButtonCallAzureFunction()
    {
        StartCoroutine(AzureGetRequest());
    }

    IEnumerator AzureGetRequest()
    {
        string azureUrl;

        azureUrl = getUrl;
        
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
            var hunts = JsonConvert.DeserializeObject<Hunt>(responseBody);
            Debug.Log(hunts);
/*
            for(int i = 0; i < 12; i++)
            {
                huntsArray[i] = hunts[i];
            }
*/

//            AddClueScript.location = clue.Location;
            chooseHuntScreen.SetActive(true);
            mainMenu.SetActive(false);
        }
    }
}
