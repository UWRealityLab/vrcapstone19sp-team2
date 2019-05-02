using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.input

namespace Valve.VR.InteractionSystem {
    public class FlashLightSwitch : MonoBehaviour
    {
        public SteamVR_Input_Sources RightHandInputSource = SteamVR_Input_Sources.RightHand;
        // Update is called once per frame
        private void OnAttachedToHand(Hand hand)
        {
            if (SteamVR_Actions._default.Squeeze.GetAxis(RightHandInputSource).Equals(1))
            {
                Light light = this.GetComponentInChildren<Light>();
                if (light.enabled)
                {
                    light.enabled = false;
                } else
                {
                    light.enabled = true;
                }
                //light.enabled = false;
                Debug.Log("mmp");
            }
        }
    }
}
