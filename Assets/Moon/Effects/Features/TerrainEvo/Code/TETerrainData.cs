using UnityEngine;
using System.Collections.Generic;

public class TETerrainData : ScriptableObject {
	public int	shardEdgeLength				= 500;

	public int								minShardX, maxShardX, minShardZ, maxShardZ;
	public List<TETerrainShardData>			shards;

	[System.NonSerialized]
	public Dictionary<uint, int> m_shardMapping;


	static public TETerrainData Create() {
		var data = ScriptableObject.CreateInstance<TETerrainData>();
		return data;
	}

#if UNITY_EDITOR
	static public TETerrainData Create(string name) {
		var data = Create();
		UnityEditor.AssetDatabase.CreateAsset(data, string.Format("Assets/{0}_TerrainData.asset", name));
		return data;
	}
#endif

	public bool IsInited { get { return shards != null; } }

	public void Init(int shardEdgeLength) {
		this.shardEdgeLength = shardEdgeLength;
		shards = new List<TETerrainShardData>();
	}

	static uint MakeKey(int x, int z) { return ((uint)((x + 32767) & 0xFFFF) << 16) | (uint)((z + 32767) & 0xFFFF); }

	public void AddShard(int x, int z, TETerrainShardData data) {
		Debug.Assert(!m_shardMapping.ContainsKey(MakeKey(x,z)));
		m_shardMapping[MakeKey(x,z)] = shards.Count;
		shards.Add(data);
		data.owner = this;
		data.shardX = x;
		data.shardZ = z;
		RecalculateMinMax();
	}

	public void RemoveShard(int x, int z) {
		var shardKey = MakeKey(x, z);
		shards.RemoveAt(m_shardMapping[shardKey]);
		m_shardMapping.Remove(shardKey);
		RemapShards();
		RecalculateMinMax();
	}

	public TETerrainShardData GetShard(int x, int z) {
		int idx;
		if(m_shardMapping.TryGetValue(MakeKey(x, z), out idx))
			return shards[idx] ;
		return null;
	}

	void OnEnable() {
		m_shardMapping = new Dictionary<uint,int>();

		if(shards != null)
			RemapShards();
	}

	void RemapShards() {
		for(int i = 0; i < shards.Count; ++i) {
			var s = shards[i];
			m_shardMapping[MakeKey(s.shardX, s.shardZ)] = i;
		}
	}

	void RecalculateMinMax() {
		minShardX = minShardZ = shards.Count > 0 ? int.MaxValue : 0;
		maxShardX = maxShardZ = shards.Count > 0 ? int.MinValue : 0;

		foreach(var shard in shards) {
			minShardX = Mathf.Min(shard.shardX, minShardX);
			minShardZ = Mathf.Min(shard.shardZ, minShardZ);
			maxShardX = Mathf.Max(shard.shardX + 1, maxShardX);
			maxShardZ = Mathf.Max(shard.shardZ + 1, maxShardZ);
		}
	}
}
