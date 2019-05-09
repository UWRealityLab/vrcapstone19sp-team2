//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Editor for VRGunHandlerRef. Can open the gun handler window
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace VRWeaponInteractor
{

	[CustomEditor(typeof(VRGunHandlerRef))]
	public class VRGunHandlerRefEditor : Editor
	{
		// target component
		public VRGunHandlerRef gunHandlerRef = null;

		public void OnEnable()
		{
			gunHandlerRef = (VRGunHandlerRef)target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			SerializedProperty currentGun = serializedObject.FindProperty("gunHandler");
			currentGun.objectReferenceValue = EditorGUILayout.ObjectField("Gun Handler", currentGun.objectReferenceValue, typeof(VRGunHandler), true);
			serializedObject.ApplyModifiedProperties();

			if (currentGun.objectReferenceValue != null)
			{
				if (GUILayout.Button("Open Gun Handler Editor"))
				{
					GunHandlerWindow newWindow = (GunHandlerWindow)EditorWindow.GetWindow(typeof(GunHandlerWindow), true, "Gun Handler", true);
					newWindow.gunHandler = (VRGunHandler)currentGun.objectReferenceValue;
					newWindow.Init();
					newWindow.weaponTab = GunHandlerWindow.WeaponTab.MAIN;
				}
				if (GUILayout.Button("Select Gun Handler"))
				{
					Selection.activeGameObject = ((VRGunHandler)currentGun.objectReferenceValue).gameObject;
				}
			}
		}
	}
}
#endif