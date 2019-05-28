using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class RadioAudio : MonoBehaviour
{
    public AudioClip audio1;
    public AudioClip audio2;
    public AudioClip audio3;
    public AudioSource audioSource;
    Text text;

    public CircularDrive knob;
    public RadioMonitor monitor;
    public GameManagerScript manager;

    // Update is called once per frame
    void Update()
    {
        Text text = this.GetComponent<Text>();

        if (text != null && text.text != null)
        {

            if (text.text.Equals("87.5"))
            {
                if (audioSource.clip == null || !audioSource.clip.Equals(audio2))
                {
                    audioSource.clip = audio2;
                    audioSource.Play();
                }
            } else if (text.text.Equals("90.0"))
            {
                if (audioSource.clip == null || !audioSource.clip.Equals(audio3))
                {
                    audioSource.clip = audio3;
                    audioSource.Play();

                    knob.rotateGameObject = false;
                    monitor.Freeze = true;
                    StartCoroutine(Wait());
                }
            } else
            {
                if (audioSource.clip == null || !audioSource.clip.Equals(audio1))
                {
                    audioSource.clip = audio1;
                    audioSource.Play();
                }
            }
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(5);
        knob.rotateGameObject = true;
        monitor.Freeze = false;
        manager.CompleteTask(GameManagerScript.TaskTypes.RADIO);
        manager.TriggerTask(GameManagerScript.TaskTypes.DESK, GameManagerScript.EventTypes.AFTER_RADIO_MILITARY);
        manager.TriggerEvent(GameManagerScript.EventTypes.AFTER_RADIO_MILITARY);
    }
}
