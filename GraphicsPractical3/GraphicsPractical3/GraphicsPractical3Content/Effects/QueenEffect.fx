float4x4 World;
float4x4 View;
float4x4 Projection;

// TODO: add effect parameters here.

Texture2D QueenTexture;

// added: the sampler
sampler QueenTextureSampler = sampler_state
{
	Texture = <QueenTexture>;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};


struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 texturecoordinates :TEXCOORD0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 texturecoordinates : TEXCOORD0;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // TODO: add your vertex shader code here.

	output.texturecoordinates = input.texturecoordinates;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.

	float4 color;
	color = tex2D(QueenTextureSampler, input.texturecoordinates);
	//Y = 0.3R + 0.59G + 0.11B.
	float y = 0.3*color.x + 0.59*color.y + 0.11*color.z;
	color.xyz = (y, y, y);
	color.w = 0.0f;
    return color;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
