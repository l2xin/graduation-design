﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

//----------------------------------------------------------------
            // Copyright © 2015 NCSpeedLight
            //
            // Describle: 卡通描边着色
		    // Created By:hsu
		    // Date:20151010
		    // Modify History:
//----------------------------------------------------------------*/
Shader "NCSpeedLight/ToonShading/NormalOutline"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MainColor("MainColor",Color) = (1,1,1,1)
		_OutlineColor("OutlineColor",Color)=(0,0,0,1)

		_Outline("Outline",Range(0,1))=0.02
		_Strength("Strength",Range(0,5)) = 1.38
	   // _Factor("Factor",range(1,100))=1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGINCLUDE
		#include "UnityCG.cginc"
	    sampler2D _MainTex;
	    float4 _MainTex_ST;
	    float _Outline;
	    float4 _MainColor;
	    float4 _OutlineColor;
	    float _Strength;
		ENDCG
		
		/*Pass{
	      Tags{"LightMode"="ForwardBase"}
	      Cull Front
	      Lighting On
	      ZWrite On
	        
	      CGPROGRAM
	      #pragma vertex vert
	      #pragma fragment frag
	      #pragma multi_compile_fwdbase
	      #include "UnityCG.cginc"
	        
	      struct a2v{
	          float4 vertex:POSITION;
	          float3 normal:NORMAL;
	      };
	        
	      struct v2f{
	          float4 pos:POSITION;
	          float3 viewSpacePos:TEXCOORD0;
	      };
	        
	      v2f vert(a2v v){
	          v2f o;  
              float4 pos = mul( UNITY_MATRIX_MV, v.vertex);   
              float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, v.normal);    
              normal.z = -0.5;  
              pos = pos + float4(normalize(normal),0) * _Outline;  
              o.pos = mul(UNITY_MATRIX_P, pos);  
              o.viewSpacePos = mul( UNITY_MATRIX_MV, v.vertex);  
                
              return o;  
	      }
	        
	      float4 frag(v2f i):COLOR{
	          return _OutlineColor;
	      }
	      ENDCG
	    }*/

//        Pass{
//            Tags{"LightMode"="Always"}
//            Cull Front
//            ZWrite On
//            
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            #include "UnityCG.cginc"
//            
//	        float _Factor;
//            
//            struct v2f{
//                float4 pos:SV_POSITION;
//            };
//             
//            v2f vert (appdata_full v) {
//			    v2f o;
//			    o.pos=mul(UNITY_MATRIX_MVP,v.vertex);
//			    float3 dir=normalize(v.vertex.xyz);
//			    float3 dir2=v.normal;
//			    float D=dot(dir,dir2);
//			    D=(D/_Factor+1)/(1+1/_Factor);
//		    	dir=lerp(dir2,dir,D);
//		    	dir= mul ((float3x3)UNITY_MATRIX_IT_MV, dir);
//		    	float2 offset = TransformViewToProjection(dir.xy);
//		    	offset=normalize(offset);
//		    	o.pos.xy += offset * o.pos.z *_Outline;
//			    return o;
//	    	}
//            
//            fixed4 frag(v2f i):COLOR{
//                return _OutlineColor;
//            }
//            ENDCG
//        }
	    
	    Pass{
	        Tags{"LightMode"="ForwardBase"}
	        Cull Back
	        Lighting On
	        
	        CGPROGRAM
	        #pragma vertex vert
	        #pragma fragment frag
	        #pragma multi_compile_fwdbase
	        
	        #include "UnityCG.cginc"
	        #include "Lighting.cginc"
	        #include "AutoLight.cginc"
	        #include "UnityShaderVariables.cginc"
	        
	        struct a2v{
	            float4 vertex:POSITION;
	            float3 normal:NORMAL;
	            float4 texcoord:TEXCOORD0;
	            float4 tangent:TANGENT;
	            float4 color:COLOR;
	        };
	        
	        struct v2f{
	            float4 pos:POSITION;
	            float2 uv:TEXCOORD0;
	            float3 normal:TEXCOORD1;
	            float4 color:COLOR;
	            LIGHTING_COORDS(3,4)
	        };
	        
	        v2f vert(a2v v){
	            v2f o;
	            o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
	            o.normal = mul((float3x3)unity_ObjectToWorld,SCALED_NORMAL);
	            o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
	            o.color = v.color;
	            TRANSFER_VERTEX_TO_FRAGMENT(o);
	            return o;
	        }
	        
	        float4 frag(v2f i):COLOR{
	            float4 c;
	            c = tex2D(_MainTex,i.uv);
	            
	            float3 lightColor = UNITY_LIGHTMODEL_AMBIENT.xyz;
                
              //  c.rgb*=lightColor;
                c.rgb*=_MainColor;
                c.rgb*=_Strength;
                return c;
	        }
	        ENDCG
	    }	    
	}
}
