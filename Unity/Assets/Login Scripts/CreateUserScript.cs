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

public class CreateUserScript : MonoBehaviour
{
    // Input fields for username and password
    public InputField username;
    public InputField password;
    public InputField password2;
    public Text passwordErrorText;
    public Button createAccountButton;

    // Post url and strings to save username/password
    readonly string postUrl = "https://functionapplicationgroupx.azurewebsites.net/api/users/create/?username=";
    private string myUsername;
    private string myPassword;
    private string myPassword2;

    private void Start()
    {
        // disable CreateAccount button
        createAccountButton.interactable = false;

        // Event listeners to get username/password data from user
        username.onEndEdit.AddListener(GetUsername);
        password.onEndEdit.AddListener(GetPassword);
        password2.onEndEdit.AddListener(GetPassword2);
    }

    private void GetUsername(string input)
    {
        myUsername = input;
    }

    private void GetPassword(string input)
    {
        myPassword = input;
    }

    private void GetPassword2(string input)
    {
        myPassword2 = input;

        // Check that passwords match
        if(myPassword == myPassword2)
        {
            // disable PasswordErrorText
            passwordErrorText.text = "";

            if (ValidatePassword(myPassword))
            {
                // enable CreateAccount button
                createAccountButton.interactable = true;
            }
        }
        else
        {
            // enable PasswordErrorText
            passwordErrorText.text = "Passwords must match!";

            // disable CreateAccount button
            createAccountButton.interactable = false;
        }
    }

    private bool ValidatePassword(string pw)
    {
        if(pw.Length > 5)
        {
            return true;
        }
        else { return false; }
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
        Debug.Log(HashPassword(myPassword));
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
            Debug.LogError(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

        }
    }
}
