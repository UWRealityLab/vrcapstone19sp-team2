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
    void Start()
    {
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
        if (passcodeGood)
        {
            circularDrive.limited = false;
        }
        else
        {
            float currentRotation = this.gameObject.transform.localEulerAngles.z;
            circularDrive.minAngle = currentRotation;
            circularDrive.maxAngle = currentRotation + 1.0f;
            circularDrive.limited = true;
        }

    }

    private void LateUpdate()
    {
        float rotation = float.Parse(this.GetComponent<LinearMapping>().value.ToString("0.00"));
        //Debug.Log("opener rotation: " + this.gameObject.transform.localEulerAngles.z);

        if (this.gameObject.transform.localEulerAngles.z > 280.0f && passcodeGood)
        {
            okToOpenDoor = true;
        } else
        {
            okToOpenDoor = false;
        }
    }
}
