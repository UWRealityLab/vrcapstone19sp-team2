using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SafeBoxOpenDoor : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (this.GetComponentInChildren<SafeboxOpener>().okToOpenDoor)
        {

            //Destroy(GetComponent<IgnoreHovering>());
            GetComponent<Animator>().SetTrigger("OpenDoor");
        }
    }

    public void DoorRotationTrigger()
    {
        Destroy(GetComponent<IgnoreHovering>());
        this.GetComponent<CircularDrive>().enabled = true;
    }

    public void DoorOpen()
    {
        GameManagerScript manager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        manager.TriggerEvent(GameManagerScript.EventTypes.SAFEBOX_OPEN, 1);
    }
}
