using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{

    public bool lightOn = false;

    public GameObject lamp1, lamp2, lamp3;

    private void Start()
    {
        lamp1.GetComponentInChildren<Light>().enabled = lightOn;
        lamp2.GetComponentInChildren<Light>().enabled = lightOn;
        lamp3.GetComponentInChildren<Light>().enabled = lightOn;
    }

    public void switchLight()
    {
        lightOn = !lightOn;
        lamp1.GetComponentInChildren<Light>().enabled = lightOn;
        lamp2.GetComponentInChildren<Light>().enabled = lightOn;
        lamp3.GetComponentInChildren<Light>().enabled = lightOn;
    }
}
