#define KERNEL_SIZE 7

float4x4 World;
float4x4 View;
float4x4 Projection;

// TODO: add effect parameters here.
// R:
float BlurDistanceX;
float BlurDistanceY;
// added: TLV for the texture to be blurred
Texture2D t;
// R: 
float BlurKernel[KERNEL_SIZE];



// added: the sampler
sampler TextureSampler = sampler_state
{
    Texture = <t>;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	// Allocate an empty output struct
	VertexShaderOutput output = (VertexShaderOutput)0;

	// Do the matrix multiplications for perspective projection and the world transform
	float4 worldPosition = mul(input.Position, World);
    float4 viewPosition  = mul(worldPosition, View);
	output.Position      = mul(viewPosition, Projection);

	// added: pass on the texture coordinate for the cobblestone
	output.TexCoord = input.TexCoord;
	return output;
}

float4 HorizontalBlurFunction(VertexShaderOutput input) : COLOR0
{
	// define the output variable
	float4 color;
	color = float4(0, 0, 0, 0);

	// 
	[unroll] for (int i = 0; i < KERNEL_SIZE; i++)
	{
		float2 BlurLocation = input.TexCoord;
		BlurLocation.x += (i-((KERNEL_SIZE-1)/2))*BlurDistanceX;
		color += tex2D(TextureSampler, BlurLocation) * BlurKernel[i];
	}
	
	return color;
}

float4 VerticalBlurFunction(VertexShaderOutput input) : COLOR0
{
	// define the output variable
	float4 color;
	color = float4(0, 0, 0, 0);

	// 
	[unroll] for (int i = 0; i < KERNEL_SIZE; i++)
	{
		float2 BlurLocation = input.TexCoord;
		BlurLocation.y += (i-((KERNEL_SIZE-1)/2))*BlurDistanceY;
		color += tex2D(TextureSampler, BlurLocation) * BlurKernel[i];
	}
	
	return color;
}
technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 HorizontalBlurFunction();
    }
}
technique Technique2
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 VerticalBlurFunction();
    }
}

