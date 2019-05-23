using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ChainScript : MonoBehaviour
{
    public Transform ChainStart;
    public GameObject ChainEnd;
    public GameObject Hand;
    public GameObject Teleport;
    public float LengthOffset = 0.0f;
    public bool enableHaptics = true;
    public bool enableTeleport = false;
    public bool broke = false;

    // Haptics
    public SteamVR_Action_Vibration hapticAction;

    protected float MaxLength;

    // Start is called before the first frame update
    void Start()
    {
        MaxLength = (ChainEnd.transform.position - ChainStart.position).magnitude + LengthOffset;
        // Teleport.GetComponent<Teleport>().enabled = enableTeleport;

        // Cancel TeleportHint
        Teleport.GetComponent<Teleport>().CancelTeleportHint();
    }

    // Update is called once per frame
    void Update()
    {
        if (!broke)
        {
            // Hide Left Hand
            Hand.GetComponent<Hand>().HideController(true);

            Vector3 handDirection = (Hand.transform.position - ChainStart.position).normalized;
            float handDistance = (Hand.transform.position - ChainStart.position).magnitude;

            ChainEnd.transform.position = ChainStart.position + handDirection * Mathf.Min(handDistance, MaxLength);

            if (handDistance > MaxLength + LengthOffset)
            {
                Pulse();
            }
        }
    }

    public void Break()
    {
        if (!broke)
        {
            broke = true;
            // Show hand
            Hand.GetComponent<Hand>().ShowController(true);
            // disable self
            this.GetComponent<ChainScript>().enabled = false;
            // Attach end to hand
            //Hand.GetComponent<Hand>().AttachObject(ChainEnd, GrabTypes.Scripted, Valve.VR.InteractionSystem.Hand.AttachmentFlags.ParentToHand | Valve.VR.InteractionSystem.Hand.AttachmentFlags.DetachFromOtherHand | Valve.VR.InteractionSystem.Hand.AttachmentFlags.TurnOnKinematic, ChainEnd.transform.Find("AttachmentOffset"));
            ChainEnd.transform.parent = Hand.transform;
            ChainEnd.transform.localPosition = ChainEnd.transform.Find("AttachmentOffset").localPosition;
            ChainEnd.transform.localEulerAngles = ChainEnd.transform.Find("AttachmentOffset").localEulerAngles;
            // Enable Teleport
            Teleport.GetComponent<Teleport>().enabled = true;
            // Enable Hint
            Teleport.GetComponent<Teleport>().ShowTeleportHint();
        }
    }

    private void Pulse()
    {
        if (enableHaptics)
        {
            hapticAction.Execute(0, 100f / 1000000f, 1000000f / 100f, 1, SteamVR_Input_Sources.LeftHand);
        }
    }
}
