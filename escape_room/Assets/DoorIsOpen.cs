using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorIsOpen : MonoBehaviour
{
    public Transform open;

    private SecretRoomEnterTrigger enterTrigger;
    public SecretDoorSwitch doorSwitch;

    // Start is called before the first frame update
    void Start()
    {
        enterTrigger = GameObject.Find("EnterCollider").GetComponent<SecretRoomEnterTrigger>();
    }

    // Update is called once per frame
    public void doorOpen() {
        //Debug.Log("Door is at open position");
        //this.GetComponent<AudioSource>().enabled = false;

        // Trigger
        GameManagerScript manager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        manager.TriggerEvent(GameManagerScript.EventTypes.SECRETE_DOOR_OPEN);
    }

    public void doorClose()
    {
        if (enterTrigger.entered)
        {
            GameManagerScript manager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            manager.TriggerEvent(GameManagerScript.EventTypes.DOOR_CLOSED_WHILE_IN);
        }
        doorSwitch.enable();
    }

    public void PlaySound()
    {
        this.GetComponent<AudioSource>().Play();
    }
}
