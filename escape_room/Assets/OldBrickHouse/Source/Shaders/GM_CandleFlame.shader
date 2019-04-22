// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GabroMedia/GM_CandleFlame"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		Blend One Zero , SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#pragma target 2.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _TextureSample0;
		uniform float _Cutoff = 0.5;

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_TexCoord2 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			// *** BEGIN Flipbook UV Animation vars ***
			// Total tiles of Flipbook Texture
			float fbtotaltiles3 = 4.0 * 4.0;
			// Offsets for cols and rows of Flipbook Texture
			float fbcolsoffset3 = 1.0f / 4.0;
			float fbrowsoffset3 = 1.0f / 4.0;
			// Speed of animation
			float fbspeed3 = _Time[1] * 40.0;
			// UV Tiling (col and row offset)
			float2 fbtiling3 = float2(fbcolsoffset3, fbrowsoffset3);
			// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
			// Calculate current tile linear index
			float fbcurrenttileindex3 = round( fmod( fbspeed3 + 0.0, fbtotaltiles3) );
			fbcurrenttileindex3 += ( fbcurrenttileindex3 < 0) ? fbtotaltiles3 : 0;
			// Obtain Offset X coordinate from current tile linear index
			float fblinearindextox3 = round ( fmod ( fbcurrenttileindex3, 4.0 ) );
			// Multiply Offset X by coloffset
			float fboffsetx3 = fblinearindextox3 * fbcolsoffset3;
			// Obtain Offset Y coordinate from current tile linear index
			float fblinearindextoy3 = round( fmod( ( fbcurrenttileindex3 - fblinearindextox3 ) / 4.0, 4.0 ) );
			// Reverse Y to get tiles from Top to Bottom
			fblinearindextoy3 = (int)(4.0-1) - fblinearindextoy3;
			// Multiply Offset Y by rowoffset
			float fboffsety3 = fblinearindextoy3 * fbrowsoffset3;
			// UV Offset
			float2 fboffset3 = float2(fboffsetx3, fboffsety3);
			// Flipbook UV
			half2 fbuv3 = uv_TexCoord2 * fbtiling3 + fboffset3;
			// *** END Flipbook UV Animation vars ***
			float4 tex2DNode1 = tex2D( _TextureSample0, fbuv3 );
			o.Emission = tex2DNode1.rgb;
			o.Alpha = 1;
			clip( tex2DNode1.r - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13801
1927;29;1906;1004;1266;465;1;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1023,-111;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;4;-916,64;Float;False;Constant;_Float0;Float 0;1;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;5;-915,153;Float;False;Constant;_Float1;Float 1;1;0;40;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TFHCFlipBookUVAnimation;3;-659,-17;Float;False;0;0;5;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-363,-93;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;165,-110;Float;False;True;0;Float;ASEMaterialInspector;0;0;Unlit;GabroMedia/GM_CandleFlame;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Off;0;0;False;0;0;Custom;0.5;True;False;0;True;TransparentCutout;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;0;One;OneMinusSrcAlpha;2;SrcAlpha;OneMinusSrcAlpha;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;0;2;0
WireConnection;3;1;4;0
WireConnection;3;2;4;0
WireConnection;3;3;5;0
WireConnection;1;1;3;0
WireConnection;0;2;1;0
WireConnection;0;10;1;1
ASEEND*/
//CHKSM=984308086416C8DA4A5064387F841B91834697C6