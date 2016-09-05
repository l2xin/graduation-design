//----------------------------------------------------------------
            // Copyright © 2015 NCSpeedLight
            //
            // Describle:
		    // Created By:hsu
		    // Date:20151010
		    // Modify History:
//----------------------------------------------------------------*/
Shader "NCSpeedLight/UI/BackgroundBlurEffect" 
{
	Properties{
	    _OffsetXY("Offset XY", Range(-5, 5)) = 1
	    [MaterialToggle] _DrawRect ("Draw Rect", Float ) = 0
	    _RectCenterX("Rect Pos X",Range(0,1)) =0
	    _RectCenterY("Rect Pos Y",Range(0,1)) =0
	    _RectWidth("Rect Width",Range(0,1)) =0
	    _RectHeight("Rect Height",Range(0,1)) =0
	    _RectBlurPower("Rect Blur Power",Range(0.1,2))=0.5
	    //_Rect("Rect",Float2)={1,1}
	    
    }
	SubShader{
		// Draw ourselves after all opaque geometry
		Tags{ "Queue" = "Transparent" }
		// Grab the screen behind the object into _GrabTexture
		GrabPass{}
		// Render the object with the texture generated above
		Pass{

	        CGPROGRAM
            #pragma debug
            #pragma vertex vert
            #pragma fragment frag 
            #pragma target 3.0

			sampler2D _GrabTexture : register(s0);
			float _OffsetXY;
			float _DrawRect;
			float _RectCenterX;
			float _RectCenterY;
			float _RectWidth;
			float _RectHeight;
			float _RectBlurPower;
			struct data {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 position : POSITION;
				float4 screenPos : TEXCOORD0;
			};

			v2f vert(data i){
				v2f o;
				o.position = mul(UNITY_MATRIX_MVP, i.vertex);
				o.screenPos = o.position;
				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				float2 screenPos = i.screenPos.xy  / i.screenPos.w ;
				float depth = _OffsetXY*0.0005;
				screenPos.x = (screenPos.x + 1) * 0.5;
#if UNITY_UV_STARTS_AT_TOP
                //D3D
				screenPos.y =  1-(screenPos.y + 1) * 0.5;
#else
                //OpenGL
				screenPos.y =  (screenPos.y + 1) * 0.5;
#endif	
				half4 sum = half4(0.0h, 0.0h, 0.0h, 0.0h);
				sum += tex2D(_GrabTexture, float2(screenPos.x - 5.0 * depth, screenPos.y + 5.0 * depth)) * 0.025;
				sum += tex2D(_GrabTexture, float2(screenPos.x + 5.0 * depth, screenPos.y - 5.0 * depth)) * 0.025;

				sum += tex2D(_GrabTexture, float2(screenPos.x - 4.0 * depth, screenPos.y + 4.0 * depth)) * 0.05;
				sum += tex2D(_GrabTexture, float2(screenPos.x + 4.0 * depth, screenPos.y - 4.0 * depth)) * 0.05;

				sum += tex2D(_GrabTexture, float2(screenPos.x - 3.0 * depth, screenPos.y + 3.0 * depth)) * 0.09;
				sum += tex2D(_GrabTexture, float2(screenPos.x + 3.0 * depth, screenPos.y - 3.0 * depth)) * 0.09;

				sum += tex2D(_GrabTexture, float2(screenPos.x - 2.0 * depth, screenPos.y + 2.0 * depth)) * 0.12;
				sum += tex2D(_GrabTexture, float2(screenPos.x + 2.0 * depth, screenPos.y - 2.0 * depth)) * 0.12;

				sum += tex2D(_GrabTexture, float2(screenPos.x - 1.0 * depth, screenPos.y + 1.0 * depth)) *  0.15;
				sum += tex2D(_GrabTexture, float2(screenPos.x + 1.0 * depth, screenPos.y - 1.0 * depth)) *  0.15;

				sum += tex2D(_GrabTexture, screenPos - 5.0 * depth) * 0.025;
				sum += tex2D(_GrabTexture, screenPos - 4.0 * depth) * 0.05;
				sum += tex2D(_GrabTexture, screenPos - 3.0 * depth) * 0.09;
				sum += tex2D(_GrabTexture, screenPos - 2.0 * depth) * 0.12;
				sum += tex2D(_GrabTexture, screenPos - 1.0 * depth) * 0.15;
				sum += tex2D(_GrabTexture, screenPos) * 0.16;
				sum += tex2D(_GrabTexture, screenPos + 5.0 * depth) * 0.15;
				sum += tex2D(_GrabTexture, screenPos + 4.0 * depth) * 0.12;
				sum += tex2D(_GrabTexture, screenPos + 3.0 * depth) * 0.09;
				sum += tex2D(_GrabTexture, screenPos + 2.0 * depth) * 0.05;
				sum += tex2D(_GrabTexture, screenPos + 1.0 * depth) * 0.025;
				
				if(_DrawRect==1){
#if UNITY_UV_STARTS_AT_TOP

//					if(screenPos.x>(_RectCenterX-_RectWidth/2) && screenPos.x<(_RectCenterX+_RectWidth/2) 
//					&& (1-screenPos.y)>(_RectCenterY-_RectHeight/2) && (1-screenPos.y)<(_RectCenterY+_RectHeight/2))
//					{
//						sum*=_RectBlurPower;
//					}
				    if(screenPos.x>(_RectCenterX-_RectWidth/2) && screenPos.x<(_RectCenterX+_RectWidth/2) 
				    && screenPos.y>(_RectCenterY-_RectHeight/2) && screenPos.y<(_RectCenterY+_RectHeight/2)){
						sum*=_RectBlurPower;
					}
#else
					if(screenPos.x>(_RectCenterX-_RectWidth/2) && screenPos.x<(_RectCenterX+_RectWidth/2) 
				    && (1-screenPos.y)>(_RectCenterY-_RectHeight/2)&& (1-screenPos.y)<(_RectCenterY+_RectHeight/2)){
						sum*=_RectBlurPower;
					}
#endif
				}
				return sum / 2;
			}
			ENDCG
		}
	}
	Fallback Off
}