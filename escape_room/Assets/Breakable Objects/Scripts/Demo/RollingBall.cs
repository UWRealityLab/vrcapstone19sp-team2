using UnityEngine;
using System;


public class RollingBall:MonoBehaviour{
    public void FixedUpdate() {
    if (Input.GetKey ("up")||Input.GetKey ("w")) {
    GetComponent<Rigidbody>().AddForce (Camera.main.transform.forward * 450);
    }
    if (Input.GetKey ("down")||Input.GetKey ("s")) {
    GetComponent<Rigidbody>().AddForce (Camera.main.transform.forward * -250);
    }
    if (Input.GetKey ("right")||Input.GetKey ("d")) {
    transform.Rotate(Camera.main.transform.up,Space.World);
    }
    if (Input.GetKey ("left")||Input.GetKey ("a")) {
    transform.Rotate(Camera.main.transform.up,Space.World);
    }
    //if (Input.GetKey ("space")||Input.GetKey ("x")) {
    //}
    //var both = Mathf.Abs(rigidbody.velocity.x*.01) +Mathf.Abs(rigidbody.velocity.z*.01);
    //var foo = Vector3(1+both,1-both,1+both);
    //transform.localScale =foo;
    }
//function OnMouseOver(){

//var b:GameObject = Instantiate(bullet,Camera.main.transform.position ,Camera.main.transform.rotation);
//b.rigidbody.AddForce (Camera.main.transform.forward * 350);


//}
}