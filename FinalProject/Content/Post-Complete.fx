float4x4 MatrixTransform;
texture2D modelTexture;
texture2D filterTexture;
float imageWidth;
float imageHeight;
float colorRedValue;
float colorGreenValue;
float colorBlueValue;
float offset = 0;


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
	output.UV1 = float4(3 / imageWidth, 0, 0, 3 / imageHeight);
	return output;
}

float4 pxlShRed(VS_OUTPUT input) : COLOR
{
	float4 colorin, colorout;
	colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	colorout = colorin;

	if ((colorin.g + colorin.b) < colorin.r)
	{
		colorout.r = colorin.r;
		colorout.g = 0;
		colorout.b = 0;
	}
	else
	{
		colorout.rgb = dot(colorin.rgb, colorin.rgb) * dot(colorin.rgb, float3(2, 1, 2));
	}

return colorout;
}

float4 pxlShGreen(VS_OUTPUT input) : COLOR
{
	float4 colorin, colorout;
	colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	colorout = colorin;

	if ((colorin.r + colorin.b) < colorin.g)
	{
		colorout.r = 0;
		colorout.g = colorin.g;
		colorout.b = 0;
	}
	else
	{
		colorout.rgb = dot(colorin.rgb, colorin.rgb) * dot(colorin.rgb, float3(2, 1, 2));
	}

return colorout;
}

float4 pxlShBlue(VS_OUTPUT input) : COLOR
{
	float4 colorin, colorout;
	colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	colorout = colorin;

	if ((colorin.r + colorin.g) < colorin.b)
	{
		colorout.r = 0;
		colorout.g = 0;
		colorout.b = colorin.b;
	}
	else
	{
		colorout.rgb = dot(colorin.rgb, colorin.rgb) * dot(colorin.rgb, float3(2, 1, 2));
	}

return colorout;
}

float4 pxlShNormal(VS_OUTPUT input) : COLOR
{
	float4 colorin;
	float4 colorout;
	colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	colorout = colorin;

	return colorout;
}

float4 pxlShNoRGBetter(VS_OUTPUT input) : COLOR
{
	float4 colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	float4 colorout;
	colorout = colorin;
	colorout.r = tex2D(TextureSampler, colorin.r).r;
	colorout.g = tex2D(TextureSampler, colorin.g).g;
	if ((colorin.r + colorin.g) > colorin.b)
	{
		colorout.r = colorin.r;
		colorout.g = colorin.g;
		colorout.b = 0;
	}
	else
	{
		colorout.rgb = dot(colorin.rgb, colorin.rgb) * dot(colorin.rgb, float3(2, 1, 2));
	}
	return colorout;
}

float4 pxlShNoGBBetter(VS_OUTPUT input) : COLOR
{
	float4 colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	float4 colorout;
	colorout = colorin;
	colorout.g = tex2D(TextureSampler, colorin.g).g;
	colorout.b = tex2D(TextureSampler, colorin.b).b;
	if ((colorin.g + colorin.b) > colorin.r)
	{
		colorout.r = 0;
		colorout.g = colorin.g;
		colorout.b = colorin.b;
	}
	else
	{
		colorout.rgb = dot(colorin.rgb, colorin.rgb) * dot(colorin.rgb, float3(2, 1, 2));
	}
	return colorout;
}

float4 pxlShNoRBBetter(VS_OUTPUT input) : COLOR
{
	float4 colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	float4 colorout;
	colorout = colorin;
	colorout.r = tex2D(TextureSampler, colorin.r).r;
	colorout.b = tex2D(TextureSampler, colorin.b).b;
	if ((colorin.r + colorin.b) > colorin.g)
	{
		colorout.r = colorin.r;
		colorout.g = 0;
		colorout.b = colorin.b;
	}
	else
	{
		colorout.rgb = dot(colorin.rgb, colorin.rgb) * dot(colorin.rgb, float3(2, 1, 2));
	}
	return colorout;
}

