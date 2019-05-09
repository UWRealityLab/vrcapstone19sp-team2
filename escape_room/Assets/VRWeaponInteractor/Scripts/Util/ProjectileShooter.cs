//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Instantiates specified prefab when Activate is called (called by VRExampleButton)
// Tells an attached Mover the direction to move if one is found.
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using System.Collections;

namespace VRWeaponInteractor
{

	public class ProjectileShooter : MonoBehaviour {

		public GameObject projectilePrefab;

		public void Activate()
		{
			GameObject projectileInstance = (GameObject)Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity);
			projectileInstance.transform.position = transform.position+(transform.forward*0.1f);
			Mover projectile = projectileInstance.GetComponent<Mover>();
			if (projectile != null) projectile.direction = transform.forward;
		}
	}
}
#endif