using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldHouse_Lights : MonoBehaviour {

	public bool isItOn = false;

	public ReflectionProbe[] probesAffected;
	public GameObject[] lampObjects;

	private Raycaster_Interactions ri;

	private enum switchType
	{
		Indoor,
		Outdoor
	}

	private switchType typeOfSwitch;

	void Start(){
		if (transform.name.Contains ("Lightswitch_02")) {
			typeOfSwitch = switchType.Outdoor;
		} else {
			typeOfSwitch = switchType.Indoor;
		}
		ri = FindObjectOfType<Raycaster_Interactions> ();
	}

	public Animation lightSwitchAnimation(){
		if (!transform.GetComponent<Animation> ()) {
			return transform.parent.GetComponent<Animation> ();
		} else {
			return transform.GetComponent<Animation> ();
		}
	}

	public void SwitchLight(){
		List<Renderer> renderers = new List<Renderer> ();
		List<Light> lights = new List<Light> ();

		//Toggle panel indicator light

		if (typeOfSwitch.Equals (switchType.Indoor)) {
			Renderer switchRenderer = GetComponent<Renderer> ();
			switchRenderer.material.SetColor ("_EmissionColor", IndicatorColor ());
			DynamicGI.SetEmissive (switchRenderer, IndicatorColor ());
		}

		//back to room lamp

		AudioSource.PlayClipAtPoint (switchSound (), transform.position);

		foreach (var go in lampObjects) {
			renderers.Add (go.GetComponent<Renderer> ());
			lights.Add(go.transform.parent.GetComponentInChildren<Light>());
			lights.Add(go.GetComponentInChildren<Light>());
		}

		foreach (var l in lights) {
			if (l != null)
				l.enabled = !isItOn;
		}

		foreach (var r in renderers) {
			r.material.SetColor ("_EmissionColor", ColorToSet ());
			DynamicGI.SetEmissive (r, ColorToSet ());
		}

		foreach (var rp in probesAffected) {
			rp.RenderProbe ();
			StartCoroutine (secondaryBake (rp));
		}


		isItOn = !isItOn;

	}

	private AudioClip switchSound() {
		if (typeOfSwitch == switchType.Outdoor) {
			return ri.outdoorLightswitch;
		} else {
			if (isItOn) {
				return ri.indoorLightOn;
			} else {
				return ri.indoorLightOff;
			}
		}
	}

	private IEnumerator secondaryBake(ReflectionProbe rp){
		yield return new WaitForSeconds (1.5f);
		rp.RenderProbe ();
	}

	private Color ColorToSet() {
		Color c;
		if (!isItOn) {
			c = Color.white * Mathf.LinearToGammaSpace (10f);
		} else {
			c = Color.white * Mathf.LinearToGammaSpace (0.001f);
		}
		return c;
	}

	private Color IndicatorColor() {
		Color c;
		if (isItOn) {
			c = Color.white * Mathf.LinearToGammaSpace (10f);
		} else {
			c = Color.white * Mathf.LinearToGammaSpace (0.001f);
		}
		return c;
	}

}
