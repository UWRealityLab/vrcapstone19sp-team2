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
        
        [SerializeField]
        private GameObject muzzleflashPrefab;
        [SerializeField]
        private Transform muzzlePoint;
        [EnumFlags]
        public Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags;
        //attachedObject.originalParent = objectToAttach.transform.parent != null ? objectToAttach.transform.parent.gameObject : null;
        public GameObject theBullet;
        public Transform barrelEnd;
        public bool enable;
        public int bulletSpeed;
        public float despawnTime = 3.0f;

        public bool shootAble = true;
        public float waitBeforeNextShot = 0.25f;
        private void HandAttachedUpdate(Hand hand)
        {

            //Debug.Log("<b>[SteamVR Interaction]</b> Pickup: " + hand.GetGrabStarting().ToString());
            //if (SteamVR_Actions._default.Squeeze.GetAxis(LeftInputSource).Equals(1))
            if (enable)
            {
                if (hand.GetGrabStarting() == GrabTypes.Pinch)
                {
                    //shootBullet();
                    if (shootAble)
                    {
                        shootAble = false;
                        Shoot();
                        StartCoroutine(ShootingYield());
                    }
                    FireWeapon();

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
            var bullet = Instantiate(theBullet, barrelEnd.position, barrelEnd.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

            Destroy(bullet, despawnTime);
        }

        private void Start()
        {
            audiosource = GetComponent<AudioSource>();
            ani = GetComponent<Animation>();
            //anim = bullet.GetComponent<Animation>();
            //audiosource.clip = mag;
        }

        private void FireWeapon()
        {
            audiosource.clip = fire;
            audiosource.Play();
            var muzzleflash = Instantiate(muzzleflashPrefab, muzzlePoint.position, muzzlePoint.rotation);
            Destroy(muzzleflash.gameObject, 0.5f);
        }

        //private void shootBullet()
        //{
        //var bull = Instantiate(bullet, bulletPoint.position, bulletPoint.rotation);

        //anim.Play();
        //Destroy(bull.gameObject, 0.5f);
        //}

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name == "Mag Script")
            {
                //Debug.Log(other.gameObject.name);
                //Destroy(other.gameObject);
                //var bullet = Instantiate(other.gameObject, muzzlePoint.transform.position, muzzlePoint.transform.rotation);
                //transform.position = other.gameObject.transform.position;
                //other.gameObject.transform.position = muzzlePoint.position;
            }
        }

        public void loadMagazine()
        {
            ani.Play();
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
    }
}