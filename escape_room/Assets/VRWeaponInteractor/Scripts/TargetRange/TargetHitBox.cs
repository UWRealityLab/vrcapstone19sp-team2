using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRWeaponInteractor
{
	public class TargetHitBox : MonoBehaviour 
	{
		public TargetRangeTarget target;
		public int points;

		public void Damage(int damage)
		{
			target.PlayHit(points);
		}
	}
}