using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class LightmappingManager : MonoBehaviour {

    [SerializeField]
    public  Renderer[] sceneRenderers;

    public LightMapDataContainerObject lighmapDataContainer;

    [SerializeField]
    public Texture2D[] lightMapTexturesFar;

    void Awake()
    {
        SetLightMapData();
        SetLightMapTextures();
    }


   public void SetLightMapTextures()
    {
        if (lightMapTexturesFar == null || lightMapTexturesFar.Length <= 0)
            return;

            LightmapData[] lightmapData = new LightmapData[lightMapTexturesFar.Length];

            for (int i = 0; i < lightMapTexturesFar.Length; i++)
            {
                lightmapData[i] = new LightmapData();
                lightmapData[i].lightmapColor = lightMapTexturesFar[i];
             
        }
#if UNITY_2018
            LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
#endif
            LightmapSettings.lightmaps = lightmapData;
        
    }

  public  void SetLightMapData()
    {
        if (sceneRenderers.Length <= 0)
            return;

        for (int i = 0; i < sceneRenderers.Length; i++)
        {
            if (sceneRenderers[i])
            {
                sceneRenderers[i].lightmapIndex = lighmapDataContainer.lightmapIndexes[i];
#if UNITY_2018
                sceneRenderers[i].lightmapScaleOffset = lighmapDataContainer.lightmapOffsetScales[i];
#else
				sceneRenderers[i].lightmapScaleOffset = lighmapDataContainer.lightmapOffsetScales[i];
#endif
            }
        }

    }
}
