using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeboxSwitch : MonoBehaviour
{
    public bool isOn;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void enable()
    {
        isOn = true;
    }

    public void disable()
    {
        isOn = false; 
    }
}
