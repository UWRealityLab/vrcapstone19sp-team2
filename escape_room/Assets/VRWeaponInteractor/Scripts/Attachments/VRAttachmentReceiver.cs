//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// The Attachment Receiver is on a trigger collider object attached to a weapon
// This stores an ID for checking this is the correct receiver and links to the
// connected gunhandler.
//
//=============================================================================
using UnityEngine.Events;


#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRWeaponInteractor
{
	public class VRAttachmentReceiver : MonoBehaviour 
	{
		public VRGunHandler gunHandler;
		public int attachmentId;

		public UnityEvent attachEvent;
		public UnityEvent detachEvent;

		private VRAttachment _currentAttachment;

		public VRAttachment currentAttachment
		{
			get { return _currentAttachment; }
			set { _currentAttachment = value; }
		}
	}
}
#endif