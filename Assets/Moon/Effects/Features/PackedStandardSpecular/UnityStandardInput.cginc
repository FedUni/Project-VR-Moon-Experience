#ifndef UNITY_STANDARD_INPUT_INCLUDED
#define UNITY_STANDARD_INPUT_INCLUDED

#include "UnityCG.cginc"
#include "UnityShaderVariables.cginc"
#include "UnityStandardConfig.cginc"
#include "UnityPBSLighting.cginc" // TBD: remove
#include "UnityStandardUtils.cginc"

//---------------------------------------
// Directional lightmaps & Parallax require tangent space too
#if (_NORMALMAP || !DIRLIGHTMAP_OFF || _PARALLAXMAP)
	#define _TANGENT_TO_WORLD 1 
#endif

//---------------------------------------
half4			_Color;
half			_Cutoff;

Texture2D		_MainTex;
SamplerState	sampler_MainTex;
float4			_MainTex_ST;

Texture2D		_DetailAlbedoMap;
float4			_DetailAlbedoMap_ST;

Texture2D		_BumpMap;
float4			_BumpMap_ST;
half			_BumpScale;

Texture2D		_DetailMask;
Texture2D		_DetailNormalMap;
half			_DetailNormalMapScale;

Texture2D		_SpecGlossMap;
half			_Glossiness;

Texture2D		_OcclusionMap;
half			_OcclusionStrength;

sampler2D		_ParallaxMap;
half			_Parallax;
half			_UVSec;

half4 			_EmissionColor;
sampler2D		_EmissionMap;

Texture2D		_PackedBaseMap;
SamplerState	sampler_PackedBaseMap;
float			_DetailArrayMaxIndex;
float			_DetailAlbedoGrayscaleStrength;

float			_UseFullAlbedo;
float			_UseDitheredEdges;
float			_DebugDisableDetail;

Texture2D		_NoiseTexture;
SamplerState	sampler_NoiseTexture;
float			_NoiseUVScale;

Texture2DArray	_AlbedoArray;
Texture2DArray	_PackedArray;
Texture2DArray	_NormalArray;
SamplerState	sampler_AlbedoArray;
SamplerState	sampler_PackedArray;
SamplerState	sampler_NormalArray;
float4			_AlbedoScale[8];
float4			_PackedValues[8];
float4			_PackedValues2[8];

//-------------------------------------------------------------------------------------
// Input functions

struct VertexInput
{
	float4 vertex	: POSITION;
	half3 normal	: NORMAL;
	float2 uv0		: TEXCOORD0;
	float2 uv1		: TEXCOORD1;
#if defined(DYNAMICLIGHTMAP_ON) || defined(UNITY_PASS_META)
	float2 uv2		: TEXCOORD2;
#endif
#ifdef _TANGENT_TO_WORLD
	half4 tangent	: TANGENT;
#endif
};

struct SourceDataSet {
	float3	albedo;
#ifdef _NORMALMAP
	float3	normal;
#endif
	float	occlusion;
	float3	specular;
	float	smoothness;
	float	displacement;
};

float4 TexCoords(VertexInput v)
{
	float4 texcoord;
	texcoord.xy = TRANSFORM_TEX(v.uv0, _MainTex); // Always source from uv0
	texcoord.zw = TRANSFORM_TEX(((_UVSec == 0) ? v.uv0 : v.uv1), _BumpMap);
	return texcoord;
}

float3 BlendNormalsFactor(float3 n1, float3 n2, float factor) {
	return normalize(float3(n1.xy + n2.xy * factor, n1.z * LerpOneTo(n2.z, factor)));
}

SourceDataSet ComposeSourceDataSet_Material(
	const float4 texcoords,
	const float3 posWorld,
	const float3 normalWorld
) {
	SourceDataSet sds = (SourceDataSet)0;
	 
	sds.albedo = _MainTex.Sample(sampler_MainTex, texcoords.xy).rgb * _Color.rgb;
#ifdef _NORMALMAP
	sds.normal = UnpackScaleNormal(_BumpMap.Sample(sampler_MainTex, texcoords.xy), _BumpScale);
#endif
	const float4 packedBase = _PackedBaseMap.Sample(sampler_PackedBaseMap, texcoords.xy);
	sds.occlusion = LerpOneTo(packedBase.r, _OcclusionStrength);
	sds.specular = _SpecColor;
	sds.smoothness = min(1.f, packedBase.g * 2.f * _Glossiness);
	sds.displacement = packedBase.a;

	if(_DebugDisableDetail)
		return sds;

	float packedDithered = packedBase.b;
#if SUPPORTS_GATHERBLUE
	if(_UseDitheredEdges) {
		float2 worldNoise = _NoiseTexture.Sample(sampler_NoiseTexture, texcoords.xy * _NoiseUVScale).rg;
		worldNoise += _NoiseTexture.Sample(sampler_NoiseTexture, texcoords.xy * _NoiseUVScale * 5.f).rg;
		const float4 packedNeighbours = _PackedBaseMap.GatherBlue(sampler_PackedBaseMap, texcoords.xy);

		float2 packedWidthHeight;
		_PackedBaseMap.GetDimensions(packedWidthHeight.x, packedWidthHeight.y);
		float2 filterFracs = frac(packedWidthHeight * texcoords.xy - 0.5 + 1.0 / 512.0);

		float packedX1 = frac(worldNoise.x) > frac(filterFracs.x) ? packedNeighbours.x : packedNeighbours.y;
		float packedX2 = frac(worldNoise.x) > frac(filterFracs.x) ? packedNeighbours.w : packedNeighbours.z;
		packedDithered = frac(worldNoise.y) > frac(filterFracs.y) ? packedX2 : packedX1;
	}
#endif

	const float detailFactor = 1.f;
	const float3 detailUV = float3(texcoords.zw, packedDithered * _DetailArrayMaxIndex + 0.25);

	float4 detailPacked = _PackedArray.Sample(sampler_PackedArray, detailUV);
	if(_UseFullAlbedo) {
		float3 detailAlbedo = _AlbedoArray.Sample(sampler_AlbedoArray, detailUV).rgb;
		sds.albedo *= LerpWhiteTo(detailAlbedo * unity_ColorSpaceDouble.rgb, detailFactor);
	} else {
		sds.albedo *= saturate((detailPacked.a - 0.5) * _DetailAlbedoGrayscaleStrength + 0.5) * 2;
	}
	sds.occlusion = min(sds.occlusion, LerpOneTo(detailPacked.r, _OcclusionStrength * detailFactor));
	sds.smoothness = sds.smoothness * LerpOneTo(detailPacked.g * 2.f, detailFactor);

#ifdef _NORMALMAP
	float3 detailNormal = UnpackScaleNormal(_NormalArray.Sample(sampler_NormalArray, detailUV), 1);
	sds.normal = BlendNormalsFactor(sds.normal, detailNormal, detailFactor);
#endif

	return sds;
}


half3 Emission(float2 uv)
{
#ifndef _EMISSION
	return 0;
#else
	return tex2D(_EmissionMap, uv).rgb * _EmissionColor.rgb;
#endif
}

float4 Parallax (float4 texcoords, half3 viewDir)
{
#if !defined(_PARALLAXMAP) || (SHADER_TARGET < 30)
	// SM20: instruction count limitation
	// SM20: no parallax
	return texcoords;
#else
	half h = tex2D (_ParallaxMap, texcoords.xy).g;
	float2 offset = ParallaxOffset1Step (h, _Parallax, viewDir);
	return float4(texcoords.xy + offset, texcoords.zw + offset);
#endif
}
			
#endif // UNITY_STANDARD_INPUT_INCLUDED
