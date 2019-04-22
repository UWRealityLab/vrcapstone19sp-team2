using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System;
using System.IO;
using UnityEditor.SceneManagement;

public class LightMappingPortingManager : EditorWindow {

	bool onlyActive = true;
	private string unityVersion;

    // Add menu named "My Window" to the Window menu
    [MenuItem ("Window/LightMapping Porting Manager")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		LightMappingPortingManager window = (LightMappingPortingManager)EditorWindow.GetWindow (typeof (LightMappingPortingManager));
		window.Show();
	}


	void OnGUI () 
	{
		GUILayout.Label ("LightMapping Porting Manager", EditorStyles.boldLabel);

		unityVersion = UnityEditorInternal.InternalEditorUtility.GetFullUnityVersion ();

		bool isversion4 = unityVersion.StartsWith ("4.");

		GUILayout.Label ("Unity Version: "+ unityVersion);

		EditorGUI.BeginDisabledGroup (!isversion4);
		GUILayout.Label ("GET 4.x LIGHTING DATA");

		
		if (GUILayout.Button ("Asign LightMapData"))
		{
			AssignComponentToObjects(onlyActive);
		}


		onlyActive = EditorGUILayout.Toggle ("Only Active Objects", onlyActive);
	
		EditorGUI.EndDisabledGroup ();
		EditorGUI.BeginDisabledGroup (isversion4);
		GUILayout.Label ("SET LIGHTING DATA TO 5.x");

        if (GUILayout.Button ("Get LightMapData"))
		{
			GetLightMapsAndData();
		}

		EditorGUI.EndDisabledGroup ();

        GUILayout.Space(50);

        if (GUILayout.Button("Delete LightMapData from renderers"))
        {
            DeleteComponentFromObjects();
        }
    }

	void GetLightMapsAndData()
	{
        if (GameObject.FindObjectOfType<LightmappingManager>())
            return;

        LightMapDataContainerObject lightmapDataAsset = ScriptableObject.CreateInstance<LightMapDataContainerObject>();

        LightMapData[] lighmapdata = Resources.FindObjectsOfTypeAll<LightMapData>() as LightMapData[];

        Renderer[] rendererlist = new Renderer[lighmapdata.Length];
        int[] indexes = new int[lighmapdata.Length];
        Vector4[] lightmapOffsetScales = new Vector4[lighmapdata.Length];

        for (int i=0;i< lighmapdata.Length;i++)
        {
            rendererlist[i] = lighmapdata[i].m_RendererInfo.renderer;
            indexes[i] = lighmapdata[i].m_RendererInfo.lightmapIndex;
            lightmapOffsetScales[i] = lighmapdata[i].m_RendererInfo.lightmapOffsetScale;
        }

        lightmapDataAsset.lightmapIndexes = indexes;
        lightmapDataAsset.lightmapOffsetScales = lightmapOffsetScales;

        //AssetDatabase.CreateAsset(lightmapDataAsset,"Assets/LightMapData_"+Path.GetFileNameWithoutExtension(EditorApplication.currentScene));
		AssetDatabase.CreateAsset(lightmapDataAsset,"Assets/LightMapData_"+Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().ToString()));
        AssetDatabase.SaveAssets();

        GameObject lightmappingManager = EditorUtility.CreateGameObjectWithHideFlags("LightmappingManager",HideFlags.None,new Type[] {typeof(LightmappingManager) });

        lightmappingManager.GetComponent<LightmappingManager>().sceneRenderers = rendererlist;
        lightmappingManager.GetComponent<LightmappingManager>().lighmapDataContainer = lightmapDataAsset;
        lightmappingManager.GetComponent<LightmappingManager>().SetLightMapData();

    }
        

    void DeleteComponentFromObjects()
	{
		Renderer[] rendererObjects = Resources.FindObjectsOfTypeAll <Renderer> ().Where(c => c.gameObject.hideFlags == HideFlags.None).ToArray() as Renderer[]; 

		foreach (Renderer g in rendererObjects ) 
		{
			if(g.gameObject.GetComponent<LightMapData>())
			{
				DestroyImmediate(g.gameObject.GetComponent<LightMapData>());
			}
		}
	}

	void AssignComponentToObjects(bool OnlyActive)
	{
		Renderer[] rendererObjects;

		if (OnlyActive)
		{
			 rendererObjects = GameObject.FindObjectsOfType <Renderer> () as Renderer[]; 

		} else 
		{
			rendererObjects = Resources.FindObjectsOfTypeAll <Renderer> ().Where(c => c.gameObject.hideFlags == HideFlags.None).ToArray()as Renderer[]; 
		}

		foreach (Renderer g in rendererObjects as Renderer[]) 
		{
			if(!g.gameObject.GetComponent<LightMapData>())
			{
				g.gameObject.AddComponent<LightMapData> ().GetInfo();
			}
		}

	}
}
