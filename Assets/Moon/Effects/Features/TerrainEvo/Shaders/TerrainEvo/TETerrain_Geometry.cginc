#ifndef TETERRAIN_GEOMETRY_CGINC
#define TETERRAIN_GEOMETRY_CGINC

#include "TEBrushBase.cginc"

struct a2v { float3 vertex	: POSITION; };
struct a2vStatic {
	float3 vertex	: POSITION;
	float3 normal	: NORMAL;
	float2 uv		: TEXCOORD0;
};
struct v2h { float3 pos		: NORMALPOS; };
struct h2d { float2 pos		: NORMALPOS; };
struct hc2d {
	float edges[3]/*4*/		: SV_TessFactor;
	float inside/*[2]*/		: SV_InsideTessFactor;
};
struct d2f {
	float4 pos				: SV_POSITION;
	float3 uv				: TEXCOORD0;
	float3 worldPos			: WORLDPOS;
	float3 worldNrm			: WORLDNRM;
};

#ifdef SHADOW_CASTER
	#include "TETerrain_Shading.cginc"
#endif

float3 CalculateHeightNormal_X(float2 uv, int mip) {
	float h_m1 = u_Heightmap.SampleLevel(sampleru_Heightmap, uv, mip, int2(0, -1)).r;
	float h_p1 = u_Heightmap.SampleLevel(sampleru_Heightmap, uv, mip, int2(0, 1)).r;
	float hm1_ = u_Heightmap.SampleLevel(sampleru_Heightmap, uv, mip, int2(-1, 0)).r;
	float hp1_ = u_Heightmap.SampleLevel(sampleru_Heightmap, uv, mip, int2(1, 0)).r;

	// TODO: Upload proper y scale
	return normalize(float3(hm1_ - hp1_, 4e-4f * 1.f, h_m1 - h_p1));
}


v2h vertex(a2v v) {
	v2h o;
	o.pos = v.vertex.xyz;
	return o;
}


hc2d hull_const(InputPatch<v2h, 3>/*4*/ patch, uint patchID : SV_PrimitiveID) {
    hc2d o;

#if 1
	const float D = 1.f / 75.f;
	const float MT1 = 31;// 23.f;
	const float MT0 = 1.f;

	// TODO: Use error maps to control tesselation where needed.

	float3 localCamPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos.xyz, 1.f));

	float3 ec0 = (patch[0].pos + patch[1].pos) * 0.5f;
	float3 ec1 = (patch[1].pos + patch[2].pos) * 0.5f;
	float3 ec2 = (patch[2].pos + patch[0].pos) * 0.5f;
//	float2 ec2 = (patch[2].pos + patch[3].pos) * 0.5f;
//	float2 ec3 = (patch[3].pos + patch[0].pos) * 0.5f;

	float d0 = 1.f - saturate(length(localCamPos - ec0) * D);
	float d1 = 1.f - saturate(length(localCamPos - ec1) * D);
	float d2 = 1.f - saturate(length(localCamPos - ec2) * D);
//	float d3 = 1.f - saturate(length(localCamPos - ec3) * D);

	o.edges[0] = MT0 + MT1 * d0 * d0 * d0 * d0;
	o.edges[1] = MT0 + MT1 * d1 * d1 * d1 * d1;
	o.edges[2] = MT0 + MT1 * d2 * d2 * d2 * d2;
//	o.edges[3] = MT0 + MT1 * d3 * d3;
#else
	o.edges[0] = o.edges[1] = o.edges[2] /*= o.edges[3]*/ = 8;
#endif

//    o.inside[0] = (o.edges[0] + o.edges[1]) / 2.f;
//    o.inside[1] = (o.edges[2] + o.edges[3]) / 2.f;

	o.inside = (o.edges[0] + o.edges[1] + o.edges[2]) / 3.f;

    return o;
}

[domain("tri")]/*quad*/
[partitioning("fractional_odd")]/*even*/
[outputtopology("triangle_cw")]
[outputcontrolpoints(3)]/*4*/
[patchconstantfunc("hull_const")]
[maxtessfactor(8.0)]
h2d hull(InputPatch<v2h, 3>/*4*/ patch, uint cpID : SV_OutputControlPointID) {
    h2d o;
    o.pos = patch[cpID].pos.xz;
    return o;
}

