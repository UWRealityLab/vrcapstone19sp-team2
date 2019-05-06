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
        Debug.Log(value);
        if (text == null)
            text = this.GetComponent<Text>();
        if (text == null)
            return;
        if (text.text == null)
            text.text = "";
        string str = value.ToString("0.0");
        float flt = float.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
        flt = flt * 25.0f + 85.0f;
        string res = flt.ToString().Contains(".") ? flt.ToString() : flt.ToString() + ".0";
        text.text = res;
        //Debug.Log("Radio Monitor done: " + res);
    }
}
