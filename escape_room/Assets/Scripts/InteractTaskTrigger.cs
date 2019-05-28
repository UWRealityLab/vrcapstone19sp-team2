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

    protected virtual void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();

        if (startingGrabType != GrabTypes.None)
        {
            GameManagerScript manager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            string name = this.gameObject.name;
            
            if (name == "MusicBox")
            {
                manager.CompleteTask(GameManagerScript.TaskTypes.FIND_MUSIC_BOX);
            } else if (name == "IceCubeWithKey")
            {
                manager.CompleteTask(GameManagerScript.TaskTypes.KEY);
                manager.TriggerTask(GameManagerScript.TaskTypes.ICE, UIContent.UI_DELAY_SECONDS);
            } else if (name == "key")
            {
                manager.CompleteTask(GameManagerScript.TaskTypes.ICE);
            } else if (name == "flaregun")
            {
                manager.CompleteTask(GameManagerScript.TaskTypes.FIND_FLARE);
            } else if (name == "WPN_M9_Magazine_new")
            {
                manager.CompleteTask(GameManagerScript.TaskTypes.AMMO);
            } else if (name == "WPN_M9_Laser_new")
            {
                manager.TriggerTask(GameManagerScript.TaskTypes.AMMO);
            }
        }
    }
}
