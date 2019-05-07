using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class KeyHoldTrigger : MonoBehaviour
{
    public GameObject keyPos;
    public GameObject key;

    private bool executed = false;

    // Start is called before the first frame update
    void Start()
    {
        keyPos.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!executed && other.name == key.name)
        {
            executed = true;
            Debug.Log("entered");
            keyPos.SetActive(true);
            key.GetComponent<Interactable>().attachedToHand.DetachObject(key, false);
            key.SetActive(false);
            GetComponentInParent<Animator>().SetTrigger("Open");
            this.gameObject.AddComponent<IgnoreHovering>();
        }
    }
}
