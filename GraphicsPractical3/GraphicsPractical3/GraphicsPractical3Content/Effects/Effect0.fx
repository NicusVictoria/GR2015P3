#define MAX_LIGHTS 5

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 InverseTransposeWorld;
float4 LightPositions[MAX_LIGHTS];
float4 LightColors[MAX_LIGHTS];

// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position3D : POSITION0;
	float4 Normal3D : NORMAL0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 Normal3D : TEXCOORD0;
	float4 WorldPosition : TEXCOORD1;
	float4 PixelPosition : TEXCOORD2;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

// R: added functions
// R: LambertianColor() calculates the Lambertian shading of each vertex
float4 LambertianColor(float4 normal, float3 location,  float3 lightPosition, float4 lightColor, float4 pixelPosition)
{
	// define the output variable
	float4 color;

	// define the direction of the light
	float3 lightDirection = normalize(location-lightPosition);	

	// extract the rotation+scale matrix from the World matrix
	float3x3 rotateAndScale = (float3x3) InverseTransposeWorld;
	
	// rotate and scale the normal according to world transformations
	float3 rotatedNormal = mul(normal.xyz, rotateAndScale);

	// inverse and normalize the normal
	float3 inversedNormal = normalize(mul(-1, rotatedNormal));

	// calculate the dot product between the light direction and the inversed normal
	// to determine the light intensity
	float dotProduct = dot(inversedNormal, lightDirection);

	// BlinnPhong
	// added:  Define all undefined variables of the formula:
	float3 cameraPosition = {0.0f, 50.0f, 100.0f};
	float3 v = normalize(pixelPosition.xyz-cameraPosition.xyz);

	// added: calculate h; the bisector of the angle between the direction of the light and the viewingdirection
	float3 h = normalize(v+lightDirection);
	
	// calculate the final coloadded: DiffuseColor * DiffuseIntensity + AmbientLight
	color.xyz = lightColor * max(0.0f, dotProduct) + lightColor * 25.0f * pow(max(0.0f, dot(inversedNormal, h)), 1000);
	// set alpha to zero
	color.w = 0.0f;
	
	// return the color
	return color;
}


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position3D, World);
    float4 viewPosition = mul(worldPosition, View);
    float4 PixelPosition = mul(viewPosition, Projection);
	output.Position = PixelPosition;
	
	
	output.Normal3D = input.Normal3D;
	output.WorldPosition = worldPosition;
	output.PixelPosition = PixelPosition;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = float4(0,0,0,0);
	[unroll] for (uint i = 0; i < MAX_LIGHTS; i++)
	{
		float4 lightWorldPosition = mul(LightPositions[i], World);
		color += LambertianColor(input.Normal3D, input.WorldPosition.xyz, lightWorldPosition.xyz, LightColors[i], input.PixelPosition);
	}
	return color;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.
		// R: updated to Shader Model 3
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}


