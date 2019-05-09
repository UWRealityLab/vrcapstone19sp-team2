#if VRInteraction
//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Button for changing fire mode
//
//=============================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class VRChangeFireModeButton : VRInteractableItem 
	{
		public VRGunHandler targetWeapon;
		public List<VRGunHandler.FiringMode> modes = new List<VRGunHandler.FiringMode>();

		private int index;

		override protected void Init()
		{
			ChangeFiringMode();
			base.Init();
		}

		override protected void Step()
		{
		}

		override public bool Pickup(VRInteractor hand)
		{
			index++;
			if (index >= modes.Count) index = 0;
			if (pickupEvent != null) pickupEvent.Invoke();
			ChangeFiringMode();
			return false;
		}
      //  void OnTriggerEnter(Collider other)
    //    {
      //      if (other.tag == "Lindexfinger")
      //      {
       //         index++;
       //         if (index >= modes.Count) index = 0;
		//		ChangeFiringMode();
                //   return false;
       //     }
     //   }
     public void Changefireremote()
        {
            index++;
            if (index >= modes.Count) index = 0;
            if (pickupEvent != null) pickupEvent.Invoke();
            ChangeFiringMode();
          //  return false;
        }
		override public void Drop(bool addControllerVelocity, VRInteractor hand = null)
		{
		}

		void ChangeFiringMode()
		{
			targetWeapon.firingMode = modes[index];
		}
	}

}
#endif