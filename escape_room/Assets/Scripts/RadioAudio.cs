using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadioAudio : MonoBehaviour
{
    public AudioClip audio1;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = audio1;   
    }

    // Update is called once per frame
    void Update()
    {
        Text text = this.GetComponent<Text>();
        if (text != null && text.text != null)
        {
            if (text.text.Length != 0)
            {

            }
        }
    }
}
