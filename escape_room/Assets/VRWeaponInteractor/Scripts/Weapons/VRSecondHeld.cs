//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// SecondHeld inherits from VRInteractableItem but overrides default pickup, drop
// behaviour to tell the gun handler to take into account the other controller when
// positioning. The actual positional code can be found in VRGunHandler at the
// beginning of the Step method.
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using System.Collections;
using VRInteraction;

namespace VRWeaponInteractor
{

	public class VRSecondHeld : VRInteractableItem
	{
		public float recoilKick = 1f;
		public float angularRecoilKick = 0.25f;
		public float recoilRecovery = 1f;
		public float angularRecoilMultiStep = 0.2f;

		public VRGunHandler gunHandler;

		override protected void Init()
		{
			base.Init();
			if (gunHandler == null)
			{
				foreach(VRInteractableItem item in parents)
				{
					if (item == null || item.GetType() != typeof(VRGunHandler)) continue;
					gunHandler = (VRGunHandler)item;
					break;
				}
			}
		}

		override protected void Step()
		{
			if (heldBy == null || gunHandler == null || gunHandler.heldBy != null) return;
			base.Step();
		}

		override public bool Pickup(VRInteractor hand)
		{
			if (gunHandler != null)
			{
				heldBy = hand;
				gunHandler.usingSecondHeld = true;
				CheckIK(true, hand);
				PlaySound(pickupSound);
				return true;
			}
			return false;
		}

		override public void Drop(bool addControllerVelocity, VRInteractor hand = null)
		{
			if (gunHandler != null)
			{
				gunHandler.usingSecondHeld = false;
				CheckIK(false, hand);
				PlaySound(dropSound);
				heldBy = null;
				if (gunHandler.heldBy == null) gunHandler.Drop(addControllerVelocity, hand);
			} else heldBy = null;
		}
	}

}
#endif