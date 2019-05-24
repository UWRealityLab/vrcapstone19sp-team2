
/***************************************************************************
*                                                                          *
*  Copyright (c) Raphaël Ernaelsten (@RaphErnaelsten)                      *
*  All Rights Reserved.                                                    *
*                                                                          *
*  NOTICE: Aura 2 is a commercial project.                                 * 
*  All information contained herein is, and remains the property of        *
*  Raphaël Ernaelsten.                                                     *
*  The intellectual and technical concepts contained herein are            *
*  proprietary to Raphaël Ernaelsten and are protected by copyright laws.  *
*  Dissemination of this information or reproduction of this material      *
*  is strictly forbidden.                                                  *
*                                                                          *
***************************************************************************/

uniform half3 ambientColorBottom;
uniform half3 ambientColorHorizon;
uniform half3 ambientColorTop;
uniform half4 ambientShAr;
uniform half4 ambientShAg;
uniform half4 ambientShAb;
uniform half4 ambientShBr;
uniform half4 ambientShBg;
uniform half4 ambientShBb;
uniform half4 ambientShC;
uniform uint ambientMode;

half3 ComputeAmbientLighting(half4 viewDirection, half scattering)
{
    half3 ambientLight;

    if (ambientMode == 3) // Flat ambient
    {
        ambientLight = ambientColorHorizon;
    }
    else if (ambientMode == 1) // Gradient ambient
    {
        half gradient = viewDirection.y;
        half3 color = lerp(ambientColorBottom, ambientColorHorizon, saturate(gradient + 1.0f)); // bottom to horizon
        color = lerp(color, ambientColorTop, saturate(gradient));								// horizon to top
		
		half3 meanColor = (ambientColorBottom + ambientColorHorizon + ambientColorTop) * 1.0f / 3.0f;	// does it work? yes
		color = lerp(color, meanColor, scattering);														// is it mathematically correct? probably not
		
        ambientLight = color;
    }
	else // Skybox ambient
    {
        viewDirection.xyz *= scattering; // Henyey-Greenstein phase function (https://bartwronski.files.wordpress.com/2014/08/bwronski_volumetric_fog_siggraph2014.pdf#page=55)
        ambientLight = EvaluateSphericalHarmonics(viewDirection, ambientShAr, ambientShAg, ambientShAb, ambientShBr, ambientShBg, ambientShBb, ambientShC);
    }
    
    return ambientLight;
}

