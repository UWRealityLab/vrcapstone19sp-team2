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
        [SerializeField]
        private GameObject muzzleflashPrefab;
        [SerializeField]
        private Transform muzzlePoint;
        public GameObject spawnedItem;
        [EnumFlags]
        public Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags;
        //attachedObject.originalParent = objectToAttach.transform.parent != null ? objectToAttach.transform.parent.gameObject : null;
        public GameObject theBullet;
        public Transform barrelEnd;

        public int bulletSpeed;
        public float despawnTime = 3.0f;

        public bool shootAble = true;
        public float waitBeforeNextShot = 0.25f;
        private void HandAttachedUpdate(Hand hand)
        {

            //Debug.Log("<b>[SteamVR Interaction]</b> Pickup: " + hand.GetGrabStarting().ToString());
            //if (SteamVR_Actions._default.Squeeze.GetAxis(LeftInputSource).Equals(1))

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
            //anim = bullet.GetComponent<Animation>();

        }
        private void FireWeapon()
        {
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


    }
}