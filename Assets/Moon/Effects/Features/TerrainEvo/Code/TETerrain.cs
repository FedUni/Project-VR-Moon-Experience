using UnityEngine;
using System.Collections.Generic;

[SelectionBase]
[ExecuteInEditMode]
public partial class TETerrain : MonoBehaviour {
	/*[HideInInspector]*/ public TETerrainData				terrainData;
	/*[HideInInspector]*/ public TETerrainMaterialData		materialData;
	/*[HideInInspector]*/ public TETerrainMeshData			meshData;
	/*[HideInInspector]*/ public List<TETerrainShard>		shards;

	public bool vegetationEnabled;
	public bool vegetationCullingEnabled = true;
	public bool vegetationCullingEnabled2 = true;
	public bool vegetationCastShadows;
	public float vegetationShadowDistance = 50f;
	public bool vegetationGenerationEnabled;
	public bool vegetationGenerationEnabledEditor = true;
	public bool vegetationReconstructedBlend;
	public bool vegetationDeferredTransmission;
	public float vegetationDensityScale = 1f;


	public int	farMeshEdgeLength;
	public int	farMeshEdgeQuadCount;

	public int	defaultMeshEdgeLength;
	public int	defaultMeshEdgeQuadCount;

	public int	nearMeshEdgeLength;
	public int	nearMeshEdgeQuadCount;

	/*[HideInInspector]*/[SerializeField] bool m_isInitialized;
	
	public bool IsInited { get { return m_isInitialized; } }
	

	public void ForceRebindMaterials() {
		if(shards != null)
			foreach(var shard in shards)
				shard.ForceRebindMaterials();
	}

	void Awake() {
		Awake_Detail();
	}

	void OnEnable() {
		// Populates all MaterialPropertyBlocks with shard specific data.
		// Needs to happen after every serialization (playmode transition, domain reload etc)
		ForceRebindMaterials();

		if(vegetationEnabled)
			OnEnable_Detail();
	}
	void OnDisable() {
		OnDisable_Detail();
	}
}
