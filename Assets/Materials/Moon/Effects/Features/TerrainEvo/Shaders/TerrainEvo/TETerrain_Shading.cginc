#ifndef TETERRAIN_SHADING_CGINC
#define TETERRAIN_SHADING_CGINC

struct FragmentCommonData
{
	half3 diffColor, specColor;
	half oneMinusReflectivity, oneMinusRoughness;
	half3 normalWorld, eyeVec, posWorld;
	half alpha;
};

inline UnityGI FragmentGI (
	float3 posWorld, 
	half occlusion, half4 i_ambientOrLightmapUV, half atten, half oneMinusRoughness, half3 normalWorld, half3 eyeVec,
	UnityLight light, bool reflections)
{
	UnityGIInput d;
	d.light = light;
	d.worldPos = posWorld;
	d.worldViewDir = -eyeVec;
	d.atten = atten;
	#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
		d.ambient = 0;
		d.lightmapUV = i_ambientOrLightmapUV;
	#else
		d.ambient = i_ambientOrLightmapUV.rgb;
		d.lightmapUV = 0;
	#endif
	d.boxMax[0] = unity_SpecCube0_BoxMax;
	d.boxMin[0] = unity_SpecCube0_BoxMin;
	d.probePosition[0] = unity_SpecCube0_ProbePosition;
	d.probeHDR[0] = unity_SpecCube0_HDR;

	d.boxMax[1] = unity_SpecCube1_BoxMax;
	d.boxMin[1] = unity_SpecCube1_BoxMin;
	d.probePosition[1] = unity_SpecCube1_ProbePosition;
	d.probeHDR[1] = unity_SpecCube1_HDR;

	if(reflections) {
		Unity_GlossyEnvironmentData g;
		g.roughness = 1 - oneMinusRoughness;
		g.reflUVW = reflect(eyeVec, normalWorld);

		return UnityGlobalIllumination(d, occlusion, normalWorld, g);
	} else {
		return UnityGlobalIllumination(d, occlusion, normalWorld);
	}
}

UnityLight DummyLight (half3 normalWorld) {
	UnityLight l = (UnityLight)0;
	l.dir = half3(0,1,0);
	l.ndotl = LambertTerm (normalWorld, l.dir);
	return l;
}

float3 UnpackScaledNormalMap(half4 packednormal, half2 scaledUnpack)
{
	float3 normal;
	normal.xy = (packednormal.wy * scaledUnpack.x + scaledUnpack.y);
	normal.z = sqrt(1.f - saturate(dot(normal.xy, normal.xy)));
	return normal;
}		

float3 CalculateHeightNormalMipped(float2 uv) {
	float h_m1 = u_Heightmap.Sample(sampleru_Heightmap, uv, int2(0, -1)).r;
	float h_p1 = u_Heightmap.Sample(sampleru_Heightmap, uv, int2(0, 1)).r;
	float hm1_ = u_Heightmap.Sample(sampleru_Heightmap, uv, int2(-1, 0)).r;
	float hp1_ = u_Heightmap.Sample(sampleru_Heightmap, uv, int2(1, 0)).r;

	// TODO: Upload proper y scale
	// TODO: Temp hacked until mips are added back
	return normalize(float3(hm1_ - hp1_, 1e-3f, h_m1 - h_p1));
}

float3 CalculateHeightNormal(float2 uv, int mip) {
	float h_m1 = u_Heightmap.SampleLevel(sampleru_Heightmap, uv, mip, int2(0, -1)).r;
	float h_p1 = u_Heightmap.SampleLevel(sampleru_Heightmap, uv, mip, int2(0, 1)).r;
	float hm1_ = u_Heightmap.SampleLevel(sampleru_Heightmap, uv, mip, int2(-1, 0)).r;
	float hp1_ = u_Heightmap.SampleLevel(sampleru_Heightmap, uv, mip, int2(1, 0)).r;
	// TODO: Upload proper y scale
	return normalize(float3(hm1_ - hp1_, 4e-4f * 1.f, h_m1 - h_p1));
}

