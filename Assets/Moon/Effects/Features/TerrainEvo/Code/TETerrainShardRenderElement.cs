using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TETerrainShardRenderElement : MonoBehaviour {
	public TETerrainShard					owner;
	public TETerrainShardData.ElementType	elementType;
	public int								edgeLength;
	public int								edgeQuadCount;
	public int								subX;
	public int								subZ;
	public int								parentEdgeElements;
	public bool								isTemp;
	public Mesh mesh { get; private set; }
	public int elementHash {
		get {
			return ((owner.shardData.shardX + 128) << 24) | ((owner.shardData.shardZ + 128) << 16) | (subX << 12) | (subZ << 8) | edgeLength | edgeQuadCount;
		}
	}

	MaterialPropertyBlock m_materialPropertyBlock;
	
	public void ForceRebindMaterials(bool mpbOnly = false) {
		if(mpbOnly) {
			var m = GetComponent<MeshRenderer>();
			m.SetPropertyBlock(m_materialPropertyBlock);
			return;
		}

		if(m_materialPropertyBlock == null)
			m_materialPropertyBlock = new MaterialPropertyBlock();
		else
			m_materialPropertyBlock.Clear();

		var fParentEdgesRcp = 1f / (float)parentEdgeElements;
		//var heightTexScale = new Vector2(fParentEdgesRcp, fParentEdgesRcp);
		var heightTexScale = new Vector2(1f / edgeLength, 1f / edgeLength);
		var heightTexOffset = new Vector2(subX * fParentEdgesRcp, subZ * fParentEdgesRcp);
		m_materialPropertyBlock.SetTexture("u_Heightmap", owner.shardData.HeightTexture);
		m_materialPropertyBlock.SetVector("u_Heightmap_ST", new Vector4(heightTexScale.x, heightTexScale.y, heightTexOffset.x, heightTexOffset.y));
		if(owner.shardData.HeightDataAdapter != null && owner.shardData.HeightDataAdapter.GetReferenceTexture())
			m_materialPropertyBlock.SetTexture("u_HeightmapRef", owner.shardData.HeightDataAdapter.GetReferenceTexture());
		else
			m_materialPropertyBlock.SetTexture("u_HeightmapRef", owner.shardData.HeightTexture);
		m_materialPropertyBlock.SetVector("u_HeightmapRef_ST", new Vector4(heightTexScale.x, heightTexScale.y, heightTexOffset.x, heightTexOffset.y));

		m_materialPropertyBlock.SetTexture("u_Controlmap", owner.shardData.ControlTexture);
		m_materialPropertyBlock.SetTexture("u_Colormap", owner.shardData.ColorTexture);

		owner.owner.materialData.RebindMaterial(elementType, m_materialPropertyBlock);

		var mr = GetComponent<MeshRenderer>();
		mr.SetPropertyBlock(m_materialPropertyBlock);
	}

	void Update() {
#if UNITY_EDITOR
		if(m_materialPropertyBlock == null || m_materialPropertyBlock.GetTexture("u_Heightmap") == null || m_materialPropertyBlock.GetTexture("u_Controlmap") == null || m_materialPropertyBlock.GetTexture("u_Colormap") == null)
			ForceRebindMaterials();
#endif
	}
}
