using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OldHouseAnimations : MonoBehaviour {

	//Handles animation playing. Exposed variables can be accessed in Raycaster_Interactions class (it derives from this class).

	protected Animation houseAnimation;
	protected Animation smallGateAnimation;
	protected Animation largeGateAnimation;
	protected Animation lightSwitch;
	protected OldBrickHouse.Door door;

	private const string DOOR_FRONT = "Door_Front_Open";
	private const string DOOR_BACK = "Door_Back";
	private const string TRAPDOOR = "Trapdoor_Open";
	private const string DOOR_INTERIOR = "Door_Interior";
	private const string DOOR_INTERIOR_02 = "Door_Interior_02";
	private const string DOOR_INTERIOR_03 = "Door_Interior_03";
	private const string DOOR_INTERIOR_04 = "Door_Interior_04";
	private const string DOOR_INTERIOR_05 = "Door_Interior_05";
	private const string DOOR_INTERIOR_06 = "Door_Interior_06";
	private const string DOOR_INTERIOR_07 = "Door_Interior_07";
	private const string W_ATTIC_01 = "Window_Attic_01";
	private const string W_ATTIC_02 = "Window_Attic_02";

	public AudioClip[] doorHandleSounds;
	public AudioClip trapdoorOpenSound;
	public AudioClip trapdoorCloseSound;

	public AudioClip outdoorLightswitch;
	public AudioClip indoorLightOn;
	public AudioClip indoorLightOff;

	public AudioClip smallGate;
	public AudioClip largeGate_01;
	public AudioClip largeGate_02;

	public bool showCrosshair;

	protected abstract void Start();
	protected abstract void Update ();

	protected Vector3 audioSourcePosition;

	protected void PlayAnimation(string clipName) {
		if (!door.thisIsGate) {
			//Play audio
			if (doorAnimation ().Equals (TRAPDOOR)) {
				AudioSource.PlayClipAtPoint (trapdoorOpenSound, audioSourcePosition);
			} else {
				if (doorHandleSounds.Length > 0)
					AudioSource.PlayClipAtPoint (RandomHandleClip (), audioSourcePosition);
			}
			//End of audio
			PlayAnimationClip(houseAnimation, clipName, false);
		} else {
			//Check if it's small gate or large gate
			if (isItSmallGate ()) {
				AudioSource.PlayClipAtPoint (smallGate, audioSourcePosition);
				PlayAnimationClip (smallGateAnimation, smallGateAnimation.clip.name, false);
			} else { 
				PlayAnimationClip (largeGateAnimation, LargeGateClip ().name, false);
				if (door.gateType == OldBrickHouse.Door.gates.Large_Gate_01)
					StartCoroutine(PlayAudioDelayed (largeGate_01, 1.5f));
				else
					AudioSource.PlayClipAtPoint (largeGate_02, audioSourcePosition);
			}
		}

		door.isItOpen = !door.isItOpen;
		
	}

	protected void PlayBackwards(string clipName) {
		if (!door.thisIsGate) {
			PlayAnimationClip (houseAnimation, clipName, true);
			if (doorAnimation ().Equals (TRAPDOOR)) {
				AudioSource.PlayClipAtPoint (trapdoorCloseSound, audioSourcePosition);	
			} else {
				StartCoroutine (PlayAudioDelayed (RandomHandleClip (), 0.8f));
			}
		} else {
			if (isItSmallGate ()) {
				PlayAnimationClip (smallGateAnimation, smallGateAnimation.clip.name, true);
			} else { //Large gate
				if (door.gateType == OldBrickHouse.Door.gates.Large_Gate_01)
					AudioSource.PlayClipAtPoint (largeGate_01, audioSourcePosition);
				PlayAnimationClip (largeGateAnimation, LargeGateClip ().name, true);
			}

		}
		door.isItOpen = !door.isItOpen;
	}

	protected void AnimateLightswitch(bool backward){
		PlayAnimationClip (lightSwitch, "Switch", backward);
	}

	private AudioClip RandomHandleClip () {
		int id = Random.Range (0, doorHandleSounds.Length);
		return doorHandleSounds [id];
	}

	private void PlayAnimationClip(Animation animation, string clip, bool reversed) {
		if (!reversed) {
			animation [clip].speed = 1f;
			animation [clip].time = 0;
			animation.PlayQueued (clip);
		} else {
			animation [clip].speed = -1f;
			animation [clip].time = animation [clip].length;
			animation.Play (clip);
		}
	}

	private IEnumerator PlayAudioDelayed(AudioClip clip, float delay) {
		yield return new WaitForSeconds (delay);
		AudioSource.PlayClipAtPoint (clip, audioSourcePosition);
	}

	protected bool isAnimationPlaying(){
		if (smallGateAnimation == null || largeGateAnimation == null) {
			if (houseAnimation.isPlaying || isLightSwitchPlaying ()) {
				return true;
			} else {
				return false;
			}
		} else { //If no gates in scene
			if (largeGateAnimation.isPlaying || smallGateAnimation.isPlaying || houseAnimation.isPlaying || isLightSwitchPlaying ())
				return true;
			else
				return false;
		}
	}

	private bool isLightSwitchPlaying() {
		if (lightSwitch == null) {
			return false;
		} else {
			if (lightSwitch.isPlaying) {
				return true;
			} else {
				return false;
			}
		}
	}

	protected string doorAnimation() {
		if (door.doorType.Equals (OldBrickHouse.Door.ID.Front_Door)) {
			return DOOR_FRONT;
		} else if (door.doorType.Equals (OldBrickHouse.Door.ID.Trapdoor)) {
			return TRAPDOOR;
		} else if (door.doorType.Equals (OldBrickHouse.Door.ID.Door_Interior)) {
			return DOOR_INTERIOR;	
		} else if (door.doorType.Equals (OldBrickHouse.Door.ID.Door_Interior_02)) {
			return DOOR_INTERIOR_02;	
		} else if (door.doorType.Equals (OldBrickHouse.Door.ID.Door_Interior_03)) {
			return DOOR_INTERIOR_03;
		} else if (door.doorType.Equals (OldBrickHouse.Door.ID.Door_Interior_04)) {
			return DOOR_INTERIOR_04;
		} else if (door.doorType.Equals (OldBrickHouse.Door.ID.Door_Interior_05)) {
			return DOOR_INTERIOR_05;
		} else if (door.doorType.Equals (OldBrickHouse.Door.ID.Door_Interior_06)) {
			return DOOR_INTERIOR_06;
		} else if (door.doorType.Equals (OldBrickHouse.Door.ID.Door_Interior_07)) {
			return DOOR_INTERIOR_07;
		} else if (door.doorType.Equals (OldBrickHouse.Door.ID.Backdoor)) {
			return DOOR_BACK;
		} else if (door.doorType.Equals (OldBrickHouse.Door.ID.Window_Attic_01)) {
			return W_ATTIC_01;
		} else if (door.doorType.Equals (OldBrickHouse.Door.ID.Window_Attic_02)) {
			return W_ATTIC_02;
		} 
		else {
			throw new UnityException ("Not implemented doortype");
		}
	}

	private bool isItSmallGate() {
		
		if (door.gateType.Equals (OldBrickHouse.Door.gates.Small_Gate))
			return true;
		else
			return false;
	}

	private AnimationClip LargeGateClip() {
		if (!isItSmallGate ()) {
			if (door.gateType.Equals (OldBrickHouse.Door.gates.Large_Gate_01)) {
				return largeGateAnimation ["Gate_01"].clip;
			} else if (door.gateType.Equals (OldBrickHouse.Door.gates.Large_Gate_02)) {
				return largeGateAnimation ["Gate_02"].clip;
			} else
				throw new UnityException ("Unrecognized value");
		} else {
			return null;
		}
	}
		
}
