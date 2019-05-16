using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MusicBoxScript : MonoBehaviour
{
    public GameObject HandleTrigger;
    public GameObject SecreteDoor;
    public AudioClip SecretDoorMoveSound;

    public void MusicBoxOpenTrigger()
    {
        HandleTrigger.GetComponent<HandleTrigger>().MusicBoxOpen();
    }

    public void SecreteDoorTrigger()
    {
        AudioSource secretDoorAudioSource = SecreteDoor.GetComponent<AudioSource>();
        secretDoorAudioSource.clip = SecretDoorMoveSound;
        secretDoorAudioSource.Play();
        SecreteDoor.GetComponent<Animator>().SetTrigger("move");
    }
    
    public void EnableHandleTrigger()
    {
        AudioSource secretDoorAudioSource = SecreteDoor.GetComponent<AudioSource>();
        secretDoorAudioSource.clip = SecretDoorMoveSound;
        secretDoorAudioSource.Play();
        SecreteDoor.GetComponent<Animator>().SetTrigger("move");
        Destroy(HandleTrigger.GetComponent<IgnoreHovering>());
    }
}
