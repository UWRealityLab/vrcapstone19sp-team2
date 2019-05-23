using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllersManager : MonoBehaviour {
	private enum materialsState {materialTransparent, materialNormal, materialHighlight }

	[HideInInspector]
	public bool isActive = false;									// True when gamepad is Active and displayed
	public Material mainMat;										// Reference to gamepad Material
	public Material mainMatTransparent;								// Reference to the transparent material of gamepad
	public Material mainMatHighLighted;								// Reference to the highlight material of gamepad

	protected List<GameObject> buttons = new List<GameObject>();	// Reference to every button GameObject of gamepad
	protected GameObject meshs;										// Reference to the Meshs GameObject object
	protected Dictionary<int, GameObject> highlightedButtons = new Dictionary<int, GameObject>();	//

	public void startFadeIn () {
		StartCoroutine ("ControllerFadeIn");
	}
	
	public void startFadeOut () {
		StartCoroutine ("ControllerFadeOut");
	}

	// Activate the GamePad model
	public void setActive (bool active_p) {
		isActive = active_p;
		if (meshs != null) {
			meshs.SetActive (isActive);
		}
		if (!active_p) {
			ClearData();
			StopAllCoroutines ();
		}
	}

	// Set the Y position of the GamePad GameObject
	public void SetYPosition(float posY_p)
	{
		transform.localPosition = new Vector3 (transform.localPosition.x, posY_p, transform.localPosition.z);
	}

	// Set the Y position of the GamePad GameObject
	public void SetXPosition(float posX_p)
	{
		transform.localPosition = new Vector3 (posX_p, transform.localPosition.y, transform.localPosition.z);
	}
	
	// Translate GamePad GameObject on Y axis
	public void TranslatePositionY(float deltaY_p)
	{
		transform.localPosition = transform.localPosition + new Vector3 (0, deltaY_p, 0);
	}

	// Highlight a button
	virtual public void HighlightButton (int index_p) {
		if (index_p >= buttons.Count) {
			return;
		}
		
		highlightedButtons.Add (index_p, buttons [index_p]);
		
		buttons [index_p].GetComponent<Renderer> ().sharedMaterial = mainMatHighLighted;
		
		HightLightEffect highlight = buttons [index_p].GetComponent<HightLightEffect> ();
		if (highlight != null) {
			highlight.enabled = true;
			highlight.UpdateMaterial ();
			
			// Synchronize every button's highlight effect timer
			if(highlightedButtons.Count > 1) {
				buttons [index_p].GetComponent<HightLightEffect> ().SetTimeReference ((new List<GameObject>(highlightedButtons.Values)).ToArray()[0].GetComponent<HightLightEffect> ().GetTimeReference ());
			}
		}
	}

	// Set the material for each buttons, except those who are highlighted
	void SetButtonsMaterial(materialsState material_p) {
		foreach (Transform child in meshs.transform) {
			if(!highlightedButtons.ContainsValue(child.gameObject)) {
				switch(material_p)
				{
				case materialsState.materialTransparent:
					child.GetComponent<Renderer> ().sharedMaterial = mainMatTransparent;
					break;
				case materialsState.materialNormal:
					child.GetComponent<Renderer> ().sharedMaterial = mainMat;
					break;
				case materialsState.materialHighlight:
					child.GetComponent<Renderer> ().sharedMaterial = mainMatHighLighted;
					break;
				}
			}
		}
	}

	// Reset all data parameters
	virtual protected void ClearData() {
		foreach (GameObject go in highlightedButtons.Values) {
			go.GetComponent<HightLightEffect> ().enabled = false;
			go.GetComponent<Renderer> ().sharedMaterial = mainMatTransparent;
		}
		highlightedButtons.Clear();
	}

	// Fade in transition
	IEnumerator ControllerFadeIn()
	{
		float t = 0f;
		SetButtonsMaterial (materialsState.materialTransparent);
		Color init = mainMatTransparent.color;
		init.a = 0;
		mainMatTransparent.color = init;
		Color finish = init;
		finish.a = 1;
		
		while (t <= 1) {
			mainMatTransparent.color = Color.Lerp (init, finish, t);
			t += Time.deltaTime;
			yield return null;
		}
		
		SetButtonsMaterial (materialsState.materialNormal);
		yield return null;
	}
	
	// Fade away transition
	IEnumerator ControllerFadeOut()
	{
		float t = 0f;
		SetButtonsMaterial (materialsState.materialTransparent);
		Color init = mainMatTransparent.color;
		init.a = 0.9f;
		mainMatTransparent.color = init;
		Color finish = init;
		finish.a = 0;
		
		while (t <= 1) {
			mainMatTransparent.color = Color.Lerp (init, finish, t);
			t += Time.deltaTime;
			yield return null;
		}
		ClearData ();
		yield return null;
	}
}
