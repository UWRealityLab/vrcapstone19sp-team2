using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RadioAudio : MonoBehaviour
{
    public AudioClip audio1;
    public AudioClip audio2;
    public AudioClip audio3;
    public AudioSource audioSource;
    Text text;
    public string currentChannel;

    // Update is called once per frame
    void Update()
    {
        Text text = this.GetComponent<Text>();
        //Debug.Log("entering audio");
        //Debug.Log(text);
        //Debug.Log(text.text);
        if (text != null && text.text != null)
        {
            if (text.text.Equals("85.0"))
            {
                if (audioSource.clip == null || !audioSource.clip.Equals(audio1))
                {
                    audioSource.clip = audio1;
                    audioSource.Play();
                    currentChannel = text.text;
                }
            } else if (text.text.Equals("87.5"))
            {
                if (audioSource.clip == null || !audioSource.clip.Equals(audio2))
                {
                    audioSource.clip = audio2;
                    audioSource.Play();
                    currentChannel = text.text;
                }
            } else if (text.text.Equals("90.0"))
            {
                if (audioSource.clip == null || !audioSource.clip.Equals(audio3))
                {
                    audioSource.clip = audio3;
                    audioSource.Play();
                    currentChannel = text.text;
                }
            }
        }
    }


}
