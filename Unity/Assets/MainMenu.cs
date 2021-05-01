using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static int UserId = 2;
    public static string HuntName;
    public static string GeneralLocation;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    // Get the user's UserId, HuntName, and GeneralLocation for this hunt
    public void AddHunt()
    {
        // Use Louis's UserId for now
        UserId = 2;
        HuntName = "The Great Madison Hunt";
        GeneralLocation = "Madison, WI";
    }

    public void GoToFindLocation()
    {
        SceneManager.LoadScene(2);
    }
}
