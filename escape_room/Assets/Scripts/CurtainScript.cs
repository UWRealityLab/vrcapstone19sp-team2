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
        // Trigger
        GameManagerScript manager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        manager.CompleteTask(GameManagerScript.TaskTypes.CURTAIN);
        manager.TriggerEvent(GameManagerScript.EventTypes.CURTAIN_OPEN);
    }
}
