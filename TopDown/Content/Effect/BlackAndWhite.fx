sampler TextureSampler : register(s0);
Texture2D  myTex2D;
float DarknessLevel;

cbuffer cbPerLight : register(c1)
{	
	float4x4 WorldViewProjection;
	float3 LightColor;
	float LightIntensity;
};

struct VertexIn
{
	float2 Position : SV_POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexOut
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
};

VertexOut VS(VertexIn vin)
{
	VertexOut vout;
	
	vout.Position = mul(float4(vin.Position.x, vin.Position.y, 0.0, 1.0), WorldViewProjection);	
	vout.TexCoord = vin.TexCoord;

	return vout;
}

float4 GetComputedColor(float alpha)
{
	alpha = abs(alpha);
	float3 lightColor = LightColor * alpha;
	lightColor = pow(abs(lightColor), LightIntensity);
	return float4(lightColor, 0.8);
}


float4 PSPointLight(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{	
    float4 tex;
	tex = myTex2D.Sample(TextureSampler, texCoord.xy);
	

	float halfMagnitude = length(texCoord - float2(0.5,0.5));
	float alpha = saturate(1.0 - halfMagnitude*2.0);
	return tex;
}


/*
float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    float4 tex;
	tex = myTex2D.Sample(TextureSampler, texCoord.xy);
	//tex += myTex2D.Sample(TextureSampler, texCoord.xy + (0.005)) * .2f;

	return tex*(color1/DarknessLevel);
}*/


technique Technique1
{
    pass Pass1
    {
		//VertexShader = compile vs_4_0_level_9_3 VS();
        PixelShader = compile ps_4_0_level_9_3 PSPointLight();  
    }
}