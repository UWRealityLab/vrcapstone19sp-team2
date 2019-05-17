using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MusicBoxScript : MonoBehaviour
{
    public GameObject HandleTrigger;
    public GameObject SecreteDoor;
    public AudioClip KeyInsertionSound;

    public void MusicBoxOpenTrigger()
    {
        HandleTrigger.GetComponent<HandleTrigger>().MusicBoxOpen();
    }

    public void SecreteDoorTrigger()
    {
        SecreteDoor.GetComponent<Animator>().SetTrigger("move");
    }
    
    public void EnableHandleTrigger()
    {
        SecreteDoor.GetComponent<Animator>().SetTrigger("move");
        Destroy(HandleTrigger.GetComponent<IgnoreHovering>());
    }

    public void KeyInsertionTrigger()
    {
        AudioSource audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = KeyInsertionSound;
        audioSource.Play();
    }
}
