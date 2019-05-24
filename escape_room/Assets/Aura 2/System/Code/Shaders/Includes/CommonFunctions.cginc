
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

///-----------------------------------------------------------------------------------------
///			Noise functions
///-----------------------------------------------------------------------------------------
#include "SphericalHarmonics.cginc"

///-----------------------------------------------------------------------------------------
///			Noise functions
///-----------------------------------------------------------------------------------------
#include "TextureSampling.cginc"

///-----------------------------------------------------------------------------------------
///			Noise functions
///-----------------------------------------------------------------------------------------
#include "Noise.cginc"

///-----------------------------------------------------------------------------------------
///			GetBiasedDepth
///			Bias the depth towards the camera
///-----------------------------------------------------------------------------------------
float GetBiasedNormalizedDepth(float normalizedDepth, float biasCoefficient)
{
	// https://www.desmos.com/calculator/kwnyuioj2z
	float inverseDepth = max(0.000001f, 1.0f - normalizedDepth);
    return 1.0f - pow(inverseDepth, biasCoefficient);
}

///-----------------------------------------------------------------------------------------
///			ApplyDepthBiasToNormalizedPosition
///			Apply depth bias to normalized position
///-----------------------------------------------------------------------------------------
void ApplyDepthBiasToNormalizedPosition(inout float4 normalizedPosition)
{
    normalizedPosition.w = normalizedPosition.z;
    normalizedPosition.z = GetBiasedNormalizedDepth(normalizedPosition.w, Aura_DepthBiasCoefficient);
}

///-----------------------------------------------------------------------------------------
///			GetNormalizedLocalPosition
///			Gets the normalized coordinates from the thread id and the Aura_BufferTexelSize
///-----------------------------------------------------------------------------------------
float3 GetNormalizedLocalPosition(uint3 id)
{
    return ((float3) id + 0.5f) * Aura_BufferTexelSize.xyz;
}

///-----------------------------------------------------------------------------------------
///			GetNormalizedLocalPositionWithDepthBias
///			Gets the volume normalized coordinates with a depth biased towards the camera
///-----------------------------------------------------------------------------------------
float4 GetNormalizedLocalPositionWithDepthBias(uint3 id)
{
    float3 normalizedLocalPos = GetNormalizedLocalPosition(id);
    float biasedDepth = GetBiasedNormalizedDepth(normalizedLocalPos.z, Aura_DepthBiasCoefficient);
    return float4(normalizedLocalPos.xy, biasedDepth, normalizedLocalPos.z); 
}

///-----------------------------------------------------------------------------------------
///			GetNormalizedLocalDepth
///			Gets the normalized depth from the thread id and the Aura_BufferTexelSize
///-----------------------------------------------------------------------------------------
float GetNormalizedLocalDepth(uint idZ)
{
    return ((float) idZ + 0.5f) * Aura_BufferTexelSize.z;
}

///-----------------------------------------------------------------------------------------
///			GetNormalizedLocalPositionWithDepthBias
///			Gets the volume normalized coordinates with a depth biased towards the camera
///-----------------------------------------------------------------------------------------
float GetNormalizedLocalDepthWithDepthBias(uint idZ)
{
    float normalizedLocalDepth = GetNormalizedLocalDepth(idZ);
    float biasedDepth = GetBiasedNormalizedDepth(normalizedLocalDepth, Aura_DepthBiasCoefficient);
    return biasedDepth; 
}

///-----------------------------------------------------------------------------------------
///			GetCameraSpaceDepth
///			Gets the camera space depth from the normalized volume depth
///-----------------------------------------------------------------------------------------
float GetCameraSpaceDepth(half normalizedDepth)
{
    return lerp(cameraRanges.x, cameraRanges.y, normalizedDepth);
}

///-----------------------------------------------------------------------------------------
///			GetWorldPosition
///			Gets the world position from normalized coordinates and the corners' position of the frustum
///-----------------------------------------------------------------------------------------
float3 GetWorldPosition(float3 normalizedLocalPos, half4 cornersPosition[8])
{
	float3 AtoB = lerp(cornersPosition[0].xyz, cornersPosition[1].xyz, normalizedLocalPos.x);
	float3 DtoC = lerp(cornersPosition[3].xyz, cornersPosition[2].xyz, normalizedLocalPos.x);
	float3 nearBottomToTop = lerp(DtoC, AtoB, normalizedLocalPos.y);

	float3 EtoF = lerp(cornersPosition[4].xyz, cornersPosition[5].xyz, normalizedLocalPos.x);
	float3 HtoG = lerp(cornersPosition[7].xyz, cornersPosition[6].xyz, normalizedLocalPos.x);
	float3 farBottomToTop = lerp(HtoG, EtoF, normalizedLocalPos.y);

	float3 worldPosition = lerp(nearBottomToTop, farBottomToTop, normalizedLocalPos.z);

	return worldPosition;
}

