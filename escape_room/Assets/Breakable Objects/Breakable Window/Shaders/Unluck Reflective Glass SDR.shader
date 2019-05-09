Shader "Unluck Software/ReflectiveGlass" {
	Properties {
		_Color ("Main Color", Color) = (0.5,0.5,0.5,0.25)
		_MainTex ("Base (RGB) RefStrGloss (A)", 2D) = "white" {}
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078
		_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
		_Cube ("Reflection Cubemap", Cube) = "black" { TexGen CubeReflect }
	}
	SubShader {
		Tags {
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}
		LOD 300
		Cull Off
   	    ZWrite Off
		CGPROGRAM
			#pragma surface surf BlinnPhong alpha nolightmap
			sampler2D _MainTex;
			fixed4 _Color;
			samplerCUBE _Cube;
			fixed4 _ReflectColor;
			half _Shininess;
			
			struct Input {
				float2 uv_MainTex;
				float3 worldRefl;
	 			float4 color : COLOR;
			};	
			void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.rgb * IN.color.rgb * _Color;		
			o.Gloss = tex.a;
			o.Alpha = tex.a * _Color.a;
			o.Specular = _Shininess;			
			fixed4 reflcol = texCUBE (_Cube, IN.worldRefl);
			o.Emission = reflcol.rgb * _ReflectColor.rgb;
		}
		ENDCG
	}
	FallBack "Transparent/VertexLit"
}