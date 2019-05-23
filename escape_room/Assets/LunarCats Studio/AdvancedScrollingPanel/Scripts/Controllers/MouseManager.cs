using UnityEngine;
using System.Collections;

public class MouseManager : ControllersManager {

	public Material roleMat;										// Reference to gamepad Material
	public Material roleMatTransparent;								// Reference to the transparent material of gamepad
	public Material roleMatHighLighted;								// Reference to the highlight material of gamepad

	// Reference to buttons GameObjects of Mouse
	public GameObject leftButton;
	public GameObject rightButton;
	public GameObject roleButton;
	public GameObject body;

	// Use this for initialization
	void Start () {
		meshs = transform.Find ("Meshs").gameObject;
		
		// Add each buttons to the list
		buttons.Add (leftButton);			// 0
		buttons.Add (rightButton);			// 1
		buttons.Add (roleButton);			// 2
		buttons.Add (body);					// 3
	}

}
