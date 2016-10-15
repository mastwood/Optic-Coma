#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float ambient;
float4 ambientColor;
float lightAmbient;

Texture ColorMap;
sampler ColorMapSampler = sampler_state
{
	texture = <ColorMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

Texture ShadingMap;
sampler ShadingMapSampler = sampler_state
{
	texture = <ShadingMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

Texture NormalMap;
sampler NormalMapSampler = sampler_state
{
	texture = <NormalMap>;
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
	float normal = tex2D(NormalMapSampler, texCoords).rgb;
		
	if (normal > 0.0f)
	{
		//float4 finalColor = color2 * ambientColor;
		//finalColor += shading;
		//return finalColor;

		// Darker
		float4 finalColor = color2 * ambientColor * ambient;
		finalColor += (shading * color2) * lightAmbient;
		
		return finalColor;
	}
	else
	{
		return float4(0, 0, 0, 0);
	}
}

technique DeferredCombined2
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 CombinedPixelShader();
	}
}