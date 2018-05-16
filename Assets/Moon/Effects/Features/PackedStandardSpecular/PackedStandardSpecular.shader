Shader "Adam/Packed Standard (Specular setup)" {
	Properties {
		_MainTex("Albedo", 2D) = "white" {}
		_Color("Albedo Tint", Color) = (1,1,1,1)
		
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 2.0)) = 0.5
		_SpecColor("Specular Color", Color) = (0.2,0.2,0.2)

		_BumpScale("Normal Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_OcclusionStrength("Occlusion Strength", Range(0.0, 1.0)) = 1.0
		
		[Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0

		_PackedBaseMap("Occlusion(R) Smoothness(G) Layer(B) Height(A) Map", 2D) = "gray" {}
		_DetailArrayMaxIndex("Detail Array Max Index", Float) = 0.0
		_AlbedoArray("Detail Albedo (RGB) Array", 2DArray) = "" {}
		_PackedArray("Detail Occlusion(R) Smoothness(G) Height(A) Array", 2DArray) = "" {}
		_NormalArray("Detail Normal Array", 2DArray) = "" {}
		_DetailAlbedoGrayscaleStrength("Detail Albedo Strength", Range(0.0, 6.0)) = 1.0
		_UseFullAlbedo("Use Full Albedo", Float) = 0.0
		_UseDitheredEdges("Use Dithered Edges", Float) = 0.0
		_DebugDisableDetail("Debug Disable Detail", Float) = 0.0

		_NoiseTexture("Noise (RG) Map", 2D) = "gray" {}
		_NoiseUVScale("Noise UV Scale", Range(1.0, 600.0)) = 75.0

		_DirtWetnessTex("Dirt(RGB) Wetness(A) Map", 2D) = "gray" {}

		// Blending state
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
	}

	CGINCLUDE
		#define UNITY_SETUP_BRDF_INPUT SpecularSetup 
	ENDCG

	SubShader
	{
		Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
		LOD 300
	
		// ------------------------------------------------------------------
		//  Deferred pass
		Pass
		{
			Name "DEFERRED"
			Tags { "LightMode" = "Deferred" }

			CGPROGRAM
			#pragma target 5.0
			#pragma only_renderers d3d11

			#pragma multi_compile _PACKED_BASE_PASS
			#pragma multi_compile _DETAIL_MULX2

			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _SPECGLOSSMAP

			#pragma multi_compile ___ UNITY_HDR_ON
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
			
			#pragma vertex vertDeferred
			#pragma fragment fragDeferred

			#define SUPPORTS_GATHERBLUE 1
			#include "UnityStandardCore.cginc"

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

#define UNITY_PASS_META 1
#include "UnityCG.cginc"
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
	o.pos = UnityMetaVertexPosition(v.vertex, v.uv1.xy, v.uv2.xy, unity_LightmapST, unity_DynamicLightmapST);
	o.uv = v.uv0.xyxy;
	return o;
}

float4 frag_meta_temp(v2f_meta i) : SV_Target{
	UnityMetaInput o;
	UNITY_INITIALIZE_OUTPUT(UnityMetaInput, o);

	// disabled for env package
	o.Albedo = 0.3;
	o.Emission = 0.f;

	return UnityMetaFragment(o);
}

			ENDCG
		}
	} //Subshader

		
	SubShader
	{
		Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
		LOD 200
	
		// ------------------------------------------------------------------
		//  Deferred pass
		Pass
		{
			Name "DEFERRED"
			Tags { "LightMode" = "Deferred" }

			CGPROGRAM
			#pragma target 3.0
			#pragma only_renderers d3d11

			#pragma multi_compile _PACKED_BASE_PASS
			#pragma multi_compile _DETAIL_MULX2

			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _SPECGLOSSMAP

			#pragma multi_compile ___ UNITY_HDR_ON
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
			
			#pragma vertex vertDeferred
			#pragma fragment fragDeferred

			#define SUPPORTS_GATHERBLUE 0
			#include "UnityStandardCore.cginc"

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

#define UNITY_PASS_META 1
#include "UnityCG.cginc"
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
	o.pos = UnityMetaVertexPosition(v.vertex, v.uv1.xy, v.uv2.xy, unity_LightmapST, unity_DynamicLightmapST);
	o.uv = v.uv0.xyxy;
	return o;
}

float4 frag_meta_temp(v2f_meta i) : SV_Target{
	UnityMetaInput o;
	UNITY_INITIALIZE_OUTPUT(UnityMetaInput, o);

	// disabled for env package
	o.Albedo = 0.3;
	o.Emission = 0.f;

	return UnityMetaFragment(o);
}

			ENDCG
		}
	} //Subshader


	FallBack "Standard (Specular setup)"
	CustomEditor "PackedStandardShaderGUI"
}
