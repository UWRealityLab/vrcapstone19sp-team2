using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckGems : MonoBehaviour {

	public PanelMessageBox panelMessage;
	public GameObject gem1;
	public GameObject gem2;

	public void OnTriggerEnter(Collider other)
	{
		List<string> list = new List<string> ();

		if (!gem1.activeInHierarchy) {
			if (!gem2.activeInHierarchy) {
				list.Add ("Yeah you got all the precious gems !! :)");
				list.Add ("You finished perfectly this demonstration.");
			} else {
				list.Add ("You got one gem, this is pretty good");
				list.Add ("You finished nicely this demonstration.");
			}
		} else {
			list.Add ("You didn't got any gem but this is ok");
			list.Add ("You finished this demonstration.");
		}

		list.Add ("Thanks for purchasing this Asset !");
		list.Add ("Don't forget to leave us a review on the AssetStore if you want to support us.");

		panelMessage.SetMessageList (list);
		panelMessage.StartMessageDisplay ();

		Destroy (this);
	}
}
