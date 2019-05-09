//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// VRReloader when attached to a trigger collider will show the available ammo
// for the currently held weapon and if bullets are available will refill the magazine
// or add a new magazine. Use AddBullets method to add bullets to the magazine id.
//
//=============================================================================
#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRInteraction;
using UnityEngine.UI;

namespace VRWeaponInteractor
{
	public class VRReloader : MonoBehaviour 
	{
		/*[System.Serializable]
		public class Clip
		{
			public int magazineId;
			public int bullets;
		}

		public Text text;
		public string preIdToNames;
		public string postIdToNames = "250=p250 Rounds,50=Desert Rounds";
		public bool onlyReloadEmpty;
		public List<Clip> clips = new List<Clip>();

		private Dictionary<int, int> _bulletsDictionary = new Dictionary<int, int>();
		private Dictionary<int, string> preNames = new Dictionary<int, string>();
		private Dictionary<int, string> postNames = new Dictionary<int, string>();
		private int displayId;

		public void AddBullets(int magazineId, int bullets)
		{
			if (!_bulletsDictionary.ContainsKey(magazineId)) _bulletsDictionary.Add(magazineId, 0);
			_bulletsDictionary[magazineId] += bullets;
			UpdateDisplay();
		}

		public int GetBullets(int magazineId)
		{
			if (!_bulletsDictionary.ContainsKey(magazineId))
				return 0;
			return _bulletsDictionary[magazineId];
		}

		public void UpdateDisplay()
		{
			if (!_bulletsDictionary.ContainsKey(displayId) || text == null) return;
			string prePend = "";
			if (preNames != null && preNames.ContainsKey(displayId))
				prePend = preNames[displayId];
			string postPend = "";
			if (postPend != null && postNames.ContainsKey(displayId))
				postPend = postNames[displayId];
			text.text = prePend + _bulletsDictionary[displayId] + postPend;
		}

		private Dictionary<int, string> ConvertStringToDictionary(string input)
		{
			if (input == "" || input == null) return null;
			Dictionary<int, string> dict = new Dictionary<int, string>();
			string[] ids = input.Split(',');
			foreach(string id in ids)
			{
				if (id == "" || id == null) continue;
				string[] idName = id.Split('=');
				dict.Add(int.Parse(idName[0]), idName[1]);
			}
			return dict;
		}

		void Start()
		{
			//Init pre and post name dictionaries
			preNames = ConvertStringToDictionary(preIdToNames);
			postNames = ConvertStringToDictionary(postIdToNames);

			foreach(Clip clip in clips) AddBullets(clip.magazineId, clip.bullets);

			UpdateDisplay();
			VREvent.Listen("Pickup", OnPickup);
		}

		void OnPickup(params object[] args)
		{
			VRInteractableItem item = (VRInteractableItem)args[0];
			if (item.GetType() != typeof(VRGunHandler)) return;
			VRGunHandler gunHandler = (VRGunHandler)item;

			displayId = gunHandler.attachmentId;
			UpdateDisplay();
		}

		void OnTriggerEnter(Collider col)
		{
			VRGunHandler gunHandler = col.GetComponent<VRGunHandler>();
			if (gunHandler == null) return;


			if (!_bulletsDictionary.ContainsKey(gunHandler.attachmentId))
					return;
			int bulletsAvailable = _bulletsDictionary[gunHandler.attachmentId];
			

			if (!onlyReloadEmpty && gunHandler.currentMagazine != null && gunHandler.currentMagazine.bulletCount < gunHandler.currentMagazine.clipSize-1)
			{
				bulletsAvailable += gunHandler.currentMagazine.bulletCount + (gunHandler.hasBullet?1:0);
				Destroy(gunHandler.currentMagazine.item.gameObject);
				gunHandler.currentMagazine = null;
			}

			if (gunHandler.currentMagazine == null)
			{
				GameObject magazinePrefab = null;
				VRAttachment.AttachmentPosition attachmentPosition = null;
				VRGunHandler.AttachmentPrefabs attachmentPrefab = null;
				foreach(VRGunHandler.AttachmentPrefabs attachmentPrefabs in gunHandler.attachmentPrefabs)
				{
					VRMagazine magazine = attachmentPrefabs.attachmentPrefab.GetComponentInChildren<VRMagazine>();
					if (magazine != null)
					{
						magazinePrefab = attachmentPrefabs.attachmentPrefab;
						attachmentPosition = magazine.GetAttachmentPosition(attachmentPrefabs.attachmentReceiver);
						attachmentPrefab = attachmentPrefabs;
						break;
					}
				}
				if (magazinePrefab == null)
				{
					Debug.LogError("Couldn't find magazine in attachments");
					return;
				}

				GameObject magazineInstance = (GameObject)Instantiate(magazinePrefab, Vector3.zero, Quaternion.identity);

				magazineInstance.transform.SetParent(gunHandler.item);
				gunHandler.currentMagazine = magazineInstance.GetComponentInChildren<VRMagazine>();
				if (gunHandler.currentMagazine != null || gunHandler.currentMagazine.item != null)
				{
					gunHandler.currentMagazine.item.localPosition = attachmentPosition.localPosition;
					gunHandler.currentMagazine.item.localRotation = attachmentPosition.localRotation;
					attachmentPrefab.attachmentReceiver.currentAttachment = null;
					gunHandler.currentMagazine.TryLoadIntoGun(attachmentPrefab.attachmentReceiver);
					if (gunHandler.useChamberBullet) gunHandler.currentMagazine.bulletCount--;
				} else
				{
					if (gunHandler.currentMagazine != null && gunHandler.currentMagazine.item == null)
						Debug.LogError("Magazine Script does not have a reference to item");
					else
						Debug.LogError("Weapon magazine prefab has no VRMagazine script attached");
				}
				if (bulletsAvailable >= gunHandler.currentMagazine.clipSize)
				{
					bulletsAvailable -= gunHandler.currentMagazine.clipSize;
				} else
				{
					int diff = gunHandler.currentMagazine.clipSize - bulletsAvailable;
					gunHandler.currentMagazine.bulletCount -= diff;
					bulletsAvailable = 0;
				}

				_bulletsDictionary[gunHandler.attachmentId] = bulletsAvailable;
				UpdateDisplay();

				if (!gunHandler.hasBullet)
				{
					gunHandler.InstantiateNewLoadedBullet(null);
				}
				if (gunHandler.slide != null && gunHandler.slide.pulled) gunHandler.slide.Release();
				if (gunHandler.loadMagazine != null) AudioSource.PlayClipAtPoint(gunHandler.loadMagazine, gunHandler.item.position);
			}
		}*/
	}
}
#endif
