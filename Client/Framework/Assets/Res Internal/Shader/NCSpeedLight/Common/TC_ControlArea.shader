// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

//----------------------------------------------------------------
            // Copyright © 2015 NCSpeedLight
            // 附带Control贴图，控制局部颜色
            // Describle:
		    // Created By:hsu
		    // Date:20151010
		    // Modify History:
//----------------------------------------------------------------*/
Shader "NCSpeedLight/Common/TC_ControlArea"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ControlTex ("Control Tex",2D) ="white"{}
		_MainColor("MainColor",Color) = (1,1,1,1)
		_ControlAreaColor("ControlAreaColor",Color)= (1,1,1,1)
		_Strength("Strength",Range(0,5)) = 1.38
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 200
		
		CGINCLUDE
		#include "UnityCG.cginc"
	    sampler2D _MainTex;
	    float4 _MainTex_ST;
	    sampler2D _ControlTex;
	    float4 _ControlTex_ST;
	    float4 _MainColor;
	    float _Strength;
	    float4 _ControlAreaColor;
		ENDCG
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
	        
	        fixed4 frag(v2f i):COLOR{
	           fixed4 base = tex2D(_MainTex,i.uv);
	           fixed4 control =tex2D(_ControlTex,i.uv);
	           
	           fixed3 lightColor = UNITY_LIGHTMODEL_AMBIENT.xyz;
	           if(control.r >0 ){
	              float maxx =  max(base.r,base.g);
	              maxx = max (maxx,base.b); 
	        	  float minn =  min(base.r,base.g);
	       		  minn = min (minn,base.b); 
	              float newR = _ControlAreaColor.r*(maxx-minn) +minn;
	              float newG = _ControlAreaColor.g*(maxx-minn) +minn;
	              float newB = _ControlAreaColor.b*(maxx-minn) +minn;
	              fixed4 newColor = fixed4(newR,newG,newB,1);
	              newColor.rgb*=lightColor;
                  newColor.rgb*=_MainColor;
                  newColor.rgba*=_Strength;
                  return newColor;
	           }
               base.rgb*=lightColor;
               base.rgb*=_MainColor;
               base.rgba*=_Strength;
               return base;
	        }
	        ENDCG
	    }	    
	}
}