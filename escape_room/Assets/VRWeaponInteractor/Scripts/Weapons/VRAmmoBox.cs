#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class VRAmmoBox : VRInteractableItem
	{
		public GameObject ammoBeltPrefab;
		public List<GameObject> bulletBelts = new List<GameObject>();

		private Rigidbody body;

		override public bool Pickup(VRInteractor hand)
		{
			if (bulletBelts.Count == 0) return false;

			Destroy(bulletBelts[bulletBelts.Count-1]);
			bulletBelts.RemoveAt(bulletBelts.Count-1);

			if (body == null) body = GetComponentInParent<Rigidbody>();
			GameObject ammoBeltInstance = Instantiate<GameObject>(ammoBeltPrefab, hand.getControllerAnchorOffset.position, hand.getControllerAnchorOffset.rotation);
			VRBeltMagazine beltMagazine = ammoBeltInstance.GetComponentInChildren<VRBeltMagazine>();
			hand.hoverItem = beltMagazine;
			hand.TryPickup();
			return false;
		}

		/*virtual public void Drop(bool addControllerVelocity)
		{
			base.Drop(addControllerVelocity);
		}*/
	}
}
#endif