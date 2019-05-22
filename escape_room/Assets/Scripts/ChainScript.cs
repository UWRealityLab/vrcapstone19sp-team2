using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ChainScript : MonoBehaviour
{
    public Transform ChainStart;
    public Transform ChainEnd;
    public GameObject Hand;
    public float LengthOffset = 0.0f;
    public bool enableHaptics = true;

    // Haptics
    public SteamVR_Action_Vibration hapticAction;

    protected float MaxLength;

    // Start is called before the first frame update
    void Start()
    {
        MaxLength = (ChainEnd.position - ChainStart.position).magnitude + LengthOffset;
    }

    // Update is called once per frame
    void Update()
    {
        // Hide Left Hand
        Hand.GetComponent<Hand>().HideController(true);

        Vector3 handDirection = (Hand.transform.position - ChainStart.position).normalized;
        float handDistance = (Hand.transform.position - ChainStart.position).magnitude;

        ChainEnd.position = ChainStart.position + handDirection * Mathf.Min(handDistance, MaxLength);

        if (handDistance > MaxLength + LengthOffset)
        {
            Pulse();
        }
    }

    public void Break()
    {
        // Show hand
        Hand.GetComponent<Hand>().ShowController(true);
        // Disable kinemetics
        this.GetComponent<Rigidbody>().isKinematic = false;
        // disable self
        this.GetComponent<ChainScript>().enabled = false;
    }

    private void Pulse()
    {
        if (enableHaptics)
        {
            hapticAction.Execute(0, 100f / 1000000f, 1000000f / 100f, 1, SteamVR_Input_Sources.LeftHand);
        }
    }
}
