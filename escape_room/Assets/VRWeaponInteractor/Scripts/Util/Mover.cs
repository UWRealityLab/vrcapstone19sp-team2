//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Add's constant velocity in specified direction. When Damage is received activates
// gravity. Used in the projectile shooter example from pressing the red button in the
// example scene.
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using System.Collections;

namespace VRWeaponInteractor
{

	[RequireComponent (typeof(Rigidbody))]
	public class Mover : MonoBehaviour {

		public float speed = 0.01f;
		public Vector3 direction;

		Rigidbody body;
		float elapsedTime = 0;
		float moveTime = 0;
		bool alive = true;

		void Start()
		{
			body = GetComponent<Rigidbody>();
			if (body == null) Debug.LogError("No rigidbody attached to gameobject");
		}

		void Update()
		{
			if (!alive || body == null) return;
			elapsedTime += Time.deltaTime;
			if (elapsedTime > moveTime)
			{
				moveTime = elapsedTime + 0.01f;
				body.velocity = direction*speed;
			}
		}

		public void Damage(DamageInfo info)
		{
			alive = false;
			body.AddExplosionForce(100, info.velocity, 100);
			body.useGravity = true;
		}
	}

}
#endif