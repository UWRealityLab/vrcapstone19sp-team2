using UnityEngine;
using System.Collections;

public class BookUIButtonController : MonoBehaviour {

	private AnimatedBookController animatedBookController;

	// Use this for initialization
	void Start () {
		animatedBookController = FindObjectOfType<AnimatedBookController> ();
	}
	
	public void CallTurnNextPage() {
		animatedBookController.TurnToNextPage ();
	}

	public void CallTurnPreviousPage() {
		animatedBookController.TurnToPreviousPage ();
	}
}
