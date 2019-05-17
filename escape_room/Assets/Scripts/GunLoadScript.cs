using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class GunLoadScript : MonoBehaviour
{
    private Interactable interactable;
    public bool loaded;
    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
        loaded = false;
    }

    // Update is called once per frame
    void Update()
    {
    }


    private void HandHoverUpdate(Hand hand)
    {
         GrabTypes startingGrabType = hand.GetGrabStarting();

        if (loaded && interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
        {
            Debug.Log("Load");
            GetComponent<Animation>().Play();
            this.gameObject.AddComponent<IgnoreHovering>();
        }
    }
}
