//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: A linear mapping value that is used by other components
//
//=============================================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
    //-------------------------------------------------------------------------
    public class LinearMapping : MonoBehaviour
    {
        public float value;
        Text text;

        private void Update()
        {
            if (text == null)
                text = this.GetComponent<Text>();
            text.text = value.ToString();
            Debug.Log(value);
        }
    }
}
