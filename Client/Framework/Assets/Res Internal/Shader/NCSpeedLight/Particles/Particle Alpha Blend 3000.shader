//----------------------------------------------------------------
            // Copyright © 2015 NCSpeedLight
            //
            // Describle:
		    // Created By:hsu
		    // Date:20151010
		    // Modify History:
//----------------------------------------------------------------*/
Shader "NCSpeedLight/Particles/Alpha Blended 3000"
{
	Properties
	{
	    _TintColor("Tint Color",Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Main Texture", 2D) = "white" {}
		_ReplaceTex ("Replace Texture", 2D) = "white" {}
		_InvFade("Soft Particles Factor",Range(0.01,3.0))=1.0
		_ReserveFlag("Reserve the Picture",float)=0
		_ReplaceFlag("Replace the Picture",float)=0
		_ColorA("Alpha",float) = 1
	}
	Category{
	    Tags{
	        "Queue"="Transparent"
	        "IgnoreProjector"="True"
	        "RenderType"="Transparent"
	    }
	    Blend SrcAlpha OneMinusSrcAlpha
	    AlphaTest Greater .01
	    ColorMask RGB
	    Cull Off
	    Lighting Off 
		ZWrite Off
	    Fog {Color(0,0,0,0)}
	    
	    SubShader{
	        Pass{
	            CGPROGRAM
	            #pragma vertex vert
	            #pragma fragment frag
	            #pragma multi_compile_particles
	            
	            #include "UnityCG.cginc"
	            sampler2D _MainTex;
	            sampler2D _ReplaceTex;

	            fixed4 _TintColor;
	            float _ReserveFlag;
	            float _ReplaceFlag;
	            float _ColorA;
	   
	            
	            struct appdata_t {
	                float4 vertex:POSITION;
	                fixed4 color:COLOR;
	                float2 texcoord:TEXCOORD0;
	            };
	            
	            struct v2f{
	                float4 vertex:POSITION;
	                fixed4 color:COLOR;
	                float2 texcoord:TEXCOORD0;
	                #ifdef SOFTPARTICLES_ON
	                float4 projPos:TEXCOORD1;
	                #endif
	            };
	            
	            float4 _MainTex_ST;
	            float4 _ReplaceTex_ST;
	            
	            v2f vert(appdata_t v){
	                v2f o;
	                o.vertex = mul(UNITY_MATRIX_MVP,v.vertex);
	                #ifdef SOFTPARTICLES_ON
	                o.projPos = ComputeScreenPos(o.vertex);
	                COMPUTE_EYEDEPTH(o.projPos.z);
	                #endif
	                o.color = v.color;
	                
	                float2 f = v.texcoord;
	                if(_ReserveFlag == 1){
	                    f = float2(1-v.texcoord.x,v.texcoord.y);
	                }else if(_ReserveFlag == 2){
	                    f = float2(v.texcoord.x,1-v.texcoord.y);
	                }
	                
	                if(_ReplaceFlag>0){
	                    o.texcoord = TRANSFORM_TEX(f,_ReplaceTex);
	                }else{
	                    o.texcoord = TRANSFORM_TEX(f,_MainTex);
	                }
	                return o;
	            }
	            
	            sampler2D _CameraDepthTexture;
	            float _InvFade;
	            
	            fixed4 frag(v2f i):COLOR{
	                #ifdef SOFTPARTICLES_ON
	                float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(i.projPos))));
	                float partZ = i.projPos.z;
	                float fade = saturate(_InvFade*(sceneZ - partZ));
	                i.color.a*= fade;
	                #endif
	                if(_ReplaceFlag>0){
	                    i.color = 2.0f*i.color*_TintColor*tex2D(_ReplaceTex,i.texcoord);
	                }else{
	                    i.color = 2.0f*i.color*_TintColor*tex2D(_MainTex,i.texcoord);
	                }
	                i.color.a*=_ColorA;
	                return i.color;
	            }
				ENDCG
	        }
	    }
	}
}
