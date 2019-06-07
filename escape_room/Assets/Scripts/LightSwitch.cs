using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class LightSwitch : MonoBehaviour
{
    public Transform startPosition;
    public Transform endPosition;
    public AudioClip switchSound;

    private bool on = false;

    protected virtual void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();

        if (startingGrabType != GrabTypes.None)
        {
            on = !on;
            playSound();
            this.gameObject.transform.localPosition = on ? startPosition.localPosition : endPosition.localPosition;
        }
    }

    private void playSound()
    {
        AudioSource audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = switchSound;
        audioSource.Play();
    }
}
