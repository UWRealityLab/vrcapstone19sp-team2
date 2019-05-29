using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SafeboxOpener : MonoBehaviour
{
    bool passcodeGood;
    public bool okToOpenDoor;
    GameObject sib;
    CircularDrive circularDrive;

    private Interactable interactable;
    void Start()
    {
        interactable = this.GetComponent<Interactable>();
        passcodeGood = false;
        circularDrive = this.GetComponent<CircularDrive>();
        //tryNumber = 1;
    }

    // Update is called once per frame
    void Update()
    {
        sib = this.gameObject.transform.parent.GetChild(1).gameObject;
        //Debug.Log(sib.name);

        passcodeGood = sib.GetComponent<SafeBoxPasscode>().pass;
        //if (passcodeGood)
        //{
        //    Destroy(GetComponent<IgnoreHovering>());
        //}
        //else if (!this.gameObject.GetComponent<IgnoreHovering>())
        //{
        //    this.gameObject.AddComponent<IgnoreHovering>();
        //}
    }

    //private void LateUpdate()
    //{
    //    float rotation = float.Parse(this.GetComponent<LinearMapping>().value.ToString("0.00"));
    //    // Debug.Log("opener rotation: " + this.gameObject.transform.localEulerAngles.z);

    //    if (this.gameObject.transform.localEulerAngles.z > 90.0f && passcodeGood)
    //    {
    //        // TODO add open sound
    //        okToOpenDoor = true;
    //    } else
    //    {
    //        okToOpenDoor = false;
    //    }
    //}

    //public void EnableOpener()
    //{
    //    Destroy(GetComponent<IgnoreHovering>());
    //}

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();

        if (passcodeGood && interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
        {
            GetComponent<Animator>().SetTrigger("open");
        } else if (!passcodeGood && interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
        {
            GetComponent<Animator>().SetTrigger("openFail");
        }
    }

    public void EnableDoorOpen()
    {
        okToOpenDoor = true;
    }
}
