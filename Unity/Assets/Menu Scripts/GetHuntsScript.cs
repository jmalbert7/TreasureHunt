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
    public TextMeshProUGUI[] huntsArray = new TextMeshProUGUI[12];
    public static int[] huntIds = new int[12];

    readonly string getUrl = "https://functionapplicationgroupx.azurewebsites.net/api/hunts/list";

    // Start is called before the first frame update
    void Start()
    {

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
            string responseBody = www.downloadHandler.text;
            List<Hunt> hunts = JsonConvert.DeserializeObject<List<Hunt>>(responseBody);

            for(int i = 0; i < hunts.Count; i++)
            {
                huntsArray[i].text = hunts[i].HuntName + ", " + hunts[i].GeneralLocation;
                huntIds[i] = hunts[i].HuntId;
            }

            chooseHuntScreen.SetActive(true);
            mainMenu.SetActive(false);
        }
    }
}
