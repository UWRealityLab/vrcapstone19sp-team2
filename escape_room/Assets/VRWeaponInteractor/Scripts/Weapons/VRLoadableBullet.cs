//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Loadable bullet inherits from VRInteractableItem and checks if it's currently
// being inserted into a weapon or magazine and tells it to begin loading
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using System.Collections;
using VRInteraction;

namespace VRWeaponInteractor
{

	public class VRLoadableBullet : VRInteractableItem 
	{
		public int bulletId;

		protected bool canBeLoaded;

		VRAmmoPack _ammoPack;
		public VRAmmoPack ammoPack
		{
			get { return _ammoPack; }
			set { _ammoPack = value; }
		}

		override protected void OnEnable()
		{
			base.OnEnable();
			canBeLoaded = false;
			StartCoroutine(LoadedCooldown());
		}

		override protected void OnDisable()
		{
			base.OnDisable();
			StopCoroutine(LoadedCooldown());
		}

		IEnumerator LoadedCooldown()
		{
			yield return new WaitForSeconds(1f);
			canBeLoaded = true;
		}

		void OnTriggerEnter(Collider col)
		{
			if (!enabled || interactionDisabled || (!canBeLoaded && heldBy == null)) return;
			VRBulletReceiver bulletReceiver = col.GetComponent<VRBulletReceiver>();
			if (bulletReceiver != null)
			{
				if (bulletReceiver.gunHandler != null)
				{
					if (bulletReceiver.gunHandler.LoadBullet(this))
					{
						if (_ammoPack != null) _ammoPack.BulletPickedUp(item.gameObject);
					}
				} else if (bulletReceiver.magazine != null)
				{
					if (bulletReceiver.magazine.LoadBullet(this))
					{
						if (_ammoPack != null) _ammoPack.BulletPickedUp(item.gameObject);
					}
				} else if (bulletReceiver.ammoPack != null)
				{
					if (bulletReceiver.ammoPack.BulletDroppedOff(item, bulletReceiver.transform))
					{
						if (_ammoPack != null && _ammoPack != bulletReceiver.ammoPack) _ammoPack.BulletPickedUp(item.gameObject);
					}
				}
			}
		}
	}
}
#endif