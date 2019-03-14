using UnityEngine;
using System.Collections.Generic;

// NOTE: Texture/RenderTexture format support in Unity is awful! Even worse when you need to copy/read data. TODO: FIX IT (actually, try to convince someone else to fix it).
//
// Done some work in adding R16_UNORM / R16_UINT support and readback of such.
//

public interface ITETerrainShardDataAdapter {
	Texture GetTexture();
	Texture GetReferenceTexture();
}

public class TETerrainShardData : ScriptableObject/*, ISerializationCallbackReceiver*/ {
	public enum ElementType		{ Far, Default, Near, Unspecified }
	public enum BitDepth		{ Eight = 1, Sixteen = 2 }

	public int					heightScale			= 150;
	public int					heightOffset		= 0;

	public BitDepth				heightmapBitDepth	= BitDepth.Sixteen;
	public int					heightmapSize		= 1000;
	public int					controlMaskSize		= 1000;
	public int					colormapSize		= 250;

	public TETerrainData		owner;
	public int					shardX;
	public int					shardZ;

	public Texture2D			heightData;
	public Texture2D			controlData;
	public Texture2D			colorData;

	public ITETerrainShardDataAdapter	HeightDataAdapter	{ get; set; }
	public ITETerrainShardDataAdapter	ControlDataAdapter	{ get; set; }
	public ITETerrainShardDataAdapter	ColorDataAdapter	{ get; set; }

	public Texture						HeightTexture		{ get {
		if(HeightDataAdapter != null && HeightDataAdapter.GetTexture() == null) {
			Debug.LogError("Empty height data adapter!");
			return heightData;
		}
		return HeightDataAdapter != null ? HeightDataAdapter.GetTexture() : heightData;
	} }

	public Texture						ControlTexture		{ get {
		if(ControlDataAdapter != null && ControlDataAdapter.GetTexture() == null) {
			Debug.LogError("Empty control data adapter!");
			return controlData;
		}
		return ControlDataAdapter != null ? ControlDataAdapter.GetTexture() : controlData;
	} }

	public Texture						ColorTexture		{ get {
		if(ColorDataAdapter != null && ColorDataAdapter.GetTexture() == null) {
			Debug.LogError("Empty color data adapter!");
			return colorData;
		}
		return ColorDataAdapter != null ? ColorDataAdapter.GetTexture() : colorData;
	} }

	static public TETerrainShardData Create() {
		return ScriptableObject.CreateInstance<TETerrainShardData>();
	}

#if UNITY_EDITOR
	static public TETerrainShardData Create(string name) {
		var data = Create();
		UnityEditor.AssetDatabase.CreateAsset(data, string.Format("Assets/{0}_ShardData.asset", name));
		return data;
	}
#endif

	public bool IsInited { get { return heightData != null; } }

	public void Init(BitDepth heightmapBitDepth, int heightmapSize, int controlMaskSize, int colormapSize) {
		this.heightmapBitDepth = heightmapBitDepth;
		this.heightmapSize = heightmapSize = Mathf.NextPowerOfTwo(heightmapSize); //TODO: Render with overscan 
		this.controlMaskSize = controlMaskSize;
		this.colormapSize = colormapSize = Mathf.NextPowerOfTwo(colormapSize); //TODO: Render with overscan 

		heightData = new Texture2D(heightmapSize, heightmapSize, TextureFormat.RHalf, false, true);
		var heightDefaultData = new byte[heightmapSize * heightmapSize * 2 * 4 / 3];
		heightData.LoadRawTextureData(heightDefaultData);
		heightData.wrapMode = TextureWrapMode.Clamp;
		heightData.filterMode = FilterMode.Bilinear;
		heightData.Apply(false, false);

		controlData = new Texture2D(controlMaskSize, controlMaskSize, TextureFormat.RFloat, false, true);
		var controlZeroData = new byte[controlMaskSize * controlMaskSize * 4];
		for(int i = 0; i < controlZeroData.Length; i += 4) {
			controlZeroData[i + 0] = 0xF0;
			controlZeroData[i + 1] = 0x0F;
			controlZeroData[i + 2] = 0x00;
			controlZeroData[i + 3] = 0x00;
		}
		controlData.LoadRawTextureData(controlZeroData);
		controlData.wrapMode = TextureWrapMode.Clamp;
		controlData.filterMode = FilterMode.Point;
		controlData.Apply(false, false);

		colorData = new Texture2D(colormapSize, colormapSize, TextureFormat.RGB24, true, false);
		var colorZeroData = new Color32[colormapSize * colormapSize];
		var gray = (Color32)Color.gray.gamma;
		for(int i = 0; i < colorZeroData.Length; ++i)
			colorZeroData[i] = gray;
		colorData.SetPixels32(colorZeroData);
		colorData.wrapMode = TextureWrapMode.Clamp;
		colorData.filterMode = FilterMode.Trilinear;
		colorData.Apply(true, false);
#if UNITY_EDITOR
		UnityEditor.EditorUtility.CompressTexture(colorData, TextureFormat.DXT1, (int)TextureCompressionQuality.Fast);
#endif

#if UNITY_EDITOR
		UnityEditor.AssetDatabase.AddObjectToAsset(heightData, this);
		UnityEditor.AssetDatabase.AddObjectToAsset(controlData, this);
		UnityEditor.AssetDatabase.AddObjectToAsset(colorData, this);
		UnityEditor.EditorUtility.SetDirty(this);
#endif
	}

	//public void OnBeforeSerialize() {
	//	Debug.LogFormat("OnBeforeSerialize ShardData: {0} / {1} ({2})", shardX, shardZ, GetInstanceID());
	//	Debug.Log(heightData);
	//	Debug.Log(HeightDataAdapter);
	//	Debug.Log(HeightDataAdapter != null ? HeightDataAdapter.GetTexture() : null);
	//}
	//public void OnAfterDeserialize() {
	//	Debug.LogFormat("v ShardData: {0} / {1} ({2})", shardX, shardZ, GetInstanceID());
	//}
	//void OnValidate() {
	//	Debug.LogFormat("OnValidate ShardData: {0} / {1} ({2})", shardX, shardZ, GetInstanceID());
	//}
	//void OnEnable() {
	//	Debug.LogFormat("OnEnable ShardData: {0} / {1} ({2})", shardX, shardZ, GetInstanceID());
	//}
	//void OnDisable() {
	//	Debug.LogFormat("OnDisable ShardData: {0} / {1} ({2})", shardX, shardZ, GetInstanceID());
	//}

	void OnDestroy() {
		Debug.LogFormat("OnDestroy ShardData: {0} / {1} ({2})", shardX, shardZ, GetInstanceID());

		Object.DestroyImmediate(heightData, true);
		Object.DestroyImmediate(controlData, true);
		Object.DestroyImmediate(colorData, true);
	}
}
