using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;


public class SafeBoxPasscode : MonoBehaviour
{
    public bool pass;
    public int code;
    CircularDrive circularDrive;

    void Start()
    {
        pass = false;
        circularDrive = this.GetComponent<CircularDrive>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.gameObject.transform.parent.parent.gameObject.GetComponent<SafeboxSwitch>().isOn)
        {
            circularDrive.limited = true;
            return;
        } else
        {
            circularDrive.limited = false;
        }
        Text text = this.gameObject.transform.parent.GetChild(4).GetComponentInChildren<Text>();
        //Debug.Log("mmp " + this.gameObject.transform.parent.GetChild(4).name);
        int adjustedCode = int.Parse(text.text);
        //Debug.Log("current code: " + adjustedCode);
        if (code == adjustedCode)
        {
            pass = true;
        } else
        {
            pass = false;
        }
        //Debug.Log("***" + pass);
    }
}
