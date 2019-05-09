//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Editor for VRLoadableBullet.
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using UnityEditor;
using System.Collections;
using VRInteraction;

namespace VRWeaponInteractor
{

	[CustomEditor(typeof(VRLoadableBullet))]
	public class VRLoadableBulletEditor : VRInteractableItemEditor {

		// target component
		public VRLoadableBullet loadableBullet = null;

		public override void OnEnable()
		{
			loadableBullet = (VRLoadableBullet)target;
			base.OnEnable();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			SerializedProperty bulletId = serializedObject.FindProperty("bulletId");
			EditorGUILayout.PropertyField(bulletId);

			serializedObject.ApplyModifiedProperties();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			base.OnInspectorGUI();
		}
	}
}
#endif