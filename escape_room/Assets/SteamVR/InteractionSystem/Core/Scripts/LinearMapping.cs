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
            Debug.Log(value+"***");
            string internalText;
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
            //string res = flt.ToString().EndsWith(".0") ? flt.ToString
            string res = flt.ToString().Contains(".") ? flt.ToString() : flt.ToString() + ".0";
            text.text = res;
            //text.text = (float.Parse(value.ToString("0.00"), CultureInfo.InvariantCulture.NumberFormat) * 25.0f + 85.0f).ToString();
            //if (Mathf.Abs(value) < 0.01f)
            //{
            //    value = 1.0f;
            //}
            //if (text == null)
            //    text = this.GetComponent<Text>();
            //value = value * 25 + 85.0f;
            //int v = (int)value;
            //text.text = v.ToString();
            //Debug.Log(value);
        }
    }
}
