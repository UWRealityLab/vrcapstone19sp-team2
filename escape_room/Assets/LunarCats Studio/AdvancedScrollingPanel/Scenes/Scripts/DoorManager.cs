using UnityEngine;
using System.Collections;

public class DoorManager : MonoBehaviour {

	public GameObject player;
	public GameObject key;
	public GameObject UseKeyPanel;
	public Animator doorAnimator;
	public PanelMessageBox playerPanelMessage;
	public PanelMessageBox doorUnlockedMessage;

	private bool isDoorOpenned = false;

	private void Start()
	{
	}

	/*public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == player && this.enabled) {
			openDoor();
		}
	}*/

	public void openDoor() {
		if (!key.activeSelf && !isDoorOpenned) {
			// Open the door
			doorAnimator.enabled = true;
			// Display the player message
			playerPanelMessage.StartMessageDisplay();
			doorUnlockedMessage.StartMessageDisplay();
			UseKeyPanel.SetActive (false);

			isDoorOpenned = true;
		}
	}
}
