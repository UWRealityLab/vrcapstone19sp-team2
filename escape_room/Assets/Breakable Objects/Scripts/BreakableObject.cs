/* 	Breakable Object
	(C) Unluck Software
	http://www.chemicalbliss.com
*/
#pragma warning disable 0618

using UnityEngine;
using System.Collections;


public class BreakableObject:MonoBehaviour{
	public Transform fragments; 					//Place the fractured object
	public float waitForRemoveCollider = 1.0f; 		//Delay before removing collider (negative/zero = never)
	public float waitForRemoveRigid = 10.0f; 		//Delay before removing rigidbody (negative/zero = never)
	public float waitForDestroy = 2.0f; 			//Delay before removing objects (negative/zero = never)
	public float explosiveForce = 350.0f; 			//How much random force applied to each fragment
	public float durability = 5.0f; 				//How much physical force the object can handle before it breaks
	public ParticleSystem breakParticles;			//Assign particle system to apear when object breaks
	public bool mouseClickDestroy;					//Mouse Click breaks the object
	Transform fragmentd;							//Stores the fragmented object after break
	bool broken;                                    //Determines if the object has been broken or not 
	Transform frags;

	public void OnCollisionEnter(Collision collision) {
	    if (collision.relativeVelocity.magnitude > durability) {
	        triggerBreak();
	    }
	}
	
	public void OnMouseDown() {
		if(mouseClickDestroy){
			triggerBreak();
		}
	}
	
	public void triggerBreak() {
	    Destroy(transform.FindChild("object").gameObject);
	    Destroy(transform.GetComponent<Collider>());
	    Destroy(transform.GetComponent<Rigidbody>());
	    StartCoroutine(breakObject());
	}

	// breaks object
	public IEnumerator breakObject() {
	    if (!broken) {
	    	if(this.GetComponent<AudioSource>() != null){
	    		GetComponent<AudioSource>().Play();
	    	}
	    	broken = true;
	    	if(breakParticles!=null){
				// adds particle system to stage
				ParticleSystem ps = (ParticleSystem)Instantiate(breakParticles,transform.position, transform.rotation);
				// destroys particle system after duration of particle system
				Destroy(ps.gameObject, ps.duration); 
	    	}
			// adds fragments to stage (!memo:consider adding as disabled on start for improved performance > mem)
			fragmentd = (Transform)Instantiate(fragments, transform.position, transform.rotation); 
			// set size of fragments
			fragmentd.localScale = transform.localScale;
			frags = fragmentd.FindChild("fragments");
			foreach (Transform child in frags) {
				Rigidbody cr = child.GetComponent<Rigidbody>();
				cr.AddForce(Random.Range(-explosiveForce, explosiveForce), Random.Range(-explosiveForce, explosiveForce), Random.Range(-explosiveForce, explosiveForce));
				cr.AddTorque(Random.Range(-explosiveForce, explosiveForce), Random.Range(-explosiveForce, explosiveForce), Random.Range(-explosiveForce, explosiveForce));
	        }
	        StartCoroutine(removeColliders());
	        StartCoroutine(removeRigids());
			// destroys fragments after "waitForDestroy" delay
			if (waitForDestroy > 0) { 
	            foreach(Transform child in transform) {
	   					child.gameObject.SetActive(false);
				}				
	            yield return new WaitForSeconds(waitForDestroy);
	            GameObject.Destroy(fragmentd.gameObject); 
	            GameObject.Destroy(transform.gameObject);
				// destroys gameobject
			} else if (waitForDestroy <=0){
	        	foreach(Transform child in transform) {
	   					child.gameObject.SetActive(false);
				}
	        	yield return new WaitForSeconds(1.0f);
	            GameObject.Destroy(transform.gameObject);
	        }	
	    }
	}

	// removes rigidbodies from fragments after "waitForRemoveRigid" delay
	public IEnumerator removeRigids() {
	    if (waitForRemoveRigid > 0 && waitForRemoveRigid != waitForDestroy) {
	        yield return new WaitForSeconds(waitForRemoveRigid);
	        foreach(Transform child in frags) {
	            child.GetComponent<Rigidbody>().isKinematic = true;
	        }
	    }
	}

	// removes colliders from fragments "waitForRemoveCollider" delay
	public IEnumerator removeColliders() {
	    if (waitForRemoveCollider > 0){
	        yield return new WaitForSeconds(waitForRemoveCollider);
	        foreach(Transform child in frags) {
	            child.GetComponent<Collider>().enabled = false;
	        }
	    }
	}
}