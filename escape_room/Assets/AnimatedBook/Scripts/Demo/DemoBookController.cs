﻿using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;
using Valve.VR;

public class DemoBookController : MonoBehaviour {

    [EnumFlags]
    [Tooltip("The flags used to attach this object to the hand.")]
    public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.TurnOnKinematic;

    [Tooltip("The local point which acts as a positional and rotational offset to use while held")]
    public Transform attachmentOffset;

    [Tooltip("How fast must this object be moving to attach due to a trigger hold instead of a trigger press? (-1 to disable)")]
    public float catchingSpeedThreshold = -1;

    public ReleaseStyle releaseVelocityStyle = ReleaseStyle.GetFromHand;

    [Tooltip("The time offset used when releasing the object with the RawFromHand option")]
    public float releaseVelocityTimeOffset = -0.011f;

    public float scaleReleaseVelocity = 1.1f;

    [Tooltip("When detaching the object, should it return to its original parent?")]
    public bool restoreOriginalParent = false;
    private SteamVR_Action_Boolean LeftTurn = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("LeftTurn");
    private SteamVR_Action_Boolean RightTurn = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("RightTurn");



    protected VelocityEstimator velocityEstimator;
    protected bool attached = false;
    protected float attachTime;
    protected Vector3 attachPosition;
    protected Quaternion attachRotation;
    protected Transform attachEaseInTransform;

    public UnityEvent onPickUp;
    public UnityEvent onDetachFromHand;
    public UnityEvent<Hand> onHeldUpdate;
    private bool isOnRight;
    private bool isOnLeft;


    protected RigidbodyInterpolation hadInterpolation = RigidbodyInterpolation.None;

    protected new Rigidbody rigidbody;

    [HideInInspector]
    public Interactable interactable;

    private void Start()
    {
        isOnRight = false;
        isOnLeft = false;
    }
    //-------------------------------------------------
    protected virtual void Awake()
    {
        velocityEstimator = GetComponent<VelocityEstimator>();
        interactable = GetComponent<Interactable>();



        rigidbody = GetComponent<Rigidbody>();
        rigidbody.maxAngularVelocity = 50.0f;


        if (attachmentOffset != null)
        {
            // remove?
            //interactable.handFollowTransform = attachmentOffset;
        }

    }


    //-------------------------------------------------
    protected virtual void OnHandHoverBegin(Hand hand)
    {
        bool showHint = false;

        // "Catch" the throwable by holding down the interaction button instead of pressing it.
        // Only do this if the throwable is moving faster than the prescribed threshold speed,
        // and if it isn't attached to another hand
        if (!attached && catchingSpeedThreshold != -1)
        {
            float catchingThreshold = catchingSpeedThreshold * SteamVR_Utils.GetLossyScale(Player.instance.trackingOriginTransform);

            GrabTypes bestGrabType = hand.GetBestGrabbingType();

            if (bestGrabType != GrabTypes.None)
            {
                if (rigidbody.velocity.magnitude >= catchingThreshold)
                {
                    hand.AttachObject(gameObject, bestGrabType, attachmentFlags);
                    showHint = false;
                }
            }
        }

        if (showHint)
        {
            hand.ShowGrabHint();
        }
    }


    //-------------------------------------------------
    protected virtual void OnHandHoverEnd(Hand hand)
    {
        hand.HideGrabHint();
    }

    //-------------------------------------------------
    protected virtual void OnAttachedToHand(Hand hand)
    {
        //Debug.Log("<b>[SteamVR Interaction]</b> Pickup: " + hand.GetGrabStarting().ToString());

        hadInterpolation = this.rigidbody.interpolation;

        attached = true;

        onPickUp.Invoke();

        hand.HoverLock(null);

        rigidbody.interpolation = RigidbodyInterpolation.None;

        velocityEstimator.BeginEstimatingVelocity();

        attachTime = Time.time;
        attachPosition = transform.position;
        attachRotation = transform.rotation;

    }


    //-------------------------------------------------
    protected virtual void OnDetachedFromHand(Hand hand)
    {
        attached = false;

        onDetachFromHand.Invoke();

        hand.HoverUnlock(null);

        rigidbody.interpolation = hadInterpolation;

        Vector3 velocity;
        Vector3 angularVelocity;

        GetReleaseVelocities(hand, out velocity, out angularVelocity);

        rigidbody.velocity = velocity;
        rigidbody.angularVelocity = angularVelocity;
    }


