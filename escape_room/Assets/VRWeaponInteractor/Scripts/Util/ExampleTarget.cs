//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Used on the targets in the example scene. The takaway is the Damage method which
// is how your Damage method should look like by default to receive damage from
// VRWeaponInteractor weapons.
// As an example to show it's working rotates back and then resets when shot
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using System.Collections;

namespace VRWeaponInteractor
{

	public class ExampleTarget : MonoBehaviour {

		public float resetTime = 5f;
		public float fallTime = 1f;
		public Vector3 upRotation;
		public Vector3 downRotation;
		public Transform forwardTransform;

		private bool _animating;

		public void Damage(float damage)
		{
			if (_animating) return;
			_animating = true;
			StartCoroutine(ToggleRotation(true));
		}

		IEnumerator ToggleRotation(bool fall)
		{
			float elapsedTime = 0f;
			float t = 0f;
			Quaternion startRotation = transform.rotation;
			Quaternion targetRotation = fall ? Quaternion.LookRotation(forwardTransform.up, -forwardTransform.forward) : Quaternion.LookRotation(forwardTransform.forward, forwardTransform.up);
			while(t<=1)
			{
				elapsedTime += Time.deltaTime;
				t = elapsedTime/(fall ? fallTime : resetTime);
				transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
				yield return null;
			}
			if (!fall) _animating = false;
			else StartCoroutine(ToggleRotation(false));
		}
	}
}
#endif