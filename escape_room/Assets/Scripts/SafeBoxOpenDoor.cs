using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SafeBoxOpenDoor : MonoBehaviour
{
    // Start is called before the first frame update
    bool pass;
    CircularDrive circularDrive;
    void Start()
    {
        pass = false;
        circularDrive = this.GetComponent<CircularDrive>();
    }

    // Update is called once per frame
    void Update()
    {
        pass = this.GetComponentInChildren<SafeboxOpener>().okToOpenDoor;
        if (pass)
        {
            Destroy(GetComponent<IgnoreHovering>());
        }
    }
}
