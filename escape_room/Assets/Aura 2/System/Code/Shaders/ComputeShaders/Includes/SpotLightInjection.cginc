
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

uniform uint spotLightCount;
uniform StructuredBuffer<SpotLightParameters> spotLightDataBuffer;
uniform Texture2DArray<half> spotShadowMapsArray;
uniform Texture2DArray<half> spotCookieMapsArray;

half SampleSpotShadowMap(SpotLightParameters lightParameters, half4 shadowPosition, half2 offset)
{
	// TODO : CHECK FOR OFFSET (BECAUSE OF SHADOW BIAS)
	half shadowMapValue = 1.0f - spotShadowMapsArray.SampleLevel(_LinearClamp, half3((shadowPosition.xy + offset) / shadowPosition.w, lightParameters.shadowMapIndex), 0);
	return step(shadowPosition.z / shadowPosition.w, shadowMapValue);
}

void ComputeSpotLightInjection(SpotLightParameters lightParameters, half3 worldPosition, half3 viewVector, inout half4 accumulationColor, half scattering)
{
	half3 lightVector = normalize(worldPosition - lightParameters.lightPosition);
	half cosAngle = dot(lightParameters.lightDirection.xyz, lightVector);
	half dist = distance(lightParameters.lightPosition.xyz, worldPosition);

    [branch]
	if (dist > lightParameters.lightRange || cosAngle < lightParameters.lightCosHalfAngle)
	{
		return;
	}
	else
	{
        half scatteringCosAngle = dot(-lightVector, viewVector);
        half scatteringFactor = GetScatteringFactor(scatteringCosAngle, saturate(scattering + lightParameters.scatteringBias));
        half attenuation = scatteringFactor;
        
		half4 lightPos = mul(ConvertMatrixFloatsToMatrix(lightParameters.worldToShadowMatrix), half4(worldPosition, 1));
		half normalizedDistance = saturate(lightPos.z / lightParameters.lightRange);
        
		attenuation *= GetLightDistanceAttenuation(lightParameters.distanceFalloffParameters, normalizedDistance);
        
		half angleAttenuation = 1;
		angleAttenuation = smoothstep(lightParameters.lightCosHalfAngle, lerp(1, lightParameters.lightCosHalfAngle, lightParameters.angularFalloffParameters.x), cosAngle);
		angleAttenuation = pow(angleAttenuation, lightParameters.angularFalloffParameters.y);
		attenuation *= angleAttenuation;
        
        [branch]
        if (useSpotLightsShadows && lightParameters.shadowMapIndex > -1)
		{
			half shadowAttenuation = SampleSpotShadowMap(lightParameters, lightPos, 0);
			shadowAttenuation = lerp(lightParameters.shadowStrength, 1.0f, shadowAttenuation);
			
			attenuation *= shadowAttenuation;
		}
        
		[branch]
        if (useLightsCookies &&  lightParameters.cookieMapIndex > -1)
		{        
			half cookieMapValue = spotCookieMapsArray.SampleLevel(_LinearRepeat, half3(lightPos.xy / lightPos.w, lightParameters.cookieMapIndex), 0).x;        
            cookieMapValue = lerp(1, cookieMapValue, pow(smoothstep(lightParameters.cookieParameters.x, lightParameters.cookieParameters.y, normalizedDistance), lightParameters.cookieParameters.z));
        
			attenuation *= cookieMapValue;
		}
        
		accumulationColor.xyz += lightParameters.color * attenuation;
	}
}

void ComputeSpotLightsInjection(half3 worldPosition, half3 viewVector, inout half4 accumulationColor, half scattering)
{
    [allow_uav_condition]
	for (uint i = 0; i < spotLightCount; ++i)
	{
        ComputeSpotLightInjection(spotLightDataBuffer[i], worldPosition, viewVector, accumulationColor, scattering);
    }
}