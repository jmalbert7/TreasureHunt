using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;

public class LoginScript : MonoBehaviour
{
    // Input fields for username and password
    public InputField username;
    public InputField password;
    public TextMeshProUGUI errorText;
    public GameObject mainMenuScreen;
    public GameObject loginScreen;

    // Post url and strings to save username/password
    readonly string postUrl = "https://functionapplicationgroupx.azurewebsites.net/api/users/validate?username=";
    private string myUsername;
    private string myPassword;
    public static int gameId;
    public static int userId;
    public static int clueId;


    private void Start()
    {
        // Event listeners to get username/password data from user
        username.onEndEdit.AddListener(GetUsername);
        password.onEndEdit.AddListener(GetPassword);
        errorText = GameObject.Find("ErrorText").GetComponent<TextMeshProUGUI>();
    }

    public class Game
    {
        public int GameId { get; set; }
        public int UserId { get; set; }
        public string ClueId { get; set; }
    }

    private void GetUsername(string input)
    {
        myUsername = input;
    }

    private void GetPassword(string input)
    {
        myPassword = input;
    }

    // Function to hash user's password
    public string HashPassword(string pass)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(pass));
        byte[] result = md5.Hash;
        StringBuilder str = new StringBuilder();
        for (int i = 0; i < result.Length; i++)
        {
            str.Append(result[i].ToString("x2"));
        }
        return str.ToString();
    }

    public void OnButtonCallAzureFunction()
    {
        StartCoroutine(AzureGetRequest());
    }

    public void Display()
    {
        Debug.Log(myUsername);
        Debug.Log(myPassword);
    }

    IEnumerator AzureGetRequest()
    {
        // Parameter to pass into query string
        string passwordParam = "&password=";

        // Hash password
        myPassword = HashPassword(myPassword);

        // Create query string and send request
        string azureUrl = postUrl + myUsername + passwordParam + myPassword;
        Debug.Log(azureUrl);

        UnityWebRequest www = UnityWebRequest.Get(azureUrl);

        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Invalid username or password");
            errorText.text = "Invalid Username/password. Please try again";
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            string responseBody = www.downloadHandler.text;
            var game = JsonConvert.DeserializeObject<Game>(responseBody);
            gameId = game.GameId;
            userId = game.UserId;
            if (!String.IsNullOrEmpty(game.ClueId))
            {
                clueId = Convert.ToInt32(game.ClueId);
            }
            Debug.Log("GameId, UserId, ClueId: ");
            Debug.Log(gameId);
            Debug.Log(userId);
            Debug.Log(clueId);
            loginScreen.SetActive(false);
            mainMenuScreen.SetActive(true);

        }
    }
}