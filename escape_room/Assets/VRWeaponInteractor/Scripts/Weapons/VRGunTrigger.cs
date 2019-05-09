//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Checks controller trigger pressure and animates between two points when held
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using System.Collections;

namespace VRWeaponInteractor
{

	public class VRGunTrigger : MonoBehaviour {

		public VRGunHandler gunHandler;
		public Vector3 defaultTriggerPosition = Vector3.zero;
		public Quaternion defaultTriggerRotation = Quaternion.identity;
		public Vector3 pulledTriggerPosition = Vector3.zero;
		public Quaternion pulledTriggerRotation = Quaternion.identity;
		public bool resetToDefaultOnShoot;

		//Editor Var
		public bool triggerPulled;

		private bool trackTrigger;

		void Start () 
		{
			transform.localPosition = defaultTriggerPosition;
			transform.localRotation = defaultTriggerRotation;
			trackTrigger = true;
			if (resetToDefaultOnShoot) VREvent.Listen("PreShoot", OnShoot);
		}

		void Update()
		{
			if (gunHandler == null || gunHandler.heldBy == null) return;

			if (!trackTrigger)
			{
				float pressure = gunHandler.heldBy.vrInput.TriggerPressure;
				if (pressure < 0.1f) trackTrigger = true;
				return;
			}

			float triggerPressure = gunHandler.heldBy.vrInput.TriggerPressure;
			transform.localPosition = Vector3.Lerp(defaultTriggerPosition, pulledTriggerPosition, triggerPressure);
			transform.localRotation = Quaternion.Lerp(defaultTriggerRotation, pulledTriggerRotation, triggerPressure);
		}

		void OnShoot(object[] args)
		{
			if (!resetToDefaultOnShoot) return;
			VRGunHandler shootingGun = (VRGunHandler)args[0];
			if (shootingGun == gunHandler)
			{
				trackTrigger = false;
				transform.localPosition = defaultTriggerPosition;
				transform.localRotation = defaultTriggerRotation;
			}
		}
	}

}
#endif