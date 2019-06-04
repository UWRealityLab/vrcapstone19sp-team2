using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlashing : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject redLight, blueLight;
    public GameObject UICanvas;

    private float intensity;    
    void Start()
    {        
        intensity = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        redLight.GetComponent<Light>().intensity = Mathf.Abs(Mathf.Sin(intensity)) * 1.5f;
        blueLight.GetComponent<Light>().intensity = Mathf.Abs(Mathf.Cos(intensity)) * 1.5f;
        intensity += 0.023f;
        if(intensity >= 10000 * Mathf.PI)
        {
            intensity = 0f;
        }
    }

    public void EnableGameOverCanvas()
    {
        UICanvas.SetActive(true);
    }
    
}
