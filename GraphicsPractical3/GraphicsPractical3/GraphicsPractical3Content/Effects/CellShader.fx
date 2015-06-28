float4x4 World;
float4x4 View;
float4x4 Projection;

// N: top level variables for Cell Shading
float4 DiffuseColor;
float4x4 InverseTransposeWorld;

// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position : POSITION0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.

	float4 Normal3D : NORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	
    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.

	float4 Normal3D : TEXCOORD0;
};

// N: Cell Shading is implemented here
float4 CellShader(float4 normal)
{
	// N: define output variable
	float4 color;

	// N: define the direction of the light
	float3 lightDirection = normalize(float3(-1, -1, -1));

	// N: extract the rotation+scale matrix from the world matrix
	float3x3 rotateAndScale = (float3x3) InverseTransposeWorld;

	// N: rotate and scale the normal according to world transformations
	float3 rotatedNormal = mul(normal.xyz, rotateAndScale);

	// N: rotate and scale the normal according to world transformations
	float3 inversedNormal = normalize( -1.0f * rotatedNormal);

	// N: calculate the dot product between the light direction and the inversed normal to determine the light intensity
	float dotProduct = dot(inversedNormal, lightDirection);

	// N: 
	float f = 0;
	
	if (dotProduct > 0.25)
	{
		f = 0.33f;
	}
	
	if (dotProduct > 0.5)
	{
		f = 0.66f;
	}
	if (dotProduct > 0.75)
	{
		f = 1.0f;
	}
	

	color.xyz = DiffuseColor * f;

	color.w = 0.0f;



	return color;
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // TODO: add your vertex shader code here.
	output.Normal3D = input.Normal3D;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color;

	color =  CellShader(normalize(input.Normal3D));

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
