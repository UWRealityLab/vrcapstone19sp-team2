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
    public GameObject screen;
    public GameObject lightMapController;
    private bool freelyOff = false;

    public string lightTagName;
    public string emissiveTagName;

    public float delta = 0.001f;
    public bool passed;

    public GameManagerScript manager;

    private void Start()
    {
        allLights = GameObject.FindGameObjectsWithTag(lightTagName);
        allEmissives = GameObject.FindGameObjectsWithTag(emissiveTagName);
        LandE = GameObject.FindGameObjectsWithTag("LandE");

        if (!lightOn)
        {
            turnOff();
        }
        passed = false;
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
        screen.SetActive(false);
    }

    public void switchLight()
    {
        if (!switchB.GetComponent<LightSwitch>().on)
        {

            lightOn = false;
            freelyOff = false;
            AudioSource audioSource = this.GetComponent<AudioSource>();
            audioSource.clip = buttonSound;
            audioSource.Play();

            freelyOff = false;
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

            screen.SetActive(lightOn);
        }
        else
        {
            //Debug.Log("gtmdsb");
            passed = true;
            lightOn = true;
            freelyOff = true;
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
            screen.SetActive(true);

            // Trigger
            manager.CompleteTask(GameManagerScript.TaskTypes.LIGHT);
            manager.TriggerEvent(GameManagerScript.EventTypes.AFTER_LIGHT_ON);
        }
    }
}
