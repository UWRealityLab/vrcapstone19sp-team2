using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class HandleTrigger : MonoBehaviour
{
    private Interactable interactable;
    private bool open;

    // Start is called before the first frame update
    void Start()
    {
        interactable = this.GetComponent<Interactable>();
        open = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MusicBoxOpen()
    {
        open = true;
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();

        if (open && interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
        {
            Debug.Log("Open");
            GetComponentInParent<Animator>().SetTrigger("Play");
            this.GetComponent<AudioSource>().Play();
            this.gameObject.AddComponent<IgnoreHovering>();
        }
    }
}
