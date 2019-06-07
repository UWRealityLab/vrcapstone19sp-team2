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

        // trigger
        GameManagerScript manager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        manager.TriggerEvent(GameManagerScript.EventTypes.MUSIC_BOX_KEY_INSERTED);
        ShowTriggerHint(GameManagerScript.getAudioLength(GameManagerScript.EventTypes.MUSIC_BOX_KEY_INSERTED));
    }

    public void SecreteDoorTrigger()
    {
        SecreteDoor.GetComponent<Animator>().SetTrigger("open");
    }
    
    public void EnableHandleTrigger()
    {
        SecreteDoor.GetComponent<Animator>().SetTrigger("close");
        Destroy(HandleTrigger.GetComponent<IgnoreHovering>());
    }

    public void KeyInsertionTrigger()
    {
        AudioSource audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = KeyInsertionSound;
        audioSource.Play();
    }

    public void ShowTriggerHint(float delay)
    {
        StartCoroutine(HintWait(delay));
    }

    IEnumerator HintWait(float delay)
    {
        yield return new WaitForSeconds(delay);
        HandleTrigger.GetComponent<Outline>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        HandleTrigger.GetComponent<Outline>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        HandleTrigger.GetComponent<Outline>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        HandleTrigger.GetComponent<Outline>().enabled = false;
    }
}
