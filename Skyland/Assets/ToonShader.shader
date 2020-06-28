Shader "Unlit/ToonShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Col("Color", COLOR) = (1,1,1,1)
		_ShadowSen("Shadow sensitivity", Range(0.0,1.0)) = 0.3
		_Tint("Tint", Range(0.0,1.0)) = 0.5
		_Mult("Mutiplier", Range(0.0,1.0)) = 0.5
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
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
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldNormal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Col;
			float _ShadowSen;
			float _Tint;
			float _Mult;

			float Toon(float3 normal, float3 lightDir)
			{
				float NdotL = max(0.0, dot(normalize(normal), normalize(lightDir)));

				return floor(NdotL / _ShadowSen);
			}

			v2f vert(appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				col *= Toon(i.worldNormal, _WorldSpaceLightPos0.xyz) * _Mult  + _Tint;

				return col * _Col;
			}
			ENDCG
		}
	}
}
