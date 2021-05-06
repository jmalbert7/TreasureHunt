using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class ConsumeDataScript : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public GameObject inputField;

    readonly string getUrl = "https://functionapplicationgroupx.azurewebsites.net/api/user/?username=";

    private string myName;



    private void Start()
    {
        messageText = GameObject.Find("Canvas/ScreenElements/Message").GetComponent<TextMeshProUGUI>();
        messageText.text = "Type username and click CALL to get player data";
        string starter = "Press button to interact with Azure function";
        Debug.Log(starter);
    }

    public void ReadNameInput()
    {
        myName = inputField.GetComponent<Text>().text;
    }

    public void OnButtonCallAzureFunction()
    {
        StartCoroutine(AzureGetRequest());
    }


    public class User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string HashedPassword { get; set; }
    }


    private void deserialize(string strJSON)
    {
        try
        {
            User curUser = JsonConvert.DeserializeObject<User>(strJSON);
            Debug.Log(curUser.UserId);
            Debug.Log(curUser.Username);
            Debug.Log(curUser.HashedPassword);
            messageText.text = curUser.Username;
        }
        catch (Exception ex)
        {
            Debug.Log("There was an error: " + ex.Message.ToString());
        }
    }


    IEnumerator AzureGetRequest()
    {
        string azureUrl = getUrl + myName;
        UnityWebRequest www = UnityWebRequest.Get(azureUrl);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
        }
        else
        {


            //string[] results = www.downloadHandler.text.Split(',');
            //foreach (string s in results)
            //{
            //    Debug.Log(s);
            //}
            //messageText.text = results[1];

            // deserialize(www.downloadHandler.text);

            //string data = "{ \"UserId\":\"3\",\"Username\":\"llc21\",\"HashedPassword\":\"15C4683193F210CA9C640AF9241E8C18\"}";
            //deserialize(data);

            string userData = www.downloadHandler.text;
            userData = userData.Remove(userData.IndexOf("["), 1);
            userData = userData.Remove(userData.IndexOf("]"), 1);
            deserialize(userData);
        }
    }
}
