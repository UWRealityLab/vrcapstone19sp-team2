//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Magazine inherits from VRInteractableItem and acts as a container for bullets.
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRInteraction;

namespace VRWeaponInteractor
{

	public class VRMagazine : VRAttachment {

		//Settings
		public bool infiniteAmmo = false;
		public int clipSize = 16;
		public int bulletId;
		public bool addBulletOnLoad = true; //If there is a slide you can chose not to load the bullet as the magazine is entered and instead wait for the slide to be pulled
		public bool replaceBulletsWithSpentCasings;
		public bool startFull = true;

		//References
		public BoxCollider bulletReceiver;

		//Variables
		//public Vector3 defaultLoadedPosition = Vector3.zero;
		//public Vector3 entryPosition = Vector3.zero;
		//public Quaternion defaultRotation = Quaternion.identity;

		public GameObject bulletPrefab;
		public Transform bulletParent;
		public List<bool> bulletVisible = new List<bool>();
		public List<Vector3> bulletPositions = new List<Vector3>();
		public List<Quaternion> bulletRotations = new List<Quaternion>();
		public List<Vector3> bulletEjectionPositions = new List<Vector3>();
		public AudioClip loadBulletSound;

		//References
		protected List<GameObject> loadedBullets = new List<GameObject>();
		//protected VRGunHandler _currentGun;

		//Variables
		//protected bool autoLoading;
		//protected bool loaded = false;
		protected int _bulletCount;
		protected bool _canAcceptBullets = true;
		protected bool _initialized;
		//protected Vector3 controllerPickupPosition;
		//protected Vector3 itemPickupPosition;

		//	Getters / Setters
		public int bulletCount
		{
			get 
			{
				if (infiniteAmmo) return 1;
				return _bulletCount;
			}
			set { _bulletCount = value; }
		}

		public bool canAcceptBullets
		{
			get { return _canAcceptBullets; }
			set { _canAcceptBullets = value; }
		}

		void Awake () 
		{
			Init();
		}

		override protected void Init()
		{
			if (_initialized) return;
			_initialized = true;

			if (startFull) _bulletCount = clipSize;
			if (bulletPrefab != null)
			{
				//	Spawn bullets
				for(int i=0; i<bulletVisible.Count;i++)
				{
					if (!bulletVisible[i])
					{
						loadedBullets.Add(null);
						continue;
					}
					if (bulletPositions.Count <= i) break;
					if (bulletRotations.Count <= i) break;

					GameObject newBullet = (GameObject)Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
					newBullet.transform.SetParent(bulletParent == null ? item : bulletParent);
					newBullet.transform.localPosition = bulletPositions[i];
					newBullet.transform.localRotation = bulletRotations[i];
					VRInteractableItem bulletItem = newBullet.GetComponentInChildren<VRInteractableItem>();
					if (bulletItem != null)
					{
						bulletItem.interactionDisabled = true;
						bulletItem.parents.Add(this);
					}
					VRInteractableItem.FreezeItem(newBullet, true);
					loadedBullets.Add(newBullet);
				}
			}
			base.Init();
			//	Lock the magazine to the gun or activate physics
			if (currentReceiver != null)
			{
				item.SetParent(currentReceiver.gunHandler.item);
				AttachmentPosition attachmentPosition = GetAttachmentPosition(currentReceiver);
				item.localPosition = attachmentPosition.localPosition;
				item.localRotation = attachmentPosition.localRotation;
				VRInteractableItem.FreezeItem(item.gameObject, false, false, true);
				attachedToGun = true;
			}
		}

		public bool LoadBullet(VRLoadableBullet bullet, bool forwardFromGun = false)
		{
			if (bullet == null || bulletId != bullet.bulletId || !canAcceptBullets) return false;
			if (!forwardFromGun && currentReceiver != null && attachedToGun && currentReceiver.gunHandler.slide != null) return false;
			if (bulletCount >= clipSize || (loadedBullets.Count > bulletCount && loadedBullets[bulletCount] != null)) return false;
			if (bullet.heldBy != null) bullet.heldBy.Drop();
			_bulletCount++;
			PlaySound(loadBulletSound);
			if (bulletPrefab != null)
			{
				if (bulletVisible.Count <= bulletCount-1 || !bulletVisible[bulletCount-1] || bulletPositions.Count <= bulletCount-1 || bulletRotations.Count <= bulletCount-1)
				{
					Destroy(bullet.item.gameObject);
				} else
				{
					bullet.item.SetParent(bulletParent == null ? item : bulletParent);
					bullet.item.localPosition = bulletPositions[bulletCount-1];
					bullet.item.localRotation = bulletRotations[bulletCount-1];
					VRInteractableItem bulletItem = bullet.item.GetComponentInChildren<VRInteractableItem>();
					if (bulletItem != null) 
					{
						bulletItem.interactionDisabled = true;
						bulletItem.parents.Clear();
						bulletItem.parents.Add(this);
					}
					VRInteractableItem.FreezeItem(bullet.item.gameObject, true);
					loadedBullets[bulletCount-1] = bullet.item.gameObject;
				}
			} else
			{
				Destroy(bullet.item.gameObject);
			}
			return true;
		}

		virtual public bool TakeBullet()
		{
			if (!_initialized) Init();
			if (infiniteAmmo) return true;
			if (bulletCount <= 0) return false;

			if (bulletVisible.Count >= bulletCount && bulletVisible[bulletCount-1] && loadedBullets[bulletCount-1] != null)
			{
				Destroy(loadedBullets[bulletCount-1]);
				loadedBullets[bulletCount-1] = null;
				if (replaceBulletsWithSpentCasings && currentReceiver != null && currentReceiver.gunHandler.spentBulletPrefab != null)
				{
					//Find first empty slot
					int emptyIndex = 0;
					for(int i=loadedBullets.Count-1; i<loadedBullets.Count;i--)
					{
						if (loadedBullets[i] == null)
						{
							emptyIndex = i;
							break;
						}
					}
					//Instanstiate spent casing into slot
					GameObject bulletCasing = (GameObject)Instantiate(currentReceiver.gunHandler.spentBulletPrefab);
					bulletCasing.transform.parent = bulletParent == null ? item : bulletParent;
					bulletCasing.transform.localPosition = bulletPositions[emptyIndex];
					bulletCasing.transform.localRotation = bulletRotations[emptyIndex];

					DestroyIn destroyIn = bulletCasing.GetComponentInChildren<DestroyIn>();
					if (destroyIn != null) destroyIn.enabled = false;

					VRInteractableItem.FreezeItem(bulletCasing, true);

					loadedBullets[emptyIndex] = bulletCasing;
				}
			}

			_bulletCount--;
			return true;
		}

		override public void AttachToGunHandler(VRAttachmentReceiver receiver)
		{
			if (receiver == null) return;
			receiver.gunHandler.currentMagazine = this;
			base.AttachToGunHandler(receiver);
		}

		override public void DetatchFromGunHandler(VRAttachmentReceiver receiver)
		{
			receiver.gunHandler.currentMagazine = null;
			base.DetatchFromGunHandler(receiver);
		}

		override protected void OnDisable()
		{
			if (currentReceiver != null) currentReceiver.gunHandler.currentMagazine = null;
			base.OnDisable();
		}
	}
}
#endif