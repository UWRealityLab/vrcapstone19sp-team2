using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{

    public bool lightOn = false;

    // public GameObject lamp1, lamp2, lamp3;

    public GameObject[] allLights;

    public GameObject switchA, switchB, switchC;

    public string lightTagName;

    public Transform AStart, AEnd, BStart, BEnd, CStart, CEnd;

    public float delta = 0.001f;

    private void Start()
    {
        allLights = GameObject.FindGameObjectsWithTag(lightTagName);
        foreach (GameObject l in allLights)
        {
            l.GetComponent<Light>().enabled = lightOn;
        }
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
        }

        bool AAtStart = AStart.transform.localPosition.y - switchA.transform.localPosition.y <= delta;
        bool BAtStart = BStart.transform.localPosition.y - switchB.transform.localPosition.y <= delta;
        bool CAtStart = CStart.transform.localPosition.y - switchC.transform.localPosition.y <= delta;

        if (AAtStart && BAtStart && CAtStart)
        {
            lightOn = true;
           
            foreach (GameObject l in allLights)
            {
                l.GetComponent<Light>().enabled = lightOn;
            }
        }
    }
}
