//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Slide inherits from VRInteractableItem and replaces standard pickup, drop
// behaviour to slide along two points and comminicate to VRGunHandler to eject
// bullets. Also animates back and forward when shooting
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRInteraction;

namespace VRWeaponInteractor
{

	public class VRGunSlide : VRSecondHeld 
	{
		public Vector3 defaultPosition;
		public Vector3 pulledPosition;
		public bool useExtraPositions;
		public List<Vector3> extraPositions = new List<Vector3>();
		public List<Quaternion> extraRotations = new List<Quaternion>();
		public Quaternion defaultRotation;
		public Quaternion pulledRotation;
		public bool useAsSecondHeld;
		public bool animateSlide = true;
		public float slideAnimationTime = 0.2f;
		public bool useScaleCalculation;
		public bool slideSpring = true;
		public bool lockSlideBack = true;
		public bool onlyLockSlideWithMag = false;

		//Editor Vars
		public bool slidePulled;

		private bool _pulled = false;
		private bool _readyToFire = true;
		private Vector3 orignalControllerPos;
		private bool _active = false;
		private int pulledIndex;
		protected Vector3 controllerPickupPosition;
		protected Vector3 itemPickupPosition;
		private bool _slideAnimating;

		public bool pulled
		{
			get { return _pulled; }
		}
		public bool readyToFire
		{
			get { return _readyToFire; }
		}

		override protected void Init()
		{
			base.Init();
			item.SetParent(gunHandler.item);
			item.localPosition = defaultPosition;
			item.localRotation = defaultRotation;
		}

		override protected void Step()
		{
			if (!_active) return;
			if (heldBy != null)
			{
				if (useAsSecondHeld && gunHandler != null && gunHandler.heldBy == null)
				{
					base.Step();
					return;
				}

				Vector3 posDif = controllerPickupPosition - GetLocalControllerPositionToParentTransform(heldBy, this, gunHandler.item);
				Vector3 newItemPos = itemPickupPosition - posDif;

				if (useExtraPositions)
				{
					bool tempPulled = false;
					if (pulledIndex == 0)
					{
						item.localPosition = VRUtils.ClosestPointOnLine(defaultPosition, extraPositions[0], newItemPos);
						item.localRotation = Quaternion.Lerp(defaultRotation, extraRotations[0], VRUtils.TPositionBetweenPoints(defaultPosition, extraPositions[0], item.localPosition));
						if (item.localPosition == extraPositions[0]) pulledIndex++;
						else if (item.localPosition == defaultPosition)
							_readyToFire = true;
					} else if (pulledIndex >= extraPositions.Count)
					{
						var oldPosition = item.localPosition;
						item.localPosition = VRUtils.ClosestPointOnLine(extraPositions[pulledIndex-1], pulledPosition, newItemPos);
						item.localRotation = Quaternion.Lerp(extraRotations[pulledIndex-1], pulledRotation, VRUtils.TPositionBetweenPoints(extraPositions[pulledIndex-1], pulledPosition, item.localPosition));
						if (item.localPosition == pulledPosition)
						{
							tempPulled = true;
							if (oldPosition != pulledPosition) gunHandler.PlaySound(gunHandler.slidePulled);
							Pulled();
						}
						else if (item.localPosition == extraPositions[pulledIndex-1]) pulledIndex--;
					} else
					{
						item.localPosition = VRUtils.ClosestPointOnLine(extraPositions[pulledIndex-1], extraPositions[pulledIndex], newItemPos);
						item.localRotation = Quaternion.Lerp(extraRotations[pulledIndex-1], extraRotations[pulledIndex], VRUtils.TPositionBetweenPoints(extraPositions[pulledIndex-1], extraPositions[pulledIndex], item.localPosition));
						if (item.localPosition == extraPositions[pulledIndex]) pulledIndex++;
						else if (item.localPosition == extraPositions[pulledIndex-1]) pulledIndex--;
					}
					if (!tempPulled) _pulled = false;
				} else
				{
					Vector3 newDefaultPosition = defaultPosition;
					if (lockSlideBack && onlyLockSlideWithMag && gunHandler.currentMagazine != null && _pulled && !gunHandler.hasBullet)
						newDefaultPosition = pulledPosition + ((defaultPosition - pulledPosition)*0.1f);
					var oldPosition = item.localPosition;
					item.localPosition = VRUtils.ClosestPointOnLine(newDefaultPosition, pulledPosition, newItemPos);
					item.localRotation = Quaternion.Lerp(defaultRotation, pulledRotation, VRUtils.TPositionBetweenPoints(newDefaultPosition, pulledPosition, item.localPosition));
					if (item.localPosition == pulledPosition)
					{
						if (oldPosition != pulledPosition) gunHandler.PlaySound(gunHandler.slidePulled);
						if (!onlyLockSlideWithMag || (gunHandler.currentMagazine != null || gunHandler.hasBullet)) Pulled();
					} else if (item.localPosition == defaultPosition)
					{
						_readyToFire = true;
						_pulled = false;
					} else if (!onlyLockSlideWithMag || gunHandler.currentMagazine == null || gunHandler.currentMagazine.bulletCount != 0 || !pulled || gunHandler.hasBullet)
					{
						_pulled = false;
					}
					else
					{
						_pulled = true;
					}
				}
			} else if (slideSpring)
			{
				if (!_pulled)
				{
					item.localPosition = Vector3.MoveTowards(item.localPosition, defaultPosition, 0.005f/gunHandler.item.localScale.magnitude);
					item.localRotation = Quaternion.Lerp(defaultRotation, pulledRotation, VRUtils.TPositionBetweenPoints(defaultPosition, pulledPosition, item.localPosition));
					if (item.localPosition == defaultPosition)
					{
						pulledIndex = 0;
						_readyToFire = true;
						_active = false;
					}
				}
			} else if (!slideSpring)
			{
				_active = false;
			}
		}

