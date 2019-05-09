#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class VRBeltMagazine : VRMagazine 
	{
		public List<GameObject> beltBullets = new List<GameObject>();

		override public bool Pickup(VRInteractor hand)
		{
			return base.Pickup(hand);
		}

		override public bool TakeBullet()
		{
			if (beltBullets.Count != 0)
			{
				while(beltBullets.Count != 0 && beltBullets[beltBullets.Count-1] == null) beltBullets.RemoveAt(beltBullets.Count-1);
				Destroy(beltBullets[beltBullets.Count-1]);
				beltBullets.RemoveAt(beltBullets.Count-1);
			}
			if (bulletCount == 1)
			{
				Eject();
				Destroy(item.gameObject);
				return true;
			}
			return base.TakeBullet();
		}
	}
}
#endif