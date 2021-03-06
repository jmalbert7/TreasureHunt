using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class AddClueScript : MonoBehaviour
{
    public InputField riddleInput;
    public TextMeshProUGUI clueNumberText;
    public GameObject updateClueButton;
    public GameObject prevClueButton;
    public GameObject finishHuntButton;
    public TextMeshProUGUI updateClueText;

    readonly string getUrl = "https://functionapplicationgroupx.azurewebsites.net/api/clues/create/?huntid=";
    readonly string getNextClueUrl = "https://functionapplicationgroupx.azurewebsites.net/api/clues/next/?lastclueid=";
    //    readonly string postUrl = "https://functionapplicationgroupx.azurewebsites.net/api/clues/create/";

    public static int clueNumber;
    public static int newestClueNumber;
    public static string riddle;
    public static string location;
    public static int firstFlag;
    public static int lastFlag; 
    public static int lastClueId;
    public static int clueId;   // only used for updating a clue or getting the next clue

    private void Start()
    {
        riddleInput.onEndEdit.AddListener(GetRiddle);
        DisableButton(updateClueButton);
        DisableButton(prevClueButton);
        updateClueText.text = "";
        clueNumber = 1;
        newestClueNumber = 1;
        clueNumberText.text = "Clue #1";
        firstFlag = 1;
        lastFlag = 0;
    }

    public void ResetHunt()
    {
        DisableButton(prevClueButton);
        updateClueText.text = "";
        clueNumber = 1;
        newestClueNumber = 1;
        clueNumberText.text = "Clue #1";
        firstFlag = 1;
        lastFlag = 0;
    }

    public void EnableButton(GameObject onButton)
    {
        onButton.SetActive(true);
    }

    public void DisableButton(GameObject offButton)
    {
        offButton.SetActive(false);
    }

    private void GetRiddle(string input)
    {
       riddle = input;
    }

    public void ClearFirstFlag()
    {
        firstFlag = 0;
    }
    public void SetLastFlag()
    {
        lastFlag = 1;
    }
    public void UpdateInputField()
    {
        if(clueNumber < newestClueNumber)
        {
            riddleInput.text = riddle;
        }
        else
        {
            riddleInput.text = "";
        }
    }
    public void IncrementClueNumber()
    {
        clueNumber++;
        if(clueNumber > 1)
        {
            EnableButton(prevClueButton);
        }
        if(clueNumber > newestClueNumber)
        {
            newestClueNumber = clueNumber;
        }
        if(clueNumber == newestClueNumber - 1)
        {
            EnableButton(finishHuntButton);
        }
        else if(clueNumber == newestClueNumber)
        {
            DisableButton(updateClueButton);
            updateClueText.text = "";
        }
    }
    public void DecrementClueNumber()
    {
        clueNumber--;
        if(clueNumber <= 1)
        {
            DisableButton(prevClueButton);
        }
        if(clueNumber < newestClueNumber)
        {
            EnableButton(updateClueButton);
            updateClueText.text = "Save Updates";
        }
        if(clueNumber < newestClueNumber - 1)
        {
            DisableButton(finishHuntButton);
        }
    }
    public void UpdateClueNumberText()
    {
        clueNumberText.text = "Clue #" + clueNumber.ToString();
    }

    public void OnButtonCallAzureFunction()
    {
        if(clueNumber == newestClueNumber || lastFlag == 1)
        {
            StartCoroutine(AzureGetRequest());
        }
        else if(clueNumber == newestClueNumber - 1)
        {
            // set lastClueId for the next CreateClue request
            lastClueId = clueId;
            IncrementClueNumber();
            UpdateClueNumberText();
            UpdateInputField();
        }
        else // get the next clue by making a request to GetNextClueById
        {
            StartCoroutine(AzureGetNextClueByIdRequest());
        }
    }

     IEnumerator AzureGetRequest()
    {
        if(UseCurrentButton.usingCurrentLocation == true)
        {
            location = UseCurrentButton.latitude.ToString() + "," + UseCurrentButton.longitude.ToString();
        }
        else
        {
            location = ReverseGeocodeOnClick.latitude.ToString() + "," + ReverseGeocodeOnClick.longitude.ToString();
        }
        string azureUrl;
        string firstFlagParam = "&firstflag=";
        string lastClueIdParam = "&lastclueid=";
        string lastFlagParam = "&lastflag=";
        string locationParam = "&location=";
        string riddleParam = "&riddle=";

        if (firstFlag == 1 && lastFlag == 1)    // single clue hunt
        {
            azureUrl = getUrl + MainMenu.huntId + locationParam + location + riddleParam + riddle + firstFlagParam + firstFlag + lastFlagParam + lastFlag;
        }
        else if (firstFlag == 1)    // first clue in multi-clue hunt
        {
            azureUrl = getUrl + MainMenu.huntId + locationParam + location + riddleParam + riddle + firstFlagParam + firstFlag;
        }
        else if (lastFlag == 1)     // last clue in multi-clue hunt
        {
            azureUrl = getUrl + MainMenu.huntId + locationParam + location + riddleParam + riddle + lastFlagParam + lastFlag + lastClueIdParam + lastClueId;
        }
        else                        // middle clue in multi-clue hunt
        {
            azureUrl = getUrl + MainMenu.huntId + locationParam + location + riddleParam + riddle + lastClueIdParam + lastClueId;
        }
        
        Debug.Log(location);
        Debug.Log(azureUrl);
        UnityWebRequest www = UnityWebRequest.Get(azureUrl);
    
        //Form to make post request
        //List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
        //wwwForm.Add(new MultipartFormDataSection("huntid", "1"));
       // wwwForm.Add(new MultipartFormDataSection("firstflag", "1"));
        //wwwForm.Add(new MultipartFormDataSection("lastflag", "0"));
       // wwwForm.Add(new MultipartFormDataSection("lastclueid", "1"));
       // wwwForm.Add(new MultipartFormDataSection("location", "coord"));
        //wwwForm.Add(new MultipartFormDataSection("riddle", "riddle"));

        // Try this form type
        //WWWForm form = new WWWForm();
        //form.AddField("huntid", 1);
        //form.AddField("firstflag", 1);
        //form.AddField("lastflag", 0);
        //form.AddField("lastclueid", "1");
        //form.AddField("location", "coord");
        //form.AddField("riddle", "riddle");
        //UnityWebRequest www = UnityWebRequest.Post(postUrl, form);

        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            lastClueId = Convert.ToInt32(www.downloadHandler.text);
            Debug.Log(www.downloadHandler.text);
            IncrementClueNumber();
            UpdateClueNumberText();
            UpdateInputField();
        }
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
            responseBody = responseBody.Remove(responseBody.IndexOf("["), 1);
            responseBody = responseBody.Remove(responseBody.IndexOf("]"), 1);
            var clue = JsonConvert.DeserializeObject<GetPrevClueScript.Clue>(responseBody);
            clueId = clue.ClueId;
            Debug.Log(clue.Location);
            Debug.Log(clue.Riddle);
            firstFlag = clue.FirstFlag ? 1 : 0;
            lastFlag = clue.LastFlag ? 1 : 0;
            lastClueId = clue.LastClueId;
            location = clue.Location;
            riddle = clue.Riddle;

            IncrementClueNumber();
            UpdateClueNumberText();
            UpdateInputField();
        }
    }
 }