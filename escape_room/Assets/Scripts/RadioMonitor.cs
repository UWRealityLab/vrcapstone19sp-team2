using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System.Globalization;
using UnityEngine.UI;

public class RadioMonitor : MonoBehaviour
{
    Text text;
    // Update is called once per frame
    void Update()
    {
        LinearMapping mapping = this.GetComponent<LinearMapping>();
        float value = mapping.value;
        //Debug.Log(value);
        if (text == null)
            text = this.GetComponent<Text>();
        if (text == null)
            return;
        if (text.text == null)
            text.text = "";
        string res = FloatConversion.circularDriveValueToString(value, 1, 25.0f, 85.0f);
        if (res.Equals("110.0"))
            res = "85.0";
        text.text = res;
    }
}
