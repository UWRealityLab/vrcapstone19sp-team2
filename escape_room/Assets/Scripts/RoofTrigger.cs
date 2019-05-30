using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofTrigger : MonoBehaviour
{
    public Animator Helicopter;
    private bool executed = false;
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
        // Debug.Log(other.name);

        if (!executed && other.name == "flarebullet(Clone)")
        {
            executed = true;
            Helicopter.SetTrigger("Fly");
            this.gameObject.SetActive(false);

            // trigger
            GameManagerScript manager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            manager.TriggerEvent(GameManagerScript.EventTypes.FLARE_GUN_FIRED);
        }
    }
}
