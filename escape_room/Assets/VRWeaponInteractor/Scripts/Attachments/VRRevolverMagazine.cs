//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Extension to the magazine script that adds opening and closing and
// ejecting of bullets from the drum using gravity
//
//=============================================================================
#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class VRRevolverMagazine : VRMagazine
	{
		public Vector3 defaultPosition;
		public Quaternion defaultRotation;
		public Vector3 openPosition;
		public Quaternion openRotation;
		public float transitionTime = 0.5f;
		public VRGunHandler gunHandler;

		private VRGunHandler.FiringMode oldFiringMode;

		private bool _open;
		public bool isOpen
		{
			get { return _open; }
			set
			{
				_open = value;
				if (gunHandler == null) return;
				canAcceptBullets = _open;
				if (_open)
				{
					//Stop gun from firing
					oldFiringMode = gunHandler.firingMode;
					gunHandler.firingMode = VRGunHandler.FiringMode.SAFE;
				} else
				{
					//Allow gun to fire
					gunHandler.firingMode = oldFiringMode;
				}
			}
		}

		protected override void Init ()
		{
			base.Init();
			if (item == null || gunHandler == null) return;
			oldFiringMode = gunHandler.firingMode;
			_open = true;
			isOpen = false;
			item.localPosition = defaultPosition;
			item.localRotation = defaultRotation;
		}

		override protected void Step()
		{
		}

		override public bool Pickup(VRInteractor hand)
		{
			ToggleDrum();
			return false;
		}

		override public void Drop(bool addControllerVelocity, VRInteractor hand = null)
		{
		}

		public void ToggleDrum()
		{
			StopAllCoroutines();
			StartCoroutine(togglePosition());
		}

		IEnumerator togglePosition()
		{
			isOpen = !isOpen;

			if (isOpen && attachSound != null) PlaySound(attachSound);
			else if (!isOpen && detatchSound != null) PlaySound(detatchSound);

			if (!isOpen) EjectLoadedBulletsStop();
			Vector3 targetPosition = isOpen ? openPosition : defaultPosition;
			Quaternion targetRotation = isOpen ? openRotation : defaultRotation;
			float elapsedTime = 0;
			float t = 0;
			while (t<1)
			{
				elapsedTime += Time.deltaTime;
				item.localPosition = Vector3.Lerp(item.localPosition, targetPosition, t);
				item.localRotation = Quaternion.Lerp(item.localRotation, targetRotation, t);
				t = elapsedTime / transitionTime;
				yield return null;
			}
			if (isOpen) EjectLoadedBullets();
			item.localPosition = targetPosition;
			item.localRotation = targetRotation;
		}

		public void EjectLoadedBullets()
		{
			for(int i=0; i<loadedBullets.Count; i++)
			{
				if (loadedBullets[i] == null) continue;
				StartCoroutine(slideEjectingBullets(loadedBullets[i].transform, bulletPositions[i], bulletEjectionPositions[i], i));
			}
		}

		public void EjectLoadedBulletsStop()
		{
			for(int i=0; i<loadedBullets.Count; i++)
			{
				if (loadedBullets[i] == null) continue;
				loadedBullets[i].transform.localPosition = bulletPositions[i];
				loadedBullets[i].transform.localRotation = bulletRotations[i];
				VRInteractableItem.FreezeItem(loadedBullets[i], true);
			}
		}

		IEnumerator slideEjectingBullets(Transform bullet, Vector3 loadedPos, Vector3 ejectedPos, int index)
		{
			while(true)
			{
				//	Using gravity to slide bullet
				Vector3 direction = loadedPos - ejectedPos;
				Transform parentTrans = bulletParent == null ? item : bulletParent;
				float gravity = parentTrans.TransformPoint(ejectedPos).y - parentTrans.TransformPoint(loadedPos).y;
				bullet.localPosition = VRUtils.ClosestPointOnLine(ejectedPos, loadedPos, bullet.localPosition+(direction*gravity));

				if (bullet.localPosition == ejectedPos)
				{
					bullet.parent = null;
					VRInteractableItem bulletItem = bullet.GetComponentInChildren<VRInteractableItem>();
					if (bulletItem != null)
					{
						bulletItem.interactionDisabled = false;
						bulletItem.parents.Clear();
						if (bulletItem.GetType() == typeof(VRLoadableBullet)) _bulletCount--;
					}
					DestroyIn destroyIn = bullet.GetComponentInChildren<DestroyIn>();
					if (destroyIn != null) destroyIn.enabled = true;
					VRInteractableItem.UnFreezeItem(bullet.gameObject);
					loadedBullets[index] = null;
					yield break;
				}
				yield return null;
			}
		}
	}
}
#endif