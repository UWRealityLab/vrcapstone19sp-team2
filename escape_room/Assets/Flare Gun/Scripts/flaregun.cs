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
                if (currentRound > 0)
                {
                    Shoot();
                }
                else
                {
                    GetComponent<Animation>().Play("noAmmo");
                    GetComponent<AudioSource>().PlayOneShot(noAmmoSound);
                }
            }
        }

        void Shoot()
        {
            currentRound--;
            if (currentRound <= 0)
            {
                currentRound = 0;
            }

            if (spareRounds == 0)
            {
                GameManagerScript manager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                manager.TriggerEvent(GameManagerScript.EventTypes.FAILED);
            }

            GetComponent<Animation>().CrossFade("Shoot");
            GetComponent<AudioSource>().PlayOneShot(flareShotSound);


            Rigidbody bulletInstance;
            bulletInstance = Instantiate(flareBullet, barrelEnd.position, barrelEnd.rotation) as Rigidbody; //INSTANTIATING THE FLARE PROJECTILE

            bulletInstance.AddForce(barrelEnd.forward * bulletSpeed); //ADDING FORWARD FORCE TO THE FLARE PROJECTILE
            Instantiate(muzzleParticles, barrelEnd.position, barrelEnd.rotation);   //INSTANTIATING THE GUN'S MUZZLE SPARKS	
        }

        bool Reload()
        {
            if (spareRounds >= 1 && currentRound == 0)
            {
                GetComponent<AudioSource>().PlayOneShot(reloadSound);
                spareRounds--;
                currentRound++;
                GetComponent<Animation>().CrossFade("Reload");
                return true;
            }
            return false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (GetComponent<Interactable>().attachedToHand)
            {
                if (other.name == "FlareRoundTrigger" && other.gameObject.GetComponentInParent<Interactable>().attachedToHand)
                {
                    if (Reload())
                    {
                        // Debug.Log("reload");
                        if (other.GetComponentInParent<Interactable>().attachedToHand)
                        {
                            other.GetComponentInParent<Interactable>().attachedToHand.DetachObject(other.transform.parent.gameObject, false);
                        }
                        other.transform.parent.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
