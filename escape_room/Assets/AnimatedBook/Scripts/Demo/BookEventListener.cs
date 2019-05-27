using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class BookEventListener : MonoBehaviour {

	//  The AnimatedBookController reference of the book to listen
	public AnimatedBookController targetBook;

	// The Unity Event to execute when the book is opened
	public UnityEvent onOpen;
	// The Unity Event to execute when the book is closed
	public UnityEvent onClose;

	// Use this for initialization
	void Start () {
		if (targetBook == null) {
			targetBook = GetComponent<AnimatedBookController>();
		}

		if (targetBook != null) {
			targetBook._onBookOpen += onBookOpen;
			targetBook._onBookClose += onBookClose;
		} else {
			Debug.LogError("The reference to the AnimatedBookController has not been set");
		}
	}

	private void onBookOpen() {
		onOpen.Invoke ();
	}

	private void onBookClose() {
		onClose.Invoke ();
	}
}
