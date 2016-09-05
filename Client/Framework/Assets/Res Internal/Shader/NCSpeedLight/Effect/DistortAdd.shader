//----------------------------------------------------------------
            // Copyright © 2015 NCSpeedLight
            //
            // Describle:
		    // Created By:hsu
		    // Date:20151010
		    // Modify History:
//----------------------------------------------------------------*/
Shader "NCSpeedLight/Effect/DistortAdd"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	    _NoiseTex("Distort Texture(RG)",2D)="white"{}
	    _TintColor("Tint Color",Color)=(0.5,0.5,0.5,0.5)
	    _Strength("Strength",range(1,10))=1
		_HeatTime("Heat Time",range(-1,1))=0
		_ForceX("Force X",range(0,1))=0.1
		_ForceY("Force Y",range(0,1))=0.1
	}
	
	Category{
	    Tags{
	        "Queue"="Transparent"
	        "RenderType"="Transparent"
	    }
	    
	    Blend SrcAlpha OneMinusSrcAlpha  // set blend cmd
	    Cull Off
	    Lighting Off
	    Zwrite Off
	    Fog{Color(0,0,0,0)}
	    BindChannels{
	        Bind "Color",color
	        Bind "Vertex",vertex
	        Bind "Texcoord",texcoord
	    }
	    
	    SubShader{
	        Pass{
	        Tags { "LightMode" = "Always" }
	            CGPROGRAM
	            #pragma vertex vert
	            #pragma fragment frag
	            #pragma fragmentoption ARB_precision_hint_fastest
	            #pragma multi_compile_particles
	            #include "UnityCG.cginc"
	            
	            fixed4 _TintColor;
	            fixed _ForceX;
	            fixed _ForceY;
	            fixed _HeatTime;
	            float4 _MainTex_ST;
	            float4 _NoiseTex_ST;
	            sampler2D _MainTex;
	            sampler2D _NoiseTex;
	            float _Strength;
	            
	            struct appdata_t{
	                float4 vertex:POSITION;
	                fixed4 color:COLOR;
	                float2 texcoord:TEXCOORD0;
	            };
	            
	            struct v2f{
	                float4 vertex :POSITION;
	                fixed4 color:COLOR;
	                float2 uvmain:TEXCOORD1;
	            };
	            
	            v2f vert(appdata_t v){
	                v2f o;
	                o.vertex = mul(UNITY_MATRIX_MVP,v.vertex);
	                o.color = v.color;
	                o.uvmain = TRANSFORM_TEX(v.texcoord,_MainTex);
	                return o;
	            }
	            
	            fixed4 frag(v2f i):COLOR{
	                //noise effect
	                fixed4 offsetColor1 = tex2D(_NoiseTex,i.uvmain+_Time.xz*_HeatTime);
	                fixed4 offsetColor2 = tex2D(_NoiseTex,i.uvmain+_Time.yx*_HeatTime);
	                i.uvmain.x += ((offsetColor1.r + offsetColor2.r) - 1) * _ForceX*_TintColor.a;
	                i.uvmain.y += ((offsetColor1.g + offsetColor2.g) - 1) * _ForceY*_TintColor.a;

					//Calculate final color
 					return _Strength* i.color*_TintColor * tex2D( _MainTex, i.uvmain);
	            }
	            ENDCG
	        }
	    }
	    
	    SubShader{
	        //fallback
	        Blend DstColor Zero
	        Pass{
	            Name "BASE"
	            SetTexture[_MainTex]{combine texture}
	        }
	    }
	}
}
