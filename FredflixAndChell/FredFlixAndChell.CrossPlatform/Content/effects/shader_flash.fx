﻿texture flash_texture;
sampler s0;
sampler flash_sampler = sampler_state{Texture = (flash_texture);};
float gameTime;
float2 scrollSpeed;
float flashRate;
float flashOffset;
float4 replace_color;

float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coords);
	coords.xy = coords.xy + ((gameTime / 2 * scrollSpeed.xy) % 1);
	float4 flash_color = tex2D(flash_sampler, coords);
	
	if(color.r == 1.0f && color.g == 1.0f && color.g == 1.0f && color.a == 1.0f){
            	return replace_color;
        }

	if (color.a > 0 && color.r == 0 && color.g == 0 && color.b == 0){
		float p =  (1-flashOffset) * (sin(gameTime * flashRate) * 0.5 + 0.5) + flashOffset;
		flash_color.rgba = p * flash_color.rgba + (1-p) * color.rgba;
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
