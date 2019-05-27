using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class CutterScript : MonoBehaviour
{
    public GameObject Joint1;
    public GameObject Joint2;
    public GameObject CutterOpen;
    public GameObject CutterClose;
    public ChainScript ChainScript;
    public GameObject ChainContacted = null;

    public bool passed;

    private float Joint1_Z_Length;
    private float Joint2_Z_Length;

    // Start is called before the first frame update
    void Start()
    {
        passed = false;
        // Euler angle is between 0 - 360. Here, we need to convert this wisely to get the actual angle.
        Joint1_Z_Length = CutterClose.transform.Find("joint1").localEulerAngles.z - CutterOpen.transform.Find("joint1").localEulerAngles.z;
        Joint2_Z_Length = 360 - CutterClose.transform.Find("joint4").localEulerAngles.z + CutterOpen.transform.Find("joint4").localEulerAngles.z;
        // Debug.Log(Joint2_Z_Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandAttachedUpdate(Hand hand)
    {
        float squeezeValue = Mathf.Max(SteamVR_Actions._default.Squeeze.GetAxis(SteamVR_Input_Sources.RightHand), SteamVR_Actions._default.Squeeze.GetAxis(SteamVR_Input_Sources.LeftHand));
        Joint1.transform.localEulerAngles = new Vector3(Joint1.transform.localEulerAngles.x, Joint1.transform.localEulerAngles.y, CutterOpen.transform.Find("joint1").localEulerAngles.z + squeezeValue * Joint1_Z_Length);
        Joint2.transform.localEulerAngles = new Vector3(Joint2.transform.localEulerAngles.x, Joint2.transform.localEulerAngles.y, CutterOpen.transform.Find("joint4").localEulerAngles.z - squeezeValue * Joint2_Z_Length);
        if (squeezeValue >= 0.99f && ChainContacted != null)
        {
            // Debug.Log("cutted");
            passed = true;
            Destroy(ChainContacted.GetComponent<CharacterJoint>());
            ChainScript.Break();
        }
    }
}
