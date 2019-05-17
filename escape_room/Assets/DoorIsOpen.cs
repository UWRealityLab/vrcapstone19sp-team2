using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorIsOpen : MonoBehaviour
{
    public Transform open;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.transform.position.Equals(open.position))
        {
            //Debug.Log("Door is at open position");
            this.GetComponent<AudioSource>().enabled = false;
        }
    }
}
