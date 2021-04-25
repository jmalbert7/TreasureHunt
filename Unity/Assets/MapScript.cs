using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapScript : MonoBehaviour
{
    /*
    string url = "";    // post request to Google Maps
    public float lat = 24.917828f;
    public float lon = 67.097096f;
    LocationInfo li;
    public int zoom = 14;
    public int mapWidth = 640;
    public int mapHeight = 640;
    public enum mapType { roadmap, satellite, hybrid, terrain };
    public mapType mapSelected;
    public int scale;

    private bool loadingMap = false;

    private IEnumerator mapCoroutine;

    IEnumerator GetGoogleMap(float lat, float lon)
    {
        url = "https://";
        loadingMap = true;
        WWW www = new WWW(url);
        yield return www;
        loadingMap = false;
        // Assign downloaded map texture to Canvas Image
        gameObject.GetComponent<RawImage>().texture = www.texture;
        StopCoroutine(mapCoroutine);
    }

    // Start is called before the first frame update
    void Start()
    {
        mapCoroutine = GetGoogleMap(lat, lon);
        StartCoroutine(mapCoroutine);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("new map");
            lat = 40.6786806f;
            lon = -073.8644250f;
            mapCoroutine = GetGoogleMap(lat, lon);
            StartCoroutine(mapCoroutine);
        } 
    }
    */
}
