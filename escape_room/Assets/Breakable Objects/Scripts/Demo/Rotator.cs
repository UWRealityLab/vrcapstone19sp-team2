using UnityEngine;
using System;


public class Rotator:MonoBehaviour
{
    public float rightSpeed = 10.0f;
    public float upSpeed = 10.0f;
    
    public void Update() {
    transform.Rotate(Vector3.right*rightSpeed*Time.deltaTime);
    transform.Rotate(Vector3.up*upSpeed*Time.deltaTime);
    }
}