		public void Shoot(bool hasBullet)
		{
			if (!animateSlide || ((_slideAnimating || !lockSlideBack) && !hasBullet)) return;
			if (_slideAnimating)
			{
				StopCoroutine(ShootAnimation(hasBullet));
				_slideAnimating = false;
			}
			StartCoroutine(ShootAnimation(hasBullet));
		}

		IEnumerator ShootAnimation(bool hasBullet)
		{
			_slideAnimating = true;
			float startTime = Time.time;
			float t = 0;
			if (!_pulled)
			{
				while (t < 1)
				{
					float currentTime = Time.time;
					t = (currentTime - startTime) / (slideAnimationTime*0.5f);
					item.localPosition = Vector3.Lerp(defaultPosition, pulledPosition, t);
					item.localRotation = Quaternion.Lerp(defaultRotation, pulledRotation, t);
					yield return null;
				}
			}
			if (lockSlideBack && !hasBullet && (!onlyLockSlideWithMag || gunHandler.currentMagazine != null))
			{
				_pulled = true;
				_slideAnimating = false;
				yield break;
			}
			_pulled = false;
			t = 0;
			startTime = Time.time;
			while (t < 1)
			{
				float currentTime = Time.time;
				t = (currentTime - startTime) / (slideAnimationTime*0.5f);
				item.localPosition = Vector3.Lerp(pulledPosition, defaultPosition, t);
				item.localRotation = Quaternion.Lerp(pulledRotation, defaultRotation, t);
				yield return null;
			}
			item.localPosition = defaultPosition;
			item.localRotation = defaultRotation;
			_slideAnimating = false;
		}

		private void Pulled()
		{
			if (_pulled) return;
			_pulled = true;
			gunHandler.SlidePulled();
		}

		public void Release()
		{
			_pulled = false;
			if (item.localPosition != defaultPosition)
			{
				_active = true;
				_readyToFire = false;
			}
		}

		override public bool Pickup(VRInteractor hand)
		{
			if (gunHandler == null) return false;

			controllerPickupPosition = GetLocalControllerPositionToParentTransform(hand, this, gunHandler.item);
			itemPickupPosition = item.localPosition;

			if (useAsSecondHeld)
			{
				bool success = base.Pickup(hand);
				if (!success) return false;
			} else
			{
				heldBy = hand;
				CheckIK(true, hand);
			}
			_active = true;
			_readyToFire = false;
			if (item.localPosition == pulledPosition)
				_pulled = true;
			return true;
		}

		override public void Drop(bool addControllerVelocity, VRInteractor hand = null)
		{
			if (!lockSlideBack || gunHandler.hasBullet || (item.localPosition != pulledPosition && (!onlyLockSlideWithMag || (gunHandler != null && (gunHandler.currentMagazine == null || gunHandler.currentMagazine.bulletCount != 0) && pulled))))
				Release();
			
			if (gunHandler != null) gunHandler.PlaySound(gunHandler.slideRelease);

			if (useAsSecondHeld) 
			{
				base.Drop(addControllerVelocity, hand);
			} else
			{
				CheckIK(false, hand);
				heldBy = null;
			}
		}
	}
}
#endif