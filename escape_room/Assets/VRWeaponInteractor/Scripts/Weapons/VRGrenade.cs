//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Grenade base class. You can inherit from this class and override the fuseTimer
// method to make a custom lead up to explosion. Like playing sounds or an animation.
//
//===================Contact Email: Sam@MassGames.co.uk===========================
#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class VRGrenade : VRInteractableItem
	{
		public float fuseTime = 4f;
		public bool useRing = true;
		public GameObject lever;
		public GameObject explosionPrefab;

		private bool timerStarted;
		private bool ringDetached;
		private bool leverEjected;

		/// <summary>
		/// This method is called by the trigger by default when holding the grenade.
		/// If you are not using the ring or the ring has been removed then this will
		/// begin the fuse timer. Used for cooking a grenade before you throw it.
		/// </summary>
		override protected void ACTION(VRInteractor hand)
		{
			if ((ringDetached || !useRing) && !leverEjected)
				ArmGrenade();
			else base.ACTION(hand);
		}

		/// <summary>
		/// If use ring is true this will allow the lever to be ejected, which will start the fuse.
		/// </summary>
		/// <param name="ring">Ring item.</param>
		public void RingPulled(VRInteractableItem ring)
		{
			if (!useRing) return;
			Joint joint = ring.GetComponent<Joint>();
			if (joint != null) Destroy(joint);
			ring.parents.Clear();
			VRInteractableItem.UnFreezeItem(ring.item.gameObject);
			ringDetached = true;
		}

		override public void Drop(bool addControllerVelocity, VRInteractor hand = null)
		{
			if ((ringDetached || !useRing) && !leverEjected)
			{
				ArmGrenade();
			}
			base.Drop(addControllerVelocity, hand);
		}

		/// <summary>
		/// This is the arming method. If there is a lever it will be ejected then the fuse timer will begin.
		/// </summary>
		virtual public void ArmGrenade()
		{
			leverEjected = true;
			if (lever != null)
			{
				VRInteractableItem leverItem = lever.GetComponentInChildren<VRInteractableItem>();
				if (leverItem != null)
				{
					leverItem.parents.Clear();
					leverItem.InteractionDisabled = false;
				}
				VRInteractableItem.UnFreezeItem(lever);
				Rigidbody leverBody = lever.GetComponent<Rigidbody>();
				if (leverBody != null)
				{
					Rigidbody grenadeBody = item.GetComponent<Rigidbody>();
					if (grenadeBody != null)
					{
						leverBody.velocity = grenadeBody.velocity;
						leverBody.angularVelocity = grenadeBody.angularVelocity;
					}
					leverBody.AddForce(lever.transform.forward*100f);
				}
				lever.transform.SetParent(null);
				DestroyIn destroyLever = lever.GetComponent<DestroyIn>();
				if (destroyLever == null) destroyLever = lever.AddComponent<DestroyIn>();
				destroyLever.seconds = 5f;
			}

			StartCoroutine(fuseTimer());
		}

		/// <summary>
		/// Countdown to explosion. If you want a custom lead up like sounds and animations then this is the method you'll want to override
		/// in an inherting class.
		/// </summary>
		/// <returns>The timer.</returns>
		virtual protected IEnumerator fuseTimer()
		{
			if (timerStarted) yield break;
			timerStarted = true;
			yield return new WaitForSeconds(fuseTime);
			if (heldBy != null) heldBy.Drop();
			if (explosionPrefab != null) Instantiate(explosionPrefab, item.position, Quaternion.identity);
			Destroy(item.gameObject);
		}
	}
}
#endif