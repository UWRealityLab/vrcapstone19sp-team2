using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintsAndNarrativeScript : MonoBehaviour
{
    public GameObject HeadUI;
    public GameObject WatchUI;

    public int NumberOfPuzzles;

    public GameObject cutter;
    public string cutterHeadText;
    public string cutterWatchText;

    public GameObject flashLight;
    public string flashLightHeadText;
    public string flashLightWatchText;

    public GameObject fuseBoxButton;
    public string fuseBoxButtonHeadText;
    public string fuseBoxButtonWatchText;

    public GameObject radioKnob;
    public string radioAtSecondChannelHeadText;
    public string radioAtSecondChannelWatchText;
    public string radioAtThirdChannelHeadText;
    public string radioAtThirdChannelWatchText;

    public GameObject iceBox;
    public string iceBoxIsGrabbedInstruction = "";
    // Start is called before the first frame update

    private Text HeadHints;
    private Text WatchHints;

    void Start()
    {
        HeadHints = HeadUI.GetComponentInChildren<Text>();
        WatchHints = WatchUI.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cutter.GetComponent<CutterScript>().passed)
        {
            //HeadHints.text = cutterHeadText;
            WatchHints.text = cutterWatchText;
            StartCoroutine(HeadHintsWait(cutterHeadText));
            cutter.GetComponent<CutterScript>().passed = false;
        }

        if (flashLight.GetComponent<FlashLightGrabbed>().passed)
        {
            StartCoroutine(HeadHintsWait(flashLightHeadText));
            WatchHints.text = flashLightWatchText;
            flashLight.GetComponent<FlashLightGrabbed>().passed = false;
        }

        if (fuseBoxButton.GetComponent<LightControl>().passed)
        {
            StartCoroutine(HeadHintsWait(fuseBoxButtonHeadText));
            WatchHints.text = fuseBoxButtonWatchText;
            fuseBoxButton.GetComponent<LightControl>().passed = false;
        }

        if (radioKnob.GetComponent<RadioAudio>().currentChannel.Equals("87.5"))
        {
            StartCoroutine(HeadHintsWait(radioAtSecondChannelHeadText));
            WatchHints.text = radioAtSecondChannelWatchText;
            radioKnob.GetComponent<RadioAudio>().currentChannel = "0.0";
        } else if (radioKnob.GetComponent<RadioAudio>().currentChannel.Equals("90.0"))
        {
            StartCoroutine(HeadHintsWait(radioAtThirdChannelHeadText));
            WatchHints.text = radioAtThirdChannelWatchText;
            radioKnob.GetComponent<RadioAudio>().currentChannel = "0.0";
        } else if (radioKnob.GetComponent<RadioAudio>().currentChannel.Equals("85.0"))
        {
            StartCoroutine(HeadHintsWait(fuseBoxButtonHeadText));
            WatchHints.text = fuseBoxButtonWatchText;
            radioKnob.GetComponent<RadioAudio>().currentChannel = "0.0";
        }


    }

    IEnumerator HeadHintsWait(string str)
    {
        HeadHints.text = str;
        yield return new WaitForSeconds(2);
        HeadHints.text = "";
    }


}
