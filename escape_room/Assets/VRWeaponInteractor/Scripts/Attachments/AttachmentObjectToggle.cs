//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Generic object toggle. Used in the example scene to toggle the torch on and off
// If you want your own button to call toggle you can inherit from this class and call the ToggleObject method
// from your own method.
//
//=============================================================================
#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class AttachmentObjectToggle : MonoBehaviour 
	{
		public GameObject targetObject;

		public AudioClip toggleOnSound;
		public AudioClip toggleOffSound;

		public UnityEvent toggleOnEvent;
		public UnityEvent toggleOffEvent;

		virtual protected void ToggleObject()
		{
			targetObject.SetActive(!targetObject.activeSelf);
			if (targetObject.activeSelf)
			{
				if (toggleOnSound != null) AudioSource.PlayClipAtPoint(toggleOnSound, transform.position);
				if (toggleOnEvent != null) toggleOnEvent.Invoke();
			} else
			{
				if (toggleOffSound != null) AudioSource.PlayClipAtPoint(toggleOffSound, transform.position);
				if (toggleOffEvent != null) toggleOffEvent.Invoke();
			}
		}

		virtual protected void BUTTON_3(VRInteractor hand)
		{
			ToggleObject();
		}
	}
}
#endif