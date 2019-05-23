using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour {

	public Transform target;

	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.LookRotation(-new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z));
	}
}
