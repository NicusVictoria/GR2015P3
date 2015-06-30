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
// This function calculates the lambertian intensity and determines the color
float4 CellShader(float4 Normal3D)
{
	// N: define output variable
	float4 color;

	// N: define the direction of the light
	float3 lightDirection = normalize(float3(-1, -1, -1));

	// N: extract the rotation+scale matrix from the world matrix
	float3x3 rotateAndScale = (float3x3) InverseTransposeWorld;

	// N: rotate and scale the normal according to world transformations
	float3 rotatedNormal = mul(Normal3D.xyz, rotateAndScale);

	// N: rotate and scale the normal according to world transformations
	float3 inversedNormal = normalize( -1.0f * rotatedNormal);

	// N: calculate the dot product between the light direction and the inversed normal to determine the light intensity
	float dotProduct = dot(inversedNormal, lightDirection);

	// N: Determine the number and distribution of intensity cells
	float colorRange[4] = { 0.0f, 0.33f, 0.67f, 1.0f };
	
	// determine which cell the intensity for this pixel lays in
	int colorIndex = 0;
	if (dotProduct > 0.25)
	{
		colorIndex = 1;
	}
	if (dotProduct > 0.5)
	{
		colorIndex = 2;
	}
	if (dotProduct > 0.75)
	{
		colorIndex = 3;
	}

	/*
	// We tried implementing ddx and ddy blurring, but we could not get it to work properly. 
	float dx = abs(ddx(colorIndex));
	float dy = abs(ddy(colorIndex));

	int hBlendIndex;
	int vBlendIndex;
	if (dx <= 0.5f)
	{
		hBlendIndex = min(3, colorIndex + 1);
	}
	else
	{

		hBlendIndex = max(0, colorIndex - 1);
	}

	if (dy <= 0.5f)
	{
		vBlendIndex = min(3, colorIndex + 1);
	}
	else
	{

		vBlendIndex = max(0, colorIndex - 1);
	}
	float hBlendFactor = lerp(colorRange[colorIndex], colorRange[hBlendIndex], dx);
	float vBlendFactor = lerp(colorRange[colorIndex], colorRange[vBlendIndex], dy);
	float combinedBlendFactor = 0.5f * vBlendFactor + 0.5f * hBlendFactor;

	color = DiffuseColor * combinedBlendFactor;
	*/
	

	// calculate the final color from the material color and the cell intensity
	color = DiffuseColor * colorRange[colorIndex];

	return color;
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	// pass the normal to the pixelshader
	output.Normal3D = input.Normal3D;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color;

	// determine color via CellShader-function
	color =  CellShader(normalize(input.Normal3D));

	return color;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
