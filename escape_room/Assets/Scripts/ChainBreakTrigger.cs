using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class ChainBreakTrigger : MonoBehaviour
{
    public GameObject Cutter;
    public ChainScript ChainScript;

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
        if (other.name == Cutter.name)
        {
            Debug.Log("cutted");
            Destroy(this.GetComponent<CharacterJoint>());
            ChainScript.Break();
        }
    }
}
