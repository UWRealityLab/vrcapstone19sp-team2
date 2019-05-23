using UnityEngine;
using UnityEngine.Events;


public class MouseEvent : MonoBehaviour {

	public UnityEvent onMouseClick;
	public UnityEvent onMouseEnter;

	// Use this for initialization
	private void Start()
	{
	}

	void OnMouseDown(){
		if (this.enabled) {
			onMouseClick.Invoke();
		}
	}

	void OnMouseEnter() {
		if (this.enabled) {
			onMouseEnter.Invoke();
		}
	}
}
