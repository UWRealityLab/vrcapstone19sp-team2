
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

#include "Common.cginc"

float4 Aura_FrustumRanges;
sampler3D Aura_VolumetricDataTexture;
sampler3D Aura_VolumetricLightingTexture;

//////////// Helper functions
float Aura2_RescaleDepth(float depth)
{
    half rescaledDepth = InverseLerp(Aura_FrustumRanges.x, Aura_FrustumRanges.y, depth);
    return GetBiasedNormalizedDepth(rescaledDepth, Aura_DepthBiasReciproqualCoefficient);
}

float3 Aura2_GetFrustumSpaceCoordinates(float4 inVertex)
{
    float4 clipPos = UnityObjectToClipPos(inVertex);	

    float z = -UnityObjectToViewPos(inVertex).z;

    float4 cameraPos = ComputeScreenPos(clipPos);
    cameraPos.xy /= cameraPos.w;
    cameraPos.z = z;

    return cameraPos.xyz;
}

//////////// Lighting
float4 Aura2_SampleDataTexture(float3 position)
{
    return SampleTexture3D(Aura_VolumetricDataTexture, position, Aura_BufferTexelSize);
}
float4 Aura2_GetData(float3 screenSpacePosition)
{
    return Aura2_SampleDataTexture(float3(screenSpacePosition.xy, Aura2_RescaleDepth(screenSpacePosition.z)));
}

void Aura2_ApplyLighting(inout float3 colorToApply, float3 screenSpacePosition, float lightingFactor, float3 lightingValue)
{
	colorToApply *= lightingValue * lightingFactor;
}
void Aura2_ApplyLighting(inout float3 colorToApply, float3 screenSpacePosition, float lightingFactor)
{
	#if defined(AURA_USE_DITHERING)
    screenSpacePosition.xy += GetBlueNoise(screenSpacePosition.xy, 0).xy;
	#endif

    float3 lightingValue = Aura2_GetData(screenSpacePosition).xyz;
	Aura2_ApplyLighting(colorToApply, screenSpacePosition, lightingFactor, lightingValue);
}

//////////// Fog
float4 Aura2_GetFogValue(float3 screenSpacePosition)
{
    return SampleTexture3D(Aura_VolumetricLightingTexture, float3(screenSpacePosition.xy, Aura2_RescaleDepth(screenSpacePosition.z)), Aura_BufferTexelSize);
}

void Aura2_ApplyFog(inout float3 colorToApply, float3 screenSpacePosition, float4 fogValue)
{
	#if defined(AURA_USE_DITHERING)
    screenSpacePosition.xyz += GetBlueNoise(screenSpacePosition.xy, 1).xyz;
	#endif

	colorToApply = (colorToApply * fogValue.w) + fogValue.xyz;
}
void Aura2_ApplyFog(inout float3 colorToApply, float3 screenSpacePosition)
{
    float4 fogValue = Aura2_GetFogValue(screenSpacePosition);
    Aura2_ApplyFog(colorToApply, screenSpacePosition, fogValue);
}

// From https://github.com/Unity-Technologies/VolumetricLighting/blob/master/Assets/Scenes/Materials/StandardAlphaBlended-VolumetricFog.shader
void Aura2_ApplyFog(inout float4 colorToApply, float3 screenSpacePosition, float4 fogValue)
{
	#if defined(AURA_USE_DITHERING)
    screenSpacePosition.xy += GetBlueNoise(screenSpacePosition.xy, 2).xy;
	#endif

	// Always apply fog attenuation - also in the forward add pass.
    colorToApply.xyz *= fogValue.w;

	// Alpha premultiply mode (used with alpha and Standard lighting function, or explicitly alpha:premul)
	#if _ALPHAPREMULTIPLY_ON
	fogValue.xyz *= colorToApply.w;
	#endif

	// Add inscattering only once, so in forward base, but not forward add.
	#ifndef UNITY_PASS_FORWARDADD
    colorToApply.xyz += fogValue.xyz;
	#endif
}
void Aura2_ApplyFog(inout float4 colorToApply, float3 screenSpacePosition)
{    
    float4 fogValue = Aura2_GetFogValue(screenSpacePosition);
    Aura2_ApplyFog(colorToApply, screenSpacePosition, fogValue);
} 