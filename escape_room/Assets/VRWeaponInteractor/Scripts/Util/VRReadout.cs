//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Readout for VRGunHandler weapons, displays current magazine bullets to a
// referenced Text. Can add a prepend and postpend text.
//
//=============================================================================
#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class VRReadout : MonoBehaviour {

		public Text text;
		public VRGunHandler gunHandler;

		public string noMagText = "No Magazine";
		public string prePend;
		public string postPend;

		private float _elapsedTime;

		void Update ()
		{
			if (text == null || gunHandler == null) return;
			_elapsedTime += Time.deltaTime;
			if (_elapsedTime > 0.01f)
			{
				_elapsedTime = 0f;
				UpdateReadout();
			}
		}

		void UpdateReadout()
		{
			if (gunHandler.currentMagazine == null)
			{
				text.text = noMagText;
				return;
			}
			text.text = prePend + (gunHandler.currentMagazine.bulletCount+(gunHandler.hasBullet?1:0)) + " | " + (gunHandler.currentMagazine.clipSize) + postPend;
		}
	}
}
#endif