float4 pxlShRR(VS_OUTPUT input) : COLOR
{
	float4 colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	float4 colorout;
	colorout = colorin;
	colorout.r = tex2D(TextureSampler, colorin.r).r;
	colorout.b = tex2D(TextureSampler, colorin.b).b;
	if ((colorin.r + colorin.b + colorin.g))
	{
		colorout.r = (1 - colorin.r);
		colorout.g = (colorin.g);
		colorout.b = (colorin.b);
	}
	else
	{
		colorout.rgb = dot(colorin.rgb, colorin.rgb) * dot(colorin.rgb, float3(2, 2, 2));
	}
	return colorout;
}

float4 pxlShRG(VS_OUTPUT input) : COLOR
{
	float4 colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	float4 colorout;
	colorout = colorin;
	colorout.r = tex2D(TextureSampler, colorin.r).r;
	colorout.b = tex2D(TextureSampler, colorin.b).b;
	if ((colorin.r + colorin.b + colorin.g))
	{
		colorout.r = (colorin.r);
		colorout.g = (1 - colorin.g);
		colorout.b = (colorin.b);
	}
	else
	{
		colorout.rgb = dot(colorin.rgb, colorin.rgb) * dot(colorin.rgb, float3(2, 2, 2));
	}
	return colorout;
}

float4 pxlShRB(VS_OUTPUT input) : COLOR
{
	float4 colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	float4 colorout;
	colorout = colorin;
	colorout.r = tex2D(TextureSampler, colorin.r).r;
	colorout.b = tex2D(TextureSampler, colorin.b).b;
	if ((colorin.r + colorin.b + colorin.g))
	{
		colorout.r = (colorin.r);
		colorout.g = (colorin.g);
		colorout.b = (1 - colorin.b);
	}
	else
	{
		colorout.rgb = dot(colorin.rgb, colorin.rgb) * dot(colorin.rgb, float3(2, 2, 2));
	}
	return colorout;
}

float4 pxlShNoGB(VS_OUTPUT input) : COLOR
{
	float4 colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	float4 colorout;
	colorout = colorin;
	colorout.g = tex2D(TextureSampler, colorin.g).g;
	colorout.b = tex2D(TextureSampler, colorin.b).b;
	return colorout;
}

float4 pxlShNoRB(VS_OUTPUT input) : COLOR
{
	float4 colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	float4 colorout;
	colorout = colorin;
	colorout.r = tex2D(TextureSampler, colorin.r).r;
	colorout.b = tex2D(TextureSampler, colorin.b).b;
	return colorout;
}

float4 pxlShNoRG(VS_OUTPUT input) : COLOR
{
	float4 colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	float4 colorout;
	colorout = colorin;
	colorout.r = tex2D(TextureSampler, colorin.r).r;
	colorout.g = tex2D(TextureSampler, colorin.g).g;
	return colorout;
}

float4 pxlShNoRGB(VS_OUTPUT input) : COLOR
{
	float4 colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	float4 colorout;
	colorout = colorin;
	colorout.r = tex2D(TextureSampler, colorin.r).r;
	colorout.g = tex2D(TextureSampler, colorin.g).g;
	colorout.b = tex2D(TextureSampler, colorin.b).b;
	return colorout;
}

float4 pxlShFilter(VS_OUTPUT input) : Color
{
	float4 tex = tex2D(TextureSampler, input.UV0);

	tex.r = tex2D(FilterSampler, float2(tex.r + offset, 0)).r;
	tex.g = tex2D(FilterSampler, float2(tex.b + offset, 0)).g;
	tex.b = tex2D(FilterSampler, float2(tex.g + offset, 0)).b;
	
	tex.a = 1;
	return tex;
}

float4 pxlRF(VS_OUTPUT input) : Color
{
	float4 colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	float4 colorout;
	colorout = colorin;
	float4 tex = tex2D(TextureSampler, input.UV0);

	tex.r = tex2D(FilterSampler, colorin.r + offset).r;
	tex.g = dot(colorin.rgb, colorin.rgb) * dot(colorin.rgb, float3(2, 1, 2));
	tex.b = dot(colorin.rgb, colorin.rgb) * dot(colorin.rgb, float3(2, 1, 2));
	
	tex.a = 1;
	return tex;
}

