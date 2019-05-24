
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

using System;

namespace Aura2API
{
    /// <summary>
    /// Bitmask representing the possible parameters for the volumetric data computation
    /// </summary>
    [Flags]
    public enum FrustumParameters
    {
        EnableNothing                           = 0,
        EnableOcclusionCulling                  = 1 << 0,
        EnableTemporalReprojection              = 1 << 1,
        EnableDenoisingFilter                   = 1 << 2,
        EnableVolumes                           = 1 << 3,
        EnableVolumesNoiseMask                  = 1 << 4,
        EnableVolumesTexture2DMask              = 1 << 5,
        EnableVolumesTexture3DMask              = 1 << 6,
        EnableAmbientLighting                   = 1 << 7,
        EnableLightProbes                       = 1 << 8,
        EnableDirectionalLights                 = 1 << 9,
        EnableDirectionalLightsShadows          = 1 << 10,
        DirectionalLightsShadowsOneCascade      = 1 << 11,
        DirectionalLightsShadowsTwoCascades     = 1 << 12,
        DirectionalLightsShadowsFourCascades    = 1 << 13,
        EnableSpotLights                        = 1 << 14,
        EnableSpotLightsShadows                 = 1 << 15,
        EnablePointLights                       = 1 << 16,
        EnablePointLightsShadows                = 1 << 17,
        EnableLightsCookies                     = 1 << 18
    }
}
