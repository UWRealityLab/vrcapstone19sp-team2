using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorIsOpen : MonoBehaviour
{
    public Transform open;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void doorOpen() {
        //Debug.Log("Door is at open position");
        this.GetComponent<AudioSource>().enabled = false;

        // Trigger
        GameManagerScript manager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        manager.TriggerEvent(GameManagerScript.EventTypes.SECRETE_DOOR_OPEN);
    }
}
