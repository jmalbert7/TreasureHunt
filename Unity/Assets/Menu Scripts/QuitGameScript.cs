using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGameScript : MonoBehaviour
{

    public GameObject mainMenuScreen;
    public GameObject loginScreen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Logout()
    {
        mainMenuScreen.SetActive(false);
        loginScreen.SetActive(true);
    }

}