///-----------------------------------------------------------------------------------------
///			TransformPositions
///			Gets the 3d texture coordinates to be used with combined Texture 3D
///-----------------------------------------------------------------------------------------
float3 TransformPoint(float3 p, float4x4 transform)
{
	return mul(transform, float4(p, 1)).xyz;
}

///-----------------------------------------------------------------------------------------
///			GetNormalizedYawPitchFromNormalizedVector
///			Compute normalized Yaw Pitch angles from a normalized direction vector
///-----------------------------------------------------------------------------------------
float2 GetNormalizedYawPitchFromNormalizedVector(float3 NormalizedVector)
{
	const float InvPi = 0.31830988618379067153776752674503f;
	const float TwoInvPi = 2.0f * InvPi;
	float Yaw = (atan2(NormalizedVector.z, NormalizedVector.x) * InvPi + 1.0f) * 0.5f;
	float Pitch = (asin(NormalizedVector.y) * TwoInvPi + 1.0f) * 0.5f;

	return float2(Yaw, Pitch);
}
///-----------------------------------------------------------------------------------------
///			GetNormalizedVectorFromNormalizedYawPitch
///			Compute a normalized direction vector from normalized Yaw Pitch angles
///-----------------------------------------------------------------------------------------
float3 GetNormalizedVectorFromNormalizedYawPitch(float Yaw, float Pitch)
{
	const float Pi = 3.1415926535897932384626433832795f;
	const float HalfPi = Pi * 0.5f;
	Yaw = (Yaw * 2.0f - 1.0f) * Pi;
	Pitch = (Pitch * 2.0f - 1.0f) * HalfPi;
	return float3(cos(Yaw) * cos(Pitch), sin(Pitch), cos(Pitch) * sin(Yaw));
}
float3 GetNormalizedVectorFromNormalizedYawPitch(float2 YawPitch)
{
	return GetNormalizedVectorFromNormalizedYawPitch(YawPitch.x, YawPitch.y);
}

///-----------------------------------------------------------------------------------------
///			GetCombinedTexture3dCoordinates
///			Gets the 3d texture coordinates to be used with combined Texture 3D
///-----------------------------------------------------------------------------------------
float3 GetCombinedTexture3dCoordinates(float3 positions, float textureWidth, float textureDepth, float index, float4x4 transform)
{
	float textureCount = textureDepth / textureWidth;
	float borderClamp = 0.5f / textureWidth;
	float offset = index / textureCount;

    float3 textureCoordinates = frac(TransformPoint(positions, transform) + +float3(0.5f, 0.5f, 0.5f));
	textureCoordinates.z /= textureCount;
	textureCoordinates.z += offset;
	textureCoordinates.z = clamp(offset + borderClamp, offset + 1.0f - borderClamp, textureCoordinates.z);

	return textureCoordinates;
}

///-----------------------------------------------------------------------------------------
///			GetExponentialValue
///			Gets "exponentialized" value based on 0->1 gradient
///-----------------------------------------------------------------------------------------
float GetExponentialValue(float value)
{
	return pow(abs(value), e);
}
///-----------------------------------------------------------------------------------------
///			GetLogarithmicValue
///			Gets "logarithmized" value based on 0->1 gradient
///-----------------------------------------------------------------------------------------
float GetLogarithmicValue(float value)
{
	return pow(abs(value), n);
}

///-----------------------------------------------------------------------------------------
///			(Clamped)InverseLerp
///			Gets the linear gradient, returning where the value locates between the low and hi thresholds
///-----------------------------------------------------------------------------------------
float InverseLerp(float lowThreshold, float hiThreshold, float value)
{
	return (value - lowThreshold) / (hiThreshold - lowThreshold);
}
float ClampedInverseLerp(float lowThreshold, float hiThreshold, float value)
{
	return saturate(InverseLerp(lowThreshold, hiThreshold, value));
}
float3 InverseLerp(float lowThreshold, float hiThreshold, float3 value)
{
	return (value - lowThreshold) / (hiThreshold - lowThreshold);
}
float3 ClampedInverseLerp(float lowThreshold, float hiThreshold, float3 value)
{
	return saturate(InverseLerp(lowThreshold, hiThreshold, value));
}