[domain("tri")]/*quad*/
d2f domain(hc2d hconst, float3/*2*/ domainUV : SV_DomainLocation, const OutputPatch<h2d, 3>/*4*/ patch) {
	d2f o;
	
	//float2 normalPos = lerp(
	//	lerp(patch[0].pos, patch[1].pos, domainUV.x),
	//	lerp(patch[3].pos, patch[2].pos, domainUV.x),
	//	domainUV.y
	//);
	float2 normalPos = patch[0].pos * domainUV.x + patch[1].pos * domainUV.y + patch[2].pos * domainUV.z;

	float2 uv = normalPos * u_Heightmap_ST.xy + u_Heightmap_ST.zw;
#ifdef TE_TDE_PICKING
	uv = normalPos * u_HeightmapRef_ST.xy + u_HeightmapRef_ST.zw;
#endif

	float3 objectPos;
	objectPos.xz = normalPos;
	objectPos.y = u_Heightmap.SampleLevel(sampleru_Heightmap, uv, 0).r;
#if defined(TE_TDE_PICKING) && !defined(TE_TDE_PICKING_USE_WORKDATA)
	// Depending on brush type, picking is either based on the reference data before 
	// the current stroke started, or the current 'live' height data.
	objectPos.y = u_HeightmapRef.SampleLevel(sampleru_HeightmapRef, uv, 0).r;
#endif

	o.worldPos = mul(unity_ObjectToWorld, float4(objectPos, 1.f)).xyz;
	o.worldNrm = CalculateHeightNormal_X(uv, 7);

	o.uv.xy = uv;
	o.uv.z = normalPos.y;

	// Default (only) displacement
#if 0 && !defined(TE_TDE_PICKING)

	uint controlX, controlZ; u_Controlmap.GetDimensions(controlX, controlZ);
	float3 controlUV = float3(o.uv.x * controlX, o.uv.y * controlZ, 0.f);
	uint controlMap = asuint(u_Controlmap.Load(controlUV));

	uint defaultDetailLayerIndex = (controlMap & MASK_DEFAULT_IN) >> MASK_DEFAULT_SHIFT;
	float4 detailUVParams = u_DetailLayersUVScaleArrayMultiIndex[defaultDetailLayerIndex];
	float4 detailUV = float4(o.worldPos.xyz * detailUVParams.x, detailUVParams.z).zyxw;

	float defaultHeight = u_TextureSetsDisplacement.SampleLevel(sampleru_TextureSetsDisplacement, detailUV.xzw, 0).a;
	float defaultDisplacementGeometryScale = u_DetailLayersDisplacementScaleOffset[defaultDetailLayerIndex].z;
	defaultHeight *= defaultHeight * defaultDisplacementGeometryScale;

	o.worldPos.y += defaultHeight;
#endif

#ifdef SHADOW_CASTER
	if(unity_LightShadowBias.z != 0.0) {
		float3 wNormal = CalculateHeightNormal(o.uv.xy, 0);
		float3 wLight = normalize(UnityWorldSpaceLightDir(o.worldPos));

		float shadowCos = dot(wNormal, wLight);
		float shadowSine = sqrt(1-shadowCos*shadowCos);
		float normalBias = unity_LightShadowBias.z * shadowSine;

		o.worldPos -= wNormal * normalBias;
	}
	o.pos = mul(UNITY_MATRIX_VP, float4(o.worldPos, 1.f));
	o.pos = UnityApplyLinearShadowBias(o.pos);
#else
	o.pos = mul(UNITY_MATRIX_VP, float4(o.worldPos, 1.f));
#endif

	return o;    
}

d2f vertexStatic(a2vStatic v) {
	d2f o;

	o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex, 1.f)).xyz;
	o.worldNrm = mul(unity_ObjectToWorld, v.normal);
	o.uv.xy = v.uv;
	o.uv.z = v.uv.y;

#ifdef SHADOW_CASTER
	if(unity_LightShadowBias.z != 0.0) {
		float3 wNormal = o.worldNrm;
		float3 wLight = normalize(UnityWorldSpaceLightDir(o.worldPos));

		float shadowCos = dot(wNormal, wLight);
		float shadowSine = sqrt(1 - shadowCos*shadowCos);
		float normalBias = unity_LightShadowBias.z * shadowSine;

		o.worldPos -= wNormal * normalBias;
	}
	o.pos = mul(UNITY_MATRIX_VP, float4(o.worldPos, 1.f));
	o.pos = UnityApplyLinearShadowBias(o.pos);
#else
	o.pos = mul(UNITY_MATRIX_VP, float4(o.worldPos, 1.f));
#endif

	return o;
}

#endif//TETERRAIN_GEOMETRY_CGINC