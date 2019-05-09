using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if VRInteraction
namespace VRWeaponInteractor
{
	public class VRSlideAnimation : MonoBehaviour {

		public VRGunHandler gunHandler;
		public Vector3 defaultPosition;
		public Vector3 pulledPosition;
		public float slideAnimationTime;

		private bool _pulled;

		void Start()
		{
			VREvent.Listen("PreShoot", Shoot);
		}

		void OnDestroy()
		{
			VREvent.Remove("PreShoot", Shoot);
		}

		public void Shoot(object[] args)
		{
			VRGunHandler sender = (VRGunHandler)args[0];
			bool hasBullet = (bool)args[1];
			if (sender != gunHandler) return;
			StopAllCoroutines();
			StartCoroutine(ShootAnimation(hasBullet));
		}

		public void Release()
		{
			if (transform.localPosition != defaultPosition)
			{
				_pulled = true;
				StopAllCoroutines();
				StartCoroutine(ShootAnimation(true));
				if (gunHandler != null) gunHandler.PlaySound(gunHandler.slideRelease);
			}
		}

		IEnumerator ShootAnimation(bool hasBullet)
		{
			float startTime = Time.time;
			float t = 0;
			if (!_pulled)
			{
				while (t < 1)
				{
					float currentTime = Time.time;
					t = (currentTime - startTime) / (slideAnimationTime*0.5f);
					transform.localPosition = Vector3.Lerp(defaultPosition, pulledPosition, t);
					yield return null;
				}
			}
			if (!hasBullet)
			{
				_pulled = true;
				yield break;
			}
			_pulled = false;
			t = 0;
			startTime = Time.time;
			while (t < 1)
			{
				float currentTime = Time.time;
				t = (currentTime - startTime) / (slideAnimationTime*0.5f);
				transform.localPosition = Vector3.Lerp(pulledPosition, defaultPosition, t);
				yield return null;
			}
			transform.localPosition = defaultPosition;
		}
	}
}
#endif