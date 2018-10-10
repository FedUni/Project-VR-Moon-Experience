#ifndef TETERRAIN_INPUTS_CGINC
#define TETERRAIN_INPUTS_CGINC

uniform Texture2D/*<unorm float>*/	u_Heightmap;
uniform SamplerState			sampleru_Heightmap;
uniform float4					u_Heightmap_ST;
uniform float4					u_Heightmap_TexelSize;

uniform Texture2D/*<unorm float>*/	u_HeightmapRef;
uniform SamplerState			sampleru_HeightmapRef;
uniform float4					u_HeightmapRef_ST;


uniform sampler2D			u_NoiseTexture;
uniform sampler2D			u_SoftNoiseTexture;

uniform Texture2D<float>	u_Controlmap;
uniform SamplerState		sampleru_Controlmap;
uniform float4				u_Controlmap_TexelSize;
uniform Texture2D<float4>	u_Colormap;
uniform SamplerState		sampleru_Colormap;

uniform Texture2DArray		u_TextureSetsAlbedo;
uniform Texture2DArray		u_TextureSetsSpecSmooth;
uniform Texture2DArray		u_TextureSetsOcclusion;
uniform Texture2DArray		u_TextureSetsNormal;
uniform Texture2DArray		u_TextureSetsDisplacement;
SamplerState				sampleru_TextureSetsAlbedo;
SamplerState				sampleru_TextureSetsSpecSmooth;
SamplerState				sampleru_TextureSetsOcclusion;
SamplerState				sampleru_TextureSetsNormal;
SamplerState				sampleru_TextureSetsDisplacement;

uniform float3				u_DetailLayersAlbedoTint[16];
uniform float3				u_DetailLayersSpecularTint[16];
uniform float4				u_DetailLayersSmtNrmDspOccScales[16];
uniform float4				u_DetailLayersDisplacementScaleOffset[16];
uniform	float4				u_DetailLayersUVScaleArrayMultiIndex[16];
uniform	float4				u_DetailLayersSlopeThresholds[16];

sampler2D	_WaterMask;
float		_WaterMaskHeightScale;
float		_WaterMaskHeightOffset;
float4		_WaterMask_ST;
Texture2D	_WaterBumpMap;
float		_WaterBumpUVScale;
float		_WaterBumpScale;
float		_WaterLevel;
float		_WaterLevelEaseRange;
float		_WaterClarity;
float3		_WaterSpecColor;
float3		_WaterAlbedoDarken;
float		_WaterGlossiness;
float		_WetnessRange;
float3		_WetnessSpecColor;
float		_WetnessGlossiness;
float3		_WetnessAlbedoDarken;
float		_WetnessFogDarkenRange;
float		_WetnessFogAddRange;
float3		_WetnessFogColor;


#endif//TETERRAIN_INPUTS_CGINC