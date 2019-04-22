	
Shader "benoculus/Reflective Transparent" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
	_MainTex ("Base (RGB) RefStrGloss (A)", 2D) = "white" {}
	_Cube ("Reflection Cubemap", Cube) = "" { TexGen CubeReflect }
	_Fresnel ("Fresnel Strength", Range(0, 2)) = 1
}
 
SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 400
CGPROGRAM
#pragma surface surf BlinnPhong alpha
#pragma target 3.0
//input limit (8) exceeded, shader uses 9
#pragma exclude_renderers d3d11_9x
 
sampler2D _MainTex;
samplerCUBE _Cube;
 
fixed4 _Color;
fixed4 _ReflectColor;
half _Shininess;
half _Fresnel;
 
struct Input {
	float2 uv_MainTex;
	float3 worldRefl;
	INTERNAL_DATA
};
 
void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 c = tex * _Color;
	o.Albedo = c.rgb;
	
	o.Gloss = tex.a;
	o.Specular = _Shininess;
	o.Normal = float4(0,0,1,0);

	//trick to compute world space normal
	IN.worldRefl = normalize( IN.worldRefl );
	float3 worldRefl = WorldReflectionVector (IN, o.Normal);
	float3 worldNormal = normalize( worldRefl - IN.worldRefl); 

	fixed4 reflcol = texCUBE (_Cube, worldRefl);
	reflcol *= tex.a * clamp(1 - dot(worldNormal, worldRefl) * _Fresnel, 0, 1);
	o.Emission = reflcol.rgb * _ReflectColor.rgb;
	o.Alpha = tex.a * _Color.a;
}
ENDCG
}
 
FallBack "Reflective/Bumped Diffuse"
}