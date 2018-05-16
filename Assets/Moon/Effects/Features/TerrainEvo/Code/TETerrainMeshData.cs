using UnityEngine;
using System.Collections.Generic;

public class TETerrainMeshData : ScriptableObject, ISerializationCallbackReceiver {
	[System.Serializable]
	class MeshInfo {
		public Mesh		mesh;
		public int		edgeLen;
		public int		edgeQuads;
		public Vector4	uvScaleOffset;
	}

	[System.Serializable]
	class MeshSet {
		public MeshInfo[] meshes;
	}


	Dictionary<int, MeshSet>		m_renderElementMeshMap;
	[SerializeField] List<int>		m_renderElements;
	[SerializeField] List<MeshSet>	m_meshSets;
	//[SerializeField] MeshInfo		m_defaultSharedMesh;

	static public TETerrainMeshData Create() {
		return ScriptableObject.CreateInstance<TETerrainMeshData>();
	}

#if UNITY_EDITOR
	static public TETerrainMeshData Create(string name) {
		var data = Create();
		UnityEditor.AssetDatabase.CreateAsset(data, string.Format("Assets/{0}_MeshData.asset", name));
		return data;
	}
#endif

	public bool IsInited { get { return true; } }

	public void Init() {
		Debug.Log("Init: " + GetInstanceID());
		m_renderElementMeshMap = new Dictionary<int, MeshSet>();
	}

	public Mesh GetMesh(TETerrainShardData.ElementType meshType, TETerrainShardRenderElement renderElement, TETerrainShardData shardData, int edgeLength, int edgeQuadCount, Vector4 uvScaleOffset, bool forceUpdateHeight, bool forceUpdateNormal, string label, bool isTemp) {
		if(isTemp) {
			return CreateMesh(shardData, edgeLength, edgeQuadCount, false, uvScaleOffset, label);
		}

		MeshSet meshSet = null;
		if(!m_renderElementMeshMap.TryGetValue(renderElement.elementHash, out meshSet)) {
			meshSet = m_renderElementMeshMap[renderElement.elementHash] = new MeshSet();
			meshSet.meshes = new MeshInfo[3];
		}

		MeshInfo meshInfo = meshSet.meshes[(int)meshType];
		if(meshInfo != null && meshInfo.mesh) {
			if(meshInfo.edgeLen == edgeLength && meshInfo.edgeQuads == edgeQuadCount && meshInfo.uvScaleOffset == uvScaleOffset) {
				if(forceUpdateHeight || forceUpdateNormal) {
					UpdateMesh(meshInfo.mesh, shardData, edgeLength, edgeQuadCount, false , uvScaleOffset, forceUpdateHeight, forceUpdateNormal);

#if UNITY_EDITOR
					UnityEditor.EditorUtility.SetDirty(this);
#endif
				}
				return meshInfo.mesh;
			}

			Object.DestroyImmediate(meshInfo.mesh, true);
		}

		meshInfo = meshSet.meshes[(int)meshType] = CreateMeshInfo(shardData, edgeLength, edgeQuadCount, false /*meshType != TETerrainShardData.ElementType.Far*/, uvScaleOffset, label);

#if UNITY_EDITOR
		UnityEditor.AssetDatabase.AddObjectToAsset(meshInfo.mesh, this);
		UnityEditor.EditorUtility.SetDirty(this);
#endif

		return meshInfo.mesh;
	}

#if UNITY_EDITOR
	public void PurgeAllData() {
		m_renderElementMeshMap.Clear();
		//m_defaultSharedMesh = null;

		var assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(UnityEditor.AssetDatabase.GetAssetPath(this));
		foreach(var asset in assets) {
			if(asset == this)
				continue;

			Object.DestroyImmediate(asset, true);
		}

		UnityEditor.EditorUtility.SetDirty(this);
	}
#endif

	public void OnAfterDeserialize() {
		m_renderElementMeshMap = new Dictionary<int, MeshSet>();

		for(int i = 0; i < m_renderElements.Count; ++i)
			m_renderElementMeshMap[m_renderElements[i]] = m_meshSets[i];

		m_renderElements.Clear();
		m_meshSets.Clear();
	}

	public void OnBeforeSerialize() {
		if(m_renderElementMeshMap == null)
			return;

		m_renderElements = new List<int>(m_renderElementMeshMap.Count);
		m_meshSets = new List<MeshSet>(m_renderElementMeshMap.Count);

		foreach(var kvp in m_renderElementMeshMap) {
			m_renderElements.Add(kvp.Key);
			m_meshSets.Add(kvp.Value);
		}
	}

	static MeshInfo CreateMeshInfo(TETerrainShardData shardData, int edgeLength, int edgeQuadCount, bool quadMesh, Vector4 uvScaleOffset, string label) {
		var mi = new MeshInfo();
		mi.mesh = CreateMesh(shardData, edgeLength, edgeQuadCount, quadMesh, uvScaleOffset, label);
		mi.edgeLen = edgeLength;
		mi.edgeQuads = edgeQuadCount;
		mi.uvScaleOffset = uvScaleOffset;
		return mi;
	}

