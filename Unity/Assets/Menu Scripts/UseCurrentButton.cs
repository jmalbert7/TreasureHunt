using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseCurrentButton : MonoBehaviour
{
    public static double latitude = 32.00005;
    public static double longitude = 33.00005;

    // variables for adding a hunt
    public Text coordinates;
    public static bool usingCurrentLocation;

    // variables for playing game
    public Toggle toggle;
    public Text coordInGame;
    public static bool usingCurLocInGame;

    public void GetGPS()
    {
        Toggle t = transform.Find("Toggle").GetComponent<Toggle>();
        if(t.isOn == true)
        {
            t.isOn = false;
            usingCurrentLocation = false;
        }
        else
        {
            t.isOn = true;
            usingCurrentLocation = true;
            StartCoroutine(StartLocationService());
        }
    }

    public void GetGPSInGame()
    {
        if(toggle.isOn == true)
        {
            toggle.isOn = false;
            usingCurLocInGame = false;
        }
        else
        {
            toggle.isOn = true;
            usingCurLocInGame = true;
            StartCoroutine(StartLocationService());
        }
    }

    private IEnumerator StartLocationService()
    {
//        coordinates.text = "Lat: " + "99.99999" + "   Long: " + "88.88888";
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser) {
            latitude = 45.001397;
            longitude = -92.16848;
            Debug.Log("User has not enabled GPS");
            yield break;
        }

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }

        // Access granted and location value could be retrieved
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();

        coordinates.text = "Lat: " + latitude.ToString() + "   Long: " + longitude.ToString();
        coordInGame.text = "Lat: " + latitude.ToString() + "   Long: " + longitude.ToString();
//        coordinates.text = "Lat: " + "99.99999" + "   Long: " + "88.88888";

        yield break;
    }
}
