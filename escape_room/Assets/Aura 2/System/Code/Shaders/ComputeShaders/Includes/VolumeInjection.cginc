
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

uniform StructuredBuffer<VolumeData> volumeDataBuffer;
uniform uint volumeCount;
uniform Texture2DArray<half4> texture2DMaskAtlasTexture;
uniform Texture3D<half4> texture3DMaskAtlasTexture;
uniform float3 texture3DMaskAtlasTextureSize;

half GetShapeGradient(VolumeData volumeData, inout half3 position)
{
    half gradient = 1;

    [branch]
	if (volumeData.shape == 1)
	{
		position = TransformPoint(position, ConvertMatrixFloatsToMatrix(volumeData.transform));
		gradient = 1.0f - saturate(position.y);
	}
	else if (volumeData.shape == 2)
	{
		position = TransformPoint(position, ConvertMatrixFloatsToMatrix(volumeData.transform));
		half x = ClampedInverseLerp(-0.5f, -0.5f + volumeData.xNegativeFade, position.x) - ClampedInverseLerp(0.5f - volumeData.xPositiveFade, 0.5f, position.x);
		half y = ClampedInverseLerp(-0.5f, -0.5f + volumeData.yNegativeFade, position.y) - ClampedInverseLerp(0.5f - volumeData.yPositiveFade, 0.5f, position.y);
		half z = ClampedInverseLerp(-0.5f, -0.5f + volumeData.zNegativeFade, position.z) - ClampedInverseLerp(0.5f - volumeData.zPositiveFade, 0.5f, position.z);
		gradient = x * y * z;
	}
	else if (volumeData.shape == 3)
	{
		position = TransformPoint(position, ConvertMatrixFloatsToMatrix(volumeData.transform));
		gradient = ClampedInverseLerp(0.5f, 0.5f - volumeData.xPositiveFade * 0.5f, length(position));
	}
	else if (volumeData.shape == 4)
	{
		position = TransformPoint(position, ConvertMatrixFloatsToMatrix(volumeData.transform));
		half y = ClampedInverseLerp(-0.5f, -0.5f + volumeData.yNegativeFade, position.y) - ClampedInverseLerp(0.5f - volumeData.yPositiveFade, 0.5f, position.y);
		half xz = ClampedInverseLerp(0.5f, 0.5f - volumeData.xPositiveFade * 0.5f, length(position.xz));
		gradient = xz * y;
	}
	else if (volumeData.shape == 5)
	{
		position = TransformPoint(position, ConvertMatrixFloatsToMatrix(volumeData.transform));
		half z = ClampedInverseLerp(1, 1.0f - volumeData.zPositiveFade * 2, position.z);
		half xy = ClampedInverseLerp(0.5f, 0.5f - volumeData.xPositiveFade * 0.5f, length(position.xy / saturate(position.z)));
		gradient = xy * z;
	}

	return gradient;
}

