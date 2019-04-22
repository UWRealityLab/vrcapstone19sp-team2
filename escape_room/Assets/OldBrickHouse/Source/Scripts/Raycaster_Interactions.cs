using UnityEngine;
using System.Collections;

public class Raycaster_Interactions : OldHouseAnimations {
	
	Camera cam;
	[Range(1, 5)]
	public float rayDistance = 2f;
	public Texture2D crosshair, eButton;

	//Gasheater
	public AudioClip heaterOn;
	public AudioClip heaterOff;
	//------------------

	int crossHairStatus = 0;

	protected override void Start () {
		ConfigureCamera ();
		foreach (var a in FindObjectsOfType<Animation>()) {
			if (a.name.Equals ("House"))
				houseAnimation = a;
			else if (a.name.Contains ("Fence_Gate_Small"))
				smallGateAnimation = a;
			else if (a.name.Contains ("Fence_Gate_Large"))
				largeGateAnimation = a;	
		}
	}

	private void ConfigureCamera() {
		cam = Camera.main;
		if (cam == null) {
			Debug.LogError ("Main camera tag not found in scene! No pre-coded interactions will work!");
			Destroy (this.gameObject);
		}
		if (cam.allowMSAA)
			cam.allowMSAA = false;
		if (!cam.allowHDR)
			cam.allowHDR = true;
		
	}
	
	protected override void Update ()
	{
		Ray ray = cam.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));
		RaycastHit hit;

		showCrosshair = isAnimationPlaying () ? false : true;

		if (!isAnimationPlaying ()) {
			if (Physics.Raycast (ray, out hit, rayDistance)) {
				if (hit.transform.GetComponent<OldBrickHouse.Door> ()) {
					crossHairStatus = 1;
					if (Input.GetKeyDown (KeyCode.E)) {
						audioSourcePosition = hit.transform.position;
						OldBrickHouse.Door d = hit.transform.GetComponent<OldBrickHouse.Door> ();
						door = d;
						if (!door.isItOpen) {
							PlayAnimation (doorAnimation ());
						} else {
							PlayBackwards (doorAnimation ());
						}
					}
				} else if (hit.transform.GetComponent<OldHouse_Lights> ()) {
					crossHairStatus = 1;
					if (Input.GetKeyDown (KeyCode.E)) {
						audioSourcePosition = hit.transform.position;
						OldHouse_Lights light = hit.transform.GetComponent<OldHouse_Lights> ();
						lightSwitch = light.lightSwitchAnimation ();
						AnimateLightswitch (light.isItOn);
						light.SwitchLight ();

					}
				} else if (hit.transform.GetComponent<Gasheater> ()) {
					crossHairStatus = 1;
					if (Input.GetKeyDown (KeyCode.E)) {
						hit.transform.GetComponent<Gasheater> ().SwitchHeater ();
					}
				} else {
					crossHairStatus = 0;
				}
			} else {
				crossHairStatus = 0;
			}
		} 
	}

	void OnGUI() {
		if (showCrosshair) {
			switch (crossHairStatus) {
			case 0:
			//Draw default crosshair if integer set to 0
				Rect rect = new Rect (Screen.width / 2, Screen.height / 2, crosshair.width, crosshair.height);
				GUI.DrawTexture (rect, crosshair);
				break;
			case 1:
			//Draw E button sprite if integer set to 1 (object recognized by raycaster)
				Rect rect2 = new Rect ((Screen.width / 2) - eButton.width / 2, (Screen.height / 2) - eButton.height / 2, eButton.width, eButton.height);
				GUI.DrawTexture (rect2, eButton);
				break;
			}		                          
		}
	}
}
