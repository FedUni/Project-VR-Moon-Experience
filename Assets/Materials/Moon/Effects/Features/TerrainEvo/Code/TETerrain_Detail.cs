
#define PLAYMODE_CACHE_MATERIAL_MAPS

using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public partial class TETerrain {
	public bool needsRegeneration { get; set; }
	static public bool globalNeedsRegeneration { get; set; }

	public int detailCellSize		= 20;
	public int detailCellEdgeCount	= 5;
	public int detailCaptureSize	= 256;

	public Shader			detailCaptureShader;
	public ComputeShader	detailSpawnShader;
	public Shader			detailDrawShader;
	public Texture2D		detailNoiseRG;


	Camera		m_detailCaptureCamera;
	Material	m_detailDrawMaterial;

	ComputeBuffer[] m_detailMeshBuffers;
	ComputeBuffer[] m_detailArgBuffers;
	ComputeBuffer	m_detailSpriteTemplatesBuffer;
	ComputeBuffer	m_detailDetailLayersBuffer;
	ComputeBuffer	m_detailMaterialStartOffsetBuffer;

	SpriteTemplate[]		m_detailSpriteTemplatesData;
	DetailLayer[]			m_detailDetailLayersData;
	uint[]					m_detailMaterialStartOffsetData;
	Dictionary<string, uint>m_detailSpriteTemplateIndexMap;

	Dictionary<TETerrainShardData, RenderTexture> m_flattenedShards;

	int[] m_args0100 = new int[] { 0, 1, 0, 0 };

	Dictionary<Camera, CommandBuffer> m_detailHookedCams;
	Dictionary<Light, CommandBuffer> m_detailHookedLights;

	Plane[] m_detailFrustumBacklitPlanes = new Plane[6];

#if PLAYMODE_CACHE_MATERIAL_MAPS
	Dictionary<Camera, Dictionary<TETerrainShardData, RenderTexture>> m_cachedFlattenedShards;
#endif

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
	struct SpriteTemplate {
		public Vector4	uvScaleOffset;
		public Vector3	widthMinMaxMedian;
		public uint		uvArrayIndex;
		public Vector3	heightMinMaxMedian;
		public uint		growsInWater;
		public Vector3	groundColorFactorBottomTopMid;
		public uint		spriteMode;
		public Vector3	groundColorFactorScaleBottomTopMid;
		public float	_pad0;
		public Vector3	groundWetnessDarkenFactorBottomTopMid;
		public float	_pad1;
		public Vector3	albedoTint;
		public float	normalScale;
		public Vector3	specularTint;
		public float	smoothnessScale;

	};

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
	struct DetailLayer {
		public Vector2	widthHeightScale;
		public float	probability;
		public uint		templateIndex;
	};

	void Awake_Detail() {
		var mf = GetComponent<MeshFilter>();
		if(!mf || !mf.sharedMesh) {
			if(!mf)
				mf = gameObject.AddComponent<MeshFilter>();
			mf.sharedMesh = new Mesh();
			mf.sharedMesh.name = "Callbackproxy";
			mf.sharedMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);
			mf.sharedMesh.SetTriangles((int[])null, 0);
		}

		if(!GetComponent<MeshRenderer>()) {
			var mr = gameObject.AddComponent<MeshRenderer>();
			mr.receiveShadows = false;
			mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
			mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
		}
	}

	void OnEnable_Detail() {
		var go = new GameObject("TETErrain_Detail_FlattenCam");
		go.hideFlags = HideFlags.HideAndDontSave; /*HideFlags.DontSave*/
		var cam = m_detailCaptureCamera = go.AddComponent<Camera>();
		cam.clearFlags = CameraClearFlags.SolidColor;
		cam.backgroundColor = Color.black;
		cam.renderingPath = RenderingPath.Forward;
		cam.useOcclusionCulling = false;
		cam.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
		cam.orthographic = true;
		cam.orthographicSize = farMeshEdgeLength / 2f;
		cam.aspect = 1f;
		cam.nearClipPlane = 0f;
		cam.farClipPlane = 2000f;
		cam.SetReplacementShader(detailCaptureShader, ""/*"Special"*/);
		cam.cullingMask = 1 << LayerMask.NameToLayer("Terrain");
		cam.enabled = false;

		m_detailDrawMaterial = new Material(detailDrawShader);
		m_detailDrawMaterial.hideFlags = HideFlags.HideAndDontSave;
		RebindDetailMaterial();

		Shader.SetGlobalInt("_ReconstructedBlend", 0);
		Shader.SetGlobalInt("_DeferredTransmission", 0);
				
		m_detailMeshBuffers = new ComputeBuffer[detailCellEdgeCount * detailCellEdgeCount];
		m_detailArgBuffers = new ComputeBuffer[detailCellEdgeCount * detailCellEdgeCount];
		for(int i = 0, n = detailCellEdgeCount * detailCellEdgeCount; i < n; ++i) {
			var meshBuffer = new ComputeBuffer(detailCellSize * detailCellSize * 7 * 7 * 4, sizeof(float) * 20, ComputeBufferType.Append);
			m_detailMeshBuffers[i] = meshBuffer;

			var argBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
			argBuffer.SetData(m_args0100);
			m_detailArgBuffers[i] = argBuffer;
		}

		m_detailSpriteTemplatesBuffer = new ComputeBuffer(32, System.Runtime.InteropServices.Marshal.SizeOf(default(SpriteTemplate)), ComputeBufferType.Default);
		m_detailDetailLayersBuffer = new ComputeBuffer(16 * 2 * 8, System.Runtime.InteropServices.Marshal.SizeOf(default(DetailLayer)), ComputeBufferType.Default);
		m_detailMaterialStartOffsetBuffer = new ComputeBuffer(16, sizeof(uint), ComputeBufferType.Default);

		m_detailSpriteTemplatesData = new SpriteTemplate[32];
		m_detailDetailLayersData = new DetailLayer[16 * 2 * 8];
		m_detailMaterialStartOffsetData = new uint[16];

		m_detailSpriteTemplateIndexMap = new Dictionary<string, uint>();

		m_flattenedShards = new Dictionary<TETerrainShardData, RenderTexture>();
#if PLAYMODE_CACHE_MATERIAL_MAPS
		m_cachedFlattenedShards = new Dictionary<Camera, Dictionary<TETerrainShardData, RenderTexture>>();
#endif

		m_detailHookedCams = new Dictionary<Camera, CommandBuffer>();
		m_detailHookedLights = new Dictionary<Light, CommandBuffer>();

		needsRegeneration = true;
	}


	void OnWillRenderObject() {
		if(m_detailHookedCams == null)
			return;

		if(
			Camera.current != Camera.main
#if UNITY_EDITOR
			&& (UnityEditor.SceneView.currentDrawingSceneView == null || UnityEditor.SceneView.currentDrawingSceneView.camera != Camera.current)
#endif
		){
			return;
		}

		//RebindDetailMaterial();

		if(!vegetationEnabled)
			return;

		if(needsRegeneration || globalNeedsRegeneration || vegetationGenerationEnabled || (vegetationGenerationEnabledEditor && Application.isEditor && !Application.isPlaying)) {
#if UNITY_5_5_OR_NEWER
			PrepareGenerationParams();
			GenerateDetailData(Camera.current);
			needsRegeneration = globalNeedsRegeneration = false;
#endif
		}

		CommandBuffer detailCommandBuffer = null;
		if(!m_detailHookedCams.TryGetValue(Camera.current, out detailCommandBuffer)) {
			detailCommandBuffer = new CommandBuffer();
			detailCommandBuffer.name = "Detail Instances";

			Camera.current.AddCommandBuffer(CameraEvent.AfterGBuffer, detailCommandBuffer);

			m_detailHookedCams[Camera.current] = detailCommandBuffer;
		}

		CommandBuffer detailCommandShadowBuffer = null;
		var activeSun = AtmosphericScatteringSun.instance ? AtmosphericScatteringSun.instance.light : null;
#if UNITY_5_6_OR_NEWER
		if(activeSun && vegetationCastShadows)
		{
			if(!m_detailHookedLights.TryGetValue(activeSun, out detailCommandShadowBuffer)) {
				detailCommandShadowBuffer = new CommandBuffer();
				detailCommandShadowBuffer.name = "Detail Shadow Instances";

				activeSun.AddCommandBuffer(LightEvent.AfterShadowMapPass, detailCommandShadowBuffer, ShadowMapPass.Directional);

				m_detailHookedLights[activeSun] = detailCommandShadowBuffer;
			}
		}
		else
#endif
		if(activeSun && !vegetationCastShadows)
		{
			CommandBuffer cmd = null;
			if(m_detailHookedLights.TryGetValue(activeSun, out cmd))
				cmd.Clear();

		}

		BuildCommandBuffers(Camera.current, activeSun, detailCommandBuffer, detailCommandShadowBuffer);
	}

	void OnDisable_Detail() {
		if(m_detailHookedCams != null) {
			foreach(var kvp in m_detailHookedCams) {
				if(kvp.Key)
					kvp.Key.RemoveCommandBuffer(CameraEvent.AfterGBuffer, kvp.Value);

				kvp.Value.Release();
			}
			m_detailHookedCams.Clear();
			m_detailHookedCams = null;
		}

		if(m_detailHookedLights != null) {
			foreach(var kvp in m_detailHookedLights) {
#if UNITY_5_6_OR_NEWER
				if(kvp.Key)
					kvp.Key.RemoveCommandBuffer(LightEvent.AfterShadowMapPass, kvp.Value);
#endif
				kvp.Value.Release();
			}
			m_detailHookedLights.Clear();
			m_detailHookedLights = null;
		}

		m_detailSpriteTemplatesData = null;
		m_detailDetailLayersData = null;
		m_detailMaterialStartOffsetData = null;
		m_detailSpriteTemplateIndexMap = null;

		if(m_detailSpriteTemplatesBuffer != null) {
			m_detailSpriteTemplatesBuffer.Release();
			m_detailSpriteTemplatesBuffer = null;
			m_detailDetailLayersBuffer.Release();
			m_detailDetailLayersBuffer = null;
			m_detailMaterialStartOffsetBuffer.Release();
			m_detailMaterialStartOffsetBuffer = null;
		}

		if(m_detailMeshBuffers != null && m_detailArgBuffers != null) {
			for(int i = 0, n = detailCellEdgeCount * detailCellEdgeCount; i < n; ++i) {
				if(m_detailMeshBuffers[i] != null)
					m_detailMeshBuffers[i].Release();
				if(m_detailArgBuffers[i] != null)
					m_detailArgBuffers[i].Release();
			}
		}
		m_detailMeshBuffers = null;
		m_detailArgBuffers = null;

		m_flattenedShards = null;

#if PLAYMODE_CACHE_MATERIAL_MAPS
		if(m_cachedFlattenedShards != null)
			foreach(var kvp in m_cachedFlattenedShards)
				foreach(var kvp2 in kvp.Value)
					Object.DestroyImmediate(kvp2.Value);

		m_cachedFlattenedShards = null;
#endif

		Object.DestroyImmediate(m_detailDrawMaterial);
		if(m_detailCaptureCamera != null)
			Object.DestroyImmediate(m_detailCaptureCamera.gameObject);
	}

	SpriteTemplate ConvertTemplate(TETerrainMaterialData.TextureSet ts, int tsi, TETerrainMaterialData.UndergrowthSpriteTemplate ugt) {
		var st = new SpriteTemplate();
		st.uvScaleOffset						= ugt.uvScaleOffset;
		st.widthMinMaxMedian					= ugt.widthMinMaxMedian;// ***
		st.uvArrayIndex							= (uint)tsi;
		st.heightMinMaxMedian					= ugt.heightMinMaxMedian;// **
		st.growsInWater							= ugt.growsInWater ? 1u : 0u;
		st.groundColorFactorBottomTopMid		= ugt.groundColorFactorBottomTopMid;
		st.spriteMode							= (uint)ugt.mode;
		st.groundColorFactorScaleBottomTopMid	= ugt.groundColorFactorScaleBottomTopMid;
		st.groundWetnessDarkenFactorBottomTopMid= ugt.groundWetnessDarkenFactorBottomTopMid;
		st.albedoTint							= (Vector4)(ts.albedoTint.linear * ugt.textureAlbedoTint.linear * ts.albedoTintScale);
		st.normalScale							= ts.normalScale;
		st.specularTint							= (Vector4)(ts.specularTint.linear * ts.specularTintScale);
		st.smoothnessScale						= ts.smoothnessScale;
		return st; 
	}

	DetailLayer ConvertLayer(uint ti, TETerrainMaterialData.UndergrowthDetailLayer ugdl) {
		var dl = new DetailLayer();
		dl.widthHeightScale	= ugdl.widthHeightScale;
		dl.probability		= ugdl.probability;
		dl.templateIndex	= ti;
		return dl;
	}

	void RebindDetailMaterial() {
		m_detailDrawMaterial.SetTexture("_AlbedoArray", materialData.undergrowthAlbedoTextureArray);
		//m_detailDrawMaterial.SetTexture("_PackedArray", materialData.undergrowthSpecSmoothTextureArray);
		m_detailDrawMaterial.SetTexture("_NormalArray", materialData.undergrowthNormalTextureArray);
	}

	void BuildCommandBuffers(Camera cam, Light light, CommandBuffer detailCommandBuffer, CommandBuffer detailCommandShadowBuffer) {
		detailCommandBuffer.Clear();
		if(detailCommandShadowBuffer != null)
			detailCommandShadowBuffer.Clear();

		var camPlanes = GeometryUtility.CalculateFrustumPlanes(cam);

		var camBacklitPlanes = 0;
		if(light) {
			var lightDir = light.transform.forward;
			for(int i = 0; i < 6; ++i) {
				if(Vector3.Dot(lightDir, camPlanes[i].normal) < 0f)
					m_detailFrustumBacklitPlanes[camBacklitPlanes++] = camPlanes[i];
			}
		}

		var focusPos = cam.transform.position;
		var relFocusPos = new Vector3(focusPos.x - transform.position.x, 0f, focusPos.z - transform.position.z);

		Vector3 relFocusCornerPos;
		relFocusCornerPos.x = Mathf.Floor(relFocusPos.x / detailCellSize) * detailCellSize;
		relFocusCornerPos.z = Mathf.Floor(relFocusPos.z / detailCellSize) * detailCellSize;
		relFocusCornerPos.y = 0;

		for(int z = -detailCellEdgeCount / 2, idx = 0; z <= detailCellEdgeCount / 2; ++z) {
			for(int x = -detailCellEdgeCount / 2; x <= detailCellEdgeCount / 2; ++x, ++idx) {
				var relCellCornerPos = relFocusCornerPos + new Vector3(x * detailCellSize, 0f, z * detailCellSize);
				var cellShardX = Mathf.FloorToInt(relCellCornerPos.x / farMeshEdgeLength);
				var cellShardZ = Mathf.FloorToInt(relCellCornerPos.z / farMeshEdgeLength);
				var shardData = terrainData.GetShard(cellShardX, cellShardZ);

				if(!shardData)
					continue;

				var cellCornerPos = new Vector3(relCellCornerPos.x + transform.position.x, -3.5f /*fixme: sample actual height for bounds*/, relCellCornerPos.z + transform.position.z);

				var cellCenterPos = cellCornerPos + new Vector3(detailCellSize * 0.5f, 0f, detailCellSize * 0.5f);
				var cellBounds = new Bounds(cellCenterPos, new Vector3(detailCellSize, 2.75f, detailCellSize));

#if SHOWIT
					var cbCol = new Color(Mathf.Abs(cellCenterPos.x * 0.43f) % 1f, Mathf.Abs((cellCenterPos.x + cellCenterPos.z) * 0.17f) % 1f, Mathf.Abs(cellCenterPos.z * 0.79f) % 1f);
					var cbCtr = cellBounds.center;
					var cbSiz = cellBounds.size * 0.999f;
					var cbMin = cellBounds.min;
					var cbMax = cbMin + Vector3.up * cellBounds.size.y;

					Debug.DrawLine(cbCtr										, cbCtr + Vector3.up * 3.5f					, cbCol);
					Debug.DrawLine(cbCtr										, cbCtr - Vector3.up * 3.5f					, cbCol);

					Debug.DrawLine(cbMin										, cbMin + new Vector3(cbSiz.x, 0f, 0f)		, cbCol);
					Debug.DrawLine(cbMin + new Vector3(cbSiz.x, 0f, 0f)			, cbMin + new Vector3(cbSiz.x, 0f, cbSiz.z)	, cbCol);
					Debug.DrawLine(cbMin + new Vector3(cbSiz.x, 0f, cbSiz.z)	, cbMin + new Vector3(0f, 0f, cbSiz.z)		, cbCol);
					Debug.DrawLine(cbMin										, cbMin + new Vector3(0f, 0f, cbSiz.z)		, cbCol);

					Debug.DrawLine(cbMax										, cbMax + new Vector3(cbSiz.x, 0f, 0f)		, cbCol);
					Debug.DrawLine(cbMax + new Vector3(cbSiz.x, 0f, 0f)			, cbMax + new Vector3(cbSiz.x, 0f, cbSiz.z)	, cbCol);
					Debug.DrawLine(cbMax + new Vector3(cbSiz.x, 0f, cbSiz.z)	, cbMax + new Vector3(0f, 0f, cbSiz.z)		, cbCol);
					Debug.DrawLine(cbMax										, cbMax + new Vector3(0f, 0f, cbSiz.z)		, cbCol);

					Debug.DrawLine(cbMin										, cbMax										, cbCol);
					Debug.DrawLine(cbMin + new Vector3(cbSiz.x, 0f, 0f)			, cbMax + new Vector3(cbSiz.x, 0f, 0f)		, cbCol);
					Debug.DrawLine(cbMin + new Vector3(cbSiz.x, 0f, cbSiz.z)	, cbMax + new Vector3(cbSiz.x, 0f, cbSiz.z)	, cbCol);
					Debug.DrawLine(cbMin + new Vector3(0f, 0f, cbSiz.z)			, cbMax + new Vector3(0f, 0f, cbSiz.z)		, cbCol);
#endif

				var visible = GeometryUtility.TestPlanesAABB(camPlanes, cellBounds);

				if(visible || !vegetationCullingEnabled) {
#if UNITY_5_5_OR_NEWER
					detailCommandBuffer.SetGlobalBuffer("meshInput", m_detailMeshBuffers[idx]);
					//detailCommandBuffer.DrawProceduralIndirect(Matrix4x4.identity, m_detailDrawMaterial, 0, MeshTopology.Points, m_detailArgBuffers[idx]);
					detailCommandBuffer.DrawProceduralIndirect(Matrix4x4.identity, m_detailDrawMaterial, 0, (MeshTopology)(MeshTopology.Triangles + 0), m_detailArgBuffers[idx]);
#endif
				}

				if(detailCommandShadowBuffer != null) {
					var drawCell = true;

					if(vegetationCullingEnabled) {
						// Cull by distance
						var distSqr = Vector3.SqrMagnitude(focusPos - cellCenterPos);
						drawCell = distSqr < vegetationShadowDistance * vegetationShadowDistance;

						// Evict non-visible cells that can not cast shadows into the visible frustum. Ideally we should do proper sweep tests, 
						// but just testing for cells 'behind' the view frustum (from the perspective of the light) is better than nothing.
						if(drawCell && !visible && vegetationCullingEnabled2) {
							var camToCell = cellCenterPos - focusPos;
							var edgePoint = cellCenterPos - camToCell.normalized * detailCellSize * 1.42f;

							for(int i = 0; i < camBacklitPlanes; ++i) {
								if(m_detailFrustumBacklitPlanes[0].GetSide(edgePoint)) {
									drawCell = false;
									break;
								}
							}
						}
					}

					if(drawCell) {
#if UNITY_5_6_OR_NEWER
						detailCommandShadowBuffer.SetGlobalBuffer("meshInput", m_detailMeshBuffers[idx]);
						//detailCommandShadowBuffer.DrawProceduralIndirect(Matrix4x4.identity, m_detailDrawMaterial, 1, MeshTopology.Points, m_detailArgBuffers[idx]);
						detailCommandShadowBuffer.DrawProceduralIndirect(Matrix4x4.identity, m_detailDrawMaterial, 1, (MeshTopology)(MeshTopology.Triangles + 0), m_detailArgBuffers[idx]);
#endif
					}
				}
			}
		}
	}

	void PrepareGenerationParams() {
		m_detailSpriteTemplateIndexMap.Clear();
		uint spriteTemplateNextIndex = 0;
		uint detailLayerNextIndex = 0;

		for(int idl = 0, ndl = materialData.detailLayers.Length; idl < ndl; ++idl) {
			var detailLayerStartIndex = detailLayerNextIndex;

			var detailLayer = materialData.detailLayers[idl];
			var ugStrongLayers = detailLayer.undergrowthStrongDetailLayers;
			//unused: var ugWeakLayers = detailLayer.undergrowthWeakDetailLayers;

			for(int iug = 0, nug = ugStrongLayers.Length; iug < nug; ++iug) {
				var ugLayer = ugStrongLayers[iug];
				var ugTemplate = materialData.UndergrowthSpriteTemplateFromGuid(ugLayer.undergrowthTemplate);
				var ugTextureSet = materialData.UndergrowthTextureSetFromGuid(ugTemplate.textureSet);
				var ugTextureSetIndex = materialData.UndergrowthTextureSetIndexFromGuid(ugTemplate.textureSet);

				uint spriteTemplateIndex;
				if(!m_detailSpriteTemplateIndexMap.TryGetValue(ugLayer.undergrowthTemplate, out spriteTemplateIndex)) {
					spriteTemplateIndex = spriteTemplateNextIndex++;
					m_detailSpriteTemplatesData[spriteTemplateIndex] = ConvertTemplate(ugTextureSet, ugTextureSetIndex, ugTemplate);
					m_detailSpriteTemplateIndexMap[ugLayer.undergrowthTemplate] = spriteTemplateIndex;
				}

				m_detailDetailLayersData[detailLayerNextIndex++] = ConvertLayer(spriteTemplateIndex, ugLayer);
			}

			m_detailMaterialStartOffsetData[idl] = (detailLayerStartIndex << 16) | (detailLayerNextIndex - detailLayerStartIndex);
		}

		m_detailSpriteTemplatesBuffer.SetData(m_detailSpriteTemplatesData);
		m_detailDetailLayersBuffer.SetData(m_detailDetailLayersData);
		m_detailMaterialStartOffsetBuffer.SetData(m_detailMaterialStartOffsetData);
	}

	void GenerateDetailData(Camera cam) {
		var focusPos = cam.transform.position;
		var relFocusPos = new Vector3(focusPos.x - transform.position.x, 0f, focusPos.z - transform.position.z);

		Vector3 relFocusCornerPos;
		relFocusCornerPos.x = Mathf.Floor(relFocusPos.x / detailCellSize) * detailCellSize;
		relFocusCornerPos.z = Mathf.Floor(relFocusPos.z / detailCellSize) * detailCellSize;
		relFocusCornerPos.y = 0;

		for(int z = -detailCellEdgeCount / 2, idx = 0; z <= detailCellEdgeCount / 2; ++z) {
			for(int x = -detailCellEdgeCount / 2; x <= detailCellEdgeCount / 2; ++x, ++idx) {
				var relCellCornerPos = relFocusCornerPos + new Vector3(x * detailCellSize, 0f, z * detailCellSize);
				var cellShardX = Mathf.FloorToInt(relCellCornerPos.x / farMeshEdgeLength);
				var cellShardZ = Mathf.FloorToInt(relCellCornerPos.z / farMeshEdgeLength);
				var shardData = terrainData.GetShard(cellShardX, cellShardZ);

				if(!shardData) {
					//Debug.LogWarningFormat("No shard data found for x: {0}  z: {1}", cellShardX, cellShardZ);
					m_detailArgBuffers[idx].SetData(m_args0100);
					continue;
				}

				var cellCornerPos = new Vector3(relCellCornerPos.x + transform.position.x, 0f, relCellCornerPos.z + transform.position.z);
				var shardCornerPos = new Vector3(cellShardX * farMeshEdgeLength + transform.position.x, 0f, cellShardZ * farMeshEdgeLength + transform.position.z);

				//var focusCornerPos = new Vector3(relFocusCornerPos.x + transform.position.x, 0f, relFocusCornerPos.z + transform.position.z);
				//var y = transform.position.y + 1f;
				//Debug.DrawLine(new Vector3(focusCornerPos.x, y, focusCornerPos.z), new Vector3(cellCornerPos.x, y, cellCornerPos.z), Color.cyan, 0f, false);
				//Debug.DrawLine(cellCornerPos + new Vector3(0f, y, 0f), cellCornerPos + new Vector3(5f, y, 5f), Color.green, 0f, true);
				//Debug.DrawLine(cellCornerPos + new Vector3(0f, y, 0f), cellCornerPos + new Vector3(detailCellSize, y, 0f), Color.green, 0f, true);
				//Debug.DrawLine(cellCornerPos + new Vector3(detailCellSize, y, 0f), cellCornerPos + new Vector3(detailCellSize, y, detailCellSize), Color.green, 0f, true);
				//Debug.DrawLine(cellCornerPos + new Vector3(detailCellSize, y, detailCellSize), cellCornerPos + new Vector3(0f, y, detailCellSize), Color.green, 0f, true);
				//Debug.DrawLine(cellCornerPos + new Vector3(0f, y, detailCellSize), cellCornerPos + new Vector3(0f, y, 0f), Color.green, 0f, true);
				//Debug.DrawLine(new Vector3(shardCornerPos.x, y, shardCornerPos.z), new Vector3(cellCornerPos.x, y, cellCornerPos.z), Color.yellow, 0f, false);

				RenderTexture flattenedRT = null;
#if PLAYMODE_CACHE_MATERIAL_MAPS
				if(Application.isPlaying) {
					Dictionary<TETerrainShardData, RenderTexture> cachedFlattenedShards = null;
					if(!m_cachedFlattenedShards.TryGetValue(cam, out cachedFlattenedShards))
						m_cachedFlattenedShards[cam] = (cachedFlattenedShards = new Dictionary<TETerrainShardData, RenderTexture>());

					if(!cachedFlattenedShards.TryGetValue(shardData, out flattenedRT)) {
						flattenedRT = new RenderTexture(detailCaptureSize, detailCaptureSize, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
						FlattenInput(shardCornerPos, flattenedRT);
						cachedFlattenedShards[shardData] = flattenedRT;
					}
				}
#endif

				if(flattenedRT == null && !m_flattenedShards.TryGetValue(shardData, out flattenedRT)) {
					flattenedRT = RenderTexture.GetTemporary(detailCaptureSize, detailCaptureSize, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
					FlattenInput(shardCornerPos, flattenedRT);
					m_flattenedShards[shardData] = flattenedRT;
				}

				GenerateInstances(focusPos, cellCornerPos, shardCornerPos, shardData, flattenedRT, m_detailMeshBuffers[idx], m_detailArgBuffers[idx]);
			}
		}

		if(m_flattenedShards.Count > 0) {
			for(var x = m_flattenedShards.GetEnumerator(); x.MoveNext();)
				RenderTexture.ReleaseTemporary(x.Current.Value);

			m_flattenedShards.Clear();
		}
	}

	RenderTexture FlattenInput(Vector3 shardCornerPos, RenderTexture re) {
		var oldShadows = QualitySettings.shadowDistance;
		QualitySettings.shadowDistance = 0f;
		var oldPixelLights = QualitySettings.pixelLightCount;
		QualitySettings.pixelLightCount = 0;

		var centerPos = shardCornerPos + new Vector3(farMeshEdgeLength * 0.5f, 0f, farMeshEdgeLength * 0.5f);
		m_detailCaptureCamera.transform.position = centerPos + Vector3.up * 1000f;

		var rtDepthId = RenderTexture.GetTemporary(detailCaptureSize, detailCaptureSize, 24, RenderTextureFormat.Depth, RenderTextureReadWrite.Default);
		var rtMatId = re ? re : RenderTexture.GetTemporary(detailCaptureSize, detailCaptureSize, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);

		//rtMatId.useMipMap = rtMatId.generateMips = false;
		rtMatId.filterMode = FilterMode.Point;

		m_detailCaptureCamera.SetTargetBuffers(rtMatId.colorBuffer, rtDepthId.depthBuffer);
		m_detailCaptureCamera.Render();
		m_detailCaptureCamera.targetTexture = null;

		RenderTexture.ReleaseTemporary(rtDepthId);

		QualitySettings.shadowDistance = oldShadows;
		QualitySettings.pixelLightCount = oldPixelLights;

		return rtMatId;
	}

	void GenerateInstances(Vector3 focusPos, Vector3 detailCornerPos, Vector3 shardCornerPos, TETerrainShardData shardData, RenderTexture matId, ComputeBuffer meshBuffer, ComputeBuffer argBuffer) {
		meshBuffer.SetCounterValue(0);

		detailSpawnShader.SetBuffer(0, "meshOutput", meshBuffer);
		detailSpawnShader.SetTexture(0, "materialID", matId);
		detailSpawnShader.SetTexture(0, "noiseRG", detailNoiseRG);

		detailSpawnShader.SetVector("focusPos", focusPos);

		detailSpawnShader.SetVector("detailCornerPos", detailCornerPos);
		detailSpawnShader.SetVector("detailCellSize", Vector4.one * detailCellSize);
		detailSpawnShader.SetVector("shardCornerPos", shardCornerPos);
		detailSpawnShader.SetVector("shardSize", Vector4.one * farMeshEdgeLength);

		detailSpawnShader.SetTexture(0, "heightmap", shardData.HeightTexture);
		detailSpawnShader.SetTexture(0, "controlmap", shardData.ControlTexture);
		detailSpawnShader.SetTexture(0, "colormap", shardData.ColorTexture);

		detailSpawnShader.SetBuffer(0, "spriteTemplates", m_detailSpriteTemplatesBuffer);
		detailSpawnShader.SetBuffer(0, "detailLayers", m_detailDetailLayersBuffer);
		detailSpawnShader.SetBuffer(0, "materialOffsetCount", m_detailMaterialStartOffsetBuffer);
		detailSpawnShader.SetInt("weakTemplatesOffset", 0);

		detailSpawnShader.SetFloat("vegetationDensityScale", vegetationDensityScale);
		
		detailSpawnShader.Dispatch(0, detailCellSize, detailCellSize, 1);

		//argBuffer.SetData(m_args0100);
		ComputeBuffer.CopyCount(meshBuffer, argBuffer, 0);

		// Separate dispatch just to setup indirect factors :(
		detailSpawnShader.SetBuffer(1, "argBuffer", argBuffer);
		detailSpawnShader.Dispatch(1, 1, 1, 1);
	}
}
