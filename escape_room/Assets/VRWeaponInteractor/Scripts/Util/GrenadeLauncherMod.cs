#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class GrenadeLauncherMod : MonoBehaviour
	{
		public float initialVelocity = 10f;

		void Start ()
		{
			VRGrenade grenade = GetComponentInChildren<VRGrenade>();
			if (grenade != null) grenade.ArmGrenade();

			Rigidbody body = GetComponent<Rigidbody>();
			if (body != null) body.velocity = transform.forward * initialVelocity;
		}
	}
}
#endif