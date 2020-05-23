float4x4 World;
float4x4 View;
float4x4 Projection;

texture MyTexture;
float3 offset;

sampler mySampler = sampler_state
{
	Texture = <MyTexture>;
};

struct VertexPositionColor
{
	float4 Position: POSITION;
	float4 Color: COLOR;
};

struct VertexPositionTexture
{
	float4 Position: POSITION;
	float2 TextureCoordinate: TEXCOORD;
};

VertexPositionTexture MyVertexShader(VertexPositionTexture input)
{
	//input.Position.xyz += offset;
	//return input;

	VertexPositionTexture output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View); 
	float4 projectionPosition = mul(viewPosition, Projection);
	output.Position = projectionPosition;
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

float4 MyPixelShader(VertexPositionTexture input) : COLOR
{

	return tex2D(mySampler, input.TextureCoordinate);
}

technique MyTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 MyVertexShader();
		PixelShader = compile ps_4_0 MyPixelShader();
	}
}