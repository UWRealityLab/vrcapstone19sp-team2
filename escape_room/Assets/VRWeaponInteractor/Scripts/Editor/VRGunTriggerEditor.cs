//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Editor for VRGunTrigger. Can open the gun handler window
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace VRWeaponInteractor
{

	[CustomEditor(typeof(VRGunTrigger))]
	public class VRGunTriggerEditor : Editor 
	{
		VRGunTrigger gunTrigger;

		public void OnEnable()
		{
			gunTrigger = (VRGunTrigger)target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			SerializedProperty gunHandler = serializedObject.FindProperty("gunHandler");

			var oldGunHandler = gunHandler.objectReferenceValue;
			EditorGUILayout.PropertyField(gunHandler);

			serializedObject.ApplyModifiedProperties();
			if (gunHandler.objectReferenceValue != null)
			{
				if (GUILayout.Button("Open Gun Handler Editor"))
				{
					GunHandlerWindow newWindow = (GunHandlerWindow)EditorWindow.GetWindow(typeof(GunHandlerWindow), true, "Gun Handler", true);
					newWindow.gunHandler = gunTrigger.gunHandler;
					newWindow.Init();
					newWindow.weaponTab = GunHandlerWindow.WeaponTab.TRIGGER;
				}

				GunHandlerWindow.TriggerSection(serializedObject);
			}
		}
	}
}
#endif