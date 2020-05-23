float4x4  World;
float4x4  View;
float4x4  Projection;
float4x4 WorldInverseTranspose;

float4 AmbientColor;
float AmbientIntensity;
float3 DiffuseLightDirection;
float4 DiffuseColor;
float DiffuseIntensity;

float3 CameraPosition;
float3 LightPosition;

float Shininess;
float4 SpecularColor;
float SpecularIntensity = 1;

struct VertexInput 
{
	float4 Position: POSITION;
	float4 Normal: NORMAL;
};

struct GouraudVertexOutput 
{
	float4 Position: POSITION;
	float4 Color: COLOR;
	//float4 Normal: TEXCOORD0;
	//float4 WorldPosition: TEXCOORD1;
};

struct PhongVertexOutput 
{
	float4 Position: POSITION;
	float4 Color: COLOR;
	float4 Normal: TEXCOORD0;
	float4 WorldPosition: TEXCOORD1;
};

struct ToonVertexOutput 
{
	float4 Position: POSITION;
	float4 Color: COLOR;
	float4 Normal: TEXCOORD0;
	float4 WorldPosition: TEXCOORD1;
};


//VertexShaders
GouraudVertexOutput GouraudVertex(VertexInput input)
{
	GouraudVertexOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	//output.WorldPosition = 0;
	//output.Normal = 0; 

	float3 L = normalize(LightPosition);
	float3 V = normalize(CameraPosition - worldPosition.xyz);
	float3 N = mul(input.Normal, WorldInverseTranspose).xyz;
	float3 R = reflect(-L, N);

	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));

	//Setting output color using the result
	float4 specular = pow(max(0, dot(V, R)), Shininess) * SpecularColor * SpecularIntensity;
	output.Color = saturate(ambient + diffuse + specular);

	return output;
}

PhongVertexOutput PhongVertex(VertexInput input)
{
	PhongVertexOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPosition;
	output.Normal = input.Normal;

	return output;
}

ToonVertexOutput ToonVertex(VertexInput input)
{
	ToonVertexOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPosition;
	output.Normal = input.Normal;

	return output;
}

//PixelShaders
float4 GouraudPixel(GouraudVertexOutput input) : COLOR
{
	return input.Color;
}

float4 PhongPixel(PhongVertexOutput input) : COLOR0
{
	float3 L = normalize(LightPosition);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 N = mul(input.Normal, WorldInverseTranspose).xyz;
	float3 R = reflect(-L,N);
	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));

	//Setting output color using the result
	float4 specular = pow(max(0,dot(V, R)), Shininess) * SpecularColor * SpecularIntensity;
	float4 color = saturate(ambient + diffuse + specular);
	color.a = 1;
	return color;
}

//Optional Toon Shader
float4 ToonPixel(ToonVertexOutput input) : COLOR0
{
	float3 L = normalize(LightPosition);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 N = mul(input.Normal, WorldInverseTranspose).xyz;
	float3 R = reflect(-L,N);
	float D = dot(V, R);
	if (D < -0.7)
	{
		return float4(0, 0, 0, 1);
	}
	else if (D < 0.2)
	{
		return float4(0.25, 0.25, 0.25, 1);
	}
	else if (D < 0.97)
	{
		return float4(0.5, 0.5, 0.5, 1);
	}
	else
	{
		return float4(1, 1, 1, 1);
	}
}

//Passes for the different shaders
technique MyTechnique 
{
	pass Pass1 {
		VertexShader = compile vs_4_0 GouraudVertex();
		PixelShader = compile ps_4_0 GouraudPixel();
	}
}
technique MyTechnique 
{
	pass Pass1 {
		VertexShader = compile vs_4_0 PhongVertex();
		PixelShader = compile ps_4_0 PhongPixel();
	}
}

technique MyTechnique 
{
	pass Pass1 {
		VertexShader = compile vs_4_0 ToonVertex();
		PixelShader = compile ps_4_0 ToonPixel();
	}
}