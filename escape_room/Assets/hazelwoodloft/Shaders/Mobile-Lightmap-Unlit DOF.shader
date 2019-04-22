// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

// Unlit shader. Simplest possible textured shader.
// - SUPPORTS lightmap
// - no lighting
// - no per-material color

Shader "Mobile/Unlit (Supports Lightmap) DOF" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue" = "Geometry+1" }
		LOD 80
		
	Pass {
		Name "FORWARD"
		
		
		CGPROGRAM
		
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma multi_compile_fwdbase
                                                           
		#include "HLSLSupport.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"


		uniform sampler2D _MainTex;
		// uniform sampler2D unity_Lightmap;
		
		uniform half _DepthFar;
		uniform half _DOFApature;
		
		// half4 unity_LightmapST;
		half4 _MainTex_ST;


    struct appdata_simple {
    	float4 vertex : POSITION;
        float4 texcoord : TEXCOORD0;
        float4 texcoord1 : TEXCOORD1;
      };

		struct v2f_surf {
			half4 pos : SV_POSITION;
			half2 uv_MainTex : TEXCOORD0;
			half2 lmap : TEXCOORD1;
	  		fixed depth : TEXCOORD2;
		};
		
			v2f_surf vert_surf (appdata_simple v) {
			v2f_surf o;
			
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.depth = abs( (1 - clamp(-mul(UNITY_MATRIX_MV, v.vertex).z / _DepthFar,0,2)) * _DOFApature);
				o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
			
			return o;
		}
		

		fixed4 frag_surf (v2f_surf IN) : COLOR {

			fixed4 c = 0;
			fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap));
			c.rgb = tex2D (_MainTex, IN.uv_MainTex).rgb * lm;
			c.a = IN.depth;
			
			return c;
		
		}

ENDCG
	}
}

FallBack "Mobile/VertexLit"
}