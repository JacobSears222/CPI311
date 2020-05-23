float4x4 World; 
float4x4 View;
float4x4 Projection; 
float4x4 WorldInverseTranspose; 
float3 CameraPosition; 
texture decalMap;
Texture2D environmentMap;

sampler tsampler1 = sampler_state
{
	texture = <decalMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

samplerCUBE SkyBoxSampler = sampler_state 
{ 
	texture = <environmentMap>;       
	magfilter = LINEAR;     
	minfilter = LINEAR;     
	mipfilter = LINEAR;     
	AddressU = Mirror;    
	AddressV = Mirror; 
};

struct VertexShaderInput
{
	float4 Position: POSITION0;
	float2 TextureCoordinate: TEXCOORD0;	
	float4 Normal: NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position: POSITION0;
	float2 TextureCoordinate: TEXCOORD0;
	float3 Reflection : TEXCOORD1;
};

VertexShaderOutput ReflectionVS(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPos = mul(input.Position, World);
	float4 viewPos = mul(worldPos, View);
	float4 projPos = mul(viewPos, Projection);
	output.Position = projPos;

	float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	float3 I = normalize(worldPos.xyz - CameraPosition);
	output.Reflection = reflect(I, N);
	output.TextureCoordinate = input.TextureCoordinate;

	return output;
};

float4 ReflectionPS(VertexShaderOutput input) : COLOR0
{
	float4 reflectedColor = texCUBE(SkyBoxSampler, input.Reflection);
	float4 decalColor = tex2D(tsampler1, input.TextureCoordinate);
	return lerp(decalColor, reflectedColor, 0.5);
	//return reflectedColor;
}

technique Reflection 
{
	pass Pass1 
	{
		VertexShader = compile vs_4_0 ReflectionVS();
		PixelShader = compile ps_4_0 ReflectionPS();
	}
}
