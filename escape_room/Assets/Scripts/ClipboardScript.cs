using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipboardScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void sendDropTrigger()
    {
        GameManagerScript manager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        manager.TriggerEvent(GameManagerScript.EventTypes.AFTER_CLIP_BOARD);
    }
}
