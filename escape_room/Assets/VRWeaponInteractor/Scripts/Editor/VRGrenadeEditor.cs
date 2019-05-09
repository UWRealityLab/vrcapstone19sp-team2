//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// VRGrenade Editor.
//
//===================Contact Email: Sam@MassGames.co.uk===========================
#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRInteraction;

namespace VRWeaponInteractor
{
	[CustomEditor(typeof(VRGrenade))]
	public class VRGrenadeEditor : VRInteractableItemEditor 
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			GUIContent fuseTimeConent = new GUIContent("Fuse Time", "Time in second after the lever has been ejected until explosion");
			SerializedProperty fuseTime = serializedObject.FindProperty("fuseTime");
			EditorGUILayout.PropertyField(fuseTime, fuseTimeConent);

			GUIContent useRingContent = new GUIContent("Use Ring", "If true will wait until the method RingPulled is called before arming." +
																	" When set to false it will arm the first time it is dropped.");
			SerializedProperty useRing = serializedObject.FindProperty("useRing");
			EditorGUILayout.PropertyField(useRing, useRingContent);

			GUIContent leverConent = new GUIContent("Lever", "The lever should be an interacable item set as a child of the grenade item. " +
															"It's item parent should be set as this script and it should be kinematic with it's " +
															"colliders set to false.");
			SerializedProperty lever = serializedObject.FindProperty("lever");
			EditorGUILayout.PropertyField(lever, leverConent);

			GUIContent explosionPrefabContent = new GUIContent("Explosion Prefab", "The explosion prefab will be instantiated when this grenade explodes");
			SerializedProperty explosionPrefab = serializedObject.FindProperty("explosionPrefab");
			EditorGUILayout.PropertyField(explosionPrefab, explosionPrefabContent);

			serializedObject.ApplyModifiedProperties();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			base.OnInspectorGUI();
		}
	}
}
#endif