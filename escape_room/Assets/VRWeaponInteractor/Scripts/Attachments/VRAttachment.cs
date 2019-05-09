//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Base Attachment script. Use this for things like scopes or silencers. The
// VRMagazine script also inherits from this script.
//
//=============================================================================
using UnityEngine.Networking;

#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class VRAttachment : VRInteractableItem
	{
		[System.Serializable]
		public class AttachmentPosition
		{
			public Vector3 localPosition;
			public Quaternion localRotation;
			public Vector3 entryPosition;
			public int gunHandlerAttachmentId;
			public int receiverAttachmentId;
			public GameObject weaponPrefab;

			//Shoot Overrides
			public bool useNewShootDirection;
			public Vector3 newShootOrigin;
			public Vector3 newShootDirection;
			public VRGunHandler.ShotMode newShotMode;
			public int newBulletsPerShot = 6;
			public float newConeSize = 2.5f;

			//Sound Overrides
			public AudioClip newFireSound;
			public AudioClip newDryFireSound;

			//Laser Pointer Overrides
			public bool useLaserSight;
			public Vector3 laserPointOrigin;
		}

		public List<AttachmentPosition> attachmentRefs = new List<AttachmentPosition>();

		//	Slide Variables
		public bool slideAttachment;
		public bool autoLoad = false;
		public float autoLoadSpeed = 1.0f;
		public bool showHighlight;

		//Events
		public UnityEvent attachEvent;
		public UnityEvent detatchEvent;

		//Sounds
		public AudioClip attachSound;
		public AudioClip detatchSound;

		//Highlight material
		public Material highlightMaterial;
		public Material readyToAttachMaterial;

		public VRAttachmentReceiver currentReceiver;
		protected GameObject highlightInstance;

		protected bool autoLoading;
		protected Vector3 controllerPickupPosition;
		protected Vector3 itemPickupPosition;
		protected bool attachedToGun;

		private AttachmentPosition currentAttachmentPosition;

		//Original Weapon Variables
		private Vector3 oldShootOrigin;
		private Vector3 oldShootDirection;
		private VRGunHandler.ShotMode oldShotMode;
		private int oldBulletsPerShot = 6;
		private bool oldLaserPointerEnabled;
		private float oldConeSize = 2.5f;
		private AudioClip oldFireSound;
		private AudioClip oldDryFireSound;
		private bool madeHighlight;
		private Transform highlightParent;

		override protected void Init()
		{
			base.Init();
			SpawnHighlight(null);
		}

		override protected void OnEnable()
		{
			base.OnEnable();

			if (parents.Count != 0)
			{
				bool found = false;
				foreach(VRInteractableItem parentItem in parents)
				{
					if (parentItem.GetType() != typeof(VRGunHandler)) continue;
					VRGunHandler gunHandler = (VRGunHandler)parentItem;
					foreach (VRGunHandler.AttachmentPrefabs attachmentPrefabs in gunHandler.attachmentPrefabs)
					{
						foreach(AttachmentPosition attachmentPosition in attachmentRefs)
						{
							if (attachmentPosition.gunHandlerAttachmentId != gunHandler.attachmentId || attachmentPosition.receiverAttachmentId != attachmentPrefabs.attachmentReceiver.attachmentId) continue;
							VRInteractableItem.FreezeItem(item.gameObject, false, false, true);
							AttachToGunHandler(attachmentPrefabs.attachmentReceiver);
							found = true;
							break;
						}
						if (found) break;
					}
					if (found) break;
				}
			}
		}

		override protected void Step()
		{
			if (!slideAttachment || currentReceiver == null)
			{
				base.Step();
				return;
			}

			if (currentAttachmentPosition == null) currentAttachmentPosition = GetAttachmentPosition(currentReceiver);

			if (heldBy != null)
			{
				Vector3 posDif = controllerPickupPosition - GetLocalControllerPositionToParentTransform(heldBy, this, currentReceiver.gunHandler.item);
				item.localPosition = VRUtils.ClosestPointOnLine(currentAttachmentPosition.entryPosition, currentAttachmentPosition.localPosition, itemPickupPosition - posDif);
			} else if (!attachedToGun)
			{
				if (autoLoading && !interactionDisabled)
				{
					// Auto moving to loaded position
					item.localPosition = Vector3.MoveTowards(item.localPosition, currentAttachmentPosition.localPosition, autoLoadSpeed);
				} else
				{
					//	Using gravity to slide attachment
					Vector3 direction = currentAttachmentPosition.localPosition - currentAttachmentPosition.entryPosition;
					float gravity = currentReceiver.gunHandler.item.TransformPoint(currentAttachmentPosition.entryPosition).y - currentReceiver.gunHandler.item.TransformPoint(currentAttachmentPosition.localPosition).y;
					item.localPosition = VRUtils.ClosestPointOnLine(currentAttachmentPosition.entryPosition, currentAttachmentPosition.localPosition, item.localPosition+(direction*gravity));
				}
			}

			float scaleMargin = 0.01f / currentReceiver.gunHandler.item.localScale.x;
			if (attachedToGun && item.localPosition != currentAttachmentPosition.localPosition)
			{
				Eject();
			} else if (item.localPosition.x < currentAttachmentPosition.entryPosition.x+scaleMargin && item.localPosition.x > currentAttachmentPosition.entryPosition.x-scaleMargin&&
				item.localPosition.y < currentAttachmentPosition.entryPosition.y+scaleMargin && item.localPosition.y > currentAttachmentPosition.entryPosition.y-scaleMargin &&
				item.localPosition.z < currentAttachmentPosition.entryPosition.z+scaleMargin && item.localPosition.z > currentAttachmentPosition.entryPosition.z-scaleMargin)
			{
				DetatchFromGun();
			} else if (item.localPosition == currentAttachmentPosition.localPosition && !attachedToGun && !interactionDisabled)
			{
				if (heldBy != null) heldBy.Drop();
				autoLoading = false;
				LoadIntoGun();
			}
		}

		override public bool Pickup(VRInteractor hand)
		{
			if (!slideAttachment)
			{
				SpawnHighlight(hand);
				if (currentReceiver != null) DetatchFromGunHandler(currentReceiver);
				return base.Pickup(hand);
			} else
			{
				if (currentReceiver != null && currentReceiver.currentAttachment == this)
				{
					//Grabbed attachment while attached
					attachedToGun = true;
					var oldCanBeHeld = canBeHeld;
					canBeHeld = false;
					bool returnVal = base.Pickup(hand);
					canBeHeld = oldCanBeHeld;
					controllerPickupPosition = GetLocalControllerPositionToParentTransform(hand, this, currentReceiver.gunHandler.item);
					itemPickupPosition = item.localPosition;
					return returnVal;
				} else if (currentReceiver != null && currentReceiver.currentAttachment == null)
				{
					//Grabbed attachment while on the track
					var oldCanBeHeld = canBeHeld;
					canBeHeld = false;
					bool returnVal = base.Pickup(hand);
					canBeHeld = oldCanBeHeld;
					controllerPickupPosition = GetLocalControllerPositionToParentTransform(hand, this, currentReceiver.gunHandler.item);
					itemPickupPosition = item.localPosition;
					return returnVal;
				} else if (currentReceiver != null && currentReceiver.currentAttachment != this)
				{
					//current gun has a reference to a attachment that is not this.
					currentReceiver = null;
				}

				SpawnHighlight(hand);
				//Grabbed attachment with no gun
				return base.Pickup(hand);
			}
		}

		override public void Drop(bool addControllerVelocity, VRInteractor hand = null)
		{
			DestroyHighlight();
			if (!slideAttachment)
			{
				base.Drop(addControllerVelocity, hand);
				LoadIntoGun();
			} else
			{
				if (currentReceiver != null)
				{
					var oldCanbeHeld = canBeHeld;
					canBeHeld = false;
					base.Drop(addControllerVelocity, hand);
					canBeHeld = oldCanbeHeld;
					return;
				}
				base.Drop(addControllerVelocity, hand);
			}
		}

		protected void LoadIntoGun()
		{
			if (currentReceiver == null) return;

			item.SetParent(currentReceiver.gunHandler.item);
			currentAttachmentPosition = GetAttachmentPosition(currentReceiver);

			if (currentAttachmentPosition == null) return;

			item.localPosition = currentAttachmentPosition.localPosition;
			item.localRotation = currentAttachmentPosition.localRotation;

			VRInteractableItem.FreezeItem(item.gameObject, false, false, true);
			AttachToGunHandler(currentReceiver);
			attachedToGun = true;
		}

		public void Eject()
		{
			if (currentReceiver == null || currentReceiver.currentAttachment != this) return;
			attachedToGun = false;
			DetatchFromGunHandler(currentReceiver);
		}

		protected void DetatchFromGun()
		{
			currentReceiver = null;
			if (heldBy == null)
			{
				Drop(false);
			} else
			{
				SpawnHighlight(heldBy);

				item.SetParent(heldBy.GetVRRigRoot);
				item.position = GetControllerPosition(heldBy);
				item.rotation = GetControllerRotation(heldBy);
				VRInteractableItem.HeldFreezeItem(item.gameObject);
			}
		}

		public AttachmentPosition GetAttachmentPosition(VRAttachmentReceiver attachmentReceiver)
		{
			foreach(AttachmentPosition attachmentRef in attachmentRefs)
			{
				if (attachmentReceiver.gunHandler.attachmentId != attachmentRef.gunHandlerAttachmentId || attachmentReceiver.attachmentId != attachmentRef.receiverAttachmentId) continue;
				return attachmentRef;
			}
			return null;
		}

		virtual public void SpawnHighlight(VRInteractor hand)
		{
			if (!showHighlight || highlightMaterial == null) return;
			if (!madeHighlight && highlightInstance == null) //Instantiate Highlight if not done already
			{
				madeHighlight = true;
				item.gameObject.SetActive(false);
				highlightInstance = (GameObject)Instantiate(item.gameObject);
				Component[] components = highlightInstance.GetComponentsInChildren<Component>();
				foreach(Component component in components)
				{
					System.Type type = component.GetType();
					if (type == typeof(Transform) || type == typeof(MeshFilter) || type == typeof(MeshRenderer)) continue;
					Destroy(component);
				}
				DestroyHighlight();
				item.gameObject.SetActive(true);
			}
			if (hand == null || (highlightInstance != null && highlightInstance.activeSelf)) return;
			VRInteractor otherController = hand.GetOtherController();
			if (otherController == null || otherController.heldItem == null || (otherController.heldItem.GetType() != typeof(VRGunHandler) && otherController.heldItem.GetType() != typeof(VRSecondHeld))) return;
			VRGunHandler gunHandler = null;
			if (otherController.heldItem.GetType() == typeof(VRSecondHeld)) gunHandler = ((VRSecondHeld)otherController.heldItem).gunHandler;
			else gunHandler = (VRGunHandler)otherController.heldItem;
			foreach(AttachmentPosition attachmentRef in attachmentRefs)
			{
				if (gunHandler.attachmentId != attachmentRef.gunHandlerAttachmentId) continue;
				highlightInstance.transform.SetParent(gunHandler.item);
				highlightInstance.transform.localPosition = attachmentRef.localPosition;
				highlightInstance.transform.localRotation = attachmentRef.localRotation;
				highlightInstance.SetActive(true);

				SetHighlightReady(false);
				break;
			}
		}

		virtual public void DestroyHighlight()
		{
			if (highlightInstance == null || !showHighlight) return;
			if (highlightParent == null)
			{
				highlightParent = new GameObject("Highlight").transform;
				highlightParent.SetParent(item);
				highlightParent.localPosition = Vector3.zero;
				highlightParent.localRotation = Quaternion.identity;
				highlightParent.localScale = Vector3.one;
			}
			highlightInstance.transform.SetParent(highlightParent);
			highlightInstance.SetActive(false);
		}

		virtual protected void SetHighlightReady(bool ready)
		{
			if (!showHighlight || (readyToAttachMaterial == null && ready)) return;
			MeshRenderer[] renderers = highlightInstance.GetComponentsInChildren<MeshRenderer>();
			Material targetMat = ready ? readyToAttachMaterial : highlightMaterial;
			foreach (MeshRenderer renderer in renderers) renderer.material = targetMat;
		}

		virtual protected void OnTriggerStay(Collider col)
		{
			if (currentReceiver != null || heldBy == null || attachedToGun) return;

			VRAttachmentReceiver newReceiver = col.GetComponent<VRAttachmentReceiver>();
			if (newReceiver == null || newReceiver == currentReceiver || newReceiver.currentAttachment != null) return;

			if (newReceiver.gunHandler == null)
			{
				Debug.LogError("Attachment receiver missing gun handler reference", newReceiver.gameObject);
				return;
			}
				
			if (GetAttachmentPosition(newReceiver) != null)
			{
				if (!slideAttachment)
				{
					currentReceiver = newReceiver;
					SetHighlightReady(true);
				} else TryLoadIntoGun(newReceiver);
			}
		}

		public void TryLoadIntoGun(VRAttachmentReceiver attachmentReceiver)
		{
			if (currentReceiver != null ||
				attachmentReceiver == null ||
				attachmentReceiver.currentAttachment != null ||
				attachmentReceiver.gunHandler.interactionDisabled ||
				interactionDisabled)
			{
				//Debug.Log(currentReceiver + " " + attachmentReceiver);
				//if (attachmentReceiver != null) Debug.Log(attachmentReceiver.currentAttachment + " " + attachmentReceiver.gunHandler.interactionDisabled + " " + interactionDisabled);
				return;
			}
			currentReceiver = attachmentReceiver;

			//Controller position or if not held current position
			Vector3 currentPosition = Vector3.zero;
			if (heldBy != null)
				currentPosition = GetLocalControllerPositionToParentTransform(heldBy, this, currentReceiver.gunHandler.item);
			else
				currentPosition = currentReceiver.gunHandler.item.InverseTransformPoint(item.position);

			AttachmentPosition attachmentPosition = GetAttachmentPosition(currentReceiver);

			Vector3 position = VRUtils.ClosestPointOnLine(attachmentPosition.entryPosition, attachmentPosition.localPosition, currentPosition);

			//Ignore collision if below entry point
			if (position == attachmentPosition.entryPosition && attachmentPosition.localPosition != attachmentPosition.entryPosition)
			{
				DetatchFromGun();
				return;
			}

			if (autoLoad) 
			{
				autoLoading = true;
			}

			DestroyHighlight();

			//Attach to gun and start along entry path
			item.SetParent(currentReceiver.gunHandler.item);
			item.localPosition = position;
			item.localRotation = attachmentPosition.localRotation;
			VRInteractableItem.FreezeItem(item.gameObject, false, false, true);

			controllerPickupPosition = currentPosition;
			itemPickupPosition = item.localPosition;
		}

		virtual protected void OnTriggerExit(Collider col)
		{
			if (attachedToGun || slideAttachment) return;
			VRAttachmentReceiver attachmentReceiver = col.GetComponent<VRAttachmentReceiver>();
			if (attachmentReceiver == null || attachmentReceiver != currentReceiver) return;

			currentReceiver = null;
			SetHighlightReady(false);
		}

		virtual public void AttachToGunHandler(VRAttachmentReceiver receiver)
		{
			if (receiver == null) return;
			currentReceiver = receiver;
			attachedToGun = true;
			receiver.currentAttachment = this;
			currentAttachmentPosition = GetAttachmentPosition(receiver);
			if (!parents.Contains(receiver.gunHandler)) parents.Add(receiver.gunHandler);
			if (receiver.gunHandler.secondHeld != null && receiver.gunHandler.secondHeld.canBeHeld && !parents.Contains(receiver.gunHandler.secondHeld)) parents.Add(receiver.gunHandler.secondHeld);

			oldShootOrigin = receiver.gunHandler.shootOrigin;
			oldShootDirection = receiver.gunHandler.shootDirection;
			oldShotMode = receiver.gunHandler.shotMode;
			oldBulletsPerShot = receiver.gunHandler.bulletsPerShot;
			oldConeSize = receiver.gunHandler.coneSize;
			if (currentAttachmentPosition.useNewShootDirection)
			{
				receiver.gunHandler.shootOrigin = currentAttachmentPosition.newShootOrigin;
				receiver.gunHandler.shootDirection = currentAttachmentPosition.newShootDirection;
				receiver.gunHandler.shotMode = currentAttachmentPosition.newShotMode;
				receiver.gunHandler.bulletsPerShot = currentAttachmentPosition.newBulletsPerShot;
				receiver.gunHandler.coneSize = currentAttachmentPosition.newConeSize;
			}
			oldLaserPointerEnabled = receiver.gunHandler.laserPointerEnabled;
			if (currentAttachmentPosition.useLaserSight)
			{
				receiver.gunHandler.laserPointerOrigin = currentAttachmentPosition.laserPointOrigin;
				receiver.gunHandler.SetLaserPointerEnabled = true;
			}
			oldFireSound = receiver.gunHandler.fireSound;
			if (currentAttachmentPosition.newFireSound != null)
			{
				receiver.gunHandler.fireSound = currentAttachmentPosition.newFireSound;
			}
			oldDryFireSound = receiver.gunHandler.dryFireSound;
			if (currentAttachmentPosition.newDryFireSound != null)
			{
				receiver.gunHandler.dryFireSound = currentAttachmentPosition.newDryFireSound;
			}

			PlaySound(attachSound);

			//NetworkTransform networkTransform = item.GetComponent<NetworkTransform>();
			//if (networkTransform != null) networkTransform.enabled = false;

			if (attachEvent != null) attachEvent.Invoke();
			if (receiver.attachEvent != null) receiver.attachEvent.Invoke();
			VREvent.Send("AttachmentAttached", new object[]{this});
		}

		virtual public void DetatchFromGunHandler(VRAttachmentReceiver receiver)
		{
			attachedToGun = false;
			receiver.currentAttachment = null;
			parents.Clear();

			receiver.gunHandler.shootOrigin = oldShootOrigin;
			receiver.gunHandler.shootDirection = oldShootDirection;
			receiver.gunHandler.shotMode = oldShotMode;
			receiver.gunHandler.bulletsPerShot = oldBulletsPerShot;
			receiver.gunHandler.coneSize = oldConeSize;

			receiver.gunHandler.SetLaserPointerEnabled = oldLaserPointerEnabled;

			receiver.gunHandler.fireSound = oldFireSound;
			receiver.gunHandler.dryFireSound = oldDryFireSound;

			PlaySound(detatchSound);

			//NetworkTransform networkTransform = item.GetComponent<NetworkTransform>();
			//if (networkTransform != null) networkTransform.enabled = true;

			if (detatchEvent != null) detatchEvent.Invoke();
			if (receiver.detachEvent != null) receiver.detachEvent.Invoke();
			VREvent.Send("AttachmentDetatched", new object[]{this});
		}

		override protected void OnDisable()
		{
			attachedToGun = false;
			currentReceiver = null;
			base.OnDisable();
		}
	}
}
#endif