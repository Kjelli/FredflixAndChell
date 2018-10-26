texture flash_texture;
sampler s0;
sampler flash_sampler = sampler_state{Texture = (flash_texture);};
float gameTime;
float2 scrollSpeed;
float flashRate;
float flashOffset;

float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coords);
	coords.xy -= (gameTime / 2000 * scrollSpeed.xy) % 1;
	float4 flash_color = tex2D(flash_sampler, coords + color.rgb);
	
	if (color.a > 0){
		float p =  (1-flashOffset) * (sin(gameTime / 1000 * flashRate) * 0.5 + 0.5) + flashOffset;
		flash_color.rgb = p * flash_color.rgb + (1-p) * color.rgb;
		return flash_color;
	}
	return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
