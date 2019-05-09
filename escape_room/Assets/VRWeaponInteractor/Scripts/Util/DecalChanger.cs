//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Used in the DecalChanger prefab and used on the ExampleM9 prefab. Can be used
// to change the decal material based on the tag of the object hit. For example
// one material when hitting wood textures and another for concrete.
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class DecalChanger : MonoBehaviour 
	{
		[System.Serializable]
		public class DecalSurface
		{
			public GameObject prefab;
			public string tag;
		}

		public GameObject defaultDecal;
		public List<DecalSurface> decals = new List<DecalSurface>();

		public void SetMaterialTo(string targetTag)
		{
			if (defaultDecal == null)
			{
				Debug.LogError("No default decal specified");
				return;
			}
			GameObject newDecal = null;
			foreach(DecalSurface surface in decals)
			{
				if (targetTag != surface.tag) continue;
				if (surface.prefab == null)
				{
					Debug.LogError("Null prefab in DecalChanger script");
					return;
				}
				newDecal = PoolingManager.instance.GetInstance(surface.prefab);
				break;
			}
			if (newDecal == null) newDecal = PoolingManager.instance.GetInstance(defaultDecal);
			PoolingManager.instance.SetupDestroyInToReset(newDecal, true);
			newDecal.transform.parent = transform.parent;
			newDecal.transform.position = transform.position;
			newDecal.transform.rotation = transform.rotation;
			newDecal.SetActive(true);
        }
	}
}
#endif
