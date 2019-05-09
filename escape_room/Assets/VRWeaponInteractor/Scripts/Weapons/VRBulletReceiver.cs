#if VRInteraction
//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Attached to collider on the weapon to link a bullet with a weapon or magazine
//
//=============================================================================


using UnityEngine;
using System.Collections;

namespace VRWeaponInteractor
{
	public class VRBulletReceiver : MonoBehaviour 
	{
		public VRGunHandler gunHandler;
		public VRMagazine magazine;
		public VRAmmoPack ammoPack;
	}

}
#endif