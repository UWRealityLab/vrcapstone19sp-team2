#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class VRPhysicalBulletRigidbody : VRPhysicalBullet
	{
		public Rigidbody body;
		public Vector3 constantForce;

		protected override void OnEnable()
		{
			body.AddForce(bulletRoot.forward * speed, ForceMode.Impulse);
		}

		protected override void Update()
		{
			body.AddForce(constantForce, ForceMode.Force);
		}
	}
}
#endif