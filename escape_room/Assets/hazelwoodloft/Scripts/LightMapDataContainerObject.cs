using UnityEngine;
using System.Collections;

[System.Serializable]
public class LightMapDataContainerObject : ScriptableObject {

    public int[] lightmapIndexes;
    public Vector4[] lightmapOffsetScales;

}
