using UnityEngine;
using System.Collections;


public class LightMapData : MonoBehaviour {
	
	[System.Serializable]
	public struct RendererInfo
	{
		public Renderer 	renderer;
		public int 			lightmapIndex;
		public Vector4 		lightmapOffsetScale;
	}

	[SerializeField]
	 public RendererInfo	m_RendererInfo;
	

	public void GetInfo()
	{
		m_RendererInfo.renderer = GetComponent<Renderer> ();
		if (m_RendererInfo.renderer) 
		{
			m_RendererInfo.lightmapIndex = m_RendererInfo.renderer.lightmapIndex;
#if UNITY_2018
			m_RendererInfo.lightmapOffsetScale = m_RendererInfo.renderer.lightmapScaleOffset;
#else
			m_RendererInfo.lightmapOffsetScale = m_RendererInfo.renderer.lightmapScaleOffset;
#endif
		}

	}
}
