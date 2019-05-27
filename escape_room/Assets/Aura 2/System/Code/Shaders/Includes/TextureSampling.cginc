
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

// Sampler declaration
#if defined(SHADER_STAGE_COMPUTE)
	#define TEXTURE3D_SAMPLER_DECLARATION Texture3D<half4>
#else
	#define TEXTURE3D_SAMPLER_DECLARATION sampler3D
#endif

// Linear sampling
half4 SampleTexture3D_Linear(TEXTURE3D_SAMPLER_DECLARATION tex, half3 texCoords)
{
    #if defined(SHADER_STAGE_COMPUTE)
        return tex.SampleLevel(_LinearClamp, texCoords, 0);
    #elif defined(SHADER_STAGE_FRAGMENT)
        return tex3D(tex, texCoords);
    #else
        return tex3Dlod(tex, half4(texCoords, 0.0f));
    #endif
}

// Cubic Texture3D Sampling
// Based on : http://www.dannyruijters.nl/cubicinterpolation/
half4 SampleTexture3D_Cubic(TEXTURE3D_SAMPLER_DECLARATION tex, half3 texCoords, half3 texelSize)
{
    texCoords /= texelSize;

	// shift the coordinate from [0,extent] to [-0.5, extent-0.5]
    half3 coord_grid = texCoords - 0.5f;
    half3 index = floor(coord_grid);
    half3 fraction = coord_grid - index;
    half3 one_frac = 1.0f - fraction;
    half3 squared = fraction * fraction;
    half3 one_sqd = one_frac * one_frac;
    
	// bspline convolution weights, without conditional statements
    half3 w0 = 1.0f / 6.0f * one_sqd * one_frac;
    half3 w1 = 2.0f / 3.0f - 0.5f * squared * (2.0f - fraction);
    half3 w2 = 2.0f / 3.0f - 0.5f * one_sqd * (2.0f - one_frac);
    half3 w3 = 1.0f / 6.0f * squared * fraction;

    half3 g0 = w0 + w1;
    half3 g1 = w2 + w3;
    half3 h0 = (w1 / g0) - 0.5f + index; //h0 = w1/g0 - 1, move from [-0.5, extent-0.5] to [0, extent]
    half3 h1 = (w3 / g1) + 1.5f + index; //h1 = w3/g1 + 1, move from [-0.5, extent-0.5] to [0, extent]


	// fetch the eight linear interpolations
	// weighting and fetching is interleaved for performance and stability reasons
	// TODO : USE LERP
    half4 tex000 = SampleTexture3D_Linear(tex, half3(h0.x, h0.y, h0.z) * texelSize);
    half4 tex100 = SampleTexture3D_Linear(tex, half3(h1.x, h0.y, h0.z) * texelSize);
    tex000 = g0.x * tex000 + g1.x * tex100; //weigh along the x-direction
    half4 tex010 = SampleTexture3D_Linear(tex, half3(h0.x, h1.y, h0.z) * texelSize);
    half4 tex110 = SampleTexture3D_Linear(tex, half3(h1.x, h1.y, h0.z) * texelSize);
    tex010 = g0.x * tex010 + g1.x * tex110; //weigh along the x-direction
    tex000 = g0.y * tex000 + g1.y * tex010; //weigh along the y-direction
    half4 tex001 = SampleTexture3D_Linear(tex, half3(h0.x, h0.y, h1.z) * texelSize);
    half4 tex101 = SampleTexture3D_Linear(tex, half3(h1.x, h0.y, h1.z) * texelSize);
    tex001 = g0.x * tex001 + g1.x * tex101; //weigh along the x-direction
    half4 tex011 = SampleTexture3D_Linear(tex, half3(h0.x, h1.y, h1.z) * texelSize);
    half4 tex111 = SampleTexture3D_Linear(tex, half3(h1.x, h1.y, h1.z) * texelSize);
    tex011 = g0.x * tex011 + g1.x * tex111; //weigh along the x-direction
    tex001 = g0.y * tex001 + g1.y * tex011; //weigh along the y-direction
    return g0.z * tex000 + g1.z * tex001;	//weigh along the z-direction
}

// Generic texture sampling function
half4 SampleTexture3D(TEXTURE3D_SAMPLER_DECLARATION tex, half3 texCoords, half3 texelSize)
{
	#if defined(AURA_USE_CUBIC_FILTERING)
		return SampleTexture3D_Cubic(tex, texCoords, texelSize);
	#else
		return SampleTexture3D_Linear(tex, texCoords);
	#endif
}
