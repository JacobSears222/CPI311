float4x4 MatrixTransform;
texture2D modelTexture;
texture2D filterTexture;
float imageWidth;
float imageHeight;
float offset = 0;

float3x3 RGB2YCbCr = 
{
	{ 0.2989f, 0.5866f, 0.1145f },
	{-0.1687f, -0.3312f, 0.5000f },
	{ 0.5000f, -0.4183f, -0.0816f } 
};

sampler TextureSampler: register(s0) = sampler_state
{
	Texture = <modelTexture>;
	AddressU = CLAMP;
	AddressV = CLAMP;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MAgFilter = LINEAR;
};

sampler FilterSampler: register(s1) = sampler_state
{
	Texture = <filterTexture>;
	AddressU = WRAP;
};


struct VS_OUTPUT
{
	float4 Pos: POSITION;
	float2 UV0: TEXCOORD0;
	float4 UV1: TEXCOORD1;
};

VS_OUTPUT vtxSh(float4 inPos : POSITION, float2 inTex : TEXCOORD0)
{
	VS_OUTPUT output;
	output.Pos = mul(inPos, MatrixTransform);
	output.UV0 = inTex;
	output.UV1 = float4(3/imageWidth, 0, 0, 3/imageHeight);
	return output;
}

float4 pxlSh(VS_OUTPUT input) : Color
{
	float4 tex = tex2D(TextureSampler, input.UV0);
	//tex.rgb = ceil(tex * 8) / 8; EX1
	
	tex.r = tex2D(FilterSampler, float2(tex.r + offset, 0)).r;
	tex.b = tex2D(FilterSampler, float2(tex.g + offset, 0)).g;
	tex.g = tex2D(FilterSampler, float2(tex.b + offset, 0)).b;
	
	//float3 ycbr = mul(RGB2YCbCr, tex.rgb); EX3 Black and White
	//tex.rgb = ycbr.r;

	//float4 tex1 = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	//float4 tex2 = tex2D(TextureSampler, input.UV0 - input.UV1.xy);
	//float4 tex3 = tex2D(TextureSampler, input.UV0 + input.UV1.zw);
	//float4 tex4 = tex2D(TextureSampler, input.UV0 - input.UV1.zw);
	////tex = (tex + tex1 + tex2 + tex3 + tex4) / 5;
	//tex = tex * 4 - (tex1 + tex2 + tex3 + tex4);
	tex.a = 1;
	return tex;
}

technique MyShader
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlSh();
	}
}