float SampleLayerDisplacement(
	const int detailLayerIndex,
	const float4 detailUV,
	const float3 detailDDX, const float3 detailDDY
){
	float height = u_TextureSetsDisplacement.SampleGrad(sampleru_TextureSetsDisplacement, detailUV.xzw, detailDDX.xz, detailDDY.xz).a;
	float2 detailDisplacementScaleOffset = u_DetailLayersDisplacementScaleOffset[detailLayerIndex].xy;
	float displacement = height * detailDisplacementScaleOffset.x + detailDisplacementScaleOffset.y;
	return displacement;
}

void EvaluateDetailLayer(
	const int detailLayerIndex,
	const float detailWeight,
	//const float weightScale, const float forcedWeight,
	const float3 worldPos, const float3 worldNormal, const float worldDistance,
	const float4 detailUV, const bool tileRotation, const bool multiProjection,
	const float3 detailDDX, const float3 detailDDY,
	inout float3 albedo, inout float4 specSmooth, inout float3 normalTan, inout float occlusion, inout int materialId
){
	float4 detailSmtNrmDspOccScales = u_DetailLayersSmtNrmDspOccScales[detailLayerIndex];
	float3 detailAlbedo = float3(0.f, 0.f, 0.f);
	float4 detailSpecSmooth = float4(0.f, 0.f, 0.f, 0.f);
	float3 detailNormal = float3(0.f, 0.f, 0.f);
	float detailOcclusion = 0.f;

	detailAlbedo = u_TextureSetsAlbedo.SampleGrad(sampleru_TextureSetsAlbedo, detailUV.xzw, detailDDX.xz, detailDDY.xz).rgb;
	detailSpecSmooth = u_TextureSetsSpecSmooth.SampleGrad(sampleru_TextureSetsSpecSmooth, detailUV.xzw, detailDDX.xz, detailDDY.xz);
	detailNormal = UnpackScaleNormal(u_TextureSetsNormal.SampleGrad(sampleru_TextureSetsNormal, detailUV.xzw, detailDDX.xz, detailDDY.xz), detailSmtNrmDspOccScales.y);
	detailOcclusion = u_TextureSetsOcclusion.SampleGrad(sampleru_TextureSetsOcclusion, detailUV.xzw, detailDDX.xz, detailDDY.xz).g;

	albedo = lerp(albedo, detailAlbedo * u_DetailLayersAlbedoTint[detailLayerIndex], detailWeight);
	specSmooth = lerp(specSmooth, detailSpecSmooth * float4(u_DetailLayersSpecularTint[detailLayerIndex], detailSmtNrmDspOccScales.x), detailWeight);
	normalTan = lerp(normalTan, detailNormal, detailWeight);
	occlusion = lerp(occlusion, detailOcclusion, detailWeight);

	if(detailWeight >= 0.5f)
		materialId = detailLayerIndex;
}

float SampleDisplacement(bool tileRotation, float2 scaleOffset, float3 uv, float2 dx, float2 dy) {
	float h = u_TextureSetsDisplacement.SampleGrad(sampleru_TextureSetsDisplacement, uv, dx, dy).a;
	return h * scaleOffset.x + scaleOffset.y;
}

