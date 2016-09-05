//----------------------------------------------------------------
            // Copyright © 2015 NCSpeedLight
            //
            // Describle:
		    // Created By:hsu
		    // Date:20151010
		    // Modify History:
//----------------------------------------------------------------*/
Shader "NCSpeedLight/Particles/Additive 4000"
{
	Properties
	{
	    _TintColor("Tint Color",Color)=(0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade("Soft Particles Factor",Range(0.01,3.0))=1.0
		_ColorA("Alpha",float) = 1
	}
	SubShader{
	    Tags{
	        "Queue"="Transparent+1000"
	        "IgnoreProjector"="True"
	        "RenderType"="Transparent"
	    }
	    Blend SrcAlpha OneMinusSrcAlpha
	    AlphaTest Greater .01
	    ColorMask RGB
	    Cull Off
	    Lighting Off
	    ZWrite Off
	    
	    LOD 400
	    
	    Pass{
	        CGPROGRAM
	        #pragma vertex vert
	        #pragma fragment frag
	        #pragma multi_compile_particles
	        #include "UnityCG.cginc"
	        sampler2D _MainTex;
	        fixed4 _TintColor;
	        float _ColorA;
	        
	        struct appdata_t{
	            float4 vertex :POSITION;
	            fixed4 color:COLOR;
	            float2 texcoord:TEXCOORD0;
	        };
	        
	        struct v2f{
	            float4 vertex :SV_POSITION;
	            fixed4 color:COLOR;
	            float2 texcoord:TEXCOORD0;
	            #ifdef SOFTPARTICLES_ON
	            float4 projPos :TEXCOORD1;
	            #endif
	        };
	        
	        float4 _MainTex_ST;

	        v2f vert(appdata_t v){
	            v2f o;
	            o.vertex = mul(UNITY_MATRIX_MVP,v.vertex);
	            #ifdef SOFTPARTICLES_ON
	            o.projPos = ComputeScreenPos(o.vertex);
	            COMPUTE_EYEDEPTH(o.projPos.z);
	            #endif
	            o.color = v.color;
	            o.color = 2.0f * o.color *_TintColor;
	            o.color.a *= _ColorA;
	            o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
	            return o;
	        }
	        
	        sampler2D_float _CameraDepthTexture;
	        float _InvFade;
	        
	        fixed4 frag(v2f i):SV_Target{
	            #ifdef SOFTPARTICLES_ON
	            float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD(i.projPos)));
	            float partZ = i.projPos.z;
				float fade = saturate (_InvFade * (sceneZ-partZ));
				i.color.a *= fade;
				#endif
				
				i.color = i.color * tex2D(_MainTex, i.texcoord);
				return i.color;
	        }
	        ENDCG
	    }
	}
	
	SubShader{
	    Tags{
	        "Queue"="Transparent+1000"
	        "IgnoreProjector"="True"
	        "RenderType"="Transparent"
	    }
	    Blend SrcAlpha OneMinusSrcAlpha
	    AlphaTest Greater .01
	    Cull Off
	    Lighting Off
	    ZWrite Off
	    Fog {Mode Off}
	    
	    LOD 80
	    
	    Pass{
	        BindChannels{
	            Bind "Color",color
	            Bind "Vertex",vertex
	            Bind "Texcoord",texcoord
	        }
	        
	        SetTexture[_MainTex]{
	            combine texture * primary
	        }
	    }
	}
}
