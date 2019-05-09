//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Editor for VRSecondHeld. Inherits from VRInteractableItemEditor
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using UnityEditor;
using System.Collections;
using VRInteraction;

namespace VRWeaponInteractor
{

	[CustomEditor(typeof(VRSecondHeld))]
	public class VRSecondHeldEditor : VRInteractableItemEditor 
	{
		// target component
		public VRSecondHeld secondHeld = null;

		override public void OnEnable()
		{
			secondHeld = (VRSecondHeld)target;
			base.OnEnable();
		}

		public override void OnInspectorGUI()
		{
			SecondHeldSection(serializedObject);

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			VRInteractableItemEditor.ItemSection(serializedObject, true, true);
		}

		static public void SecondHeldSection(SerializedObject serializedSecondHeld)
		{
			serializedSecondHeld.Update();

			SerializedProperty secondHeldRecoilKick = serializedSecondHeld.FindProperty("recoilKick");
			EditorGUILayout.PropertyField(secondHeldRecoilKick, new GUIContent("Second Held Recoil Kick"));
			SerializedProperty secondHeldAngularRecoilKick = serializedSecondHeld.FindProperty("angularRecoilKick");
			EditorGUILayout.PropertyField(secondHeldAngularRecoilKick, new GUIContent("Second Held Angular Recoil Kick"));
			SerializedProperty secondHeldRecoilRecovery = serializedSecondHeld.FindProperty("recoilRecovery");
			EditorGUILayout.PropertyField(secondHeldRecoilRecovery, new GUIContent("Second Held Recoil Recovery"));
			SerializedProperty secondHeldAngularRecoilMultiStep = serializedSecondHeld.FindProperty("angularRecoilMultiStep");
			EditorGUILayout.PropertyField(secondHeldAngularRecoilMultiStep, new GUIContent("Angular Recoild Multi Step"));

			serializedSecondHeld.ApplyModifiedProperties();
		}
	}
}
#endif