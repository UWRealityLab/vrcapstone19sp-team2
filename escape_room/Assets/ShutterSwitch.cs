using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ShutterSwitch : MonoBehaviour
{

    public GameObject shutter1;
    public GameObject shutter2;
    public GameObject switchObject;

    public Transform onPosition, offPosition;

    public string trigger;

    private float delta = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();

        if (startingGrabType != GrabTypes.None)
        {           
            shutter1.GetComponent<Animator>().SetTrigger(trigger);
            // shutter2.GetComponent<Animator>().SetTrigger(trigger);

            if (Mathf.Abs(switchObject.transform.localEulerAngles.x - onPosition.localEulerAngles.x) <= delta)
            {
                switchObject.transform.localEulerAngles = offPosition.localEulerAngles;
            } else if (Mathf.Abs(switchObject.transform.localEulerAngles.x - offPosition.localEulerAngles.x) <= delta)
            {
                switchObject.transform.localEulerAngles = onPosition.localEulerAngles;
            }
            this.gameObject.AddComponent<IgnoreHovering>();
        }

        
        /*
        doorCollider.AddComponent<IgnoreHovering>();
        redLight.SetActive(true);
        greenLight.SetActive(false);
        yield return new WaitForSeconds(2);
        // To enable hovering:
        audioSource.clip = ovenFinishedSound;
        audioSource.Play();
        Destroy(doorCollider.GetComponent<IgnoreHovering>());
        */
    }
}
