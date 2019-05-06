using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class OvenTrigger : MonoBehaviour
{
    public GameObject iceCube;
    public GameObject iceCubePos;
    
    // Start is called before the first frame update
    void Start()
    {
        iceCubePos.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == iceCube.name)
        {
            Debug.Log("entered");
            iceCubePos.SetActive(true);
            // Detach the ice cube in hand
            iceCube.GetComponent<Interactable>().attachedToHand.DetachObject(iceCube, false);
            iceCube.SetActive(false);
            GetComponentInParent<OvenScript>().CubeIn();
        }
    }
}
