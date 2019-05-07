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
    // Start is called before the first frame update
    void Start()
    {
        passcodeGood = false;
        circularDrive = this.GetComponent<CircularDrive>();
    }

    // Update is called once per frame
    void Update()
    {
        sib = this.gameObject.transform.parent.GetChild(1).gameObject;
        Debug.Log(sib.name);
        passcodeGood = sib.GetComponent<SafeBoxPasscode>().pass;
        if (passcodeGood)
        {
            circularDrive.minAngle = 0.0f;
            circularDrive.maxAngle = 300.0f;
        }
    }

    private void LateUpdate()
    {
        //Debug.Log(this.GetComponent<LinearMapping>().value + " is max value");
        float rotation = float.Parse(this.GetComponent<LinearMapping>().value.ToString("0.00"));
        if (!okToOpenDoor && rotation > 0.90)
        {
            okToOpenDoor = true;
        }
    }
}
