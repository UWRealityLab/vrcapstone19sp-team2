using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisplayPanelMessageOnTrigger : MonoBehaviour {

	public GameObject player;
	public GameObject panelMessage;
	public bool onlyOnce = false;
	public bool setMessage = false;
	[Multiline]
	public List<string> messageList = new List<string>();	// List of all messages for each pages, in case of multiple page

	private bool hasTriggered = false;
	
	public void OnTriggerEnter(Collider other)
	{
		if (onlyOnce && hasTriggered) {
			return;
		}

		if (other.gameObject == player) {
			PanelMessageBox panelBox = panelMessage.GetComponent<PanelMessageBox>();
			if(panelBox != null && panelMessage.activeInHierarchy) {
				if (setMessage && messageList.Count > 0) {
					panelBox.SetMessageList(messageList);
				}

				panelBox.StartMessageDisplay();
				hasTriggered = true;
			}
		}
	}
}
