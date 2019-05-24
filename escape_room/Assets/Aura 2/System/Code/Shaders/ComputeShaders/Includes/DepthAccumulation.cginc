
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

// https://en.wikipedia.org/wiki/Absorbance#Attenuation_coefficient
half LinearAbsorbance(float traveledDistance, float absorbanceFactor)
{
	return traveledDistance * absorbanceFactor;
}

///https://en.wikipedia.org/wiki/Optical_depth#Optical_depth
half OpticalDepth(half absorbance)
{
    return absorbance; // * log(10); We can bypass log(10) as it is a constant
}

// https://en.wikipedia.org/wiki/Transmittance#Beer-Lambert_law
half Transmittance(half opticalDepth)
{
	return saturate(exp(-opticalDepth));
}

// https://en.wikipedia.org/wiki/Opacity_(optics)#Quantitative_definition
half Opacity(half transmitance)
{
	return 1.0f - transmitance;
}

half AccumulateTransmittance(half transmittanceA, half transmittanceB)
{
	return transmittanceA * transmittanceB; // x^(a+b) = x^a * x^b
}

half4 Accumulate(half4 colorAndDensityFront, half4 colorAndDensityBack, float traveledDistance, half extinction)
{
	half absorbance = LinearAbsorbance(traveledDistance, extinction);
	half opticalDepth = OpticalDepth(absorbance);
    half transmittance = Transmittance(opticalDepth * colorAndDensityBack.w);

    half4 accumulatedLightAndTransmittance = half4(colorAndDensityFront.xyz + colorAndDensityBack.xyz * Opacity(transmittance) * colorAndDensityFront.w, AccumulateTransmittance(transmittance, colorAndDensityFront.w));
	
    return accumulatedLightAndTransmittance;
}