void fragment(
	in d2f IN,
#ifdef TE_CAPTURE_MATERIAL_ID
	out float4 outIndirectID : SV_Target0
#else
	out float4 outDiffuse : SV_Target0,			// RT0: diffuse color (rgb), occlusion (a)
	out float4 outSpecSmoothness : SV_Target1,	// RT1: spec color (rgb), smoothness (a)
	out float4 outNormal : SV_Target2,			// RT2: normal (rgb), --unused, very low precision-- (a) 
	out float4 outEmission : SV_Target3			// RT3: emission (rgb), --unused-- (a)
#endif
)
{
	float2 worldNoise = tex2D(u_NoiseTexture, IN.worldPos.xz * 0.1f).rg;
	worldNoise += tex2D(u_NoiseTexture, IN.worldPos.xz * 2.f).rg;

	uint controlX, controlZ; u_Controlmap.GetDimensions(controlX, controlZ);
	controlX -= 1; controlZ -= 1;
	float3 controlUV = float3(IN.uv.x * controlX, IN.uv.y * controlZ, 0.f);
	int3 controlUV0 = int3(floor(controlUV.x), floor(controlUV.y), 0);
	int3 controlUV1 = int3(ceil(controlUV.x), floor(controlUV.y), 0);
	int3 controlUV2 = int3(ceil(controlUV.x), ceil(controlUV.y), 0);
	int3 controlUV3 = int3(floor(controlUV.x), ceil(controlUV.y), 0);
	uint controlMap0 = asuint(u_Controlmap.Load(controlUV0));
	uint controlMap1 = asuint(u_Controlmap.Load(controlUV1));
	uint controlMap2 = asuint(u_Controlmap.Load(controlUV2));
	uint controlMap3 = asuint(u_Controlmap.Load(controlUV3));
	uint4 controlMaps = uint4(controlMap0, controlMap1, controlMap2, controlMap3);
	uint controlMapX1 = frac(worldNoise.x) > frac(controlUV.x) ? controlMaps.w : controlMaps.z;
	uint controlMapX2 = frac(worldNoise.x) > frac(controlUV.x) ? controlMaps.x : controlMaps.y;
	uint controlMap = frac(worldNoise.y) > frac(controlUV.y) ? controlMapX2 : controlMapX1;

	float4 baseLayerWeights = ((controlMaps & MASK_BASE_WEIGHT_IN) >> MASK_BASE_WEIGHT_SHIFT) / (float)MASK_BASE_WEIGHT_MAX;
	float baseLayerWeightX1 = lerp(baseLayerWeights.w, baseLayerWeights.z, frac(controlUV.x));
	float baseLayerWeightX2 = lerp(baseLayerWeights.x, baseLayerWeights.y, frac(controlUV.x));
	float baseLayerWeight = lerp(baseLayerWeightX2, baseLayerWeightX1, frac(controlUV.y));

	float4 overlayWeights = ((controlMaps & MASK_OVERLAY_WEIGHT_IN) >> MASK_OVERLAY_WEIGHT_SHIFT) / (float)MASK_OVERLAY_WEIGHT_MAX;
	float overlayWeightX1 = lerp(overlayWeights.w, overlayWeights.z, frac(controlUV.x));
	float overlayWeightX2 = lerp(overlayWeights.x, overlayWeights.y, frac(controlUV.x));
	float overlayWeight = lerp(overlayWeightX2, overlayWeightX1, frac(controlUV.y));

	float4 puddleMask4 = ((controlMaps & MASK_PUDDLE_IN) >> MASK_PUDDLE_SHIFT) / (float)MASK_PUDDLE_MAX;
	float puddleX1 = lerp(puddleMask4.w, puddleMask4.z, frac(controlUV.x));
	float puddleX2 = lerp(puddleMask4.x, puddleMask4.y, frac(controlUV.x));
	float puddleStrength = lerp(puddleX2, puddleX1, frac(controlUV.y));
	float softNoise = tex2D(u_SoftNoiseTexture, IN.worldPos.xz * 0.3f) * 2.f - 0.7f;
	puddleStrength = saturate(puddleStrength + softNoise * 0.1f);
	float waterDxDyScale = lerp(1.f, 100.f, saturate(puddleStrength - 0.6f) * _WaterClarity);

	float3 normal = CalculateHeightNormalMipped(IN.uv);

	float3 tangent = float3(normal.y, -normal.x, 0.f);
	float3 bitangent = float3(0.f, normal.z, -normal.y);
	float3x3 worldToTan = float3x3(-tangent, bitangent, normal);

	FragmentCommonData s = (FragmentCommonData)0;
	s.eyeVec = normalize(IN.worldPos.xyz - _WorldSpaceCameraPos.xyz);
	s.posWorld = IN.worldPos;

	const float3 worldPos = IN.worldPos.xyz;
	const float worldDistance = length(worldPos - _WorldSpaceCameraPos.xyz);

	float totalWeight = 0;
	float3 albedo = float3(0, 0, 0);
	float4 specSmooth = float4(0, 0, 0, 0);
	float3 normalTan = float3(0, 0, 0);
	float occlusion = 0;
	float maxHeight = -1000;
	float maxWeight = 0.f;

	uint defaultDetailLayerIndex = (controlMap & MASK_DEFAULT_IN) >> MASK_DEFAULT_SHIFT;
	uint baseDetailLayerIndex = (controlMap0 & MASK_BASE_IN) >> MASK_BASE_SHIFT;
	uint4 overlayDetailLayerIndices = (controlMaps & MASK_OVERLAY_IN) >> MASK_OVERLAY_SHIFT;
	uint overlayDetailLayerIndex = min(overlayDetailLayerIndices.x, min(overlayDetailLayerIndices.y, min(overlayDetailLayerIndices.z, overlayDetailLayerIndices.w)));

	uint materialId = defaultDetailLayerIndex;

	float4 detailUVParams = u_DetailLayersUVScaleArrayMultiIndex[defaultDetailLayerIndex];
	bool tileRotation = (bool)detailUVParams.y;
	bool multiProjection = (bool)detailUVParams.w;
	float4 detailUV = float4(worldPos.xyz * detailUVParams.x, detailUVParams.z);

	float3 detailDDX = ddx(detailUV.xyz) * waterDxDyScale;
	float3 detailDDY = ddy(detailUV.xyz) * waterDxDyScale;

	float defaultHeight = SampleDisplacement(tileRotation, u_DetailLayersDisplacementScaleOffset[defaultDetailLayerIndex].xy, detailUV.xzw, detailDDX.xz, detailDDY.xz);

	float detailWeight = 1.f;
	EvaluateDetailLayer(
		defaultDetailLayerIndex,
		detailWeight,
		worldPos, normal, worldDistance,
		detailUV, tileRotation, multiProjection, detailDDX, detailDDY,
		albedo, specSmooth, normalTan, occlusion, materialId
	);

	float baseLerpHeight = 0.f;
	detailUVParams = u_DetailLayersUVScaleArrayMultiIndex[baseDetailLayerIndex];
	detailUV = float4(worldPos.xyz * detailUVParams.x, detailUVParams.z);
	detailDDX = ddx(detailUV.xyz);
	detailDDY = ddy(detailUV.xyz);

	[branch]
	if(baseDetailLayerIndex != MASK_BASE_MAX) {
		tileRotation = (bool)detailUVParams.y;
		multiProjection = (bool)detailUVParams.w;

		detailDDX *= waterDxDyScale;
		detailDDY *= waterDxDyScale;

		float baseHeight = SampleDisplacement(tileRotation, u_DetailLayersDisplacementScaleOffset[baseDetailLayerIndex].xy, detailUV.xzw, detailDDX.xz, detailDDY.xz);

		baseLerpHeight = lerp(baseHeight * baseLayerWeight, max(baseHeight, defaultHeight + 0.1f), baseLayerWeight);
		detailWeight = saturate((baseLerpHeight - defaultHeight) / 0.05);

		EvaluateDetailLayer(
			baseDetailLayerIndex,
			detailWeight,
			worldPos, normal, worldDistance,
			detailUV, tileRotation, multiProjection, detailDDX, detailDDY,
			albedo, specSmooth, normalTan, occlusion, materialId
		);
	}

	float maxPrevHeight = max(baseLerpHeight, defaultHeight);
	detailUVParams = u_DetailLayersUVScaleArrayMultiIndex[overlayDetailLayerIndex];
	detailUV = float4(worldPos.xyz * detailUVParams.x, detailUVParams.z);
	detailDDX = ddx(detailUV.xyz);
	detailDDY = ddy(detailUV.xyz);
	[branch]
	if(overlayDetailLayerIndex != MASK_OVERLAY_MAX) {
		tileRotation = (bool)detailUVParams.y;
		multiProjection = (bool)detailUVParams.w;
		detailUV = float4(worldPos.xyz * detailUVParams.x, detailUVParams.z);

		detailDDX *= waterDxDyScale;
		detailDDY *= waterDxDyScale;

		float overlayHeight = SampleDisplacement(tileRotation, u_DetailLayersDisplacementScaleOffset[overlayDetailLayerIndex].xy, detailUV.xzw, detailDDX.xz, detailDDY.xz);

		float4 slopeThresholds = u_DetailLayersSlopeThresholds[overlayDetailLayerIndex];
		float slopeWeight = normal.y < slopeThresholds.z ? saturate((normal.y - slopeThresholds.x) / (slopeThresholds.y - slopeThresholds.x)) : (1.f - saturate((normal.y - slopeThresholds.z) / (slopeThresholds.w - slopeThresholds.z)));

		overlayWeight *= slopeWeight;
		float overlayLerpHeight = lerp(overlayHeight * overlayWeight, max(overlayHeight, maxPrevHeight + 0.1f), overlayWeight);
		detailWeight = saturate((overlayLerpHeight - maxPrevHeight) / 0.05);

		EvaluateDetailLayer(
			overlayDetailLayerIndex,
			detailWeight,
			worldPos, normal, worldDistance,
			detailUV, tileRotation, multiProjection, detailDDX, detailDDY,
			albedo, specSmooth, normalTan, occlusion, materialId
		);
	}

	float3 worldNormal = mul(normalTan, worldToTan);
	float waterFade = 1;
	[branch]
	if(puddleStrength > 0.f) {
		float puddleDepthToH = maxPrevHeight - puddleStrength * 2.f;
		float puddleDepthToP = puddleStrength * 2.f - maxPrevHeight;

		float waterDepthDarkenN = saturate(puddleDepthToP / _WetnessFogDarkenRange);
		float waterDepthAddN = saturate(puddleDepthToP / _WetnessFogAddRange);
		float waterNoise = 1;//frac(sin(dot(IN.uv.xy, float2(9.1379, 11.3932))) * 41982.97351) * 0.2 + 0.9;

		waterFade = puddleDepthToP < _WaterLevelEaseRange ? saturate(puddleDepthToP / _WaterLevelEaseRange) : 1.f;
		float wetnessFade = puddleDepthToH < _WetnessRange ? saturate(puddleDepthToH / _WetnessRange) : 1.f;
		half3 albedoWet = pow(albedo, 1.5f);
		occlusion = lerp(occlusion, 1, waterFade);

		half3 outAlbedo = lerp(albedoWet, albedo, wetnessFade);
		albedoWet = albedoWet * LerpOneTo(occlusion, pow(waterDepthDarkenN, 0.33)) * saturate(1.f - waterDepthDarkenN);
		albedoWet += _WetnessFogColor * (waterDepthAddN * waterNoise);

		outAlbedo = lerp(outAlbedo, albedoWet, waterFade);
		albedo = outAlbedo;

		float wetGloss = saturate(specSmooth.a * 2.2f);
		specSmooth.rgb = lerp(_WetnessSpecColor, specSmooth.rgb, wetnessFade);
		specSmooth.a = lerp(wetGloss, specSmooth.a, wetnessFade);
		specSmooth.rgb = lerp(specSmooth.rgb, _WaterSpecColor, waterFade);
		specSmooth.a = lerp(specSmooth.a, _WaterGlossiness, waterFade);

		worldNormal = lerp(worldNormal, float3(0.f, 1.f, 0.f), min(waterFade, 0.995f));
	}

	worldNormal = normalize(worldNormal);

	s.specColor = specSmooth.rgb;
	s.oneMinusRoughness = specSmooth.a;
	s.diffColor = EnergyConservationBetweenDiffuseAndSpecular(albedo, s.specColor, /*out*/s.oneMinusReflectivity);
	s.normalWorld = worldNormal;

	float4 dynamicLM = 0;
#ifdef DYNAMICLIGHTMAP_ON
	dynamicLM.zw = IN.uv.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#endif

	UnityGI gi = FragmentGI(s.posWorld, occlusion, dynamicLM, 1.f, s.oneMinusRoughness, s.normalWorld, s.eyeVec, DummyLight(s.normalWorld), false);

	half3 color = UNITY_BRDF_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect).rgb;
	color += UNITY_BRDF_GI(s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, occlusion, gi);

#ifdef TE_CAPTURE_MATERIAL_ID
	outIndirectID.rgb = exp2(-gi.indirect.diffuse);
	outIndirectID.a = materialId / 16.f;
#else
	outDiffuse = half4(s.diffColor, occlusion);
	outSpecSmoothness = half4(s.specColor, s.oneMinusRoughness);
	outNormal = half4(s.normalWorld * 0.5f + 0.5f, 1.f);
	outEmission = half4(color, 1.f);
	
#ifndef UNITY_HDR_ON
	outEmission.rgb = exp2(-outEmission.rgb);
#endif
#endif
}
 
#endif//TETERRAIN_SHADING_CGINC
