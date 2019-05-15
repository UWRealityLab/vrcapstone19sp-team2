using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{

    public bool lightOn = false;
    public AudioClip buttonSound;

    // public GameObject lamp1, lamp2, lamp3;

    private GameObject[] allLights;
    private GameObject[] allEmissives;
    private GameObject[] LandE;

    public GameObject switchA, switchB, switchC;
    public GameObject lightMapController;

    public string lightTagName;
    public string emissiveTagName;

    public Transform AStart, AEnd, BStart, BEnd, CStart, CEnd;

    public float delta = 0.001f;

    private void Start()
    {
        allLights = GameObject.FindGameObjectsWithTag(lightTagName);
        allEmissives = GameObject.FindGameObjectsWithTag(emissiveTagName);
        LandE = GameObject.FindGameObjectsWithTag("LandE");

        if (!lightOn)
        {
            turnOff();
        }
    }

    private void turnOff()
    {
        foreach (GameObject l in allLights)
        {
            l.GetComponent<Light>().enabled = lightOn;
        }
        foreach (GameObject l in allEmissives)
        {
            l.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }
        foreach (GameObject l in LandE)
        {
            l.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
            l.GetComponent<Light>().enabled = lightOn;
        }
        lightMapController.GetComponent<LightMapSwitcher>().SwapLightmaps(1);
    }

    public void switchLight()
    {
        bool AAtEnd = switchA.transform.localPosition.y - AEnd.localPosition.y <= delta;
        bool BAtEnd = switchB.transform.localPosition.y - BEnd.localPosition.y <= delta;
        bool CAtEnd = switchC.transform.localPosition.y - CEnd.localPosition.y <= delta;
               
        if (AAtEnd && BAtEnd && CAtEnd)
        {
            lightOn = false;
            foreach (GameObject l in allLights)
            {
                l.GetComponent<Light>().enabled = lightOn;
            }
            foreach (GameObject l in allEmissives)
            {
                l.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
            }
            foreach (GameObject l in LandE)
            {
                l.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                l.GetComponent<Light>().enabled = lightOn;
            }
            lightMapController.GetComponent<LightMapSwitcher>().SwapLightmaps(1);
        }

        bool AAtStart = AStart.transform.localPosition.y - switchA.transform.localPosition.y <= delta;
        bool BAtStart = BStart.transform.localPosition.y - switchB.transform.localPosition.y <= delta;
        bool CAtStart = CStart.transform.localPosition.y - switchC.transform.localPosition.y <= delta;

        if (AAtStart && BAtStart && CAtStart)
        {
            lightOn = true;
            AudioSource audioSource = this.GetComponent<AudioSource>();
            audioSource.clip = buttonSound;
            audioSource.Play();
            foreach (GameObject l in allLights)
            {
                l.GetComponent<Light>().enabled = lightOn;
            }
            foreach (GameObject l in allEmissives)
            {
                l.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            }
            foreach (GameObject l in LandE)
            {
                l.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                l.GetComponent<Light>().enabled = lightOn;
            }
            lightMapController.GetComponent<LightMapSwitcher>().SwapLightmaps(0);
        }
    }
}
