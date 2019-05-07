using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;


public class SafeBoxPasscode : MonoBehaviour
{
    public bool pass;
    // Start is called before the first frame update
    void Start()
    {
        pass = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<LinearMapping>().value != 0)
        {
            pass = true;
        }
        Debug.Log("***" + pass);
    }
}
