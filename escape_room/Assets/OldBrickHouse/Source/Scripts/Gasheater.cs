using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gasheater : MonoBehaviour {

	public bool isThisOn = true;
	Renderer current;
	//Light light;
	Color offColor = Color.red * Mathf.LinearToGammaSpace (0.00001f);
	Color onColor = Color.red * Mathf.LinearToGammaSpace (8f);
	Raycaster_Interactions ri;

	void Start(){
		current = GetComponent<Renderer> ();
		//light = GetComponentInChildren<Light> ();
		ri = FindObjectOfType<Raycaster_Interactions> ();
	}

	public void SwitchHeater() {
		SetEmission ();
		GetComponentInChildren<Light> ().enabled = !isThisOn;
		if (isThisOn)
			AudioSource.PlayClipAtPoint (ri.heaterOff, transform.position);
		else {
			AudioSource.PlayClipAtPoint (ri.heaterOn, transform.position);
		}
		isThisOn = !isThisOn;
	}

	private void SetEmission() {
		
		Color activeColor;
		if (isThisOn)
			activeColor = offColor;
		else
			activeColor = onColor;
		
		current.material.SetColor ("_EmissionColor", activeColor);
		DynamicGI.SetEmissive (current, activeColor);
	}

}
