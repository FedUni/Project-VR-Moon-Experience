using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TETerrainMaterialData : ScriptableObject {
	public enum TextureArraySize {
		x512 = 512, x1024 = 1024, x2048 = 2048, x4096 = 4096
	}

	[System.Flags]
	public enum SpriteMode {
		Oriented	= 1,
		Billboard	= 2,
		FaceUp		= 4,
		Cross		= 8,
		Cross4		= 16,
	}

	[System.Serializable]
	public class UndergrowthTemplate {
		public string		name	= "<No name>";
		public string		guid	= System.Guid.NewGuid().ToString("N");

		public void SetNewGuid() {
			guid = System.Guid.NewGuid().ToString("N");
		}
	}

	[System.Serializable]
	public class UndergrowthSpriteTemplate : UndergrowthTemplate {
		public string		textureSet;
		public Color		textureAlbedoTint;
		public Vector4		uvScaleOffset;
		public Vector3		widthMinMaxMedian;
		public Vector3		heightMinMaxMedian;
		public Vector3		groundColorFactorBottomTopMid;
		public Vector3		groundColorFactorScaleBottomTopMid;
		public Vector3		groundWetnessDarkenFactorBottomTopMid;
		public bool			growsInWater;
		public SpriteMode	mode;
		//public float		baseProbability;

		public void SetDefaults() {
			SetNewGuid();

			name = "<No name>";
			textureSet = string.Empty;

			textureAlbedoTint						= Color.white;
			uvScaleOffset							= new Vector4(1f, 1f, 0f, 0f);
			widthMinMaxMedian						= new Vector3(0.3f, 0.8f, 0.6f);
			heightMinMaxMedian						= new Vector3(0.2f, 0.5f, 0.4f);
			groundColorFactorBottomTopMid			= new Vector3(0.6f, 0.4f, 0.5f);
			groundColorFactorScaleBottomTopMid		= new Vector3(1.0f, 1.0f, 1.0f);
			groundWetnessDarkenFactorBottomTopMid	= new Vector3(0.3f, 0.1f, 0.1f);
			growsInWater							= true;
			mode									= SpriteMode.Oriented;
			//baseProbability						= 1f;
		}
	}

	[System.Serializable]
	public class UndergrowthMeshTemplate : UndergrowthTemplate {
		public string		textureSet;
		public Vector4		uvScaleOffset;
		public Vector3		widthMinMaxMedian;
		public Vector3		heightMinMaxMedian;
		public float		probability;
		public bool			xrotated90degrees;

		public void SetDefaults() {
			SetNewGuid();

			name = "<No name>";
			textureSet = string.Empty;

			uvScaleOffset		= new Vector4(1f, 1f, 0f, 0f);
			widthMinMaxMedian	= new Vector3(0.3f, 0.8f, 0.6f);
			heightMinMaxMedian	= new Vector3(0.2f, 0.5f, 0.4f);
			probability			= 1f;
			xrotated90degrees	= false;
		}

	}


	[System.Serializable]
	public class OvergrowthTemplate {
		public Mesh[]		meshLODs;
		public Vector3		xzScaleMinMaxMedian;
		public Vector3		yScaleMinMaxMedian;
		public Vector2		coneRotationAngleMedian;
		public float		probability;
	}

	[System.Serializable]
	public class UndergrowthDetailLayer {
		public string	name = "<No name>";
		public string	guid = System.Guid.NewGuid().ToString("N");

		public string	undergrowthTemplate;
		public Vector2	widthHeightScale;
		public float	probability;

		public void SetNewGuid() {
			guid = System.Guid.NewGuid().ToString("N");
		}

		public void SetDefaults() {
			SetNewGuid();

			name = "<No name>";
			undergrowthTemplate = string.Empty;

			widthHeightScale	= new Vector2(1.0f, 1.0f);
			probability			= 1f;
		}
	}

	[System.Serializable]
	public class TextureSet {
		// TODO: (needs custom textures which I'm too lazy to create right now)
		//
		//	Suggested texture channel layout:
		//		DXT1: Albedo
		//		DXT5: specular, smoothness, occlusion, height
		//
		//	Modifiers:
		//		Albedo:		HDR Tint
		//		Spec:		HDR Tint
		//		Smoothness:	Scale
		//		AO:			Strength
		//		Height:		Range
		//
		//	Blending, normals and displacement all calculated from height channel.
		//

		public string		name;
		public string		guid;

		public Texture2D	albedo;
		public Color		albedoTint;
		public float		albedoTintScale;

		public Texture2D	specSmooth;
		public Color		specularTint;
		public float		specularTintScale;
		public float		smoothnessScale;

		public Texture2D	normal;
		public float		normalScale;

		public Texture2D	displacement;
		public float		displacementScale;
		public float		displacementOffset;
		public float		displacementGeomScale;

		public Texture2D	occlusion;
		public float		occlusionScale;

		public float		uvScale;
		
		public void SetNewGuid() {
			guid				= System.Guid.NewGuid().ToString("N");
		}

		public void SetDefaults() {
			SetNewGuid();

			name					= "<No name>";

			//albedo				= Texture2D.whiteTexture;
			albedoTint				= Color.white;
			albedoTintScale			= 1f;

			//specSmooth			= Texture2D.blackTexture;
			specularTint			= Color.white;
			specularTintScale		= 1f;
			smoothnessScale			= 1f;

			//normal				= Texture2D.whiteTexture;
			normalScale				= 1f;

			//displacement			= Texture2D.blackTexture;
			displacementScale		= 1f;
			displacementOffset		= 0f;
			displacementGeomScale	= 1f;

			//occlusion			= Texture2D.whiteTexture;
			occlusionScale = 1f;

			uvScale				= 1f;
		}
	}

	[System.Serializable]
	public class DetailLayer {
		public string					name;
		public string					guid;

		public string					textureSet;
		public Color					textureAlbedoTint;
		public Color					textureSpecularTint;
		public float					textureSmoothnessScale;
		public float					textureNormalScale;
		public float					textureDisplacementScale;
		public float					textureDisplacementOffset;
		public float					textureDisplacementGeomScale;
		public float					textureOcclusionScale;
		public Vector2					textureUVScale;
		public bool						textureUVMultiProjection;
		public bool						textureUVTileRotation;

		public Vector4					slopeThreshold;

		public UndergrowthDetailLayer[] undergrowthStrongDetailLayers;
		public UndergrowthDetailLayer[] undergrowthWeakDetailLayers;

		public void SetNewGuid() {
			guid = System.Guid.NewGuid().ToString("N");
		}

		public void SetDefaults() {
			SetNewGuid();

			name					= "<No name>";
			textureSet				= string.Empty;

			Reset();
		}

		public void Reset() {
			textureAlbedoTint				= Color.white;
			textureSpecularTint				= Color.white;
			textureSmoothnessScale			= 1f;
			textureNormalScale				= 1f;
			textureDisplacementScale		= 1f;
			textureDisplacementOffset		= 0f;
			textureDisplacementGeomScale	= 0.25f;
			textureOcclusionScale			= 1f;
			textureUVScale					= Vector2.one;
			textureUVMultiProjection		= false;
			textureUVTileRotation			= false;

			slopeThreshold					= new Vector4(0f, 0f, 1f, 1f);

			undergrowthStrongDetailLayers	= new UndergrowthDetailLayer[0];
			undergrowthWeakDetailLayers		= new UndergrowthDetailLayer[0];
		}
	}

	public Shader						placeholderShader;
	public Shader						nearShader;
	public Shader						defaultShader;
	public Shader						farShader;

	public Material						placeholderMaterial;
	public Material						nearMaterial;
	public Material						defaultMaterial;
	public Material						farMaterial;

	public Texture2D					gridTexture;
	public Texture2D					noiseTexture;

	public TextureArraySize				textureArraySize			= TextureArraySize.x2048;
	public TextureSet[]					textureSets					= new TextureSet[0];
	public TextureArraySize				undergrowthTextureArraySize	= TextureArraySize.x1024;
	public TextureSet[]					undergrowthTextureSets		= new TextureSet[0];
	public UndergrowthSpriteTemplate[]	undergrowthSpriteTemplates	= new UndergrowthSpriteTemplate[0];
	public UndergrowthMeshTemplate[]	undergrowthMeshTemplates	= new UndergrowthMeshTemplate[0];
	public OvergrowthTemplate[]			overgrowthTemplates			= new OvergrowthTemplate[0];
	public DetailLayer[]				detailLayers				= new DetailLayer[0];

	public Texture2DArray				albedoTextureArray;
	public Texture2DArray				specSmoothTextureArray;
	public Texture2DArray				occlusionTextureArray;
	public Texture2DArray				normalTextureArray;
	public Texture2DArray				displacementTextureArray;

	public Texture2DArray				undergrowthAlbedoTextureArray;
	public Texture2DArray				undergrowthSpecSmoothTextureArray;
	public Texture2DArray				undergrowthNormalTextureArray;
	
	public bool IsInited { get { return placeholderMaterial; } }

	public TextureSet 	TextureSetFromGuid		(string guid)	{ return textureSets.SingleOrDefault(x=> x.guid == guid); }
	public int		  	TextureSetIndexFromGuid	(string guid)	{ return ((IList<TextureSet>)textureSets).IndexOf(TextureSetFromGuid(guid)); }
	public DetailLayer	DetailLayerFromGuid		(string guid)	{ return detailLayers.SingleOrDefault(x => x.guid == guid); }
	public int		  	DetailLayerIndexFromGuid(string guid)	{ return ((IList<DetailLayer>)detailLayers).IndexOf(DetailLayerFromGuid(guid)); }

	public TextureSet UndergrowthTextureSetFromGuid(string guid) {
		for(int i = 0, n = undergrowthTextureSets.Length; i < n; ++i)
			if(undergrowthTextureSets[i].guid == guid)
				return undergrowthTextureSets[i];
		return null;
	}
	public int  UndergrowthTextureSetIndexFromGuid(string guid) {
		for(int i = 0, n = undergrowthTextureSets.Length; i < n; ++i)
			if(undergrowthTextureSets[i].guid == guid)
				return i;
		return -1;
	}
	public UndergrowthSpriteTemplate UndergrowthSpriteTemplateFromGuid(string guid) {
		for(int i = 0, n = undergrowthSpriteTemplates.Length; i < n; ++i)
			if(undergrowthSpriteTemplates[i].guid == guid)
				return undergrowthSpriteTemplates[i];
		return null;
	}
	public int UndergrowthSpriteTemplateIndexFromGuid(string guid) {
		for(int i = 0, n = undergrowthSpriteTemplates.Length; i < n; ++i)
			if(undergrowthSpriteTemplates[i].guid == guid)
				return i;
		return -1;
	}
	
	public Material GetMaterial(TETerrainShardData.ElementType materialType) {
		switch(materialType) {
			case TETerrainShardData.ElementType.Far:	return farMaterial;
			case TETerrainShardData.ElementType.Default:return defaultMaterial;
			case TETerrainShardData.ElementType.Near:	return nearMaterial;
			default: return null;
		}
	}
	

	Vector4 RemapTreshold(Vector4 v) {
		// Incredibly inefficient, but easy to iterate on for now

		var r = v.w - v.x;

		if(v.x == v.y)
			v.x -= 1e-3f;
		else
			v.y = v.x + Mathf.Max(r * v.y, 1e-10f);

		if(v.z == v.w)
			v.w += 1e-3f;
		else
			v.z = v.x + Mathf.Max(r * v.z, 1e-10f);

		return v;
	}

    public void RebindMaterial(TETerrainShardData.ElementType kind, MaterialPropertyBlock mpb) {
		if(textureSets == null || textureSets.Length == 0 || albedoTextureArray == null)
			return;

        if(kind == TETerrainShardData.ElementType.Default) {
			var detailLayersCount = detailLayers.Length;

			var u_DetailLayersAlbedoTint				= new Vector4[detailLayersCount];
			var u_DetailLayersSpecularTint				= new Vector4[detailLayersCount];
			var u_DetailLayersSmtNrmDspOccScales		= new Vector4[detailLayersCount];
			var u_DetailLayersDisplacementScaleOffset	= new Vector4[detailLayersCount];
			var u_DetailLayersUVScaleArrayMultiIndex	= new Vector4[detailLayersCount];
			var u_DetailLayersSlopeThresholds			= new Vector4[detailLayersCount];

			for(int i = 0; i < detailLayersCount; ++i) {
				var dl = detailLayers[i];
				var ts = TextureSetFromGuid(dl.textureSet);

				u_DetailLayersAlbedoTint[i]				= ts.albedoTint.linear * ts.albedoTintScale * dl.textureAlbedoTint.linear;
				u_DetailLayersSpecularTint[i]			= ts.specularTint.linear * ts.specularTintScale * dl.textureSpecularTint.linear;
				u_DetailLayersSmtNrmDspOccScales[i]		= new Vector4(ts.smoothnessScale * dl.textureSmoothnessScale, ts.normalScale * dl.textureNormalScale, ts.displacementScale * dl.textureDisplacementScale, ts.occlusionScale * dl.textureOcclusionScale);
				u_DetailLayersDisplacementScaleOffset[i]= new Vector4(ts.displacementScale * dl.textureDisplacementScale, ts.displacementOffset + dl.textureDisplacementOffset, ts.displacementGeomScale * dl.textureDisplacementGeomScale, 0f);
				u_DetailLayersUVScaleArrayMultiIndex[i]	= new Vector4(-1f / (ts.uvScale * dl.textureUVScale.x), dl.textureUVTileRotation ? 1.5f : 0f, 0.25f + (float)TextureSetIndexFromGuid(dl.textureSet), dl.textureUVMultiProjection ? 1.5f : 0f );
				u_DetailLayersSlopeThresholds[i]		= RemapTreshold(dl.slopeThreshold);
			}
			mpb.SetVectorArray("u_DetailLayersAlbedoTint"				, u_DetailLayersAlbedoTint);
			mpb.SetVectorArray("u_DetailLayersSpecularTint"				, u_DetailLayersSpecularTint);
			mpb.SetVectorArray("u_DetailLayersSmtNrmDspOccScales"		, u_DetailLayersSmtNrmDspOccScales);
			mpb.SetVectorArray("u_DetailLayersDisplacementScaleOffset"	, u_DetailLayersDisplacementScaleOffset);
			mpb.SetVectorArray("u_DetailLayersUVScaleArrayMultiIndex"	, u_DetailLayersUVScaleArrayMultiIndex);
			mpb.SetVectorArray("u_DetailLayersSlopeThresholds"			, u_DetailLayersSlopeThresholds);
		}
	}
}
