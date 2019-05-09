//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Used on the Sword Example, delivers damage using SendMessageUpwards with the Damage
// method name. Holds a collection of swing and hits that play when moving
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRInteraction;

namespace VRWeaponInteractor
{

	[RequireComponent (typeof(Rigidbody))]
	[RequireComponent (typeof(Collider))]
	[RequireComponent (typeof(AudioSource))]
	public class ApplyDamage : MonoBehaviour {

		public int damage = 10;
		public bool onlyPlaySoundsWhenHeld = true;
		public VRInteractableItem interactableItem;
		public List<AudioClip> swingSounds = new List<AudioClip>();
		public List<AudioClip> hitSounds = new List<AudioClip>();

		private Rigidbody selfBody;
		private AudioSource audioSource;
		private bool canPlay = true;

		void OnCollisionEnter(Collision col)
		{
			DamageInfo damageInfo = new DamageInfo();
			damageInfo.damage = damage;
			if (interactableItem != null && interactableItem.heldBy != null)
				damageInfo.velocity = interactableItem.heldBy.Velocity;
			else damageInfo.velocity = selfBody.velocity;
			col.transform.SendMessageUpwards("Damage", damageInfo, SendMessageOptions.DontRequireReceiver);
			if ((!onlyPlaySoundsWhenHeld || (interactableItem != null && interactableItem.heldBy != null)) && hitSounds.Count > 0)
			{
				audioSource.clip = hitSounds[Random.Range(0, hitSounds.Count)];
				audioSource.Play();
			}
		}

		void Start()
		{
			selfBody = GetComponent<Rigidbody>();
			audioSource = GetComponent<AudioSource>();
		}

		void Update()
		{
			if (interactableItem == null || swingSounds.Count == 0 || (onlyPlaySoundsWhenHeld && interactableItem.heldBy == null)) return;
			Vector3 velocity = Vector3.zero;
			if (interactableItem.heldBy != null)
				velocity = interactableItem.heldBy.Velocity;
			else
				velocity = selfBody.velocity;

			if (velocity.magnitude > 1)
			{
				if (canPlay)
				{
					audioSource.clip = swingSounds[Random.Range(0, swingSounds.Count)];
					audioSource.Play();
					canPlay = false;
				}
			} else
				canPlay = true;
		}
	}
}
#endif