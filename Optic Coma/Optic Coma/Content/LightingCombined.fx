#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

struct VertexShaderInput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = input.Color;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return input.Color;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
float ambient;
float4 ambientColor;
float lightAmbient;

Texture ColorMap;
sampler ColorMapSampler = sampler_state {
	texture = <ColorMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

Texture ShadingMap;
sampler ShadingMapSampler = sampler_state {
	texture = <ShadingMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

float4 CombinedPixelShader(float4 color : COLOR0, float2 texCoords : TEXCOORD0) : COLOR0
{
	float4 color2 = tex2D(ColorMapSampler, texCoords);
	float4 shading = tex2D(ShadingMapSampler, texCoords);

	float4 finalColor = (color2 * ambientColor * ambient);

	finalColor += color2 * shading * lightAmbient;

	return finalColor;
}

technique DeferredCombined
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 CombinedPixelShader();
	}
}