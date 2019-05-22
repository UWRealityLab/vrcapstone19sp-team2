using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class CutterScript : MonoBehaviour
{
    public Transform Joint1;
    public Transform Joint2;
    public GameObject CutterOpen;
    public GameObject CutterClose;

    private float Joint1_Z_Length;
    private float Joint2_Z_Length;

    // Start is called before the first frame update
    void Start()
    {
        Joint1_Z_Length = CutterClose.transform.Find("joint1").localRotation.z - CutterOpen.transform.Find("joint1").localRotation.z;
        Joint2_Z_Length = CutterClose.transform.Find("joint4").localRotation.z - CutterOpen.transform.Find("joint4").localRotation.z;
        Joint1.Rotate(0, 0, CutterOpen.transform.Find("joint1").localRotation.z + Joint1_Z_Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandAttachedUpdate(Hand hand)
    {
        float squeezeValue = SteamVR_Actions._default.Squeeze.GetAxis(SteamVR_Input_Sources.RightHand);
        if (squeezeValue != 0.0f)
        {
            Joint1.Rotate(new Vector3(0, 0, 1), CutterOpen.transform.Find("joint1").localRotation.z + squeezeValue * Joint1_Z_Length);
        }
    }
}
