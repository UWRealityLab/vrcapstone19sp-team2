using System.Collections;
using System.Collections.Generic;
using Valve.VR.InteractionSystem;
using UnityEngine;
using UnityEngine.UI;

public class SafeBoxMonitor : MonoBehaviour
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
        if (!this.gameObject.transform.parent.parent.parent.gameObject.GetComponent<SafeboxSwitch>().isOn)
        {
            text.text = "";
            return;
        } else
        {
            text.text = "0";
        }
        //string str = value.ToString("0.0");
        //float flt = float.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
        //flt = flt * 25.0f + 85.0f;
        //string res = flt.ToString().Contains(".") ? flt.ToString() : flt.ToString() + ".0";
        string currentCode =
            FloatConversion.circularDriveValueToString(value, 3, 100f, 0f);
        int currentCodeNum = int.Parse(currentCode.Substring(0, currentCode.IndexOf(".")));
        int adjustedCode = 100 - ((int)System.Math.Round(currentCodeNum / 1.0)) * 1;
        if (adjustedCode == 100)
            adjustedCode = 0;
        text.text = adjustedCode.ToString();
        //Debug.Log("Radio Monitor done: " + res);
    }
}
