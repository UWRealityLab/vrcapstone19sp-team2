//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Fades a given material between the start and end colour.
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using System.Collections;

namespace VRWeaponInteractor
{
	public class Fade : MonoBehaviour {

		public bool fadeOnStart;
		public Material material;
		public Color startColour;
		public Color endColour;
		//	In seconds
		public float transitionTime = 1f;

		private bool fadingIn;

		void Start () 
		{
			if (fadeOnStart) FadeIn();
		}

		public void FadeIn()
		{
			StartCoroutine(FadeTransition(true));
		}

		public void FadeOut()
		{
			StartCoroutine(FadeTransition(false));
		}

		IEnumerator FadeTransition(bool fadeIn)
		{
			fadingIn = fadeIn;

			Color currentColour = fadeIn ? startColour : endColour;
			Color targetColour = fadeIn ? endColour : startColour;
			float t = 0;
			while(t<1)
			{
				if (fadingIn != fadeIn) yield break;
				material.SetColor("_TintColor", Color.Lerp(currentColour, targetColour, t));
				t+=Time.deltaTime/transitionTime;
				yield return null;
			}
		}
	}
}
#endif