//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// VR Bow
//
//=============================================================================
#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class VRBow : VRInteractableItem
	{
		public GameObject arrowPrefab;
		public Animator animator;
		public string bowPullAnimName;
		public string bowShootAnimName;
		public float distanceMultiplier = 1f;
		public float arrowPower = 100f;
		public Transform arrowDefaultPoint;
		public Transform arrowPulledPoint;
		public bool autoLoadNextArrow = true;

		private bool _usingSecondHeld;
		private Transform secondHeldTransform;
		private VRArrow _arrowInstance;
		private VRInteractor _hand;

		public bool usingSecondHeld
		{
			get { return _usingSecondHeld; }
			set { _usingSecondHeld = value; }
		}

		public VRArrow arrowInstance
		{
			get { return _arrowInstance; }
			set 
			{
				_arrowInstance = value;
				if (_arrowInstance != null)
				{
					_arrowInstance.canBeHeld = false;
					VRInteractableItem.FreezeItem(_arrowInstance.item.gameObject, true);
					usingSecondHeld = true;
					if (animator != null) animator.speed = 0f;
				} else
				{
					usingSecondHeld = false;
				}
			}
		}

		private void Start()
		{
			if (animator != null) animator.speed = 0f;
		}

		private float ArrowDist()
		{
			return Vector3.Distance(_arrowInstance.item.position, arrowDefaultPoint.position)*distanceMultiplier;
		}

		override protected void Step()
		{
			base.Step();

			if (_arrowInstance == null) return;

			float pullDist = ArrowDist();
			if (_arrowInstance.heldBy == null)
			{
				ShootArrow(pullDist);
				if (autoLoadNextArrow && _hand != null && _hand.heldItem == this) AddArrowToOtherHand(_hand);
				return;
			}

			if (animator != null && !string.IsNullOrEmpty(bowPullAnimName)) animator.Play(bowPullAnimName,0,pullDist);
			_arrowInstance.item.position = VRUtils.ClosestPointOnLine(arrowDefaultPoint.position, arrowPulledPoint.position, _arrowInstance.heldBy.getControllerAnchorOffset.position);
			_arrowInstance.item.rotation = arrowDefaultPoint.rotation;
		}

		public override bool Pickup (VRInteractor hand)
		{
			_hand = hand;
			if (autoLoadNextArrow) AddArrowToOtherHand(hand);
			return base.Pickup (hand);
		}

		override protected IEnumerator PickingUp(VRInteractor heldBy)
		{
			//Check if second held is being held.
			if (arrowInstance != null && arrowInstance.heldBy != null) yield break;

			yield return StartCoroutine(base.PickingUp(heldBy));
		}

		public override void Drop (bool addControllerVelocity, VRInteractor hand)
		{
			_hand = null;
			if (arrowInstance != null) 
			{
				ShootArrow(ArrowDist());
			}
			base.Drop (addControllerVelocity, hand);
		}

		private void ShootArrow(float arrowDist)
		{
			if (_arrowInstance == null || _arrowInstance.heldBy != null) return;
			_arrowInstance.canBeHeld = true;
			if (arrowDist < 0.1f && _hand != null)
			{
				GetComponent<Collider>().enabled = false;
				StartCoroutine(ReEnableCollider());
				VRInteractor otherHand = _hand.GetOtherController();
				if (otherHand != null)
				{
					otherHand.hoverItem = arrowInstance;
					otherHand.TryPickup();
				}
			} else 
			{
				if (_arrowInstance.heldBy != null)
				{
					_arrowInstance.heldBy.Drop();
				} else
				{
					VRInteractableItem.UnFreezeItem(_arrowInstance.item.gameObject);
					_arrowInstance.DisableHover();
				}
				if (_arrowInstance.selfBody != null)
				{
					_arrowInstance.selfBody.velocity = item.forward*((arrowDist-0.1f)*arrowPower);
				}
			}
			arrowInstance = null;

			if (animator != null && !string.IsNullOrEmpty(bowShootAnimName))
			{
				animator.speed = 1f;
				animator.Play(bowShootAnimName,0,0.79f-(arrowDist*0.5f));
			}
		}

		IEnumerator ReEnableCollider()
		{
			yield return new WaitForSeconds(1f);
			GetComponent<Collider>().enabled = true;
		}

		private void AddArrowToOtherHand(VRInteractor hand)
		{
			VRInteractor otherHand = hand.GetOtherController();
			if (otherHand != null && otherHand.heldItem == null)
			{
				GameObject arrowInstance = Instantiate<GameObject>(arrowPrefab, otherHand.getControllerAnchorOffset.position, Quaternion.identity);
				otherHand.hoverItem = arrowInstance.GetComponentInChildren<VRInteractableItem>(true);
				otherHand.hoverItem.item.rotation = otherHand.getControllerAnchorOffset.rotation * otherHand.hoverItem.GetLocalHeldRotation(otherHand);
				otherHand.TryPickup();
			}
		}

		override protected Vector3 GetHeldPositionDelta()
		{
			if (usingSecondHeld && arrowInstance != null && heldBy != null)
			{
				Transform heldByTransform = heldBy.getControllerAnchorOffset;
				if (secondHeldTransform == null)
				{
					GameObject secondHeldObject = new GameObject("SecondHeldController");
					secondHeldTransform = secondHeldObject.transform;
					secondHeldTransform.SetParent(item);
					secondHeldTransform.localRotation = Quaternion.Inverse(base.GetLocalHeldRotation(_heldBy));
				}
				secondHeldTransform.position = heldByTransform.position;
				return (secondHeldTransform.TransformPoint(GetLocalHeldPosition(heldBy))) - item.position;
			} else return base.GetHeldPositionDelta();
		}

		override public Quaternion GetLocalHeldRotation(VRInteractor hand)
		{
			if (usingSecondHeld && arrowInstance != null && arrowInstance.heldBy != null && heldBy != null)
			{
				Vector3 heldControllerPos = hand.getControllerAnchorOffset.position;
				Vector3 secondHeldControllerPos = arrowInstance.heldBy.getControllerAnchorOffset.position;
				Vector3 controllersVectorDirection = (heldControllerPos - secondHeldControllerPos).normalized;
				Quaternion controllersDirection = Quaternion.LookRotation(controllersVectorDirection);
				Vector3 heldWorldPosition = hand.getControllerAnchorOffset.TransformPoint(base.GetLocalHeldPosition(hand));
				Vector3 secondHeldLocalPosition = arrowInstance.GetLocalHeldPosition(arrowInstance.heldBy);
				secondHeldLocalPosition *= 3f;
				Vector3 secondHeldWorldPosition = VRUtils.TransformPoint(arrowInstance.heldBy.getControllerAnchor.position, controllersDirection, arrowInstance.heldBy.getControllerAnchor.lossyScale, secondHeldLocalPosition);
				secondHeldWorldPosition += (controllersVectorDirection*1f);
				Vector3 worldHeldPositionDirectionVector = secondHeldWorldPosition - heldWorldPosition;
				return Quaternion.LookRotation(worldHeldPositionDirectionVector, hand.getControllerAnchorOffset.up);

			} else return base.GetLocalHeldRotation(hand);
		}

		override protected Quaternion GetHeldRotationDelta()
		{
			Transform heldByTransform = heldBy.getControllerAnchorOffset;
			if (usingSecondHeld && arrowInstance != null && arrowInstance.heldBy != null)
			{
				return (GetLocalHeldRotation(heldBy)) * Quaternion.Inverse(item.rotation);
			} else
			{
				return (heldByTransform.rotation*GetLocalHeldRotation(heldBy)) * Quaternion.Inverse(item.rotation);
			}
		}
	}
}
#endif