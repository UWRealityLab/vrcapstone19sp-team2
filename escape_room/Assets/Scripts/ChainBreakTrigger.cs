using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ChainBreakTrigger : MonoBehaviour
{
    public GameObject CutterTrigger;
    public CutterScript CutterScript;

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
        if (other.name == CutterTrigger.name)
        {
            CutterScript.ChainContacted = this.gameObject;
        }
    }
}
