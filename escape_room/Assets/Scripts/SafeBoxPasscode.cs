using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;


public class SafeBoxPasscode : MonoBehaviour
{
    public bool pass;
    //public List<bool> matches;
    //private List<int> passcode;
    int code = 66;
    // Start is called before the first frame update
    void Start()
    {
        pass = false;
        //passcode = new List<int> { 10, 20, 30 };
        //matches = new List<bool> { false, false, false };
    }

    // Update is called once per frame
    void Update()
    {
        //float rawValue = this.GetComponent<LinearMapping>().value;
        //string currentCode =
        //    FloatConversion.circularDriveValueToString(rawValue, 3, 100f, 0f);
        //int currentCodeNum = int.Parse(currentCode.Substring(0, currentCode.IndexOf(".")));
        //int adjustedCode = 100 - ((int)System.Math.Round(currentCodeNum / 1.0)) * 1;
        //Debug.Log(adjustedCode + " adjusted code");

        //SafeboxOpener safeboxOpener = this.gameObject.transform.parent.GetChild(0).gameObject.GetComponent<SafeboxOpener>();
        //int currentTry = safeboxOpener.tryNumber;
        //int index = currentTry - 1;
        //Debug.Log("index: " + index);
        //if (index < passcode.Count && (index == 0 || (matches[index - 1] && safeboxOpener.matches[index - 1])))
        //{
        //    if (adjustedCode == passcode[index])
        //    {
        //        matches[index] = true;
        //    }
        //}
        //Text text = this.get
        Text text = this.gameObject.transform.parent.GetChild(4).GetComponentInChildren<Text>();
        Debug.Log("mmp " + this.gameObject.transform.parent.GetChild(4).name);
        int adjustedCode = int.Parse(text.text);
        Debug.Log("current code: " + adjustedCode);
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
