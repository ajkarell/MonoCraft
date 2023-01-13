#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0

float4x4 World;
float4x4 WorldInverseTranspose;
float4x4 View;
float4x4 Projection;

Texture2DArray TextureArray;

sampler textureSampler = sampler_state
{
	Texture = <Texture>;
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float3 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 FragPosition : SV_POSITION;
	float3 TexCoord : TEXCOORD0;
	float3 Normal : NORMAL0;
	float Alpha : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.FragPosition = mul(viewPosition, Projection);
 
	output.Normal = normalize(input.Normal);

	output.TexCoord = input.TexCoord;
	output.Alpha = input.Normal.w;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 textureColor = TextureArray.Sample(textureSampler, input.TexCoord);
	textureColor.a = input.Alpha;

	return textureColor;
}

technique Main
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};