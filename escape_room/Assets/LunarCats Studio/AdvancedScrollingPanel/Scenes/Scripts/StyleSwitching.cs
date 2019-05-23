using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StyleSwitching : MonoBehaviour {

	public Text styleText;
	public List<PanelMessageBox> PanelMessages = new List<PanelMessageBox>();			// List of messages that can be displayed
	private int index = 0;																// Index of the Message displayed

	void Start() {
		styleText.text = PanelMessages [index].transform.parent.parent.name;
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
	}

	// Display the current style message
	public void DisplayCurrentMessage() {
		if (index >= 0 && index < PanelMessages.Count) {
			PanelMessages [index].StartMessageDisplay ();
			styleText.text = PanelMessages [index].transform.parent.parent.name;
		}
	}

	// Close the current style message
	public void CloseCurrentMessage() {
		if (index >= 0 && index < PanelMessages.Count) {
			PanelMessages [index].CloseMessage ();
		}
	}
	
	// Close the current message and display the next style message
	public void SwitchStyle () {
		CloseCurrentMessage();
		index++;
		if (index >= PanelMessages.Count) {
			index = 0;
		}
		Invoke("DisplayCurrentMessage", 0.5f);
	}
}
