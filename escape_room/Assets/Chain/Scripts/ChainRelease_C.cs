using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChainRelease_C : MonoBehaviour {

    public string Keystroke = "";

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(Keystroke))
        {
            if (gameObject.GetComponent("HingeJoint"))
            {
                Destroy(gameObject.GetComponent("HingeJoint"));
            }

            if (gameObject.GetComponent("CharacterJoint"))
            {
                Destroy(gameObject.GetComponent("CharacterJoint"));
            }
        }
    }
}
