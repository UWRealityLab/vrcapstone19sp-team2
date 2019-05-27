using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmMOvement : MonoBehaviour {

    // Use this for initialization
    Transform head;
    Quaternion startingRot;

	void Start () {
        head = Camera.main.transform;
        startingRot = transform.rotation;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        // get Y rotation angle of head
        float Yangle = head.eulerAngles[1];

        //set rotation with lerp
        Quaternion rotation = Quaternion.Euler(0,Yangle,0);

        // arm transform rotation with lerp
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation*startingRot, 0.05f);
		
	}
}
