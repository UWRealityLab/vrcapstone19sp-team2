//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Editor for VRGunHandler. Can open the gun handler window
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using UnityEditor;
using System.Collections;
using VRInteraction;

namespace VRWeaponInteractor
{
	[CustomEditor(typeof(VRGunHandler))]
	public class VRGunHandlerEditor : VRInteractableItemEditor
	{
		// target component
		public VRGunHandler gunHandler = null;

		override public void OnEnable()
		{
			gunHandler = (VRGunHandler)target;
			base.OnEnable();
		}

		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open Gun Handler Editor"))
			{
				GunHandlerWindow newWindow = (GunHandlerWindow)EditorWindow.GetWindow(typeof(GunHandlerWindow), true, "Gun Handler", true);
				newWindow.gunHandler = gunHandler;
				newWindow.Init();
				newWindow.weaponTab = GunHandlerWindow.WeaponTab.MAIN;
			}

			serializedObject.Update();

			SerializedProperty slide = serializedObject.FindProperty("slide");
			EditorGUILayout.PropertyField(slide);
			SerializedProperty trigger = serializedObject.FindProperty("trigger");
			EditorGUILayout.PropertyField(trigger);

			serializedObject.ApplyModifiedProperties();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			base.OnInspectorGUI();
		}
	}
}
#endif