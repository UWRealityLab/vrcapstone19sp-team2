//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Attach this script to the hand controller and reference the animator your hand is using.
// Setup the Animator to have an integer variable called state, then link each possible
// state to an animation. By default it should work if you have state 0 as default and state 1
// as gripped.
// As long as this script is bellow the VRInteractor the pickup will have already been processed
// by the time the input received on this script executes, so you can check if the interactor heldItem
// is equal to null, if it's not you will be able to check what it is and use a specific animation depending
// on the object.
//
//===================Contact Email: Sam@MassGames.co.uk===========================
#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRInteraction;

public class VRHandAnimator : MonoBehaviour {

	public Animator _handAnim;

	private VRInteractor _interactor;

	public void InputReceived(string method)
	{
		if (_interactor == null) _interactor = GetComponent<VRInteractor>();
		switch(method)
		{
		case "ACTION":
		case "PICKUP_DROP":
			if (_handAnim != null) _handAnim.SetInteger("State", 1);
			break;
		case "ACTIONReleased":
		case "PICKUP_DROPReleased":
			if (_handAnim != null) _handAnim.SetInteger("State", 0);
			break;
		}
	}
}
#endif