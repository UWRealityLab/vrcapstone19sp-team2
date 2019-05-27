using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class CurtainScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject shutterSwitch;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableSwitch()
    {
        Destroy(shutterSwitch.GetComponent<IgnoreHovering>());
    }
}
