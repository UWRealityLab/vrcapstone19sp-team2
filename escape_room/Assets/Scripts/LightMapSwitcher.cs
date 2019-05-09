using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Collections;

[ExecuteInEditMode]
public class LightMapSwitcher : MonoBehaviour
{
    //GUI dropdown
    public Dropdown dropdown;

    private LightmapData[] Daydata;
    private LightmapData[] Nightdata;

    private LightmapData[][] allLightmaps;

    private Texture[] DayReflections;
    private Texture[] NightReflections;

    private Texture[][] allReflections;

    private ReflectionProbe[] allReflectionProbes;
    //path to the night lightmap and reflection substitudes
    public string nightpath = "";



    // Use this for initialization
    void Start()
    {
        //ypu can duplicate lightmap folder to break references, move to Resources, and rename back to the scene name
        if (nightpath == "") nightpath = SceneManager.GetActiveScene().name;

        Daydata = LightmapSettings.lightmaps;

        allLightmaps = new LightmapData[2][];
        allLightmaps[0] = Daydata;

        Nightdata = new LightmapData[Daydata.Length];

        for (int i = 0; i < Daydata.Length; i++)
        {
            Nightdata[i] = new LightmapData();
            Nightdata[i].lightmapColor = Resources.Load(nightpath + "/" + Daydata[i].lightmapColor.name) as Texture2D;
            Nightdata[i].lightmapDir = Resources.Load(nightpath + "/" + Daydata[i].lightmapDir.name) as Texture2D;
        }

        allLightmaps[1] = Nightdata;

        allReflectionProbes = FindObjectsOfType<ReflectionProbe>();

        DayReflections = new Texture[allReflectionProbes.Length];
        NightReflections = new Texture[allReflectionProbes.Length];
        Debug.Log("hi");

        for (int i = 0; i < allReflectionProbes.Length; i++)
        {
            DayReflections[i] = allReflectionProbes[i].customBakedTexture;
            Debug.Log(DayReflections[i].name);
            NightReflections[i] = Resources.Load(nightpath + "/" + DayReflections[i].name) as Texture;
            allReflectionProbes[i].mode = ReflectionProbeMode.Custom;
            allReflectionProbes[i].customBakedTexture = DayReflections[i];
        }

        allReflections = new Texture[2][];
        allReflections[0] = DayReflections;
        allReflections[1] = NightReflections;

        /*
        dropdown.onValueChanged.AddListener(delegate
        {
            SwapLightmaps(dropdown.value);
        });
        */
    }

    public void SwapLightmaps(int option)
    {

        LightmapSettings.lightmaps = allLightmaps[option];
        for (int i = 0; i < allReflectionProbes.Length; i++)
        {
            allReflectionProbes[i].customBakedTexture = allReflections[option][i];
        }
    }
}