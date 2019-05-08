using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;


public static class FloatConversion
{
    public static string circularDriveValueToString(
        float value, int digits, float scale, float offset)
    {
        string starting = "0";
        string extras = "";
        if (digits != 0)
            extras += ".";
        for (int i = 0; i < digits; i++)
        {
            extras += "0";
        }
        string str = value.ToString(starting + extras);
        float flt = float.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
        flt = flt * scale + offset;
        string res = flt.ToString(starting + extras);
        res = res.Contains(".") ? res : res + extras;
        return res;
    }
}
