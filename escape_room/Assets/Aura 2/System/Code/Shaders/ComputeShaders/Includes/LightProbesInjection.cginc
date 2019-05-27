
/***************************************************************************
*                                                                          *
*  Copyright (c) Rapha�l Ernaelsten (@RaphErnaelsten)                      *
*  All Rights Reserved.                                                    *
*                                                                          *
*  NOTICE: Aura 2 is a commercial project.                                 * 
*  All information contained herein is, and remains the property of        *
*  Rapha�l Ernaelsten.                                                     *
*  The intellectual and technical concepts contained herein are            *
*  proprietary to Rapha�l Ernaelsten and are protected by copyright laws.  *
*  Dissemination of this information or reproduction of this material      *
*  is strictly forbidden.                                                  *
*                                                                          *
***************************************************************************/

uniform Texture3D<half4> lightProbesCoefficientsTexture;
uniform half4 lightProbesCoefficientsTextureHalfTexelSize;

// viewDirection should be normalized, w=1.0
half3 ComputeLightProbesInjection(half4 viewDirection, half3 normalizedPos, half scattering)
{
    normalizedPos.x = lerp(lightProbesCoefficientsTextureHalfTexelSize.x, 1.0f / 3.0f - lightProbesCoefficientsTextureHalfTexelSize.x, normalizedPos.x);
    normalizedPos.y = lerp(lightProbesCoefficientsTextureHalfTexelSize.y, 1.0f - lightProbesCoefficientsTextureHalfTexelSize.y, normalizedPos.y);
    normalizedPos.z = lerp(0, 1.0f - lightProbesCoefficientsTextureHalfTexelSize.z, GetBiasedNormalizedDepth(normalizedPos.z, Aura_DepthBiasReciproqualCoefficient));

    half4 redColorCoefficients = lightProbesCoefficientsTexture.SampleLevel(_LinearClamp, saturate(normalizedPos), 0);
    normalizedPos.x += 1.0f / 3.0f;
    half4 greenColorCoefficients = lightProbesCoefficientsTexture.SampleLevel(_LinearClamp, saturate(normalizedPos), 0);
    normalizedPos.x += 1.0f / 3.0f;
    half4 blueColorCoefficients = lightProbesCoefficientsTexture.SampleLevel(_LinearClamp, saturate(normalizedPos), 0);
    
    viewDirection.xyz *= scattering; // Henyey-Greenstein phase function (https://bartwronski.files.wordpress.com/2014/08/bwronski_volumetric_fog_siggraph2014.pdf#page=55)
    return SHEvalLinearL0L1(viewDirection, redColorCoefficients, greenColorCoefficients, blueColorCoefficients);
}

