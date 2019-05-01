//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: A linear mapping value that is used by other components
//
//=============================================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Globalization;

namespace Valve.VR.InteractionSystem
{
    //-------------------------------------------------------------------------
    public class LinearMapping : MonoBehaviour
    {
        public float value;
        Text text;

        private void Update()
        {
            //Debug.Log(value+"***");
            if (text == null)
                text = this.GetComponent<Text>();
            if (text == null)
                return;
            Debug.Log(text);
            if (text.text == null)
                text.text = "";
            string str = value.ToString("0.0");
            float flt = float.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
            flt = flt * 25.0f + 85.0f;
            string res = flt.ToString().Contains(".") ? flt.ToString() : flt.ToString() + ".0";
            text.text = res;
        }
    }
}
