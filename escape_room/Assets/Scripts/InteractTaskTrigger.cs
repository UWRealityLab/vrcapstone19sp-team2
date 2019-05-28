using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class InteractTaskTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    protected virtual void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();

        if (startingGrabType != GrabTypes.None)
        {
            GameManagerScript manager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            string name = this.gameObject.name;
            
            if (name == "MusicBox" || name == "HandleTrigger" || name == "KeyHole")
            {
                manager.CompleteTask(GameManagerScript.TaskTypes.FIND_MUSIC_BOX);
                manager.TriggerEvent(GameManagerScript.EventTypes.MUSIC_BOX_TOUCHED);
                manager.TriggerTask(GameManagerScript.TaskTypes.KEY_HINT, GameManagerScript.EventTypes.MUSIC_BOX_TOUCHED);
            } else if (name == "IceCubeWithKey")
            {
                manager.CompleteTask(GameManagerScript.TaskTypes.KEY);
                manager.TriggerEvent(GameManagerScript.EventTypes.ICE_CUBE_TOUCHED);
                manager.TriggerTask(GameManagerScript.TaskTypes.ICE, GameManagerScript.EventTypes.ICE_CUBE_TOUCHED);
            } else if (name == "key")
            {
                manager.CompleteTask(GameManagerScript.TaskTypes.ICE);
                manager.TriggerEvent(GameManagerScript.EventTypes.PICKED_UP_KEY);
            } else if (name == "flaregun")
            {
                manager.CompleteTask(GameManagerScript.TaskTypes.FIND_FLARE);
                manager.TriggerTask(GameManagerScript.TaskTypes.CURTAIN);
            }
            else if (name == "WPN_M9_Magazine_new")
            {
                manager.CompleteTask(GameManagerScript.TaskTypes.AMMO);
            } else if (name == "WPN_M9_Laser_new")
            {
                manager.TriggerEvent(GameManagerScript.EventTypes.PICKED_UP_GUN);
                manager.TriggerTask(GameManagerScript.TaskTypes.AMMO, GameManagerScript.EventTypes.PICKED_UP_GUN);
            } else if (name == "fusebox_cover")
            {
                manager.CompleteTask(GameManagerScript.TaskTypes.FIND_FUSE);
            } else if (name == "Clipboard")
            {
                manager.CompleteTask(GameManagerScript.TaskTypes.KEY_HINT);
                manager.TriggerEvent(GameManagerScript.EventTypes.AFTER_CLIP_BOARD);
                manager.TriggerTask(GameManagerScript.TaskTypes.KEY, GameManagerScript.EventTypes.AFTER_CLIP_BOARD);
            } else if (name == "Flashlight")
            {
                manager.TriggerEvent(GameManagerScript.EventTypes.AFTER_FLASHLIGHT);
            } else if (name == "Main Book")
            {
                manager.TriggerEvent(GameManagerScript.EventTypes.AFTER_PICK_UP_DIARY);
            }
        }
    }
}
