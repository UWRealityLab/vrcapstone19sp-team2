#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class VRRevolverSpeedLoader : VRInteractableItem
	{
		public GameObject bulletPrefab;
		public int clipSize;
		public List<Vector3> bulletPositions = new List<Vector3>();
		public List<Quaternion> bulletRotations = new List<Quaternion>();

		override protected void Init()
		{
			base.Init();
			if (item == null || bulletPrefab == null || bulletPositions.Count == 0) return;

			var oldRot = item.localRotation;
			item.localRotation = Quaternion.identity;
			for(int i=0; i<bulletPositions.Count; i++)
			{
				GameObject bulletInstance = (GameObject)Instantiate(bulletPrefab);
				bulletInstance.transform.parent = item;
				bulletInstance.transform.localPosition = bulletPositions[i];
				bulletInstance.transform.localRotation = bulletRotations[i];
				VRInteractableItem interactableItem = bulletInstance.GetComponentInChildren<VRInteractableItem>();
				if (interactableItem != null)
				{
					interactableItem.parents.Add(this);
				}
				VRInteractableItem.FreezeItem(bulletInstance, false, false, true);
			}
			item.localRotation = oldRot;
		}
	}
}
#endif