//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Data class holding information relating to a shot
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using System.Collections;

namespace VRWeaponInteractor
{
	public class DamageInfo 
	{
		public int damage;
		public VRGunHandler weapon;
		public RaycastHit hitInfo;
		public Vector3 velocity;
	}
}
#endif