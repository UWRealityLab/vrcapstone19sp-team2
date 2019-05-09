//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// This Script should be attached to the gunhandler object and will forward select input actions
// onto any attachment that is currently attached to this gunhandler.
//
//=============================================================================
#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class AttachmentInputForwarder : MonoBehaviour 
	{
		VRGunHandler gunHandler;

		void Start()
		{
			gunHandler = GetComponent<VRGunHandler>();
			if (gunHandler == null)
			{
				Debug.LogError("No VRGunHandler script attached to object", gameObject);
			}
		}

		virtual protected void ForwardMessage(VRInteractor hand, string message)
		{
			foreach(VRGunHandler.AttachmentPrefabs attachmentprefab in gunHandler.attachmentPrefabs)
			{
				if (attachmentprefab.attachmentReceiver == null || attachmentprefab.attachmentReceiver.currentAttachment == null) continue;
				attachmentprefab.attachmentReceiver.currentAttachment.gameObject.SendMessage(message, hand, SendMessageOptions.DontRequireReceiver);
			}
		}

		virtual protected void BUTTON_1(VRInteractor hand){ForwardMessage(hand, "BUTTON_1");}
		virtual protected void BUTTON_1Released(VRInteractor hand){ForwardMessage(hand, "BUTTON_1Released");}
		virtual protected void BUTTON_2(VRInteractor hand){ForwardMessage(hand, "BUTTON_2");}
		virtual protected void BUTTON_2Released(VRInteractor hand){ForwardMessage(hand, "BUTTON_2Released");}
		virtual protected void BUTTON_3(VRInteractor hand){ForwardMessage(hand, "BUTTON_3");}
		virtual protected void BUTTON_3Released(VRInteractor hand){ForwardMessage(hand, "BUTTON_3Released");}
		virtual protected void BUTTON_4(VRInteractor hand){ForwardMessage(hand, "BUTTON_4");}
		virtual protected void BUTTON_4Released(VRInteractor hand){ForwardMessage(hand, "BUTTON_4Released");}
	}
}
#endif