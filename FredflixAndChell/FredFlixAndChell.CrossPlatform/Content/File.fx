texture rainbow;
sampler s0;
sampler rainbow_sampler = sampler_state{Texture = (rainbow);};
float gameTime;

float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coords);
	coords.xy += (gameTime / 1000) % 1;
	float4 rainbow_color = tex2D(rainbow_sampler, coords);

	
	if (color.a > 0){
		return rainbow_color;
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
