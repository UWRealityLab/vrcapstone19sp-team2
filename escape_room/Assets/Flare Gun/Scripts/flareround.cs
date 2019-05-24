using UnityEngine;
using System.Collections;


namespace Valve.VR.InteractionSystem { 
    public class flareround : MonoBehaviour {
        public AudioClip pickupSound;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update() {

        }

        protected virtual void HandHoverUpdate(Hand hand)
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();

            if (startingGrabType == GrabTypes.Grip)
            {
                GetComponent<AudioSource>().PlayOneShot(pickupSound);
            }
        }

    }
}

