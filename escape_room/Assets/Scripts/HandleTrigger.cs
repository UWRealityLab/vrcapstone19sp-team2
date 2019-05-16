using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class HandleTrigger : MonoBehaviour
{
    public AudioClip musicBoxSound;
    //public GameObject secretDoor;
    //public AudioClip secretDoorOpenSound;

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
            //Debug.Log("Open");
            AudioSource audioSource = this.GetComponent<AudioSource>();
            audioSource.clip = musicBoxSound;
            audioSource.Play();
            
            // Need to find where triggers door open
            //AudioSource secretDoorAudioSource = secretDoor.GetComponent<AudioSource>();
            //secretDoorAudioSource.clip = secretDoorOpenSound;
            //secretDoorAudioSource.Play();

            GetComponentInParent<Animator>().SetTrigger("Play");
            this.gameObject.AddComponent<IgnoreHovering>();
        }
    }
}
