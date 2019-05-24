using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHeadUI : MonoBehaviour
{
    public GameObject cutter;
    private string cutterDoneInstruction = "Go find Light";

    public GameObject flashLight;
    private string flashLightGrabbedInstruction = "Do you find a fuse box on the wall?";

    public GameObject fuseBoxButton;
    private string fuseBoxOpennedInstruction = "What's wrong with the radio?";

    public GameObject radioKnob;
    private string radioIsAt107 = "Hmm..I may need to check the freezer.";

    public GameObject iceBox;
    private string iceBoxIsGrabbedInstruction = "";
    // Start is called before the first frame update

    private Text hints;
    void Start()
    {
        hints = this.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cutter.GetComponent<CutterScript>().passed)
        {
            hints.text = cutterDoneInstruction;
            cutter.GetComponent<CutterScript>().passed = false;
        }

        if (flashLight.GetComponent<FlashLightGrabbed>().passed)
        {
            Debug.Log("flash light true");
            hints.text = flashLightGrabbedInstruction;
            flashLight.GetComponent<FlashLightGrabbed>().passed = false;
        }
    }
}
