using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Valve.VR.InteractionSystem
{
    public class Magazine : MonoBehaviour
    {
        public GameObject handler;
        public GameObject gun;

        // Start is called before the first frame update
        void Start()
        {
            //Physics.IgnoreCollision(shaftRB.GetComponent<Collider>(), Player.instance.headCollider);
        }

        // Update is called once per frame
        public void OnTriggerEnter(Collider other)
        {

            if (other.gameObject.name == "MCollider")
            {
                this.gameObject.AddComponent<IgnoreHovering>();
                //hand.DetachObject(gameObject, restoreOriginalParent);
                Destroy(this.GetComponent<Pickable>());
                Destroy(this.GetComponent<Rigidbody>());

                if (this.GetComponent<Interactable>().attachedToHand)
                {
                    //this.GetComponent<Rigidbody>();
                    this.GetComponent<Interactable>().attachedToHand.DetachObject(this.gameObject, false);
  
                }
                //var magazine = Instantiate(this.gameObject, this.gameObject.transform.position, this.gameObject.transform.rotation);
                //this.gameObject.SetActive(false);
                
                Destroy(this.GetComponent<Interactable>());
                
                handler.SetActive(true);
                //this.transform.SetParent(other.gameObject.transform);
                //this.gameObject.transform.position = other.gameObject.transform.position;
                gun.GetComponent<shooter>().enable = true;
                gun.GetComponent<shooter>().loadMagazine();
                Destroy(this.gameObject);

            }
        }

        public void OnTriggerStay(Collider other)
        {
            if (other.gameObject.name == "MCollider")
            {
                //Debug.Log(other.gameObject.name);
                //hand.DetachObject(gameObject, restoreOriginalParent);
                //Destroy(gameObject);
                //var bullet = Instantiate(gameObject, other.gameObject.transform.position, other.gameObject.transform.rotation);
                //this.transform.position = other.gameObject.transform.position;
            }
        }
    }
}
