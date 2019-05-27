using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class FlashLightGrabbed : MonoBehaviour
{
    public bool passed;

    private Interactable interactable;
    // Start is called before the first frame update
    void Start()
    {
        passed = false;
        interactable = this.GetComponent<Interactable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (interactable.attachedToHand != null && startingGrabType != GrabTypes.None)
        {
            passed = true;
        }
    }
}
