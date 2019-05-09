//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Central weapon script inherits from VRInteractableItem adding shooting and
// ejecting parts. Can spawn fx's on fire, uses SendMessageUpward method
// with Damage as the name and a class call DamageInfo to holds data relating to
// the shot. Found in the FireRaycast method if you want to change it :).
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class VRGunHandler : VRInteractableItem 
	{
		public enum FiringMode
		{
			SEMI_AUTOMATIC,
			FULLY_AUTOMATIC,
			BURST,
			PUMP_OR_BOLT_ACTION,
			SAFE
		}

		public enum ShotMode
		{
			SINGLE_SHOT,
			SHOTGUN_SPREAD,
			MACHINE_GUN_SPREAD
		}

		public enum FireType
		{
			RAYCAST,
			PHYSICAL
		}

		[System.Serializable]
		public class AttachmentPrefabs
		{
#if UNITY_EDITOR
			public GameObject attachmentPrefab;
#endif
			public VRAttachmentReceiver attachmentReceiver;
			public bool startLoaded;
			public bool isPrefab;
		}

		//Settings
		public FiringMode firingMode;
		public ShotMode shotMode;
		public Vector3 defaultPosition;
		public Quaternion defaultRotation;
		public int bulletId;
		public int damage = 10;
		public float bulletForce = 10;
		public List<string> shootLayers = new List<string>();
		public bool ignoreShootLayers;
		public bool autoEjectEmptyMag;
		public bool useChamberBullet = true;
		public bool ejectCasingOnFire = true;
		public bool ejectCasingOnSlidePull;
		public bool ejectBulletOnSlidePull = true;
		public bool requireMagToShoot;
		public FireType fireType;
		public string damageMethodName = "Damage";

		//References
		public VRGunTrigger trigger;
		public VRGunSlide slide;
		public BoxCollider bulletReceiver;
		public VRSecondHeld secondHeld;
		public List<Animation> playOnFire = new List<Animation>();
		public List<Animation> playOnSuccessfulFire = new List<Animation>();

		//Attachments
		public int attachmentId;
		public List<AttachmentPrefabs> attachmentPrefabs = new List<AttachmentPrefabs>();

		//Prefabs
		public GameObject bulletDecalPrefab;
		public List<GameObject> spawnOnFire = new List<GameObject>();
		public GameObject loadedBulletPrefab;
		public GameObject spentBulletPrefab;
		public Material laserPointerMat;
		public Material tracerMat;
		public GameObject physicalBullet;

		//Variables
		public float fireRate = 0.1f; //Seconds between shots when automatic
		public int bulletsPerShot = 6;
		public float coneSize = 2.5f;
		public float recoilKick = 1f;
		public float angularRecoilKick = 0.25f;
		public float recoilRecovery = 1f;
		public float angularRecoilMultiStep = 0.2f;
		public Vector3 shootOrigin;
		public Vector3 shootDirection;
		public Vector3 loadedBulletPosition;
		public Quaternion loadedBulletRotation;
		public Vector3 bulletEjectionDirection;
		public float bulletEjectionPush = 0.01f;
		public Vector3 firingOrigin;
		public Vector3 firingDirection;
		public Vector3 laserPointerOrigin;
		public bool laserPointerEnabled;
		public int playOnFireCount = 0;
		public int playOnSuccessfulFireCount = 0;
		public int spawnOnFireCount = 0;
		public List<Vector3> spawnOnFirePositions = new List<Vector3>();
		public List<Quaternion> spawnOnFireRotations = new List<Quaternion>();
		public float laserPointerLineWidth = 0.01f;
		public float tracerLineWidth = 0.01f;

		//Sounds
		public AudioClip fireSound;
		public AudioClip dryFireSound;
		public AudioClip slidePulled;
		public AudioClip slideRelease;
		public AudioClip loadMagazine;
		public AudioClip unloadMagazine;

		//Instances
		private GameObject loadedBulletInstance;
		private VRMagazine magazine;
		private GameObject fireLinesObj;
		private List<LineRenderer> fireLines = new List<LineRenderer>();
		private LineRenderer laserPointerLine;
		private GameObject casing;
		private float _recoilKick = -1f;
		private float _angularRecoilKick = -1f;
		private float _recoilRecovery = -1f;
		private float _angularRecoilMultiStep = -1f;
		private float _angularRecoilMulti = 1f;
		private float _timeSinceLastShot;
		public VRMagazine currentMagazine
		{
			get { return magazine; }
			set 
			{
				magazine = value;
				if (magazine != null && useChamberBullet && (slide == null || magazine.addBulletOnLoad)) LoadBullet(false, true);
			}
		}

		public bool SetLaserPointerEnabled
		{
			set
			{
				laserPointerEnabled = value;
				if (laserPointerEnabled)
				{
					if (laserPointerMat == null)
					{
						Debug.LogError("No laser pointer material set");
						return;
					}
					if (heldBy == null) return;
					if (laserPointerLine == null)
					{
						laserPointerLine = gameObject.AddComponent<LineRenderer>();
						laserPointerLine.material = laserPointerMat;
						laserPointerLine.startWidth = laserPointerLine.endWidth = laserPointerLineWidth;
						laserPointerLine.positionCount = 2;
					}
				}
				if (laserPointerLine != null) laserPointerLine.enabled = laserPointerEnabled;
			}
		}

		private bool _usingSecondHeld;
		public bool usingSecondHeld
		{
			get { return _usingSecondHeld; }
			set 
			{ 
				_usingSecondHeld = value;
				if (heldBy != null && _usingSecondHeld)
				{
					_recoilKick = secondHeld.recoilKick;
					_angularRecoilKick = secondHeld.angularRecoilKick;
					_recoilRecovery = secondHeld.recoilRecovery;
					_angularRecoilMultiStep = secondHeld.angularRecoilMultiStep;
				} else if (heldBy != null)
				{
					SetDefaultRecoil();
				}
			}
		}

		private bool fireLock;

		public void ToggleFireLock(bool lockFire)
		{
			fireLock = lockFire;
		}

		private void SetDefaultRecoil()
		{
			_recoilKick = recoilKick;
			_angularRecoilKick = angularRecoilKick;
			_recoilRecovery = recoilRecovery;
			_angularRecoilMultiStep = angularRecoilMultiStep;
		}

		private bool _hasBullet = false;
		public bool hasBullet
		{
			get { return _hasBullet; }
		}

		//Private variables
		private bool triggerHeld = false;
		private float elapsedTimeSinceLastTriggerDown;
		private float elapsedTimeHeld;
		private float lastFired;
		private int shotsFiredWhileHeld;

		override protected void Init()
		{
			if (attachmentPrefabs.Count != 0) item.gameObject.SetActive(false);
			/*foreach(AttachmentPrefabs attachmentPrefabs in attachmentPrefabs)
			{
				if (attachmentPrefabs.startLoaded && attachmentPrefabs.attachmentPrefab != null)
				{
					//VRMagazine posMag = null;
					if (attachmentPrefabs.isPrefab)
					{
						GameObject attachmentInstance = (GameObject)Instantiate(attachmentPrefabs.attachmentPrefab, Vector3.zero, Quaternion.identity);

						attachmentInstance.transform.SetParent(item);
						attachmentInstance.transform.localScale = attachmentPrefabs.attachmentPrefab.transform.localScale;
						VRAttachment attachment = attachmentInstance.GetComponentInChildren<VRAttachment>();

						VRAttachment.AttachmentPosition attachmentPosition = attachment.GetAttachmentPosition(attachmentPrefabs.attachmentReceiver);
						attachmentInstance.transform.localPosition = attachmentPosition.localPosition;
						attachmentInstance.transform.localRotation = attachmentPosition.localRotation;

						VRInteractableItem.FreezeItem(attachment.item.gameObject, false, false, true);
						attachment.AttachToGunHandler(attachmentPrefabs.attachmentReceiver);

						//posMag = attachmentInstance.GetComponentInChildren<VRMagazine>();
					}
				}
			}*/
			if (attachmentPrefabs.Count != 0) item.gameObject.SetActive(true);

			if (tracerMat != null)
			{
				fireLinesObj = new GameObject(item.name + " Fire Lines");
				fireLinesObj.transform.parent = item;
				if (shotMode == ShotMode.SHOTGUN_SPREAD)
				{
					for(int i=0; i<bulletsPerShot; i++)
					{
						GameObject fireLineObj = new GameObject("Line " + (i+1));
						fireLineObj.transform.SetParent(fireLinesObj.transform);
						LineRenderer fireLine = fireLineObj.AddComponent<LineRenderer>();
						fireLine.material = tracerMat;
						fireLine.startWidth = fireLine.endWidth = tracerLineWidth;
						fireLine.positionCount = 2;
						fireLines.Add(fireLine);
					}
				} else
				{
					LineRenderer fireLine = fireLinesObj.AddComponent<LineRenderer>();
					fireLine.material = tracerMat;
					fireLine.startWidth = fireLine.endWidth = tracerLineWidth;
					fireLine.positionCount = 2;
					fireLines.Add(fireLine);
				}
				fireLinesObj.SetActive(false);
			}

			base.Init();
		}

		override protected void Step()
		{
			if (heldBy == null) return;

			base.Step();

			if (laserPointerEnabled && laserPointerMat != null)
			{
				Ray ray = new Ray(item.TransformPoint(shootOrigin), item.TransformDirection(shootDirection));
				RaycastHit hit;
				bool hitSomething = false;
				if (shootLayers == null || shootLayers.Count == 0)
					hitSomething = Physics.Raycast(ray, out hit, 100);
				else
				{
					LayerMask raycastLayerMask = 1 << LayerMask.NameToLayer(shootLayers[0]);
					for (int i=1 ; i<shootLayers.Count ; i++)
					{
						raycastLayerMask |= 1 << LayerMask.NameToLayer(shootLayers[i]);
					}
					if (ignoreShootLayers) raycastLayerMask = ~raycastLayerMask;
					hitSomething = Physics.Raycast(ray, out hit, 100, raycastLayerMask);
				}
				if (laserPointerLine == null)
				{
					laserPointerLine = gameObject.AddComponent<LineRenderer>();
					laserPointerLine.material = laserPointerMat;
					laserPointerLine.startWidth = laserPointerLine.endWidth = laserPointerLineWidth;
					laserPointerLine.positionCount = 2;
				}
				Vector3 endPoint = Vector3.zero;
				if (hitSomething)
				{
					endPoint = hit.point;
				} else
				{
					endPoint = ray.origin+(ray.direction*100);
				}
				laserPointerLine.SetPosition(0, item.TransformPoint(laserPointerOrigin));
				laserPointerLine.SetPosition(1, endPoint);
			}

			elapsedTimeSinceLastTriggerDown += Time.deltaTime;
			elapsedTimeHeld += Time.deltaTime;
			switch(firingMode)
			{
			case FiringMode.SEMI_AUTOMATIC:
			case FiringMode.PUMP_OR_BOLT_ACTION:
				if (triggerHeld)
				{
					Shoot();
					triggerHeld = false;
				}
				break;
			case FiringMode.FULLY_AUTOMATIC:
				if (heldBy.vrInput.ActionPressed("ACTION") && triggerHeld)
				{
					if (elapsedTimeSinceLastTriggerDown > lastFired+fireRate)
					{
						lastFired = elapsedTimeSinceLastTriggerDown;
						Shoot();
					}
				}
				break;
			case FiringMode.BURST:
				if (heldBy.vrInput.ActionPressed("ACTION") && triggerHeld && shotsFiredWhileHeld < 3)
				{
					if (elapsedTimeSinceLastTriggerDown > lastFired+fireRate)
					{
						shotsFiredWhileHeld++;
						lastFired = elapsedTimeSinceLastTriggerDown;
						Shoot();
					}
				}
				break;
			}
		}

		override protected void ACTION(VRInteractor hand)
		{
			if (hand.heldItem == null)
			{
				base.ACTION(hand);
				return;
			}
			if (_pickingUp) return;
			elapsedTimeSinceLastTriggerDown = 0;
			lastFired = 0;
			switch(firingMode)
			{
			case FiringMode.SEMI_AUTOMATIC:
			case FiringMode.PUMP_OR_BOLT_ACTION:
				triggerHeld = true;
				break;
			case FiringMode.FULLY_AUTOMATIC:
				triggerHeld = true;
				break;
			case FiringMode.BURST:
				triggerHeld = true;
				shotsFiredWhileHeld = 0;
				break;
			}
		}

		override protected void ACTIONReleased(VRInteractor hand)
		{
			base.ACTIONReleased(hand);
			if (_pickingUp) return;
			switch(firingMode)
			{
			case FiringMode.FULLY_AUTOMATIC:
				triggerHeld = false;
				break;
			}
		}

		virtual protected void BUTTON_4(VRInteractor hand)
		{
			if (hand.heldItem == null || _pickingUp || currentMagazine == null) return;

			currentMagazine.Eject();
		}

		virtual protected void BUTTON_2(VRInteractor hand)
		{
			if (hand.heldItem == null || _pickingUp) return;

			VRRevolverMagazine revolverMag = item.GetComponentInChildren<VRRevolverMagazine>();
			if (revolverMag != null) revolverMag.ToggleDrum();

			if (hasBullet || casing != null || slide == null) return;
			if (currentMagazine != null)
			{
				_hasBullet = magazine.TakeBullet();
				if (_hasBullet)
				{
					PlaySound(slideRelease);
					slide.Release();
					VRSlideAnimation[] slideAnimations = item.GetComponentsInChildren<VRSlideAnimation>();
					foreach(VRSlideAnimation slideAnimation in slideAnimations)
						slideAnimation.Release();
					InstantiateNewLoadedBullet(null);
				}
			} else
				slide.Release();
		}

		override public bool Pickup(VRInteractor hand)
		{
			if (laserPointerEnabled && laserPointerLine != null) laserPointerLine.enabled = true;
			CheckForAttachments(hand, true);
			return base.Pickup(hand);
		}

		virtual protected void CheckForAttachments(VRInteractor hand, bool pickup)
		{
			if (attachmentPrefabs.Count == 0 || hand == null) return;
			VRInteractor otherController = hand.GetOtherController();
			if (otherController == null || otherController.heldItem == null || otherController.heldItem.GetType() != typeof(VRAttachment)) return;
			VRAttachment attachment = (VRAttachment)otherController.heldItem;
			if (pickup)
				attachment.SpawnHighlight(otherController);
			else
				attachment.DestroyHighlight();
		}

		override protected IEnumerator PickingUp(VRInteractor heldBy)
		{
			//Check if second held is being held.
			if (secondHeld != null && secondHeld.heldBy != null) yield break;

			yield return StartCoroutine(base.PickingUp(heldBy));
		}

		override public void Drop(bool addControllerVelocity, VRInteractor hand = null)
		{
			if (laserPointerLine != null) laserPointerLine.enabled = false;
			if (secondHeld != null && !secondHeld.canBeHeld && secondHeld.heldBy != null) secondHeld.heldBy.Drop();
			CheckForAttachments(hand, false);
			base.Drop(addControllerVelocity, hand);
		}

		override protected void CheckIK(bool pickingUp, VRInteractor hand)
		{
			base.CheckIK(pickingUp, hand);
			if (hand == null || hand.ikTarget == null) return;
			if ((hand.vrInput.LeftHand && leftHandIKPoseName != "") ||
				(!hand.vrInput.LeftHand && rightHandIkPoseName != ""))
			{
				//	Method is in HandPoseControllerWeapon.cs, found by opening the FinalIKPrefabs package in the integrations folder (make sure to open the FinalIK package in VRInteraction first).
				hand.GetVRRigRoot.BroadcastMessage("CheckForTrigger", new object[] { this, hand.vrInput.LeftHand ? leftHandIKPoseName : rightHandIkPoseName, pickingUp }, SendMessageOptions.DontRequireReceiver);
			}
		}

        virtual public void Shoot()
        {
			if (fireLock || (requireMagToShoot && currentMagazine == null)) return;
            if (!useChamberBullet) LoadBullet(false);
			VREvent.Send("PreShoot", new object[] {this, _hasBullet});
			if (slide != null && !slide.readyToFire) return;
			PlayOnFire();
            if (!_hasBullet)
            {
				//if (slide != null) slide.Shoot(hasBullet);
                PlaySound(dryFireSound);
                return;
            }
            if (loadedBulletInstance != null) Destroy(loadedBulletInstance);
            _hasBullet = false;

            PlaySound(fireSound);
            if (heldBy != null) heldBy.TriggerHapticPulse(90);
            if (usingSecondHeld && secondHeld != null && secondHeld.heldBy != null) secondHeld.heldBy.TriggerHapticPulse(90);
            FXOnFire();
            EjectCasing();
            if (useChamberBullet) LoadBullet(false);
			PlayOnSuccessfulFire();
			if (slide != null) slide.Shoot(hasBullet);
			//	Should we eject the magazine?
			if (autoEjectEmptyMag && magazine != null && magazine.canBeHeld && ((!useChamberBullet && magazine.bulletCount == 0) || (useChamberBullet && !hasBullet)))
			{
				DestroyIn magDestroy = magazine.item.GetComponent<DestroyIn>();
				if (magDestroy != null) magDestroy.enabled = true;
	            magazine.Eject();
			}

			switch(fireType)
			{
			case FireType.RAYCAST:
				FireRaycast();
				break;
			case FireType.PHYSICAL:
				FirePhysical();
				break;
			}

            //Add kickback
			if (selfBody != null)
			{
				if (_recoilKick < 0f || _angularRecoilKick < 0f || _recoilRecovery < 0f || _angularRecoilMultiStep < 0f) SetDefaultRecoil();

				if (_timeSinceLastShot+0.2f < elapsedTimeHeld) _angularRecoilMulti = 1f;

				if (kickBackRoutine != null) StopCoroutine(kickBackRoutine);
				kickBackRoutine = StartCoroutine(kickBackRecovery());
				selfBody.AddForce(-item.forward.normalized*_recoilKick, ForceMode.Impulse);
				selfBody.AddTorque(-item.right.normalized*(_angularRecoilKick*_angularRecoilMulti), ForceMode.Impulse);

				_angularRecoilMulti += _angularRecoilMultiStep;
			}
            /*item.localRotation = Quaternion.RotateTowards(item.localRotation, Quaternion.LookRotation(item.InverseTransformDirection(item.forward), item.InverseTransformDirection(item.up)), (usingSecondHeld ? recoilKick * 0.25f : recoilKick));
         	if (item.transform.localScale.x >= 1)
            {
               item.localPosition = item.localPosition - item.InverseTransformVector(item.forward * 0.01f);
            }
            else if (item.transform.localScale.x <= 1) item.localPosition = item.localPosition - item.InverseTransformVector(item.forward*0.001f);
            recoiling = true;
            recoilStart = Time.time;*/
            //if (slide != null) slide.Shoot(hasBullet);
			_timeSinceLastShot = elapsedTimeHeld;
      }

		Coroutine kickBackRoutine;

		IEnumerator kickBackRecovery()
		{
			if (_recoilRecovery == 0) yield break;
			float startForce = 0.1f;
			float startTime = Time.time;
			float t = 0;
			while(t < 1f)
			{
				float totalTime = Time.time - startTime;
				t = totalTime / _recoilRecovery;
				currentFollowForce = ((followForce-startForce)*t) + startForce;
				yield return null;
			}
			currentFollowForce = followForce;
		}

		public void SlidePulled()
		{
			if (_hasBullet && ejectBulletOnSlidePull && loadedBulletInstance != null)
			{
				//Eject bullet
				LaunchCasing(loadedBulletInstance);
				loadedBulletInstance = null;
				_hasBullet = false;
			} else if (ejectCasingOnSlidePull && casing != null)
			{
				LaunchCasing(casing);
				casing = null;
			}
			LoadBullet(true);
		}

		virtual protected void LoadBullet(bool slidePulled, bool onLoad = false)
		{
			if (hasBullet || casing != null || ((!onLoad || (magazine != null && !magazine.addBulletOnLoad)) && firingMode == FiringMode.PUMP_OR_BOLT_ACTION && !slidePulled)) return;

			if (magazine != null)
			{
				_hasBullet = magazine.TakeBullet();
			}

			if (_hasBullet)
			{
				if (!slidePulled && slide != null && slide.pulled) slide.Release();
				InstantiateNewLoadedBullet(null);
			}
		}

		virtual public bool LoadBullet(VRLoadableBullet bullet)
		{
			if (bullet == null || bulletId != bullet.bulletId) return false;
			if (!useChamberBullet || hasBullet || casing != null)
			{
				if (currentMagazine != null) currentMagazine.LoadBullet(bullet, true);
				return false;
			}
			if (bullet.heldBy != null) bullet.heldBy.Drop();
			_hasBullet = true;
			if (slide != null && slide.pulled) slide.Release();
			InstantiateNewLoadedBullet(bullet.item.gameObject);
			return true;
		}

		public void InstantiateNewLoadedBullet(GameObject instance)
		{
			if (loadedBulletPrefab != null && useChamberBullet)
			{
				if (instance == null) loadedBulletInstance = (GameObject)Instantiate(loadedBulletPrefab, Vector3.zero, Quaternion.identity);
				else loadedBulletInstance = instance;
				loadedBulletInstance.transform.SetParent(magazine != null && magazine.bulletParent != null ? magazine.bulletParent : item);
				loadedBulletInstance.transform.localPosition = loadedBulletPosition;
				loadedBulletInstance.transform.localRotation = loadedBulletRotation;
				VRInteractableItem loadedBulletItem = loadedBulletInstance.GetComponentInChildren<VRInteractableItem>();
				if (loadedBulletItem != null) loadedBulletItem.enabled = false;
				VRInteractableItem.FreezeItem(loadedBulletInstance, true);
			}
		}

		virtual protected void FXOnFire()
		{
			if (spawnOnFire.Count == 0) return;

			for(int i=0; i<spawnOnFire.Count; i++)
			{
				GameObject onFireInstance = PoolingManager.instance.GetInstance(spawnOnFire[i]);
				PoolingManager.instance.SetupDestroyInToReset(onFireInstance);
				onFireInstance.transform.SetParent(item);
				onFireInstance.transform.localPosition = spawnOnFirePositions[i];
				onFireInstance.transform.localRotation = spawnOnFireRotations[i];
				onFireInstance.SetActive(true);
			}
		}

		virtual protected void PlayOnFire()
		{
			if (playOnFire.Count == 0) return;
			foreach(Animation anim in playOnFire)
			{
				if (anim == null) continue;
				anim.Rewind();
				anim.Play();
			}
		}

		virtual protected void PlayOnSuccessfulFire()
		{
			if (playOnSuccessfulFire.Count == 0) return;
			foreach(Animation anim in playOnSuccessfulFire)
			{
				if (anim == null) continue;
				anim.Rewind();
				anim.Play();
			}
		}

		virtual protected void EjectCasing()
		{
			if (spentBulletPrefab == null) return;

			GameObject nextCasing = PoolingManager.instance.GetInstance(spentBulletPrefab);
			PoolingManager.instance.SetupDestroyInToReset(nextCasing);

			if (ejectCasingOnFire)
			{
				LaunchCasing(nextCasing);
			} else if (ejectCasingOnSlidePull)
			{
				nextCasing.transform.SetParent(item);
				nextCasing.transform.localPosition = loadedBulletPosition;
				nextCasing.transform.localRotation = loadedBulletRotation;
				nextCasing.SetActive(true);
				VRInteractableItem spentBulletItem = nextCasing.GetComponentInChildren<VRInteractableItem>();
				if (spentBulletItem != null)
				{
					spentBulletItem.enabled = false;
					spentBulletItem.parents.Add(this);
					if (secondHeld != null) spentBulletItem.parents.Add(secondHeld);
				}
				VRInteractableItem.FreezeItem(nextCasing, true);
				DestroyIn destroyIn = nextCasing.GetComponent<DestroyIn>();
				if (destroyIn != null) destroyIn.enabled = false;
				casing = nextCasing;
			}
		}

		private void LaunchCasing(GameObject casing)
		{
			if (casing == null) return;
			casing.transform.SetParent(item);
			casing.transform.localPosition = loadedBulletPosition + bulletEjectionDirection*bulletEjectionPush;
			casing.transform.localRotation = loadedBulletRotation;
			casing.transform.SetParent(null);
			casing.SetActive(true);
			VRInteractableItem loadedBulletItem = casing.GetComponentInChildren<VRInteractableItem>();
			if (loadedBulletItem != null)
			{
				loadedBulletItem.parents.Clear();
				loadedBulletItem.enabled = false;
				loadedBulletItem.enabled = true;
				loadedBulletItem.InteractionDisabled = false;
			}
			VRInteractableItem.UnFreezeItem(casing);
			Rigidbody casingBody = casing.GetComponentInChildren<Rigidbody>();
			if (casingBody != null)
			{
				casingBody.isKinematic = false;
				Vector3 playerVel = Vector3.zero;
				if (heldBy != null) playerVel = heldBy.Velocity;
				else if (secondHeld != null && secondHeld.heldBy != null) playerVel = secondHeld.heldBy.Velocity;
				casingBody.velocity = playerVel + item.TransformVector(bulletEjectionDirection);
			}
			Collider playerRigCollider = Camera.main.transform.parent.GetComponent<Collider>();
			if (playerRigCollider != null && !playerRigCollider.isTrigger)
			{
				Collider[] itemColliders = casing.GetComponentsInChildren<Collider>();
				foreach(Collider itemCol in itemColliders)
				{
					if (itemCol.isTrigger) continue;
					Physics.IgnoreCollision(playerRigCollider, itemCol);
				}
			}
			DestroyIn destroyIn = casing.GetComponent<DestroyIn>();
			if (destroyIn != null) destroyIn.enabled = true;
		}

		virtual protected void FirePhysical()
		{
			GameObject newBullet = PoolingManager.instance.GetInstance(physicalBullet);
			newBullet.transform.position = item.TransformPoint(shootOrigin);
			newBullet.transform.LookAt(newBullet.transform.position + item.TransformDirection(shootDirection));

			VRPhysicalBullet physicalBulletScript = newBullet.GetComponentInChildren<VRPhysicalBullet>();
			if (physicalBulletScript != null)
			{
				physicalBulletScript.damageMethodName = damageMethodName;
				physicalBulletScript.layerMask = VRUtils.StringArrayToLayerMask(shootLayers.ToArray(), ignoreShootLayers);
				physicalBulletScript.damage = damage;
				physicalBulletScript.bulletForce = bulletForce;
				physicalBulletScript.bulletDecalPrefab = bulletDecalPrefab;
			}

			newBullet.SetActive(true);

			VREvent.Send("ShootPhysical", getSelfParam);
		}

		virtual protected void FireRaycast()
		{
			//If shoot direction is zero it has not been setup yet.
			if (shootDirection == Vector3.zero) 
			{
				Debug.LogWarning("Shoot direction not setup. Setup shoot direction in the gun handler editor");
				return;
			}

			List<Ray> rays = new List<Ray>();
			switch(shotMode)
			{
			case ShotMode.SINGLE_SHOT:
				rays.Add(new Ray(item.TransformPoint(shootOrigin), item.TransformDirection(shootDirection)));
				break;
			case ShotMode.SHOTGUN_SPREAD:
				{
					Vector3 direction = item.TransformDirection(shootDirection);
					Vector3 originPosition = item.TransformPoint(shootOrigin);
					for(int i=0; i<bulletsPerShot; i++)
					{
						rays.Add(new Ray(originPosition, VRUtils.GetConeDirection(direction, coneSize)));
					}
				}
				break;
			case ShotMode.MACHINE_GUN_SPREAD:
				{
					Vector3 direction = item.TransformDirection(shootDirection);
					Vector3 originPosition = item.TransformPoint(shootOrigin);
					rays.Add(new Ray(originPosition, VRUtils.GetConeDirection(direction, coneSize)));
				}
				break;
			}

			if (tracerMat != null)
			{
				fireLinesObj.SetActive(true);
				StartCoroutine(DisableTracerLine());
			}

			int rayIndex = 0;
			List<RaycastHit> raycastHits = new List<RaycastHit>();
			foreach(Ray ray in rays)
			{
				RaycastHit hit;
				bool hitSomething = false;

				LayerMask shootMask = VRUtils.StringArrayToLayerMask(shootLayers.ToArray(), ignoreShootLayers);

				if (shootMask == null)
					hitSomething = Physics.Raycast(ray, out hit, 100);
				else
					hitSomething = Physics.Raycast(ray, out hit, 100, shootMask);

				if (hitSomething)
				{
					raycastHits.Add(hit);
					Rigidbody hitBody = hit.transform.GetComponentInChildren<Rigidbody>();
					if (hitBody != null)
					{
						hitBody.AddForce(ray.direction*bulletForce*100);
					}
					VRGunHandler.ApplyDecal(bulletDecalPrefab, hit);

					hit.transform.SendMessageUpwards(damageMethodName, damage, SendMessageOptions.DontRequireReceiver);

                    if (tracerMat != null && fireLines.Count > rayIndex)
					{
						fireLines[rayIndex].SetPosition(0, ray.origin);
						fireLines[rayIndex].SetPosition(1, hit.point);
					}
				} else if (tracerMat != null && fireLines.Count > rayIndex)
				{
					fireLines[rayIndex].SetPosition(0, ray.origin);
					fireLines[rayIndex].SetPosition(1, ray.origin + (ray.direction*10));
				}
				rayIndex++;
			}
			VREvent.Send("Shoot", new object[] {this, raycastHits});
		}

		public static void ApplyDecal(GameObject decalPrefab, RaycastHit hit)
		{
			if (decalPrefab == null) return;

			//Draw decal on surface hit
			GameObject decal = PoolingManager.instance.GetInstance(decalPrefab);
			PoolingManager.instance.SetupDestroyInToReset(decal, true);
			decal.SetActive(true);

			Vector3 oldScale = decalPrefab.transform.localScale;
			decal.transform.parent = hit.transform;
			decal.transform.position = hit.point+(hit.normal*Random.Range(0.005f, 0.007f));
			decal.transform.LookAt(hit.point-hit.normal);
			if (hit.transform.lossyScale != Vector3.one)
			{
				decal.transform.parent = null;
				decal.transform.localScale = oldScale;
				decal.transform.parent = hit.transform;
			}
			
			DecalChanger decalChanger = decal.GetComponentInChildren<DecalChanger>();
			if (decalChanger != null) decalChanger.SetMaterialTo(hit.transform.tag); 
		}

		IEnumerator DisableTracerLine()
		{
			yield return new WaitForSeconds(0.1f);
			fireLinesObj.SetActive(false);
		}

		override public void EnableHover(VRInteractor hand = null)
		{
			base.EnableHover();
			if (slide != null) slide.EnableHover();
			if (currentMagazine != null) currentMagazine.EnableHover();
		}

		override public void DisableHover(VRInteractor hand = null)
		{
			base.DisableHover();
			if (slide != null) slide.DisableHover();
			if (currentMagazine != null) currentMagazine.DisableHover();
		}

		/*override public Vector3 GetLocalHeldPosition(IVRInteractor hand)
		{
			if (usingSecondHeld && secondHeld != null)
			{
				Vector3 normalHeld = base.GetLocalHeldPosition(hand);
				//normalHeld = base.GetLocalHeldRotation(hand) * normalHeld;
				return normalHeld;
			} else return base.GetLocalHeldPosition(hand);
		}*/
			
		Transform secondHeldTransform;

		override protected Vector3 GetHeldPositionDelta()
		{
			if (usingSecondHeld && secondHeld != null && heldBy != null)
			{
				Transform heldByTransform = heldBy.getControllerAnchorOffset;
				if (secondHeldTransform == null)
				{
					GameObject secondHeldObject = new GameObject("SecondHeldController");
					secondHeldTransform = secondHeldObject.transform;
					secondHeldTransform.SetParent(item);
					secondHeldTransform.localRotation = Quaternion.Inverse(base.GetLocalHeldRotation(_heldBy));
				}
				secondHeldTransform.position = heldByTransform.position;
				return (secondHeldTransform.TransformPoint(GetLocalHeldPosition(heldBy))) - item.position;
			} else return base.GetHeldPositionDelta();
		}

		override public Quaternion GetLocalHeldRotation(VRInteractor hand)
		{
			if (usingSecondHeld && secondHeld != null && secondHeld.heldBy != null && heldBy != null)
			{
				Vector3 heldControllerPos = hand.getControllerAnchorOffset.position;
				Vector3 secondHeldControllerPos = secondHeld.heldBy.getControllerAnchorOffset.position;
				Vector3 controllersVectorDirection = (secondHeldControllerPos - heldControllerPos).normalized;
				Quaternion controllersDirection = Quaternion.LookRotation(controllersVectorDirection);
				Vector3 heldWorldPosition = hand.getControllerAnchorOffset.TransformPoint(base.GetLocalHeldPosition(hand));
				Vector3 secondHeldLocalPosition = secondHeld.GetLocalHeldPosition(secondHeld.heldBy);
				secondHeldLocalPosition *= 3f;
				Vector3 secondHeldWorldPosition = VRUtils.TransformPoint(secondHeld.heldBy.getControllerAnchor.position, controllersDirection, secondHeld.heldBy.getControllerAnchor.lossyScale, secondHeldLocalPosition);
				secondHeldWorldPosition += (controllersVectorDirection*1f);
				Vector3 worldHeldPositionDirectionVector = secondHeldWorldPosition - heldWorldPosition;

				//Debug.Log(heldWorldPosition + " " + secondHeldWorldPosition + " " + worldHeldPositionDirectionVector);

				return Quaternion.LookRotation(worldHeldPositionDirectionVector, hand.getControllerAnchorOffset.up);
				//Vector3 secondHeldControllerWorldPos = VRUtils.TransformPoint(secondHeldControllerPos, controllersDirection, secondHeld.heldBy.getControllerAnchor.lossyScale, secondHeldLocalPosition);
				//return Quaternion.LookRotation(secondHeldControllerWorldPos - heldControllerPos,  hand.getControllerAnchorOffset.up);

				/*
				Vector3 heldControllerPos = hand.getControllerAnchorOffset.position;
				Vector3 secondHeldControllerPos = secondHeld.heldBy.getControllerAnchorOffset.position;

				Vector3 secondHeldLocalPosition = secondHeld.GetLocalHeldPosition(secondHeld.heldBy);
				//secondHeldLocalPosition.z = 0;
				secondHeldControllerPos += secondHeldLocalPosition;

				Vector3 controllersVectorDirection = (secondHeldControllerPos - heldControllerPos).normalized;
				Quaternion controllersDirection = Quaternion.LookRotation(controllersVectorDirection, hand.getControllerAnchorOffset.up);
				return controllersDirection;
				//Vector3 heldLocalPosition = base.GetLocalHeldPosition(hand);

				//Vector3 localPositionDirectionVector = VRUtils.InverseTransformPoint( heldLocalPosition - secondHeldLocalPosition;
				//Vector3 secondHeldControllerWorldPos = VRUtils.InverseTransformPoint(secondHeldControllerPos, controllersDirection, secondHeld.heldBy.getControllerAnchor.lossyScale, secondHeldLocalPosition);
				//secondHeldControllerWorldPos += (worldPositionDirectionVector*10f);
				//return Quaternion.LookRotation(secondHeldControllerWorldPos - heldControllerPos,  hand.getControllerAnchorOffset.up);
				*/

			} else return base.GetLocalHeldRotation(hand);
		}

		override protected Quaternion GetHeldRotationDelta()
		{
			Transform heldByTransform = heldBy.getControllerAnchorOffset;
			if (usingSecondHeld && secondHeld != null && secondHeld.heldBy != null)
			{
				return (GetLocalHeldRotation(heldBy)) * Quaternion.Inverse(item.rotation);
			} else
			{
				return (heldByTransform.rotation*GetLocalHeldRotation(heldBy)) * Quaternion.Inverse(item.rotation);
			}
		}
	}
}
#endif
