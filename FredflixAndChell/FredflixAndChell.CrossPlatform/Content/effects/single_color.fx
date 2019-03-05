sampler s0;

float4 single_color;
int draw;

float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coords);
	float4 finalColor = float4( 1.0f, 1.0f, 1.0f, 0.0f );
	if(draw == 0) return finalColor;

	if(color.a > 0){
		finalColor[0] = single_color.r;
		finalColor[1] = single_color.g;
		finalColor[2] = single_color.b;
		finalColor[3] = min(single_color.a, color.a);
		return finalColor;
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
