#define PanelSpaceCoord float2
#define EFFECT_CLIP_V2F(idx1) PanelSpaceCoord posInPanel : TEXCOORD##idx1;

#define TRANSFER_EFFECT_CLIPINFO(o,panelWidth,panelHeight,panelCenterAndSharpness) \
	float2 clipSpace = o.vertex.xy / o.vertex.w; \
	o.posInPanel = (clipSpace.xy + 1) * 0.5;  \
	o.posInPanel.x -= panelCenterAndSharpness.x; \
	o.posInPanel.y -= panelCenterAndSharpness.y; \
	o.posInPanel.x *= (2 / panelWidth ); \
	o.posInPanel.y *= (2 / panelHeight ); \
  
#define APPLY_EFFECT_CLIP(i,col,panelCenterAndSharpness) \
	float2 factor = (float2(1.0, 1.0) - abs(i.posInPanel))*float2(panelCenterAndSharpness.z,panelCenterAndSharpness.w); \
	col.a *= clamp( min(factor.x, factor.y),0, 1.0); \
	
