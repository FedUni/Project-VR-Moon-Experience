using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

[ExecuteInEditMode]
public class AtmosphericScatteringSun : MonoBehaviour {
	public static AtmosphericScatteringSun instance;
	
	new public Transform	transform { get; private set; }
	new public Light		light { get { return m_light; } }

	public CommandBuffer occlusionCmdBeforeScreenSpace { get { return m_occlusionCmdBeforeScreen; } }

	CommandBuffer	m_occlusionCmdAfterShadows;
	CommandBuffer	m_occlusionCmdBeforeScreen;
	Light			m_light;

	//public static System.IntPtr GetPtrFromScriptingObjectWithIntPtrField(System.Object o) {
	//	return (System.IntPtr)o.GetType().GetField("m_Ptr", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(o);
	//}

	void OnEnable() {
		//Debug.LogFormat("OnEnable: {0}: {1} / {2}", m_light ? m_light.commandBufferCount : -1, GetInstanceID(), name);

		if(instance) {
			Debug.LogErrorFormat("Not setting 'AtmosphericScatteringSun.instance' because '{0}' is already active!", instance.name);
			return;
		}

		this.transform = base.transform;
		m_light = GetComponent<Light>();
		instance = this;

		if(m_occlusionCmdAfterShadows == null) {
			m_occlusionCmdAfterShadows = new CommandBuffer();
			m_occlusionCmdAfterShadows.name = "Scatter Occlusion Pass 1";
			m_occlusionCmdAfterShadows.SetGlobalTexture("u_CascadedShadowMap", BuiltinRenderTextureType.CurrentActive);
		}

		if(m_occlusionCmdBeforeScreen == null) {
			m_occlusionCmdBeforeScreen = new CommandBuffer();
			m_occlusionCmdBeforeScreen.name = "Scatter Occlusion Pass 2";
		}

		m_light.AddCommandBuffer(LightEvent.AfterShadowMap, m_occlusionCmdAfterShadows);
		m_light.AddCommandBuffer(LightEvent.BeforeScreenspaceMask, m_occlusionCmdBeforeScreen);

		//Debug.LogFormat("+OnEnable: {0}: {1:x} / {2:x}", m_light.commandBufferCount, GetPtrFromScriptingObjectWithIntPtrField(m_occlusionCmdAfterShadows).ToInt64(), GetPtrFromScriptingObjectWithIntPtrField(m_occlusionCmdBeforeScreen).ToInt64());

		Shader.SetGlobalVector("_AtmosphericScatteringSunVector", transform.forward);
	}

#if UNITY_EDITOR
	void Update() {
		if(instance == this && transform.hasChanged) {
			Shader.SetGlobalVector("_AtmosphericScatteringSunVector", transform.forward);
			transform.hasChanged = false;
		}
	}
#endif

	void OnDisable() {
		//Debug.LogFormat("OnDisable: {0}: {1} / {2} ", m_light ? m_light.commandBufferCount : -1, GetInstanceID(), name);

		if(m_light) {
			if(m_occlusionCmdAfterShadows != null)
				m_light.RemoveCommandBuffer(LightEvent.AfterShadowMap, m_occlusionCmdAfterShadows);
			if(m_occlusionCmdBeforeScreen != null)
				m_light.RemoveCommandBuffer(LightEvent.BeforeScreenspaceMask, m_occlusionCmdBeforeScreen);

			//Debug.LogFormat("-OnDisable: {0}: {1:x} / {2:x}", m_light.commandBufferCount, GetPtrFromScriptingObjectWithIntPtrField(m_occlusionCmdAfterShadows).ToInt64(), GetPtrFromScriptingObjectWithIntPtrField(m_occlusionCmdBeforeScreen).ToInt64());
		}

		#if UNITY_EDITOR
			OnDestroy(); // release buffers
		#endif

		if(instance == this)
			Shader.SetGlobalVector("_AtmosphericScatteringSunVector", Vector3.zero);

		if(instance == null) {
			Debug.LogErrorFormat("'AtmosphericScatteringSun.instance' is already null when disabling '{0}'!", this.name);
			return;
		}

		if(instance != this) {
			Debug.LogErrorFormat("Not UNsetting 'AtmosphericScatteringSun.instance' because it points to someone else '{0}'!", instance.name);
			return;
		}

		instance = null;
	}

	void OnDestroy() {
		if(m_occlusionCmdAfterShadows != null) {
			m_occlusionCmdAfterShadows.Release();
			m_occlusionCmdAfterShadows = null;
		}
		if(m_occlusionCmdBeforeScreen != null) {
			m_occlusionCmdBeforeScreen.Release();
			m_occlusionCmdBeforeScreen = null;
		}
	}
}
