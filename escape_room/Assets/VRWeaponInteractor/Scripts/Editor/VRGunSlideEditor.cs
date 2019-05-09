//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Editor for VRGunSlide. Inherits from VRInteractableItemEditor
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using UnityEditor;
using System.Collections;
using VRInteraction;

namespace VRWeaponInteractor
{

	[CustomEditor(typeof(VRGunSlide))]
	public class VRGunSlideEditor : VRSecondHeldEditor 
	{
		// target component
		public VRGunSlide gunSlide = null;

		override public void OnEnable()
		{
			gunSlide = (VRGunSlide)target;
			base.OnEnable();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			SerializedProperty gunHandler = serializedObject.FindProperty("gunHandler");
			var oldGunHandler = gunHandler.objectReferenceValue;
			EditorGUILayout.PropertyField(gunHandler);
			if (gunHandler.objectReferenceValue != null && gunHandler.objectReferenceValue != oldGunHandler)
			{
				SerializedObject serializedGunHandler = new SerializedObject(gunHandler.objectReferenceValue);
				serializedGunHandler.Update();
				SerializedProperty gunHandlersGunSlide = serializedGunHandler.FindProperty("slide");
				gunHandlersGunSlide.objectReferenceValue = gunSlide;
				serializedGunHandler.ApplyModifiedProperties();
			}

			if (gunHandler.objectReferenceValue != null)
			{
				if (GUILayout.Button("Open Gun Handler Editor"))
				{
					GunHandlerWindow newWindow = (GunHandlerWindow)EditorWindow.GetWindow(typeof(GunHandlerWindow), true, "Gun Handler", true);
					newWindow.gunHandler = gunSlide.gunHandler;
					newWindow.Init();
					newWindow.weaponTab = GunHandlerWindow.WeaponTab.SLIDE;
				}
			}

			SerializedProperty slideItem = serializedObject.FindProperty("item");

			slideItem.objectReferenceValue = EditorGUILayout.ObjectField("Item", slideItem.objectReferenceValue, typeof(Transform), true);

			serializedObject.ApplyModifiedProperties();

			if (slideItem.objectReferenceValue == null) return;
			GunHandlerWindow.SlideSection(serializedObject);


		}
	}
}
#endif