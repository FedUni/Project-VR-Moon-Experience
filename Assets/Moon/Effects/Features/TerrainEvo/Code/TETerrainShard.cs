using UnityEngine;

public class TETerrainShard : MonoBehaviour {
	public TETerrain						owner;
	public TETerrainShardData				shardData;

	//public TETerrainShardRenderElement		renderElementFar;
	public GameObject						renderElementDefaultGroup;
	public TETerrainShardRenderElement[]	renderElementsDefault;
	//public TETerrainShardRenderElement	renderElementNear;

	public void ForceRebindMaterials(bool mpbOnly = false) {
		//renderElementFar.ForceRebindMaterials(mpbOnly);
		foreach(var re in renderElementsDefault)
			re.ForceRebindMaterials(mpbOnly);
		//renderElementNear.ForceRebindMaterials();
	}
}
