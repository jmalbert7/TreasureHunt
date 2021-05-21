using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateClueScript : MonoBehaviour
{
    readonly string updateUrl = "https://functionapplicationgroupx.azurewebsites.net/api/clues/update/?clueid=";

    public void OnButtonCallAzureFunction()
    {
        StartCoroutine(AzureUpdateClueRequest());
    }

    IEnumerator AzureUpdateClueRequest()
    {
        if(UseCurrentButton.usingCurrentLocation == true)
        {
            AddClueScript.location = UseCurrentButton.latitude.ToString() + "," + UseCurrentButton.longitude.ToString();
        }
        else
        {
            AddClueScript.location = ReverseGeocodeOnClick.latitude.ToString() + "," + ReverseGeocodeOnClick.longitude.ToString();
        }

        string azureUrl;
        string locationParam = "&location=";
        string riddleParam = "&riddle=";
        azureUrl = updateUrl + AddClueScript.clueId + locationParam + AddClueScript.location + riddleParam + AddClueScript.riddle;
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
