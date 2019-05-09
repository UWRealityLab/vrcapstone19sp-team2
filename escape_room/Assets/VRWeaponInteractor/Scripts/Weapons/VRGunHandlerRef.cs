//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Used in conjuction with VRGunHandlerRefEditor to open the weapon editor on
// the parent object. Just for your convenience :).
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using System.Collections;

namespace VRWeaponInteractor
{
	public class VRGunHandlerRef : MonoBehaviour 
	{
		public VRGunHandler gunHandler;
	}
}
#endif