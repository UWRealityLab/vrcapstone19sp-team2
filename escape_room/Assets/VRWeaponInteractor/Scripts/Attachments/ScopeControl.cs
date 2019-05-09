//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// For changing the field of view on a camera. Can also toggle a specific gameobject
// for each zoom level. Used in the example scene scope to toggle correctly sized crosshairs
// If you want your own button to call zoom you can inherit from this class and call the ChangeZoom method
// from your own method.
//
//=============================================================================
#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class ScopeControl : MonoBehaviour
	{
		public Camera scopeCamera;
		public List<float> zoomLevels = new List<float>();
		public List<GameObject> crosshairs = new List<GameObject>();
		public AudioClip incrementSound;

		private int zoomIndex;

		virtual protected void Start()
		{
			zoomIndex = -1;
			ChangeZoom();
		}

		virtual protected void BUTTON_1(VRInteractor hand)
		{
			ChangeZoom();
		}

		virtual protected void ChangeZoom()
		{
			if (zoomLevels.Count == 0 || scopeCamera == null) return;

			if (incrementSound != null) AudioSource.PlayClipAtPoint(incrementSound, transform.position);

			zoomIndex += 1;
			if (zoomIndex >= zoomLevels.Count) zoomIndex = 0;

			scopeCamera.fieldOfView = zoomLevels[zoomIndex];

			foreach(GameObject crosshair in crosshairs) crosshair.SetActive(false);
			if (zoomIndex < crosshairs.Count) crosshairs[zoomIndex].SetActive(true);
		}
	}
}
#endif