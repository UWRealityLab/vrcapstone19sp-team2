using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SecretDoorSwitch : MonoBehaviour
{
    public Animator ScreteDoor;
    private bool enabled = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open()
    {
        if (enabled)
        {
            disable();
            StartCoroutine(OpenWait());
        }
    }

    IEnumerator OpenWait()
    {
        // Disable door circular 
        // To disable hovering
        ScreteDoor.SetTrigger("open");
        yield return new WaitForSeconds(5);
        // To enable hovering:
        ScreteDoor.SetTrigger("close");
    }

    public void enable()
    {
        enabled = true;
    }

    public void disable()
    {
        enabled = false;
    }
}