///-----------------------------------------------------------------------------------------
///			LevelValue
///			Filters value between "levelLowThreshold" and "levelHiThreshold", contrast by "contrast" factor, then rescale the result between "outputLowValue" and "outputHiValue". Similar to the Levels adjustment tool in Photoshop.
///-----------------------------------------------------------------------------------------
float LevelValue(LevelsData levelsParameters, float value)
{
	float tmp = ClampedInverseLerp(levelsParameters.levelLowThreshold, levelsParameters.levelHiThreshold, value);
	tmp = saturate(lerp(0.5f, tmp, levelsParameters.contrast));
	tmp = lerp(levelsParameters.outputLowValue, levelsParameters.outputHiValue, tmp);

	return tmp;
}
float3 LevelValue(LevelsData levelsParameters, float3 value)
{
	float3 tmp;
	tmp.x = LevelValue(levelsParameters, value.x);
	tmp.y = LevelValue(levelsParameters, value.y);
	tmp.z = LevelValue(levelsParameters, value.z);

	return tmp;
}

//-----------------------------------------------------------------------------------------
//			GetScatteringFactor
//			Lights phase function. Returns the anisotropic scattering factor.
//			http://renderwonk.com/publications/s2003-course/premoze1/notes-premoze.pdf
//-----------------------------------------------------------------------------------------
float HenyeyGreensteinPhaseFunction(float cosAngle, float coefficient, float squareCoefficient)
{	
	
	float topPart = 1.0f - squareCoefficient;
	float bottomPart = sqrt(1.0f + squareCoefficient - 2.0f * coefficient * cosAngle);
	bottomPart *= bottomPart * bottomPart;
	//float bottomPart = 1.0f + squareCoefficient - 2.0f * coefficient * cosAngle; // More controllable
	//float bottomPart = pow(1.0f + squareCoefficient - 2.0f * coefficient * cosAngle, 0.75f); // More controllable
    bottomPart = rcp(bottomPart);
    return topPart * bottomPart;
}
float CornetteShanksPhaseFunction(float cosAngle, float coefficient, float squareCoefficient)
{
	return (3.0f / 2.0f) * ((1.0f + cosAngle * cosAngle) / (2.0f + squareCoefficient)) * HenyeyGreensteinPhaseFunction(cosAngle, coefficient, squareCoefficient);
}
float GetScatteringFactor(float cosAngle, float coefficient)
{
	float squareCoefficient = coefficient * coefficient;
	return quarterPi * CornetteShanksPhaseFunction(cosAngle, coefficient, squareCoefficient);
}

//-----------------------------------------------------------------------------------------
//			GetLinearDepth
//			Linearize depth/shadow maps
//			
//			Params : Values used to linearize the Z buffer (http://www.humus.name/temp/Linearize%20depth.txt)
//          x = 1-far/near
//          y = far/near
//          z = x/far
//          w = y/far
//          or in case of a reversed depth buffer (UNITY_REVERSED_Z is 1) -> Our case
//          x = -1+far/near
//          y = 1
//          z = x/far
//          w = 1/far
//-----------------------------------------------------------------------------------------
float GetLinearDepth(float depth, float4 params)
{
	return 1.0f / (params.z * depth + params.w);
}
float GetLinearDepth01(float depth, float4 params)
{
	return 1.0f / (params.z * depth + params.y);
}

//-----------------------------------------------------------------------------------------
//			GetLightDistanceAttenuation
//			Computes the distance attenuation factor for Point and Spot lights
//-----------------------------------------------------------------------------------------
half GetLightDistanceAttenuation(half2 distanceFalloffParameters, half normalizedDistance)
{
    float distanceAttenuation = ClampedInverseLerp(1.0f, distanceFalloffParameters.x, normalizedDistance);
    distanceAttenuation = pow(distanceAttenuation, distanceFalloffParameters.y);

    return distanceAttenuation;
}

//-----------------------------------------------------------------------------------------
//			ConvertMatrixFloatsToMatrix
//			Coonverts a MatrixFloats struct into a float4x4 matrix
//-----------------------------------------------------------------------------------------
half4x4 ConvertMatrixFloatsToMatrix(MatrixFloats data)
{
    return half4x4(half4(data.a.x, data.b.x, data.c.x, data.d.x), half4(data.a.y, data.b.y, data.c.y, data.d.y), half4(data.a.z, data.b.z, data.c.z, data.d.z), half4(data.a.w, data.b.w, data.c.w, data.d.w));
}