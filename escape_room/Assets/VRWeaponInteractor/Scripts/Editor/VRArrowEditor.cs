#if VRInteraction
using UnityEngine;
using UnityEditor;
using System.Collections;
using VRInteraction;

namespace VRWeaponInteractor
{
	[CustomEditor(typeof(VRArrow))]
	public class VRArrowEditor : VRInteractableItemEditor
	{

	}
}
#endif