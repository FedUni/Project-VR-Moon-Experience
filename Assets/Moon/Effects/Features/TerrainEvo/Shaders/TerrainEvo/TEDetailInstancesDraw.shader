Shader "Hidden/TerrainEvo/DetailInstancesDraw" {
Properties{
	_AlbedoArray("_AlbedoArray", 2DArray) = "white" {}
	_NormalArray("_NormalArray", 2DArray) = "white" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }

CGINCLUDE

#include "UnityCG.cginc"

struct Instance {
	float3	basePos;
	float	randomInstance;
	float4	diagonal;
	float3	indirectLighting;
	float	normalMapScale;
	float3	albedoTint;
	float	uvArrayIndex;
	float4	uvScaleOffset;
};

uniform StructuredBuffer<Instance> meshInput;

struct v2f {
	float4 vertex		: SV_POSITION;
	float3 ambient		: TEXCOORD0;
	float3 color		: TEXCOORD1;
	float4 normal		: TEXCOORD2;
	float4 uv			: TEXCOORD3;
	float4 eyeVecCamDist: TEXCOORD4;
};

void UnityClipSpaceShadowCasterPosWorld_OffsetOnly(inout float3 wPos, float3 wNormal) {
	// Important to match MVP transform precision exactly while rendering
	// into the depth texture, so branch on normal bias being zero.
	if(unity_LightShadowBias.z != 0.0) {
		float3 wLight = normalize(UnityWorldSpaceLightDir(wPos));
		float shadowCos = dot(wNormal, wLight);
		float shadowSine = sqrt(1 - shadowCos*shadowCos);
		float normalBias = unity_LightShadowBias.z * shadowSine;

		wPos -= wNormal * normalBias;
	}
}

float4 UnityClipSpaceShadowCasterPos_FromWorld(float3 wPos, float3 wNormal, float offset) {
	float4 clipPos;

	// Important to match MVP transform precision exactly while rendering
	// into the depth texture, so branch on normal bias being zero.
	if(unity_LightShadowBias.z != 0.0) {
		float3 wLight = normalize(UnityWorldSpaceLightDir(wPos));

		// apply normal offset bias (inset position along the normal)
		// bias needs to be scaled by sine between normal and light direction
		// (http://the-witness.net/news/2013/09/shadow-mapping-summary-part-1/)
		//
		// unity_LightShadowBias.z contains user-specified normal offset amount
		// scaled by world space texel size.

		float shadowCos = dot(wNormal, wLight);
		float shadowSine = sqrt(1 - shadowCos*shadowCos);
		float normalBias = unity_LightShadowBias.z * shadowSine;

		wPos -= wNormal * normalBias + offset;
	}
	clipPos = mul(UNITY_MATRIX_VP, float4(wPos, 1));
	return clipPos;
}

v2f vert(uint vertexID : SV_VertexID) {
	/*
		0,_ --- 1,4
		|     / |
		|   /   |
		2,3 --- 5,_
	*/

	const uint instanceID = vertexID / 6;
	const uint cornerID = vertexID % 6;
	const bool leftEdge = cornerID <= 3 && cornerID != 1;
	const bool bottomEdge = cornerID >= 2 && cornerID != 4;

	Instance i = meshInput[instanceID];

	float3 camVec = _WorldSpaceCameraPos - i.basePos;
	float camDist = length(camVec * float3(1.f, 0.5f, 1.f));
	float hideFactor = 1.f;
	if(i.randomInstance < 1.f - pow(1.f - saturate((camDist - 20.f) / 80.f), 6.f)) {
		i.diagonal.yw = 0;
		hideFactor = 0.f;
	}
	float flattenFactor = 1.f - saturate((camDist - 75.f) / 10.f);
	hideFactor *= flattenFactor;
	i.diagonal.yw *= flattenFactor;
i.diagonal.w = 0;

	const float3 expandDir = float3(leftEdge ? -i.diagonal.x : i.diagonal.x, bottomEdge ? 0.f : i.diagonal.y, leftEdge ? -i.diagonal.z : i.diagonal.z);

	float3 normal = cross(normalize(float3(i.diagonal.x, 0, i.diagonal.z)), float3(0, 1, 0));
	float3 upNormal = float3(0, 1, 0);

	float3 pos = i.basePos + expandDir;
#if 1
	float3 offsetPos = 0;
	if(!bottomEdge) {
		offsetPos.xz += normal.xz * i.diagonal.w;

		upNormal.y -= cos(_Time.y * 2.0f + i.basePos.z) * 0.040f + 0.020f;
	}
	pos += offsetPos * saturate(i.diagonal.y * 2);
#endif

	pos.y -= saturate(1.f - hideFactor);

	v2f o;
#if SHADOW_CASTER
	o.vertex = UnityClipSpaceShadowCasterPos_FromWorld(pos, normal, expandDir.y * 0.05f);
	o.vertex = UnityApplyLinearShadowBias(o.vertex);
#else
	o.vertex = mul(UNITY_MATRIX_VP, float4(pos, 1.f));
#endif

	o.color = bottomEdge ? i.albedoTint : (i.albedoTint + float3(1, 1, 1)) * 0.5;
o.color = i.albedoTint;
o.ambient = i.indirectLighting;
o.normal.xyz = normalize(float3(i.diagonal.x, 0.f, i.diagonal.z));
o.normal.w = bottomEdge ? 0 : 0.04 + upNormal.y * 0.04;
	o.uv = float4(leftEdge ? 0.f : 1.f, bottomEdge ? 0.f : 1.f, i.uvArrayIndex, i.normalMapScale);
	o.uv.xy = o.uv.xy * i.uvScaleOffset.xy + i.uvScaleOffset.zw;
	o.eyeVecCamDist.xyz = normalize(pos.xyz - _WorldSpaceCameraPos);
	o.eyeVecCamDist.w = camDist;

	return o;
}

ENDCG

	Pass {
		Name "DEFERRED"
		Tags{ "LightMode" = "Deferred" }

		Cull Off
		Stencil{
			ReadMask	255
			WriteMask	239
			Comp		Always
			Pass		Replace
			Fail		Keep
			ZFail		Keep
			Ref			192
		}

		CGPROGRAM

#pragma multi_compile _ UNITY_HDR_ON

#pragma target 5.0
#pragma vertex vert
#pragma fragment frag
			
#include "UnityStandardCore.cginc"

float3 _AtmosphericScatteringSunVector;
int _ReconstructedBlend;
int _DeferredTransmission;

Texture2DArray	_AlbedoArray;
//Texture2DArray	_PackedArray;
Texture2DArray	_NormalArray;
SamplerState	sampler_AlbedoArray;
//SamplerState	sampler_PackedArray;
SamplerState	sampler_NormalArray;

float kill(float2 vPos, float camDist, float alpha) {
	float idx = dot(floor(vPos % 2), float2(1.f, 2.f));
	float s = lerp(2.f, 5.f, saturate(camDist / 50.f));
	float d = alpha * s  < (idx * 0.2f + 0.21f) ? -1.f : 1.f;
	clip(d);
	return alpha > 0.95 ? 1 : idx / 3.f;
}

inline FragmentCommonData FragmentSetup(float3 albedoTint, float3 uv, float2 vPos, float camDist, out float killValue) {
	float4 albedoAlpha = _AlbedoArray.Sample(sampler_AlbedoArray, uv);

	if(_DeferredTransmission == 1) {
		clip(albedoAlpha.a - 0.25f/*_Cutoff*/);
		killValue = 0;
	}  else if(_ReconstructedBlend == 0 || vPos.x / _ScreenParams.x > 0.5) {
		clip(albedoAlpha.a - 0.25f/*_Cutoff*/);
		killValue = 0;
	} else {
		killValue = kill(vPos, camDist, albedoAlpha.a);
	}

	float4 specGloss;
	specGloss.rgb = float3(0.035, 0.035, 0.035) * saturate(0.50 + uv.y);
	specGloss.a = 3.0f * (albedoAlpha.g) * saturate(1.f - pow(1.f - uv.y * uv.y, 20.f));

	specGloss.rgba *= 1.f - clamp( ((camDist - 2.f) / 50.f), 0.0f, 0.8f );

	albedoAlpha.rgb *= albedoTint;
	albedoAlpha.rgb *= saturate(0.6 + uv.y);

	float3 specColor = specGloss.rgb;
	float oneMinusRoughness = specGloss.a;

	float oneMinusReflectivity;
	float3 diffColor = EnergyConservationBetweenDiffuseAndSpecular(albedoAlpha.rgb, specColor, /*out*/ oneMinusReflectivity);

	FragmentCommonData o = (FragmentCommonData)0;
	o.diffColor = diffColor;
	o.specColor = specColor;
	o.oneMinusReflectivity = oneMinusReflectivity;
	o.oneMinusRoughness = oneMinusRoughness;
	return o;
}


void frag(
	v2f i,
	bool isFrontFace			: SV_IsFrontFace,
	out half4 outDiffuse		: SV_Target0,			// RT0: diffuse color (rgb), occlusion (a)
	out half4 outSpecSmoothness	: SV_Target1,			// RT1: spec color (rgb), smoothness (a)
	out half4 outNormal			: SV_Target2,			// RT2: normal (rgb), --unused, very low precision-- (a) 
	out half4 outEmission		: SV_Target3			// RT3: emission (rgb), --unused-- (a)
){
	float3 tangent = i.normal.xyz;
	float3 centerNormal = cross(float3(0, 1, 0), tangent);
	float3 normal = centerNormal;
	if(i.uv.x < 0.5)
		normal = lerp(tangent, centerNormal, saturate(i.uv.x + 0.5));
	else
		normal = lerp(centerNormal, -tangent, saturate(i.uv.x - 0.5f));

	normal.y += i.uv.y * i.uv.y;
	normal.y += i.normal.w;

	normal = normalize(normal);
	float3 bitangent = cross(normal, tangent);
	if(!isFrontFace)
		normal.xz = -normal.xz;

	float3x3 worldToTan = float3x3(tangent, -bitangent, normal);

	float3 normalTan = UnpackScaleNormal(_NormalArray.Sample(sampler_NormalArray, i.uv.xyz), i.uv.w);

	float killValue;
	FragmentCommonData fcd = FragmentSetup(i.color, i.uv, i.vertex.xy, i.eyeVecCamDist.w, /*out*/ killValue);
	fcd.normalWorld = mul(normalTan, worldToTan);
	fcd.eyeVec = i.eyeVecCamDist.xyz;

	float occlusion = saturate(0.0f + i.uv.y * i.uv.y) * normal.y;

	UnityGI gi = (UnityGI)0;
	gi.indirect.diffuse = i.ambient * saturate(occlusion + 0.75);

	float3 color = UNITY_BRDF_PBS(fcd.diffColor, fcd.specColor, fcd.oneMinusReflectivity, fcd.oneMinusRoughness, fcd.normalWorld, -fcd.eyeVec, gi.light, gi.indirect);

	outDiffuse = half4(fcd.diffColor, occlusion);
	outSpecSmoothness = half4(fcd.specColor, fcd.oneMinusRoughness);
	outNormal = half4(fcd.normalWorld * 0.5 + 0.5, 1);
	outNormal.a = killValue;
	outEmission = half4(color, 1);

#ifndef UNITY_HDR_ON
	outEmission.rgb = exp2(-outEmission.rgb);
#endif
}

		ENDCG
	}

	Pass{
		Name "ShadowCaster"
		Tags{ "LightMode" = "ShadowCaster" }

		ZWrite On ZTest LEqual
		Cull Off

		CGPROGRAM
			#pragma target 5.0
			#pragma only_renderers d3d11

			#pragma multi_compile SHADOW_CASTER
			#pragma multi_compile_shadowcaster

			#pragma vertex vert
			#pragma fragment frag_shadow

			Texture2DArray	_AlbedoArray;
			SamplerState	sampler_AlbedoArray;

			void frag_shadow(v2f i) {
				float alpha = _AlbedoArray.Sample(sampler_AlbedoArray, i.uv.xyz).a;
				clip(alpha - 0.25f/*_Cutoff*/);
			}

		ENDCG
	}

}}
