Shader "NCSpeedLight/Effect/ScrollBG (3 Layers)"
{
	Properties
	{
		_MainTex ("Base Layer", 2D) = "white" {}
		_SecondTex ("2nd Layer",2D) = "white" {}
		_ThirdTex ("3rd Layer",2D) = "white" {}
		_ScrollX ("Base Layer Scroll Speed",Float) = 1.0
		_Scroll2X ("2nd Layer Scroll Speed",Float) = 1.0
		_Scroll3X ("3rd Layer Scroll Speed",Float) = 1.0
		_Multiplier ("Layer Muliplier",Float)=1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry"}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _SecondTex;
			sampler2D _ThirdTex;
			float4 _MainTex_ST;
			float4 _SecondTex_ST;
			float4 _ThirdTex_ST;
			float _ScrollX;
			float _Scroll2X;
			float _Scroll3X;
			float _Multiplier;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
				float4 uv2: TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex) + frac(float2(_ScrollX, 0.0) * _Time.y);
				o.uv.zw = TRANSFORM_TEX(v.texcoord, _SecondTex) + frac(float2(_Scroll2X, 0.0) * _Time.y);
				o.uv2.xy = TRANSFORM_TEX(v.texcoord,_ThirdTex) + frac(float2(_Scroll3X,0.0)*_Time.y);
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 firstLayer = tex2D(_MainTex, i.uv.xy);
				fixed4 secondLayer = tex2D(_SecondTex, i.uv.zw);
				fixed4 thirdLayer = tex2D(_ThirdTex,i.uv2.xy);
				
				fixed4 c = lerp(firstLayer, secondLayer,secondLayer.a);
				c= lerp(c, thirdLayer,thirdLayer.a);
				c.rgb *= _Multiplier;
				
				return c;
			}
			ENDCG
		}
	}
}
