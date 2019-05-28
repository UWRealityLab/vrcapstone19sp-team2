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
 


    }

    IEnumerator HeadHintsWait(string str)
    {
        HeadHints.text = str;
        yield return new WaitForSeconds(2);
        HeadHints.text = "";
    }


}
