
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

#define NUM_THREAD_X 8
#define NUM_THREAD_Y 8
#define NUM_THREAD_Z 1

// Time (to be set from Unity)
float time;

// SamplerStates
SamplerState _LinearClamp;
SamplerState _LinearRepeat;
SamplerState _PointClamp;
SamplerState _PointRepeat;

// Const variables
static const float pi = 3.141592653589793f;
static const float twoPi = pi * 2.0f;
static const float halfPi = pi * 0.5f;
static const float quarterPi = pi * 0.25f;
static const float e = 2.71828182845904523536f;
static const float n = 1.0f / e;

// Common variables
float4 Aura_BufferResolution;
float4 Aura_BufferTexelSize;
float4 cameraPosition;
float4 cameraRanges;
float Aura_DepthBiasCoefficient;
float Aura_DepthBiasReciproqualCoefficient;