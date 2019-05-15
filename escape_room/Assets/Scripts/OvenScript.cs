using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class OvenScript : MonoBehaviour
{
    public GameObject redLight;
    public GameObject greenLight;
    public GameObject key;
    public GameObject iceCubePos;
    public GameObject doorCollider;
    public AudioClip ovenFinishedSound;
    public AudioClip ovenDoorClosedSound;

    private bool cubeIn = false;
    private bool executed = false;

    // Start is called before the first frame update
    void Start()
    {
        redLight.SetActive(false);
        greenLight.SetActive(true);
        key.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CubeIn()
    {
        cubeIn = true;
    }

    public void StartOven()
    {
        StartCoroutine(OvenWait());
    }

    IEnumerator OvenWait()
    {
        // Disable door circular drive

        // To disable hovering
        AudioSource audioSource = this.GetComponent<AudioSource>();

        audioSource.clip = ovenDoorClosedSound;
        audioSource.Play();
        doorCollider.AddComponent<IgnoreHovering>();
        redLight.SetActive(true);
        greenLight.SetActive(false);
        yield return new WaitForSeconds(2);
        // To enable hovering:
        audioSource.clip = ovenFinishedSound;
        audioSource.Play();
        Destroy(doorCollider.GetComponent<IgnoreHovering>());
        redLight.SetActive(false);
        greenLight.SetActive(true);
        if (cubeIn && !executed)
        {
            key.SetActive(true);
            iceCubePos.SetActive(false);
            cubeIn = false;
            executed = true;
        }
    }
}