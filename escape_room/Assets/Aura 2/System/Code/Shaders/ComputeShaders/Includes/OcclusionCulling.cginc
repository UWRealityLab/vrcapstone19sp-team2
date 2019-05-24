
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

uniform Texture2D<half> occlusionTexture;

inline half SampleOcclusionTexture(uint2 occlusionTextureSamplingIndex)
{
    return occlusionTexture[occlusionTextureSamplingIndex].x;
}

inline bool IsNotOccluded(half biasedLocalDepth, uint3 id)
{
    half occlusionData = SampleOcclusionTexture(id.xy);
	half nextBiasedDepth = GetNormalizedLocalDepthWithDepthBias(id.z + 2); // TODO : NEED TO FIND A BETTER WAY OF HANDLING BIASED DEPTH
	half deltaDepth = nextBiasedDepth - biasedLocalDepth;

    return biasedLocalDepth < (occlusionData + deltaDepth);
}