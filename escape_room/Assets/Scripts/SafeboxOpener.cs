using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SafeboxOpener : MonoBehaviour
{
    bool passcodeGood;
    public bool okToOpenDoor;

    public AudioClip openFailSound;
    public AudioClip openSound;
    GameObject sib;
    private AudioSource audioSource;

    private Interactable interactable;
    void Start()
    {
        interactable = this.GetComponent<Interactable>();
        passcodeGood = false;
        audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        sib = this.gameObject.transform.parent.GetChild(1).gameObject;
        passcodeGood = sib.GetComponent<SafeBoxPasscode>().pass;
    }


    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();

        if (passcodeGood && interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
        {
            GetComponent<Animator>().SetTrigger("open");
            audioSource.clip = openSound;
        } else if (!passcodeGood && interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
        {
            audioSource.clip = openFailSound;
            GetComponent<Animator>().SetTrigger("openFail");
        }
    }

    public void EnableDoorOpen()
    {
        okToOpenDoor = true;
    }

    public void PlayOpenOrFailSound()
    {
        if (audioSource.clip != null)
            audioSource.Play();
    }
}
