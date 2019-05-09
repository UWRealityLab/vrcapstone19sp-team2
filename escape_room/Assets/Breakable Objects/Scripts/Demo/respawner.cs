using UnityEngine;
using System;


public class respawner:MonoBehaviour
{
    public Transform target;
    public Transform replace;
    Vector3 posBuffer;
    Quaternion rotBuffer;
    
    public void Start(){
    
    posBuffer=target.position;
    rotBuffer=target.rotation;
    }
    
    public void Update() {
    if (target == null)
    
    {
        Vector3 pos = posBuffer;

			if (replace != null) {
				GameObject t = (GameObject)Instantiate(replace.gameObject, pos, rotBuffer);
				target = t.GetComponent<Transform>();

			}
		}
    }
}