#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRInteraction;

namespace VRWeaponInteractor
{
	[CustomEditor(typeof(VRChangeFireModeButton))]
	public class VRChangeFireModeButtonEditor : VRInteractableItemEditor
	{
		public override void OnInspectorGUI()
		{
			VRInteractableItemEditor.ItemSection(serializedObject, false, true);

			serializedObject.Update();

			SerializedProperty targetWeapon = serializedObject.FindProperty("targetWeapon");
			SerializedProperty modes = serializedObject.FindProperty("modes");
			EditorGUILayout.PropertyField(targetWeapon);
			EditorGUILayout.PropertyField(modes, true);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif