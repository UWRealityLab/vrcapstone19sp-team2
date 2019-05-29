using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretRoomEnterTrigger : MonoBehaviour
{
    public bool entered = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "BodyCollider")
        {
            entered = true;

            // trigger
            GameManagerScript manager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            manager.TriggerEvent(GameManagerScript.EventTypes.ENTERED_SECRET_ROOM);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "BodyCollider")
        {
            entered = false;
        }
    }
}
