
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

uniform uint pointLightCount;
uniform StructuredBuffer<PointLightParameters> pointLightDataBuffer;
#if UNITY_VERSION >= 201730
uniform Texture2DArray<half3> pointShadowMapsArray;
#else
uniform Texture2DArray<half2> pointShadowMapsArray;
#endif
uniform Texture2DArray<half> pointCookieMapsArray;

half SamplePointShadowMap(PointLightParameters lightParameters, half3 samplingDirection, half2 polarCoordinates)
{
	#if UNITY_VERSION >= 201730
	half3 shadowMapValue = pointShadowMapsArray.SampleLevel(_LinearClamp, half3(polarCoordinates, lightParameters.shadowMapIndex), 0).xyz;
	half4 lightProjectionParams = half4( lightParameters.lightProjectionParameters, shadowMapValue.yz); // From UnityShaderVariables.cginc:114        
	float3 absVec = abs(samplingDirection);
	// From UnityShadowLibrary.cginc:119
    float dominantAxis = max(max(absVec.x, absVec.y), absVec.z);
		dominantAxis = max(0.00001, dominantAxis - lightProjectionParams.z);
		dominantAxis *= lightProjectionParams.w;
    half biasedReferenceDistance = -lightProjectionParams.x + lightProjectionParams.y/dominantAxis;
		biasedReferenceDistance = 1.0f - biasedReferenceDistance;
	return step(shadowMapValue.x, biasedReferenceDistance);
	#else
	half2 shadowMapValue = pointShadowMapsArray.SampleLevel(_LinearClamp, half3(polarCoordinates, lightParameters.shadowMapIndex), 0).xy;
    half biasedReferenceDistance = length(samplingDirection) * shadowMapValue.y;
        biasedReferenceDistance *= 0.97f; // bias
	return step(biasedReferenceDistance, shadowMapValue.x);
	#endif
}

void ComputePointLightInjection(PointLightParameters lightParameters, half3 worldPosition, half3 viewVector, inout half4 accumulationColor, half scattering)
{
	half3 lightVector = worldPosition - lightParameters.lightPosition;
    half3 normalizedLightVector = normalize(lightVector);
	half dist = distance(lightParameters.lightPosition, worldPosition);

	[branch]
	if (dist > lightParameters.lightRange)
	{
		return; 
	}
	else
	{
		half normalizedDistance = saturate(dist / lightParameters.lightRange);
        half scatteringCosAngle = dot(-normalizedLightVector, viewVector);
        half scatteringFactor = GetScatteringFactor(scatteringCosAngle, saturate(scattering + lightParameters.scatteringBias));
        half attenuation = scatteringFactor;
	
		attenuation *= GetLightDistanceAttenuation(lightParameters.distanceFalloffParameters, normalizedDistance);
		
        half2 polarCoordinates = GetNormalizedYawPitchFromNormalizedVector(normalizedLightVector);
		
		[branch]
        if (usePointLightsShadows && lightParameters.shadowMapIndex > -1)
		{
			half shadowAttenuation = SamplePointShadowMap(lightParameters, lightVector, polarCoordinates);
			shadowAttenuation = lerp(lightParameters.shadowStrength, 1.0f, shadowAttenuation);
		
			attenuation *= shadowAttenuation;
		}

		[branch]
        if (useLightsCookies && lightParameters.cookieMapIndex > -1)
		{        
			half cookieMapValue = pointCookieMapsArray.SampleLevel(_LinearClamp, half3(polarCoordinates, lightParameters.cookieMapIndex), 0).x;
			cookieMapValue = lerp(1, cookieMapValue, pow(smoothstep(lightParameters.cookieParameters.x, lightParameters.cookieParameters.y, normalizedDistance), lightParameters.cookieParameters.z));
        
			attenuation *= cookieMapValue;
		}

		accumulationColor.xyz += lightParameters.color * attenuation;
	}
}

void ComputePointLightsInjection(half3 worldPosition, half3 viewVector, inout half4 accumulationColor, half scattering)
{
	[allow_uav_condition]
	for (uint i = 0; i < pointLightCount; ++i)
	{
        ComputePointLightInjection(pointLightDataBuffer[i], worldPosition, viewVector, accumulationColor, scattering);
    }
}