    public virtual void GetReleaseVelocities(Hand hand, out Vector3 velocity, out Vector3 angularVelocity)
    {
        if (hand.noSteamVRFallbackCamera && releaseVelocityStyle != ReleaseStyle.NoChange)
            releaseVelocityStyle = ReleaseStyle.ShortEstimation; // only type that works with fallback hand is short estimation.

        switch (releaseVelocityStyle)
        {
            case ReleaseStyle.ShortEstimation:
                velocityEstimator.FinishEstimatingVelocity();
                velocity = velocityEstimator.GetVelocityEstimate();
                angularVelocity = velocityEstimator.GetAngularVelocityEstimate();
                break;
            case ReleaseStyle.AdvancedEstimation:
                hand.GetEstimatedPeakVelocities(out velocity, out angularVelocity);
                break;
            case ReleaseStyle.GetFromHand:
                velocity = hand.GetTrackedObjectVelocity(releaseVelocityTimeOffset);
                angularVelocity = hand.GetTrackedObjectAngularVelocity(releaseVelocityTimeOffset);
                break;
            default:
            case ReleaseStyle.NoChange:
                velocity = rigidbody.velocity;
                angularVelocity = rigidbody.angularVelocity;
                break;
        }

        if (releaseVelocityStyle != ReleaseStyle.NoChange)
            velocity *= scaleReleaseVelocity;
    }



    //-------------------------------------------------
    protected virtual IEnumerator LateDetach(Hand hand)
    {
        yield return new WaitForEndOfFrame();

        hand.DetachObject(gameObject, restoreOriginalParent);
    }


    //-------------------------------------------------
    protected virtual void OnHandFocusAcquired(Hand hand)
    {
        gameObject.SetActive(true);
        velocityEstimator.BeginEstimatingVelocity();
    }


    //-------------------------------------------------
    protected virtual void OnHandFocusLost(Hand hand)
    {
        gameObject.SetActive(false);
        velocityEstimator.FinishEstimatingVelocity();
    }





    public AnimatedBookController bookController;		// Reference to the BookController script

	public Sprite[] pageBackground;
	private int pageStyleIndex = 0;

    protected virtual void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();

        if (startingGrabType == GrabTypes.Grip)
        {
            // Disable kinetic
            GetComponent<Rigidbody>().isKinematic = false;
            hand.AttachObject(gameObject, startingGrabType, attachmentFlags, attachmentOffset);
            /*if (hand.handType.Equals(Valve.VR.SteamVR_Input_Sources.LeftHand))
            {
                bookController.TurnToNextPage();

                //hand.AttachObject(gameObject, startingGrabType, attachmentFlags, attachmentOffsetLeft);
            }
            else
            {
                bookController.TurnToNextPage();
                //hand.AttachObject(gameObject, startingGrabType, attachmentFlags, attachmentOffsetRight);
            }*/
            hand.HideGrabHint();
            
        }
    }

        private void HandAttachedUpdate(Hand hand)
    {
        // if (hand.IsGrabEnding(this.gameObject))
        if (hand.GetGrabStarting() == GrabTypes.Grip)
        {

            hand.DetachObject(gameObject, restoreOriginalParent);

            // Uncomment to detach ourselves late in the frame.
            // This is so that any vehicles the player is attached to
            // have a chance to finish updating themselves.
            // If we detach now, our position could be behind what it
            // will be at the end of the frame, and the object may appear
            // to teleport behind the hand when the player releases it.
            //StartCoroutine( LateDetach( hand ) );
        }

        if (onHeldUpdate != null)
            onHeldUpdate.Invoke(hand);
        if (RightTurn.GetState(hand.handType)) {
            if (!isOnRight)
            {
                bookController.TurnToPreviousPage();
                isOnRight = true;
            }
            //bookController.TurnToPreviousPage();
        } else if (LeftTurn.GetState(hand.handType))
        {
            if (!isOnLeft)
            {
                bookController.TurnToNextPage();

                isOnLeft = true;
            }
            //bookController.TurnToNextPage();
        }
        else
        {
            isOnRight = false;
            isOnLeft = false;
        }
    }

	public void switchPageStyle() {
		pageStyleIndex++;
		if (pageStyleIndex >= pageBackground.Length) {
			pageStyleIndex = 0;
		}
		bookController.defaultBackground = pageBackground [pageStyleIndex];

		foreach (AnimatedBookController.PageObjects page in bookController.getPageObjects()) {
			page.RectoImage.sprite = pageBackground [pageStyleIndex];
			page.VersoImage.sprite = pageBackground [pageStyleIndex];
		}
	}
}