void ComputeVolumeContribution(VolumeData volumeData, half3 jitteredWorldPosition, half3 unjitteredWorldPosition, inout half density, inout half scattering, inout half3 color, inout half globalIlluminationMask, inout half lightProbesMultiplier, inout half ambientLightingMultiplier)
{
    half gradient = GetShapeGradient(volumeData, jitteredWorldPosition);

    [branch]
	if (gradient > 0)
	{	
        half densityMask = 1.0f;
        half scatteringMask = 1.0f;
        half3 colorMask = half3(1.0f, 1.0f, 1.0f);
        half ambientMask = 1.0f;
		
        [branch]
        if (useTexture2DMasks && volumeData.texture2DMaskData.index > -1)
        {
            half3 samplingPosition = TransformPoint(jitteredWorldPosition, ConvertMatrixFloatsToMatrix(volumeData.texture2DMaskData.transform));
            half4 textureMask = texture2DMaskAtlasTexture.SampleLevel(_LinearRepeat, half3(samplingPosition.xz + float2(0.5f, 0.5f), volumeData.texture2DMaskData.index), 0); // Texture is by default projected downwards
        
            densityMask *= LevelValue(volumeData.densityTexture2DMaskLevelsParameters, textureMask.w);
            scatteringMask *= LevelValue(volumeData.scatteringTexture2DMaskLevelsParameters, textureMask.w);
            colorMask *= LevelValue(volumeData.colorTexture2DMaskLevelsParameters, textureMask.xyz);
            ambientMask *= LevelValue(volumeData.ambientTexture2DMaskLevelsParameters, textureMask.w);
        }
		
        [branch]
        if (useTexture3DMasks && volumeData.texture3DMaskData.index > -1)
        {
            half3 samplingPosition = GetCombinedTexture3dCoordinates(jitteredWorldPosition, texture3DMaskAtlasTextureSize.x, texture3DMaskAtlasTextureSize.z, (half) volumeData.texture3DMaskData.index, ConvertMatrixFloatsToMatrix(volumeData.texture3DMaskData.transform));
            half4 textureMask = texture3DMaskAtlasTexture.SampleLevel(_LinearClamp, samplingPosition, 0);
        
            densityMask *= LevelValue(volumeData.densityTexture3DMaskLevelsParameters, textureMask.w);
            scatteringMask *= LevelValue(volumeData.scatteringTexture3DMaskLevelsParameters, textureMask.w);
            colorMask *= LevelValue(volumeData.colorTexture3DMaskLevelsParameters, textureMask.xyz);
            ambientMask *= LevelValue(volumeData.ambientTexture3DMaskLevelsParameters, textureMask.w);
        }
        
        [branch]
        if (useVolumesNoise && volumeData.noiseData.enable)
        {
            half3 noisePosition = TransformPoint(unjitteredWorldPosition, ConvertMatrixFloatsToMatrix(volumeData.noiseData.transform));
            half noiseMask = snoise(half4(noisePosition, time * volumeData.noiseData.speed)) * 0.5f + 0.5f;

			densityMask *= LevelValue(volumeData.densityNoiseLevelsParameters, noiseMask);
            scatteringMask *= LevelValue(volumeData.scatteringNoiseLevelsParameters, noiseMask);
            colorMask *= LevelValue(volumeData.colorNoiseLevelsParameters, noiseMask);
            ambientMask *= LevelValue(volumeData.ambientNoiseLevelsParameters, noiseMask);
        }
        
        gradient = pow(gradient, volumeData.falloffExponent);
    
        [branch]
	    if (volumeData.injectDensity)
	    {
		    density += volumeData.densityValue * gradient * densityMask;
	    }
    
        [branch]
        if (volumeData.injectScattering)
        {
            scattering += -volumeData.scatteringValue * gradient * scatteringMask;
        }
    
        [branch]
	    if (volumeData.injectColor)
	    {
	        color += volumeData.colorValue * gradient * colorMask;
        }
    
        [branch]
        if (volumeData.injectAmbient)
        {
            ambientLightingMultiplier += volumeData.ambientLightingValue * gradient * ambientMask;
        }
		
		[branch]
        if (volumeData.useAsLightProbesProxyVolume)
        {
            globalIlluminationMask = max(globalIlluminationMask, gradient);
            lightProbesMultiplier += volumeData.lightProbesMultiplier * gradient;
        }
	}
}

void ComputeVolumesInjection(half3 jitteredWorldPosition, half3 unjitteredWorldPosition, inout half3 color, inout half density, inout half scattering, inout half globalIlluminationMask, inout half lightProbesMultiplier, inout half ambientLightingMultiplier)
{
    [allow_uav_condition]
	for (uint i = 0; i < volumeCount; ++i)
	{
        ComputeVolumeContribution(volumeDataBuffer[i], jitteredWorldPosition, unjitteredWorldPosition, density, scattering, color, globalIlluminationMask, lightProbesMultiplier, ambientLightingMultiplier);
    }

	density = max(0, density);
    scattering = saturate(scattering);
}