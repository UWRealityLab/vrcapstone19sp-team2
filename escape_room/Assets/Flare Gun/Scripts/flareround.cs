using UnityEngine;
using System.Collections;


namespace Valve.VR.InteractionSystem { 
    public class flareround : MonoBehaviour {
        private GameObject flaregun;
        private flaregun flare;
        public AudioClip pickupSound;

        // Use this for initialization
        void Start()
        {
            flaregun = GameObject.Find("flaregun");
            flare = flaregun.GetComponent<flaregun>();


        }

        // Update is called once per frame
        void Update() {

        }

        void OnTriggerEnter(Collider other)
        {

            if (other.tag == "Player" && flare.spareRounds < flare.maxSpareRounds)
            {
                GetComponent<AudioSource>().PlayOneShot(pickupSound);
                flare.spareRounds++;
                Destroy(this.gameObject, pickupSound.length);
            }

        }
    }
}

