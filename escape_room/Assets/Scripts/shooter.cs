using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Valve.VR;
namespace Valve.VR.InteractionSystem
{
    public class shooter : MonoBehaviour
    {
        public SteamVR_Input_Sources LeftInputSource = SteamVR_Input_Sources.LeftHand;
        public SteamVR_Input_Sources RightInputSource = SteamVR_Input_Sources.RightHand;
        //[SerializeField]
        //private GameObject bullet;
        //[SerializeField]
        // private Transform bulletPoint;
        //private Animation anim;

        private AudioSource audiosource;
        public AudioClip fire;
        public AudioClip mag;
        private Animation ani;
        public AudioClip sliderpull;
        public AudioClip sliderrelease;
        public AudioClip dryfire;
        public int currentRound = 1;

        [SerializeField]
        private GameObject muzzleflashPrefab;
        [SerializeField]
        private Transform muzzlePoint;

        public GameObject theBullet;
        public Transform barrelEnd;
        public bool enable;
        public int bulletSpeed;
        public float despawnTime = 3.0f;

        public bool shootAble = true;
        public float waitBeforeNextShot = 0.25f;
        private void HandAttachedUpdate(Hand hand)
        {
            if (enable)
            {
                if (hand.GetGrabStarting() == GrabTypes.Pinch)
                {
                    //shootBullet();
                    if (shootAble)
                    {
                        if (currentRound > 0)
                        {
                            shootAble = false;
                            Shoot();
                            FireWeapon();
                            StartCoroutine(ShootingYield());
                            if (currentRound == 0)
                            {
                                ani.Play("NoBulletPos");
                            }
                        } else
                        {
                            playDryfire();
                            ani.Play("NoBullet");
                        }                        
                    }
                    

                }
            }
        }

        IEnumerator ShootingYield()
        {
            yield return new WaitForSeconds(waitBeforeNextShot);
            shootAble = true;
        }
        void Shoot()
        {
            currentRound--;
            if (currentRound <= 0)
            {
                currentRound = 0;
            }
            var bullet = Instantiate(theBullet, barrelEnd.position, barrelEnd.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

            Destroy(bullet, despawnTime);
        }

        private void Start()
        {
            audiosource = GetComponent<AudioSource>();
            ani = GetComponent<Animation>();
        }

        private void FireWeapon()
        {
            ani.Play("TriggerWhenShot");
            audiosource.clip = fire;
            audiosource.Play();
            var muzzleflash = Instantiate(muzzleflashPrefab, muzzlePoint.position, muzzlePoint.rotation);
            Destroy(muzzleflash.gameObject, 0.5f);
        }

        public void loadMagazine()
        {
            // Turn off shootable while loading
            shootAble = false;

            ani.Play("M9Hammer");
            audiosource.clip = mag;
            audiosource.Play();            
        }

        public void playPull()
        {
            audiosource.clip = sliderpull;
            audiosource.Play();
        }

        public void playRelease()
        {
            audiosource.clip = sliderrelease;
            audiosource.Play();
        }

        public void playDryfire()
        {
            audiosource.clip = dryfire;
            audiosource.Play();
        }

        public void loadedTrigger()
        {
            shootAble = true;

            // trigger
            GameManagerScript manager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            manager.TriggerEvent(GameManagerScript.EventTypes.GUN_LOADED);
        }
    }
}