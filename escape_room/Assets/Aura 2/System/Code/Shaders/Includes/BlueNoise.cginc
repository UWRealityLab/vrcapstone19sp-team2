
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

static const float blueNoiseTexturesSize = 64.0f;
Texture2DArray _blueNoiseTexturesArray;
int _frameID;

#ifndef UNITY_SHADER_VARIABLES_INCLUDED
float4 _ScreenParams;
#endif

float4 GetBlueNoise(float2 screenPos, int idOffset)
{
    int4 blueNoiseSamplingPosition = int4(screenPos * _ScreenParams.xy % float2(blueNoiseTexturesSize, blueNoiseTexturesSize), (_frameID + idOffset) % blueNoiseTexturesSize, 0);
	float4 blueNoise = _blueNoiseTexturesArray.Load(blueNoiseSamplingPosition);
	blueNoise = mad(blueNoise, 2.0f, -1.0f);
	blueNoise = sign(blueNoise)*(1.0f - sqrt(1.0f - abs(blueNoise)));
	blueNoise /= 255.0f;

    return blueNoise;
}