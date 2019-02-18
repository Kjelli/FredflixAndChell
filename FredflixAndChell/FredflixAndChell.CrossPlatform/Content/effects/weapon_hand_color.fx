sampler s0;
float4 hand_color;
float4 hand_border_color;

float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coords);
	
	if(color.r == 1.0 && color.g == 0.0 && color.b == 0.0) {
		return hand_border_color;
	}
	if(color.r == 0.0 && color.g == 0.0 && color.b == 1.0) {
		return hand_color;
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
