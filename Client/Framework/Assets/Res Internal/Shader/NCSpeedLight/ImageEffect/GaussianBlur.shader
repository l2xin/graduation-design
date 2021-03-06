﻿//----------------------------------------------------------------
		// Copyright © 2015 NCSpeedLight
		//
		// Describle:
		// Created By:hsu
		// Date:20151010
		// Modify History:
//----------------------------------------------------------------*/
Shader "Air2000/ImageEffect/GaussianBlur"
{
	Properties
	{
	_MainTex("Base (RGB)", 2D) = "white" {}
	coordOffs("Coord Offsets", Vector) = (1.0, 1.0, -1.0, -1.0)
}
	CGINCLUDE
	#include "UnityCG.cginc"    
	sampler2D _MainTex;
	uniform half4 _MainTex_TexelSize;
	uniform float _blurSize;     // weight curves    
	//half curve[4] = { 0.0205, 0.0855, 0.232, 0.324 };
	half4 coordOffs; /*= half4(1.0h, 1.0h, -1.0h, -1.0h);*/
	struct v2f_withBlurCoordsSGX
	{
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		half4 offs[3] : TEXCOORD1;
	};
	v2f_withBlurCoordsSGX vertBlurHorizontalSGX(appdata_img v)
	{
		v2f_withBlurCoordsSGX o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		half2 netFilterWidth = _MainTex_TexelSize.xy * half2(1.0, 0.0) * _blurSize;
		half4 coords = -netFilterWidth.xyxy * 3.0;
		o.offs[0] = v.texcoord.xyxy + coords * coordOffs;
		coords += netFilterWidth.xyxy;
		o.offs[1] = v.texcoord.xyxy + coords * coordOffs;
		coords += netFilterWidth.xyxy;
		o.offs[2] = v.texcoord.xyxy + coords * coordOffs;
		return o;
	}
	v2f_withBlurCoordsSGX vertBlurVerticalSGX(appdata_img v)
	{
		v2f_withBlurCoordsSGX o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		half2 netFilterWidth = _MainTex_TexelSize.xy * half2(0.0, 1.0) * _blurSize;
		half4 coords = -netFilterWidth.xyxy * 3.0;
		o.offs[0] = v.texcoord.xyxy + coords * coordOffs;
		coords += netFilterWidth.xyxy;
		o.offs[1] = v.texcoord.xyxy + coords * coordOffs;
		coords += netFilterWidth.xyxy;
		o.offs[2] = v.texcoord.xyxy + coords * coordOffs;
		return o;
	}
	half4 fragBlurSGX(v2f_withBlurCoordsSGX i) : SV_Target
	{
		half2 uv = i.uv;
		half4 color = tex2D(_MainTex, i.uv) * 0.324;
		//int l = 0;
		half4 tapA = tex2D(_MainTex, i.offs[0].xy);
		half4 tapB = tex2D(_MainTex, i.offs[0].zw);
		color += (tapA + tapB) *  0.0205;

		//l++;
		tapA = tex2D(_MainTex, i.offs[1].xy);
		tapB = tex2D(_MainTex, i.offs[1].zw);
		color += (tapA + tapB) * 0.0855;

		//l++;
		tapA = tex2D(_MainTex, i.offs[2].xy);
		tapB = tex2D(_MainTex, i.offs[2].zw);
		color += (tapA + tapB) * 0.232;
		return color;
	}
	ENDCG
		SubShader{
		ZTest Off
		ZWrite Off
		Blend Off
		Pass{
		ZTest Always
		CGPROGRAM
#pragma vertex vertBlurVerticalSGX   
#pragma fragment fragBlurSGX      
		ENDCG
	}
		Pass{
			ZTest Always
			CGPROGRAM
#pragma vertex vertBlurHorizontalSGX  
#pragma fragment fragBlurSGX      
			ENDCG
		}
	}
	FallBack Off
}

