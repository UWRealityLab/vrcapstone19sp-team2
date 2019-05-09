//========= Copyright 2019, Sam Tague, All rights reserved. ===================
//
// VR arrow item. Same as a normal item but checks OnTriggerEnter and looks for
// a bow script.
//
//=============================================================================
#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class VRArrow : VRInteractableItem 
	{
		private VRBow _bow;

		private void OnTriggerEnter(Collider col)
		{
			if (heldBy == null) return;
			VRBow bow = col.GetComponent<VRBow>();
			if (bow == null || bow.arrowInstance != null) return;
			bow.arrowInstance = this;
			_bow = bow;
		}
	}
}
#endif