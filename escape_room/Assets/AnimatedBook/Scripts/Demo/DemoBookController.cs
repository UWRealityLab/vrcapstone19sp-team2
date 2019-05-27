using UnityEngine;
using System.Collections;

public class DemoBookController : MonoBehaviour {

	public AnimatedBookController bookController;		// Reference to the BookController script

	public Sprite[] pageBackground;
	private int pageStyleIndex = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Control book with Left / Right arrows
	void Update () {
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			bookController.TurnToPreviousPage();
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			bookController.TurnToNextPage();
		}
	}

	public void switchPageStyle() {
		pageStyleIndex++;
		if (pageStyleIndex >= pageBackground.Length) {
			pageStyleIndex = 0;
		}
		bookController.defaultBackground = pageBackground [pageStyleIndex];

		foreach (AnimatedBookController.PageObjects page in bookController.getPageObjects()) {
			page.RectoImage.sprite = pageBackground [pageStyleIndex];
			page.VersoImage.sprite = pageBackground [pageStyleIndex];
		}
	}
}
