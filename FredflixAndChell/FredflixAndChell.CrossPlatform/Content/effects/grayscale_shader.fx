texture flash_texture;
sampler s0;
float intensity;

float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coords);
	float4 grayscale = tex2D(s0, coords);
	float shadeOfGrey = (color.r + color.g + color.b) / 3;
	grayscale.r = shadeOfGrey;
	grayscale.g = shadeOfGrey;
	grayscale.b = shadeOfGrey;
	
	color = (1 - intensity) * color + intensity * grayscale;
	
	return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
