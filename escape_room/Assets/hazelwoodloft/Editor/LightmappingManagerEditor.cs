using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LightmappingManager))]
public class LightmappingManagerEditor : Editor {

    LightmappingManager lightmappingmanager;
   // private SerializedProperty lightmapsTextures;

    void OnEnable()
    {
        lightmappingmanager  = target as LightmappingManager;
    }

   public override void OnInspectorGUI()
    {
        this.DrawDefaultInspector();

        if (GUILayout.Button("Apply lightMaps") && lightmappingmanager != null)
        {
          
            lightmappingmanager.SetLightMapData();
            lightmappingmanager.SetLightMapTextures();
            
        }
    }

    

}
