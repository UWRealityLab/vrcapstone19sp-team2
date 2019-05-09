#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class VRTripod : VRInteractor
	{
		VRSecondHeld secondHeld;

		void OnTriggerEnter(Collider col)
		{
			secondHeld = col.gameObject.GetComponent<VRSecondHeld>();
			if (secondHeld != null)
			{
				secondHeld.Pickup(this);
            }
 
		}

		void OnTriggerExit(Collider col)
		{
			secondHeld = col.gameObject.GetComponent<VRSecondHeld>();
			if (secondHeld != null)
			{
				secondHeld.Drop(false);
				secondHeld = null;
			}
		}

		override public void TriggerHapticPulse (int frames)
		{
		//	throw new System.NotImplementedException ();
		}

		override public void Drop ()
		{
			if (secondHeld != null)
			{
				secondHeld.Drop(false);
				secondHeld = null;
			}
		}
	}
}
#endif