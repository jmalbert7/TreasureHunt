using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseCurrentButton : MonoBehaviour
{
    public void CheckBox()
    {
        Toggle t = transform.Find("Toggle").GetComponent<Toggle>();
        if(t.isOn == true)
        {
            t.isOn = false;
        }
        else
        {
            t.isOn = true;
        }
    }
}
