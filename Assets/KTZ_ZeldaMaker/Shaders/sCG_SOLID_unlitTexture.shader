// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Keetzap/sCG_SOLID_unlitTexture" 
{

	Properties
	{
		_Color("Base Color", Color) = (1,1,1,1)
		_MainTex("Base map", 2D) = "white" {}
		_Overbright("Overbright", float) = 1.0
	}

	SubShader
	{

		Tags
		{
			"Queue" = "Geometry"
			"RenderType" = "Geometry"
		}

		LOD 200

		//Cull Off //two sided
		Lighting Off
		//ZWrite Off

		//Blending mode----------------------------------------------
		//Blend SrcAlpha OneMinusSrcAlpha			// Traditional transparency
		//Blend One OneMinusSrcAlpha				// Premultiplied transparency
		//Blend One One								// Additive
		//Blend OneMinusDstColor One				// Soft Additive
		//Blend DstColor Zero						// Multiplicative
		//Blend DstColor SrcColor					// 2x Multiplicative

		//Fog{ Mode Global } // MODE: Off | Global | Linear | Exp | Exp

		Pass
		{
			CGPROGRAM

			//#pragma surface surf Standard alphatest:_Cutoff vertex:vert addshadow
			#pragma vertex vert 
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _Color;
			float _Overbright;

			struct Input 
			{
				//blahblah
				float3 viewDir;
				//blahblah
			};


			struct vertexInput
			{
				float4 vertex : POSITION;
				//float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				//float4 texcoord1 : TEXCOORD1;
				//fixed4 color : COLOR;
				//half4 texcoord2 : TEXCOORD2;
				//half4 texcoord3 : TEXCOORD3;
				//half4 texcoord4 : TEXCOORD4;
				//half4 texcoord5 : TEXCOORD5;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float2 uv_MainTex : TEXCOORD0;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput OUT;
				OUT.pos = UnityObjectToClipPos(v.vertex);

				OUT.uv_MainTex = v.texcoord.xy;

				return OUT;
			}

			half4 frag(vertexOutput IN) : COLOR
			{
				float4 DF = tex2D(_MainTex, IN.uv_MainTex);

				fixed4 Complete = float4(DF.rgb * _Color.rgb * _Overbright, DF.a * _Color.a); 

				return Complete;
			}

			ENDCG

		}//end pass

	}//end subshader

	Fallback "Fx/Flare"
}
