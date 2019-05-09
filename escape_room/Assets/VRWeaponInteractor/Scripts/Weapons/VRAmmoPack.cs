#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRInteraction;


namespace VRWeaponInteractor
{
	public class VRAmmoPack : VRInteractableItem 
	{
		[System.Serializable]
		public class AmmoType
		{
			public string name;
			public GameObject prefab;
			public Vector3 rackScale = Vector3.one;
			public Vector3 bulletOffsetPosition;
			public Vector3 bulletRotation;
			public Vector3 fallDirection = new Vector3(0f, 1f, 0f);
			public bool parentBullets;
		}

		private class BulletSlot
		{
			public GameObject bullet;
			public Transform slotTrans;
			public int slotIndex;
			public float lastRemovedTime;
		}

		public int selectedAmmoType;
		public List<Transform> slots = new List<Transform>();
		public List<AmmoType> ammoTypes = new List<AmmoType>();
		public List<string> ammoTypeNames = new List<string>();
		public AudioClip placedSound;
		public AudioClip removedSound;

		private bool initialized;
		private List<BulletSlot> bulletSlots = new List<BulletSlot>();
		private AmmoType ammoType;
		private int bulletId = -1;
		private Transform bulletScaleRef;

		override protected void OnEnable()
		{
			AmmoBeltInit();
			FillRack();
			base.OnEnable();
		}

		void Update()
		{
			//	Using gravity to slide bullet
			Transform parentTrans = item;
			foreach(BulletSlot bulletSlot in bulletSlots)
			{
				if (bulletSlot.bullet == null) continue;
				Vector3 loadedPos = bulletSlot.slotTrans.localPosition + ammoType.bulletOffsetPosition;
				Vector3 ejectedPos = bulletSlot.slotTrans.localPosition + ammoType.bulletOffsetPosition + (ammoType.fallDirection*0.02f);
				Vector3 direction = loadedPos - ejectedPos;
				float gravity = parentTrans.TransformPoint(ejectedPos).y - parentTrans.TransformPoint(loadedPos).y;
				bulletSlot.bullet.transform.localPosition = VRUtils.ClosestPointOnLine(ejectedPos, loadedPos, bulletSlot.bullet.transform.localPosition+(direction*gravity));
				if (bulletSlot.bullet.transform.localPosition == ejectedPos)
				{
					BulletEjected(bulletSlot);
				}
			}
		}

		void AmmoBeltInit()
		{
			if (initialized) return;
			if (ammoTypes.Count == 0)
			{
				Debug.LogError("No Ammo Types", gameObject);
				return;
			}
			if (selectedAmmoType > ammoTypes.Count)
			{
				Debug.LogError("Selected ammo type out of range", gameObject);
				return;
			}
			initialized = true;
			ammoType = ammoTypes[selectedAmmoType];
			Quaternion oldRot = item.rotation;
			item.rotation = Quaternion.identity;
			item.localScale = ammoType.rackScale;
			GameObject scaleRef = new GameObject("Bullet Scale Ref");
			scaleRef.transform.parent = item;
			scaleRef.transform.localPosition = Vector3.zero;
			scaleRef.transform.localRotation = Quaternion.identity;
			item.rotation = oldRot;
			bulletScaleRef = scaleRef.transform;
			for(int i=0; i<slots.Count; i++)
			{
				BulletSlot slot = new BulletSlot();
				slot.slotIndex = i;
				slot.slotTrans = slots[i];
				VRBulletReceiver bulletReceiver = slots[i].gameObject.AddComponent<VRBulletReceiver>();
				bulletReceiver.ammoPack = this;
				bulletSlots.Add(slot);
			}
			if (ammoType.prefab == null)
			{
				Debug.LogError("No prefab on picked ammo type", gameObject);
				return;
			}
			VRLoadableBullet loadableBullet = ammoType.prefab.GetComponentInChildren<VRLoadableBullet>();
			if (loadableBullet != null) bulletId = loadableBullet.bulletId;
		}

		public void FillRack()
		{
			AmmoBeltInit();
			ClearRack();
			foreach(BulletSlot bulletSlot in bulletSlots)
			{
				GameObject newBullet = (GameObject)Instantiate(ammoType.prefab, item, false);
				AttachBullet(newBullet, bulletSlot);
			}
		}

		private void BulletPickedUp()
		{
			foreach(BulletSlot bulletSlot in bulletSlots)
			{
				if (bulletSlot.bullet == null || bulletSlot.bullet.transform.parent == item) continue;
				BulletPickedUp(bulletSlot);
				break;
			}
		}

		public void BulletPickedUp(GameObject loadableBullet)
		{
			foreach(BulletSlot bulletSlot in bulletSlots)
			{
				if (bulletSlot.bullet != loadableBullet) continue;
				BulletPickedUp(bulletSlot);
			}
		}

