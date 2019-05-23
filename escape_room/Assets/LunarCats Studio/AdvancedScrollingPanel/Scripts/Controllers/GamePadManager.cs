using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePadManager : ControllersManager {

	public bool automaticOrienting = true;						// True to automatically orienting gamepad according to button displayed

	// Reference to buttons GameObjects of Gamepad
	public GameObject buttonA;
	public GameObject buttonB;
	public GameObject buttonX;
	public GameObject buttonY;
	public GameObject leftJoystick;
	public GameObject rightJoystick;
	public GameObject XPad;
	public GameObject XPadUp;
	public GameObject XPadLeft;
	public GameObject XPadDown;
	public GameObject XPadRight;
	public GameObject buttonMenu;
	public GameObject buttonLB;
	public GameObject buttonLT;
	public GameObject buttonRB;
	public GameObject buttonRT;
	public GameObject body;

	private Quaternion originOrientation;						// Origin orientation of gamepad

	// Use this for initialization
	void Start () {
		meshs = transform.Find ("Meshs").gameObject;
		originOrientation = transform.localRotation;

		// Add each buttons to the list
		buttons.Add (buttonA);					// 0
		buttons.Add (buttonB);					// 1
		buttons.Add (buttonX);					// 2
		buttons.Add (buttonY);					// 3
		buttons.Add (leftJoystick);				// 4
		buttons.Add (rightJoystick);			// 5
		buttons.Add (XPad);						// 6
		buttons.Add (XPadUp);					// 7
		buttons.Add (XPadLeft);					// 8
		buttons.Add (XPadDown);					// 9
		buttons.Add (XPadRight);				// 10
		buttons.Add (buttonMenu);				// 11
		buttons.Add (buttonLB);					// 12
		buttons.Add (buttonLT);					// 13
		buttons.Add (buttonRB);					// 14
		buttons.Add (buttonRT);					// 15
		buttons.Add (body);						// 16
	}

	// Reset all data parameters
	override protected void ClearData() {
		base.ClearData ();
		this.transform.localRotation = originOrientation;
	}

	// Highlight a button
	override public void HighlightButton (int index_p) {
		base.HighlightButton (index_p);
		
		// If button to show is either L1, R1, L2 or R2, we can automatically rotate the GamePad
		if (automaticOrienting && index_p >= 12) {
			StartCoroutine ("RotateGamePad");
		}
	}

	// Rotate GamePad 
	IEnumerator RotateGamePad()
	{
		float t = 0f;
		while (t <= 1) {
			transform.localRotation = Quaternion.Lerp (originOrientation, originOrientation * Quaternion.Euler (-60, 0, 0), t);
			t += 2*Time.deltaTime;
			yield return null;
		}
		yield return null;
	}
}
