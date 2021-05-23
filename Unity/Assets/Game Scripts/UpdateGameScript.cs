using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class UpdateGameScript : MonoBehaviour
{
    readonly string updateGameUrl = "https://functionapplicationgroupx.azurewebsites.net/api/games/update/?userid=";

    public static int userId = 1;
    public static int clueId;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnButtonCallAzureFunction()
    {
        StartCoroutine(AzureGetNextClueByIdRequest());
    }


    IEnumerator AzureGetNextClueByIdRequest()
    {
        string azureUrl;
        string clueIdParam = "&clueid=";

        azureUrl = updateGameUrl + userId + clueIdParam + clueId;

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
