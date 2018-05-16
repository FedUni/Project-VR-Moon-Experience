Shader "Hidden/TerrainEvo/TerrainStatic" {
Properties {
	_WaterBumpMap("Water Normal Map", 2D) = "bump" {}
	_WaterBumpScale("Water Normal Scale", Float) = 1.0
	_WaterLevelEaseRange("WaterLevelEaseRange", Range(0, 1)) = 0.01
	_WaterClarity("WaterClarity", Range(0, 1)) = 1.0
	_WaterAlbedoDarken("WaterAlbedoDarken", Color) = (0.7, 0.7, 0.7)
	_WaterSpecColor("WaterSpecColor", Color) = (0.4, 0.4, 0.4)
	_WaterGlossiness("WaterSmoothness", Range(0.0, 1.0)) = 0.9
	_WetnessRange("WetnessRange", Range(0, 10)) = 0.01
	_WetnessSpecColor("WetnessSpecColor", Color) = (0.33, 0.33, 0.33)
	_WetnessGlossiness("WetnessSmoothness", Range(0.0, 1.0)) = 0.8
	_WetnessAlbedoDarken("WetnessAlbedoDarken", Color) = (0.8, 0.8, 0.8)
	_WetnessFogDarkenRange("WetnessFogDarkenRange", Range(0.001, 20)) = 2
	_WetnessFogAddRange("WetnessFogAddRange", Range(0.001, 20)) = 2
	_WetnessFogColor("WetnessFogColor", Color) = (0.66, 0.55, 0.33)

	u_Heightmap("u_Heightmap", 2D) = "black"  {}
	u_NoiseTexture("Noise", 2D) = "white" {}
	u_SoftNoiseTexture("SoftNoise", 2D) = "white" {}
	u_Controlmap("Control Map", 2D) = "white" {}
	u_Colormap("Color Map", 2D) = "white" {}

	u_TextureSetsAlbedo("u_TextureSetsAlbedo", 2DArray) = "white" {}
	u_TextureSetsSpecSmooth("u_TextureSetsSpecSmooth", 2DArray) = "white" {}
	u_TextureSetsOcclusion("u_TextureSetsOcclusion", 2DArray) = "white" {}
	u_TextureSetsNormal("u_TextureSetsNormal", 2DArray) = "white" {}
	u_TextureSetsDisplacement("u_TextureSetsDisplacement", 2DArray) = "white" {}
}

SubShader {
	CGINCLUDE

#pragma target 5.0
#pragma only_renderers d3d11

#include "UnityCG.cginc"
#include "UnityPBSLighting.cginc"

#include "TETErrain_Inputs.cginc"
#include "TEBrushBase.cginc"
#include "TETErrain_Geometry.cginc"
#include "TETErrain_Shading.cginc"

	ENDCG

	Tags{ "Special" = "TETerrain" }
	//Tags{ "Queue" = "AlphaTest-1" }

	Pass{
		Name "DEFERRED"
		Tags { "LightMode" = "Deferred" }

		CGPROGRAM
			//#pragma multi_compile TE_IMMEDIATE TE_VIRTUAL_TEXTURES
			#pragma multi_compile _ TE_TDE_ENABLED

#pragma shader_feature _NORMALMAP
#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#pragma shader_feature _SPECGLOSSMAP

#pragma multi_compile ___ UNITY_HDR_ON
#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON

			#pragma vertex vertexStatic
			#pragma fragment fragment
		ENDCG
    }

	Pass {
		Name "ShadowCaster"
		Tags { "LightMode" = "ShadowCaster" }
			
		ZWrite On ZTest LEqual

		CGPROGRAM
			#pragma multi_compile SHADOW_CASTER
			#pragma multi_compile_shadowcaster

			#pragma vertex vertexStatic
			#pragma fragment fragment
		ENDCG
	}

	// ------------------------------------------------------------------
	// Extracts information for lightmapping, GI (emission, albedo, ...)
	// This pass it not used during regular rendering.
	Pass
{
	Name "META"
	Tags{ "LightMode" = "Meta" }

	Cull Off

	CGPROGRAM
#pragma vertex vert_meta_temp
#pragma fragment frag_meta_temp

#pragma shader_feature _EMISSION
#pragma shader_feature _SPECGLOSSMAP
#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature ___ _DETAIL_MULX2

#define UNITY_PASS_META 1

	//#include "UnityStandardInput.cginc"
#include "UnityMetaPass.cginc"

struct v2f_meta {
	float4 uv		: TEXCOORD0;
	float4 pos		: SV_POSITION;
};

struct VertexInput {
	float4 vertex	: POSITION;
	float2 uv0		: TEXCOORD0;
	float2 uv1		: TEXCOORD1;
	float2 uv2		: TEXCOORD2;
};

v2f_meta vert_meta_temp(VertexInput v) {
	v2f_meta o;
	o.pos = UnityMetaVertexPosition(v.vertex, v.uv0.xy, v.uv2.xy, unity_LightmapST, unity_DynamicLightmapST);
	o.uv.xy = v.uv0;
	o.uv.zw = v.uv1;
	return o;
}

float4 frag_meta_temp(v2f_meta i) : SV_Target{
	UnityMetaInput o;
	UNITY_INITIALIZE_OUTPUT(UnityMetaInput, o);

	uint controlX, controlZ; u_Controlmap.GetDimensions(controlX, controlZ);
	uint controlMap = asuint(u_Controlmap.Load(int3(i.uv.x * controlX, i.uv.y * controlZ, 0)));

	uint defaultDetailLayerIndex = (controlMap & MASK_DEFAULT_IN) >> MASK_DEFAULT_SHIFT;

	float4 detailUVParams = u_DetailLayersUVScaleArrayMultiIndex[defaultDetailLayerIndex];
	float3 detailUV = float3(i.uv.xy, detailUVParams.z);

	float3 albedo = 1;// u_Colormap.Sample(sampleru_Colormap, i.uv.xy).rgb * 2.f;
	albedo *= u_TextureSetsAlbedo.Sample(sampleru_TextureSetsAlbedo, detailUV).rgb * u_DetailLayersAlbedoTint[defaultDetailLayerIndex].rgb;

	albedo = 0.3f; // disabled for env package

	o.Albedo = albedo;
	o.Emission = 0.f;

	return UnityMetaFragment(o);
}

ENDCG
}

}}