		private void BulletPickedUp(BulletSlot bulletSlot)
		{
			VRInteractableItem loadableBullet = bulletSlot.bullet.GetComponentInChildren<VRInteractableItem>();
			VRLoadableBullet bulletScript = bulletSlot.bullet.GetComponentInChildren<VRLoadableBullet>();
			if (bulletScript != null) bulletScript.ammoPack = null;
			loadableBullet.pickupEvent.RemoveAllListeners();
			if (ammoType.parentBullets) loadableBullet.parents.Clear();
			bulletSlot.bullet = null;
			if (removedSound != null)
			{
				AudioSource source = GetComponent<AudioSource>();
				if (source == null) source = gameObject.AddComponent<AudioSource>();
				source.loop = false;
				source.clip = removedSound;
				source.Play();
			}
			foreach(BulletSlot otherSlots in bulletSlots)
			{
				otherSlots.lastRemovedTime = Time.time;
			}
		}

		void BulletEjected(BulletSlot bulletSlot)
		{
			bulletSlot.bullet.transform.parent = null;
			VRInteractableItem loadableBullet = bulletSlot.bullet.GetComponentInChildren<VRInteractableItem>();
			VRLoadableBullet bulletScript = bulletSlot.bullet.GetComponentInChildren<VRLoadableBullet>();
			if (bulletScript != null) bulletScript.ammoPack = null;
			loadableBullet.pickupEvent.RemoveAllListeners();
			if (ammoType.parentBullets) loadableBullet.parents.Clear();
			VRInteractableItem.UnFreezeItem(bulletSlot.bullet);
			bulletSlot.bullet = null;
			foreach(BulletSlot otherSlots in bulletSlots)
			{
				otherSlots.lastRemovedTime = Time.time;
			}
		}

		public bool BulletDroppedOff(Transform newBullet, Transform slotTrans)
		{
			if (newBullet == null) return false;
			VRLoadableBullet loadableBullet = newBullet.GetComponentInChildren<VRLoadableBullet>();
			if (loadableBullet == null || loadableBullet.bulletId != bulletId) return false;
			foreach(BulletSlot bulletSlot in bulletSlots)
			{
				if (bulletSlot.slotTrans != slotTrans) continue;
				if (bulletSlot.bullet != null || bulletSlot.lastRemovedTime + 1 > Time.time) return false;
				foreach(BulletSlot otherSlots in bulletSlots)
				{
					if (newBullet.gameObject == otherSlots.bullet)
						return false;
				}
				
				if (loadableBullet != null && bulletId != -1 && bulletId != loadableBullet.bulletId) return false;
				if (placedSound != null)
				{
					AudioSource source = GetComponent<AudioSource>();
					if (source == null) source = gameObject.AddComponent<AudioSource>();
					source.loop = false;
					source.clip = placedSound;
					source.Play();
				}
				AttachBullet(newBullet.gameObject, bulletSlot);
			}
			return true;
		}

		private void AttachBullet(GameObject bullet, BulletSlot bulletSlot)
		{
			VRInteractableItem loadableBullet = bullet.GetComponentInChildren<VRInteractableItem>();
			if (loadableBullet == null)
			{
				Debug.LogError("No VRInteractableItem script found on bullet prefab", gameObject);
				return;
			}
			VRLoadableBullet bulletScript = bullet.GetComponentInChildren<VRLoadableBullet>();
			if (bulletScript != null) bulletScript.ammoPack = this;
			if (loadableBullet.heldBy != null) loadableBullet.heldBy.Drop();
			if (ammoType.parentBullets) loadableBullet.parents.Add(this);

			bullet.transform.SetParent(item);
			bullet.transform.localPosition = slots[bulletSlot.slotIndex].localPosition+ammoType.bulletOffsetPosition;
			bullet.transform.localRotation = slots[bulletSlot.slotIndex].localRotation*Quaternion.Euler(ammoType.bulletRotation);
			bullet.transform.localScale = new Vector3(
				bulletScaleRef.localScale.x*ammoType.prefab.transform.localScale.x,
				bulletScaleRef.localScale.y*ammoType.prefab.transform.localScale.y,
				bulletScaleRef.localScale.z*ammoType.prefab.transform.localScale.z);
			VRInteractableItem.FreezeItem(bullet, false, false, true);
			loadableBullet.pickupEvent.AddListener(delegate{BulletPickedUp();});
			bulletSlot.bullet = bullet;
		}

		public void ClearRack()
		{
			foreach(BulletSlot bulletSlot in bulletSlots)
			{
				if (bulletSlot.bullet == null) continue;
				Destroy(bulletSlot.bullet);
				bulletSlot.lastRemovedTime = Time.time;
			}
		}
	}
}
#endif