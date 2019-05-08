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
    //public int tryNumber;
    //public List<bool> matches;
    // Start is called before the first frame update
    void Start()
    {
        passcodeGood = false;
        circularDrive = this.GetComponent<CircularDrive>();
        //tryNumber = 1;
        //matches = new List<bool> { false, false, false };
    }

    // Update is called once per frame
    void Update()
    {
        sib = this.gameObject.transform.parent.GetChild(1).gameObject;
        Debug.Log(sib.name);

        //List<bool> sibMatches = sib.GetComponent<SafeBoxPasscode>().matches;
        //SafeBoxPasscode passcodeEle = sib.GetComponent<SafeBoxPasscode>();

        //matches[tryNumber - 1] = sibMatches[tryNumber - 1];

        passcodeGood = sib.GetComponent<SafeBoxPasscode>().pass;
        //float maxAngle = 0.0f;
        //if (sibMatches[tryNumber - 1])
        //{
        //    if (tryNumber == 1)
        //    {
        //        circularDrive.freezeOnMin = false;
        //        circularDrive.freezeOnMax = true;
        //    } else if (tryNumber == 2)
        //    {
        //        circularDrive.freezeOnMin = true;
        //        circularDrive.freezeOnMax = false;
        //    }
        //}
        if (passcodeGood)
        {
            circularDrive.limited = false;
        }
        else
        {
            if (this.gameObject.transform.localEulerAngles.z <= 100.0f)
                circularDrive.limited = true;
        }

    }

    private void LateUpdate()
    {
        //Debug.Log(this.GetComponent<LinearMapping>().value + " is max value");
        float rotation = float.Parse(this.GetComponent<LinearMapping>().value.ToString("0.00"));
        //if (!okToOpenDoor)
        //    if (rotation > 0.3f * tryNumber)
        //{
        //    matches[tryNumber++ - 1] = true;
        //    if (tryNumber > matches.Count)
        //    {
        //        okToOpenDoor = true;
        //    }
        //}
        //Debug.Log("opener rotation: " + rotation);
        //if (!okToOpenDoor)
        //{
        //    if (tryNumber == 1 && rotation == 1)
        //    {
        //        tryNumber = 2;
        //        matches[0] = true;
        //    } else if (tryNumber == 2 && rotation == 1 )
        //    {
        //        tryNumber = 3;
        //        matches[1] = true;
        //    } else if (tryNumber == 3 && rotation == 1)
        //    {
        //        okToOpenDoor = true;
        //    }
        //}
        Debug.Log("opener rotation: " + this.gameObject.transform.localEulerAngles.z);

        if (this.gameObject.transform.localEulerAngles.z > 100.0f && passcodeGood)
        {
            okToOpenDoor = true;
        } else
        {
            okToOpenDoor = false;
        }
    }
}
