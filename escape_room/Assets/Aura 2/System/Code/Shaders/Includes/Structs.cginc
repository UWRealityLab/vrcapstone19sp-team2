
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

///
///			MatrixFloats
///
struct MatrixFloats
{
	half4 a;
	half4 b;
	half4 c;
	half4 d;
};

///
///			VolumeLevelsData
///
struct LevelsData
{
	half levelLowThreshold;
	half levelHiThreshold;
	half outputLowValue;
	half outputHiValue;
	half contrast;
};

///
///			TextureMaskData
///
struct TextureMaskData
{
	MatrixFloats transform;
	int index;
};

///
///			VolumetricNoiseData
///
struct VolumetricNoiseData
{
	int enable;
	MatrixFloats transform;
	half speed;
};

///
///			VolumeData
///
struct VolumeData
{
	MatrixFloats transform;
	int shape;
	/*
		Global      = 0
		Layer	    = 1
		Box         = 2
		Sphere      = 3
		Cylinder    = 4
		Cone        = 5
	*/
	half falloffExponent;
	half xPositiveFade;
	half xNegativeFade;
	half yPositiveFade;
	half yNegativeFade;
	half zPositiveFade;
	half zNegativeFade;
    int useAsLightProbesProxyVolume;
    half lightProbesMultiplier;
	TextureMaskData texture2DMaskData;
    TextureMaskData texture3DMaskData;
	VolumetricNoiseData noiseData;
	int injectDensity;
	half densityValue;
    LevelsData densityTexture2DMaskLevelsParameters;
    LevelsData densityTexture3DMaskLevelsParameters;
    LevelsData densityNoiseLevelsParameters;
    int injectScattering;
    half scatteringValue;
    LevelsData scatteringTexture2DMaskLevelsParameters;
    LevelsData scatteringTexture3DMaskLevelsParameters;
    LevelsData scatteringNoiseLevelsParameters;
    int injectColor;
    half3 colorValue;
    LevelsData colorTexture2DMaskLevelsParameters;
    LevelsData colorTexture3DMaskLevelsParameters;
    LevelsData colorNoiseLevelsParameters;
    int injectAmbient;
    half ambientLightingValue;
    LevelsData ambientTexture2DMaskLevelsParameters;
    LevelsData ambientTexture3DMaskLevelsParameters;
    LevelsData ambientNoiseLevelsParameters;
};

///
///			DirectionalShadowData
///
struct DirectionalShadowData
{
	half4 shadowSplitSqRadii;
	half4 lightSplitsNear;
	half4 lightSplitsFar;
	half4 shadowSplitSpheres[4];
	half4x4 world2Shadow[4];
	half4 lightShadowData;
};

///
///			DirectionalLightParameters
///
struct DirectionalLightParameters
{
    half3 color;
    half scatteringBias;
	half3 lightPosition;
	half3 lightDirection;
	MatrixFloats worldToLightMatrix;
	MatrixFloats lightToWorldMatrix;
	int shadowmapIndex;
	int cookieMapIndex;
	half2 cookieParameters;
	int enableOutOfPhaseColor;
    half3 outOfPhaseColor;
};

///
///			SpotLightParameters
///
struct SpotLightParameters
{
    half3 color;
    half scatteringBias;
	half3 lightPosition;
	half3 lightDirection;
	half lightRange;
	half lightCosHalfAngle;
	half2 angularFalloffParameters;
	half2 distanceFalloffParameters;
	MatrixFloats worldToShadowMatrix;
	int shadowMapIndex;
	half shadowStrength;
	int cookieMapIndex;
	half3 cookieParameters;
};

///
///			PointLightParameters
///
struct PointLightParameters
{
    half3 color;
    half scatteringBias;
    half3 lightPosition;
    half lightRange;
    half2 distanceFalloffParameters;
    MatrixFloats worldToShadowMatrix;
	#if UNITY_VERSION >= 201730
    half2 lightProjectionParameters;
	#endif
    int shadowMapIndex;
    half shadowStrength;
	int cookieMapIndex;
	half3 cookieParameters;
};

/// 
///			SphericalHarmonicsFirstBandCoefficients
///
struct SphericalHarmonicsFirstBandCoefficients
{
    half4 redColorCoefficients;
    half4 greenColorCoefficients;
    half4 blueColorCoefficients;
};