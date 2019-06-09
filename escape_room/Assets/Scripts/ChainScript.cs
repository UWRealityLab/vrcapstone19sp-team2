using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using static GameManagerScript;
using static UIContent;


public class ChainScript : MonoBehaviour
{
    public GameObject ChainStart;
    public GameObject ChainEnd;
    public GameObject Hand;
    public GameObject Teleport;
    public float LengthOffset = 0.0f;
    public bool enableHaptics = true;
    public bool enableTeleport = false;
    public bool broke = false;

    public List<GameObject> ChainParts;

    // Haptics
    public SteamVR_Action_Vibration hapticAction;

    public GameManagerScript manager;

    protected float MaxLength;
    private bool executed = false;
    private List<GameObject> chainsOnHand = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        MaxLength = (ChainEnd.transform.position - ChainStart.transform.position).magnitude + LengthOffset;
        Teleport.GetComponent<Teleport>().enabled = enableTeleport;

        // Cancel TeleportHint
        Teleport.GetComponent<Teleport>().CancelTeleportHint();
    }

    // Update is called once per frame
    void Update()
    {
        if (!executed)
        {
            executed = true;
            // Trigger
            manager.TriggerTask(TaskTypes.RELEASE, EventTypes.WAKE_UP, getAudioLength(EventTypes.TRY_TO_RELEASE) + 0.2f);
            manager.TriggerEvent(EventTypes.WAKE_UP, 0.5f);
            manager.TriggerEvent(EventTypes.TRY_TO_RELEASE, getAudioLength(EventTypes.WAKE_UP) + 1);
        }

        if (!broke)
        {
            // Hide Left Hand
            Hand.GetComponent<Hand>().HideController(true);

            Vector3 handDirection = (Hand.transform.position - ChainStart.transform.position).normalized;
            float handDistance = (Hand.transform.position - ChainStart.transform.position).magnitude;

            ChainEnd.transform.position = ChainStart.transform.position + handDirection * Mathf.Min(handDistance, MaxLength);

            if (handDistance > MaxLength + LengthOffset)
            {
                Pulse();
            }
        }
    }

    public void Break()
    {
        UpdateChainsOnHand();
        if (!broke)
        {
            broke = true;
            // Show hand
            Hand.GetComponent<Hand>().ShowController(true);
            // disable self
            // this.GetComponent<ChainScript>().enabled = false;

            // Attach end to hand
            //Hand.GetComponent<Hand>().AttachObject(ChainEnd, GrabTypes.Scripted, Valve.VR.InteractionSystem.Hand.AttachmentFlags.ParentToHand | Valve.VR.InteractionSystem.Hand.AttachmentFlags.DetachFromOtherHand | Valve.VR.InteractionSystem.Hand.AttachmentFlags.TurnOnKinematic, ChainEnd.transform.Find("AttachmentOffset"));
            ChainEnd.transform.parent = Hand.transform;
            ChainEnd.transform.localPosition = ChainEnd.transform.Find("AttachmentOffset").localPosition;
            ChainEnd.transform.localEulerAngles = ChainEnd.transform.Find("AttachmentOffset").localEulerAngles;
            // Enable Teleport
            Teleport.GetComponent<Teleport>().enabled = true;
            StartCoroutine(HintWait());
            // Trigger
            manager.CompleteTask(TaskTypes.RELEASE);
            manager.TriggerTask(TaskTypes.RADIO, EventTypes.CUTTER_CUT);
            manager.TriggerEvent(EventTypes.CUTTER_CUT);
        }
    }

    IEnumerator HintWait()
    {
        yield return new WaitForSeconds(getAudioLength(EventTypes.CUTTER_CUT));
        // Enable Hint
        Teleport.GetComponent<Teleport>().ShowTeleportHint();
    }

    private void Pulse()
    {
        if (enableHaptics)
        {
            hapticAction.Execute(0, 100f / 1000000f, 1000000f / 100f, 1, SteamVR_Input_Sources.LeftHand);
        }
    }

    public void UpdateChainsOnHand()
    {
        bool broke = ChainEnd.GetComponent<CharacterJoint>() == null;
        foreach (GameObject chainPart in ChainParts)
        {
            if (!broke)
            {
                chainPart.transform.parent = ChainEnd.transform;
                broke = chainPart.GetComponent<CharacterJoint>() == null;
            }
            else
            {
                chainPart.transform.parent = ChainStart.transform.parent;
            }
        }
    }
}
