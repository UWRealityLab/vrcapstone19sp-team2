using UnityEngine;
using System.Collections;

public class ActivateOnTrigger : MonoBehaviour {

	public GameObject target;

	public void OnTriggerEnter(Collider other)
	{
		if (target != null) {
			if(target.GetComponent<MouseEvent>() != null) {
				target.GetComponent<MouseEvent>().enabled = true;
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (target != null) {
			if(target.GetComponent<MouseEvent>() != null) {
				target.GetComponent<MouseEvent>().enabled = false;
			}
		}
	}
}
