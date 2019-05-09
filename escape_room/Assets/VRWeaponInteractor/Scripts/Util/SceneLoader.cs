#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRWeaponInteractor
{
	public class SceneLoader : MonoBehaviour 
	{
		public void LoadScene(string newScene)
		{
			SceneManager.LoadScene(newScene);
		}
	}
}
#endif