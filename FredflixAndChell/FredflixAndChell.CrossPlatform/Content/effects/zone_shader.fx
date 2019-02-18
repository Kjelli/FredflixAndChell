texture flash_texture;
sampler s0;
sampler flash_sampler = sampler_state{Texture = (zone_texture);};
float gameTime;
float flashRate;
float flashOffset;
float flashAmount;
float colorIntensity;
float2 scrollSpeed;
float4 zoneColor;

float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coords);
	coords.xy = coords.xy + ((gameTime / 2 * scrollSpeed.xy) % 1);
	float4 flash_color = tex2D(flash_sampler, coords);
	
	if (color.a > 0){
		float p =  (1 - flashOffset) * (sin(gameTime * flashRate) * 0.5 + 0.5) + flashOffset;
		p = p * flashAmount;
		flash_color.rgba = p * flash_color.rgba + (1-p) * color.rgba;
		color = flash_color;
	}
	
	color.rgb = (1 - colorIntensity) * color.rgb + colorIntensity * zoneColor.rgb;
	return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
