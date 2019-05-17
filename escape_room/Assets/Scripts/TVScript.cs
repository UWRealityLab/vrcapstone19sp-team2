using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVScript : MonoBehaviour
{
    public UnityEngine.Video.VideoClip videoClip;
    public UnityEngine.Video.VideoPlayer videoPlayer;

    void Start()
    {
        var audioSource = gameObject.AddComponent<AudioSource>();

        videoPlayer.playOnAwake = false;
        videoPlayer.clip = videoClip;
        videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.MaterialOverride;
        videoPlayer.targetMaterialRenderer = GetComponent<Renderer>();
        videoPlayer.targetMaterialProperty = "_MainTex";
        videoPlayer.audioOutputMode = UnityEngine.Video.VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);
        videoPlayer.Play();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
            }
            else
            {
                videoPlayer.Play();
            }
        }
    }
}
