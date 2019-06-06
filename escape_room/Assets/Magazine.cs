using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Valve.VR.InteractionSystem
{
    public class Magazine : MonoBehaviour
    {
        public GameObject magazine_hidden;
        public GameObject gun;


        // Update is called once per frame


        public void OnTriggerEnter(Collider other)
        {
            if (GetComponent<Interactable>().attachedToHand && gun.GetComponent<Interactable>().attachedToHand)
            {
                if (other.gameObject.name == "MCollider")
                {
                    this.GetComponent<Interactable>().highlightOnHover = false;
                    this.gameObject.AddComponent<IgnoreHovering>();
                    //hand.DetachObject(gameObject, restoreOriginalParent);
                    Destroy(this.GetComponent<Throwable>());
                    Destroy(this.GetComponent<Rigidbody>());

                    if (this.GetComponent<Interactable>().attachedToHand)
                    {
                        //this.GetComponent<Rigidbody>();
                        this.GetComponent<Interactable>().attachedToHand.DetachObject(this.gameObject, false);

                    }
                    //var magazine = Instantiate(this.gameObject, this.gameObject.transform.position, this.gameObject.transform.rotation);
                    //this.gameObject.SetActive(false);

                    Destroy(this.GetComponent<Interactable>());

                    magazine_hidden.SetActive(true);

                    // gun.GetComponent<shooter>().enable = true;
                    gun.GetComponent<shooter>().loadMagazine();
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