	static Mesh CreateMesh(TETerrainShardData shard, int edgeLength, int edgeQuadCount, bool quadMesh, Vector4 uvScaleOffset, string label) {
		var quadEdgeLength = edgeLength / (float)edgeQuadCount;
		var vertexCount = edgeQuadCount + 1;
		var rcpUVScale = 1f / (float)edgeQuadCount;
		uvScaleOffset.x *= rcpUVScale;
		uvScaleOffset.y *= rcpUVScale;

		var heightMap = shard.heightData;
		var heightScaleOffset = new Vector2(1f, 0f);

		var pos = new Vector3[vertexCount * vertexCount];
		var nrm = new Vector3[vertexCount * vertexCount];
		var uv0 = new Vector2[vertexCount * vertexCount];

		for(int z = 0; z < vertexCount; ++z) {
			for(int x = 0; x < vertexCount; ++x) {
				var uv = new Vector2(x * uvScaleOffset.x + uvScaleOffset.z, z * uvScaleOffset.y + uvScaleOffset.w);

				// TODO: Optimize normal extraction
				
				var h = heightMap.GetPixelBilinear(uv.x, uv.y).r * heightScaleOffset.x + heightScaleOffset.y;
				var hp1p0 = heightMap.GetPixelBilinear(uv.x + uvScaleOffset.x, uv.y).r * heightScaleOffset.x + heightScaleOffset.y;
				var hm1p0 = heightMap.GetPixelBilinear(uv.x - uvScaleOffset.x, uv.y).r * heightScaleOffset.x + heightScaleOffset.y;
				var hp0p1 = heightMap.GetPixelBilinear(uv.x, uv.y + uvScaleOffset.y).r * heightScaleOffset.x + heightScaleOffset.y;
				var hp0m1 = heightMap.GetPixelBilinear(uv.x, uv.y - uvScaleOffset.y).r * heightScaleOffset.x + heightScaleOffset.y;
				var normal = Vector3.Cross(
					new Vector3(quadEdgeLength * 2f, hp1p0 - hm1p0, 0f),
					new Vector3(0f, hp0p1 - hp0m1, -quadEdgeLength * 2f)
				).normalized;

				pos[z * vertexCount + x] = new Vector3(x * quadEdgeLength, h, z * quadEdgeLength);
				nrm[z * vertexCount + x] = normal;
				uv0[z * vertexCount + x] = uv;
			}
		}

		var idx = new int[edgeQuadCount * edgeQuadCount * (quadMesh ? 4 : 6)];
		for(int z = 0, i = 0; z < edgeQuadCount; ++z) {
			for(int x = 0; x < edgeQuadCount; ++x) {
				idx[i++] = (z + 0) * vertexCount + (x + 1);
				idx[i++] = (z + 0) * vertexCount + (x + 0);

				if(quadMesh) {
					idx[i++] = (z + 1) * vertexCount + (x + 0);
					idx[i++] = (z + 1) * vertexCount + (x + 1);
				} else {
					idx[i++] = (z + 1) * vertexCount + (x + 1);

					idx[i++] = (z + 0) * vertexCount + (x + 0);
					idx[i++] = (z + 1) * vertexCount + (x + 0);
					idx[i++] = (z + 1) * vertexCount + (x + 1);
				}
			}
		}

		var m = new Mesh();
		m.name = label;
		m.vertices = pos;
		m.normals = nrm;
		m.uv = uv0;
		m.SetIndices(idx, quadMesh ? MeshTopology.Quads : MeshTopology.Triangles, 0);
		return m;
	}

	static void UpdateMesh(Mesh  mesh, TETerrainShardData shard, int edgeLength, int edgeQuadCount, bool quadMesh, Vector4 uvScaleOffset, bool updateHeight, bool updateNormal) {
		var quadEdgeLength = edgeLength / edgeQuadCount;
		var vertexCount = edgeQuadCount + 1;
		var rcpUVScale = 1f / (float)edgeQuadCount;
		uvScaleOffset.x *= rcpUVScale;
		uvScaleOffset.y *= rcpUVScale;

		var heightMap = shard.heightData;
		var heightScaleOffset = new Vector2(1f, 0f);

		Vector3[] pos = updateHeight ? mesh.vertices : null;
		Vector3[] nrm = updateNormal ? mesh.normals : null;

		for(int z = 0; z < vertexCount; ++z) {
			for(int x = 0; x < vertexCount; ++x) {
				var uv = new Vector2(x * uvScaleOffset.x + uvScaleOffset.z, z * uvScaleOffset.y + uvScaleOffset.w);
				var h = heightMap.GetPixelBilinear(uv.x, uv.y).r * heightScaleOffset.x + heightScaleOffset.y;

				if(updateNormal) {
					// TODO: Optimize normal extraction
					var hp1p0 = heightMap.GetPixelBilinear(uv.x + uvScaleOffset.x, uv.y).r * heightScaleOffset.x + heightScaleOffset.y;
					var hm1p0 = heightMap.GetPixelBilinear(uv.x - uvScaleOffset.x, uv.y).r * heightScaleOffset.x + heightScaleOffset.y;
					var hp0p1 = heightMap.GetPixelBilinear(uv.x, uv.y + uvScaleOffset.y).r * heightScaleOffset.x + heightScaleOffset.y;
					var hp0m1 = heightMap.GetPixelBilinear(uv.x, uv.y - uvScaleOffset.y).r * heightScaleOffset.x + heightScaleOffset.y;
					var normal = Vector3.Cross(
						new Vector3(quadEdgeLength * 2f, hp1p0 - hm1p0, 0f),
						new Vector3(0f, hp0p1 - hp0m1, -quadEdgeLength * 2f)
					).normalized;

					nrm[z * vertexCount + x] = normal;
				}

				if(updateHeight)
					pos[z * vertexCount + x].y = h;
			}
		}

		if(updateNormal)
			mesh.normals = nrm;

		if(updateHeight) {
			mesh.vertices = pos;
			mesh.RecalculateBounds();
		}
	}
}
