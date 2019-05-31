using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathroomExitTrigger : MonoBehaviour
{
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
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "BodyCollider")
        {
            GameManagerScript manager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            manager.TriggerEvent(GameManagerScript.EventTypes.EXIT_BATHROOM);
        }
    }
}
