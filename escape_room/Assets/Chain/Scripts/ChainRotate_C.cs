using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainRotate_C : MonoBehaviour {

    public float speedx = 0.0f;
    public float speedy = 0.0f;
    public float speedz = 0.0f;

    private bool UseCenter = false;

    // Use this for initialization
    void Start () {
        //if (UseCenter) { transform.position = GetComponent("Renderer").bounds.center + transform.position; }
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(speedx * Time.deltaTime, speedy * Time.deltaTime, speedz * Time.deltaTime);
    }
}