float4 pxlGF(VS_OUTPUT input) : Color
{
	float4 colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	float4 colorout;
	colorout = colorin;
	float4 tex = tex2D(TextureSampler, input.UV0);

	tex.r = dot(colorin.rgb, colorin.rgb) * dot(colorin.rgb, float3(2, 1, 2));
	tex.g = tex2D(FilterSampler, colorin.r + offset).r;
	tex.b = dot(colorin.rgb, colorin.rgb) * dot(colorin.rgb, float3(2, 1, 2));
	
	tex.a = 1;
	return tex;
}

float4 pxlBF(VS_OUTPUT input) : Color
{
	float4 colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	float4 colorout;
	colorout = colorin;
	float4 tex = tex2D(TextureSampler, input.UV0);

	tex.r = dot(colorin.rgb, colorin.rgb) * dot(colorin.rgb, float3(2, 1, 2));
	
	tex.g = dot(colorin.rgb, colorin.rgb) * dot(colorin.rgb, float3(2, 1, 2));
	tex.b = tex2D(FilterSampler, colorin.r + offset).r;
	tex.a = 1;
	return tex;
}

float4 pxlYoshiRed(VS_OUTPUT input) : Color
{
	float4 colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	float4 colorout;
	colorout = colorin;
	float4 tex = tex2D(TextureSampler, input.UV0);

	if ((colorin.g + colorin.b) < colorin.r)
	{
		tex.r = tex2D(FilterSampler, colorin.r + offset).r;
		tex.g = 0;
		tex.b = 0;
	}
	else
	{
		colorout.rgb = dot(colorin.rgb, offset) * dot(colorin.rgb, float3(2, 1, 2));
	}

	return colorout;
}

float4 pxlYoshiGreen(VS_OUTPUT input) : Color
{
	float4 colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	float4 colorout;
	colorout = colorin;
	float4 tex = tex2D(TextureSampler, input.UV0);

	if ((colorin.r + colorin.b) < colorin.g)
	{
		tex.r = 0;
		tex.g = tex2D(FilterSampler, colorin.g + offset).g;
		tex.b = 0;
	}
	else
	{
		colorout.rgb = dot(colorin.rgb, offset) * dot(colorin.rgb, float3(2, 1, 2));
	}

	return colorout;
}

float4 pxlYoshiBlue(VS_OUTPUT input) : Color
{
	float4 colorin = tex2D(TextureSampler, input.UV0 + input.UV1.xy);
	float4 colorout;
	colorout = colorin;
	float4 tex = tex2D(TextureSampler, input.UV0);

	if (((colorin.r/2) + colorin.g) < colorin.b)
	{
		tex.r = 0;
		tex.g = 0;
		tex.b = tex2D(FilterSampler, colorin.b + offset).b;
	}
	else
	{
		colorout.rgb = dot(colorin.rgb, offset) * dot(colorin.rgb, float3(2, 1, 2));
	}

	return colorout;
}

technique MyShader1
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlShRed();
	}
}

technique MyShader2
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlShGreen();
	}
}

technique MyShader3
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlShBlue();
	}
}

technique MyShader4
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlShNormal();
	}
}

technique MyShader5
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlShNoGBBetter();
	}
}

technique MyShader6
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlShNoRBBetter();
	}
}

technique MyShader7
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlShNoRGBetter();
	}
}

technique MyShader8
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlShNoGB();
	}
}

technique MyShader9
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlShNoRB();
	}
}

technique MyShader10
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlShNoRG();
	}
}

technique MyShader11
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlShNoRGB();
	}
}

technique MyShader12
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlShRR();
	}
}

technique MyShader13
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlShRG();
	}
}

technique MyShader14
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlShRB();
	}
}

technique MyShader15
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlShFilter();
	}
}

technique MyShader16
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlRF();
	}
}

technique MyShader17
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlGF();
	}
}

technique MyShader18
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlBF();
	}
}

technique MyShader19
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlYoshiRed();
	}
}

technique MyShader20
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlYoshiGreen();
	}
}

technique MyShader21
{
	pass P0
	{
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlYoshiBlue();
	}
}