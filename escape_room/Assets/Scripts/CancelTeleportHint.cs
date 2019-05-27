using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class CancelTeleportHint : MonoBehaviour
{

    // Update is called once per frame
    void Start()
    {
        Teleport.instance.CancelTeleportHint();
    }
}