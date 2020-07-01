Shader "Kernel_Edgedetect"
{
	Properties
	{
		_MainTex ("Screen Image", 2D) = "white" {}
		_Width ("Texture Size Width", Float) = 128	
		_Height ("Texture Size Height", Float) = 128	
		_MultFact ("Multiplication Factor", Range(1.0,800.0)) = 1.0
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Width;
			float _Height;
			float _MultFact;

			v2f vert (appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed2 uv = i.uv;

				float w = 1 / _Width;
				float h = 1 / _Height;
				
				float3 tl = tex2D(_MainTex, uv + fixed2(-w, -h));
				float3 tc = tex2D(_MainTex, uv + fixed2( 0, -h));
				float3 tr = tex2D(_MainTex, uv + fixed2(+w, -h));

				float3 cl = tex2D(_MainTex, uv + fixed2(-w, 0));
				float3 cc = tex2D(_MainTex, uv);
				float3 cr = tex2D(_MainTex, uv + fixed2(+w, 0));

				float3 bl = tex2D(_MainTex, uv + fixed2(-w, +h));
				float3 bc = tex2D(_MainTex, uv + fixed2( 0, +h));
				float3 br = tex2D(_MainTex, uv + fixed2(+w, +h));

				float _Ring = -1 * _MultFact;
				float _Centre = 8 * _MultFact + 1;

				tl *= _Ring;
				tc *= _Ring;
				tr *= _Ring;

				cl *= _Ring;
				cc *= _Centre;
				cr *= _Ring;

				bl *= _Ring;
				bc *= _Ring;
				br *= _Ring;

				float3 result = tl + tc + tr + cl + cc + cr + bl + bc + br;

				return float4(result.r, result.g, result.b, 1);
			}
			ENDCG
		}
	}
}
