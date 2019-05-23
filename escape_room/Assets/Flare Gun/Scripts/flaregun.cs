using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
    public class flaregun : MonoBehaviour
    {

        public Rigidbody flareBullet;
        public Transform barrelEnd;
        public GameObject muzzleParticles;
        public AudioClip flareShotSound;
        public AudioClip noAmmoSound;
        public AudioClip reloadSound;
        public int bulletSpeed = 2000;
        public int maxSpareRounds = 5;
        public int spareRounds = 3;
        public int currentRound = 5;




        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        private void HandAttachedUpdate(Hand hand)
        {

            if (hand.GetGrabStarting() == GrabTypes.Pinch && !GetComponent<Animation>().isPlaying)
            {
                // Debug.Log("1");
                if (currentRound > 0)
                {
                   // Debug.Log("2");
                    Shoot();
                }
                else
                {
                    // Debug.Log("3");
                    GetComponent<Animation>().Play("noAmmo");
                    GetComponent<AudioSource>().PlayOneShot(noAmmoSound);
                }
            }
            if (hand.GetGrabStarting() == GrabTypes.Grip && !GetComponent<Animation>().isPlaying)
            {
                // Debug.Log("4");
                Reload();

            }

        }

        void Shoot()
        {
            currentRound--;
            if (currentRound <= 0)
            {
                currentRound = 0;
            }



            GetComponent<Animation>().CrossFade("Shoot");
            GetComponent<AudioSource>().PlayOneShot(flareShotSound);


            Rigidbody bulletInstance;
            bulletInstance = Instantiate(flareBullet, barrelEnd.position, barrelEnd.rotation) as Rigidbody; //INSTANTIATING THE FLARE PROJECTILE

            bulletInstance.AddForce(barrelEnd.forward * bulletSpeed); //ADDING FORWARD FORCE TO THE FLARE PROJECTILE
            Instantiate(muzzleParticles, barrelEnd.position, barrelEnd.rotation);   //INSTANTIATING THE GUN'S MUZZLE SPARKS	


        }

        void Reload()
        {
            if (spareRounds >= 1 && currentRound == 0)
            {
                GetComponent<AudioSource>().PlayOneShot(reloadSound);
                spareRounds--;
                currentRound++;
                GetComponent<Animation>().CrossFade("Reload");
            }

        }
    }
}
