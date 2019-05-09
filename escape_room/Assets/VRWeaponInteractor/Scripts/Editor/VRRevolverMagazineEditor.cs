//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Editor for the VRRevolver script.
//
//=============================================================================
#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VRWeaponInteractor
{
	[CustomEditor(typeof(VRRevolverMagazine))]
	public class VRRevolverMagazineEditor : VRMagazineEditor
	{
		public VRRevolverMagazine drum = null;
		SerializedObject serializedDrum;

		SerializedProperty defaultPosition;
		SerializedProperty defaultRotation;
		SerializedProperty openPosition;
		SerializedProperty openRotation;
		SerializedProperty transitionTime;
		SerializedProperty gunHandler;

		static bool open;

		override public void OnEnable()
		{
			base.OnEnable();
			drum = (VRRevolverMagazine)target;
			serializedDrum = new SerializedObject(drum);

			defaultPosition = serializedDrum.FindProperty("defaultPosition");
			defaultRotation = serializedDrum.FindProperty("defaultRotation");
			openPosition = serializedDrum.FindProperty("openPosition");
			openRotation = serializedDrum.FindProperty("openRotation");
			transitionTime = serializedDrum.FindProperty("transitionTime");
			gunHandler = serializedDrum.FindProperty("gunHandler");
		}

		public override void OnInspectorGUI()
		{
			GUILayout.Label("Drum Settings", EditorStyles.boldLabel);

			serializedDrum.Update();

			EditorGUILayout.LabelField("Default Position", defaultPosition.vector3Value.ToString("N3"));
			EditorGUILayout.LabelField("Default Rotation", defaultRotation.quaternionValue.eulerAngles.ToString("N3"));

			EditorGUILayout.LabelField("Open Position", openPosition.vector3Value.ToString("N3"));
			EditorGUILayout.LabelField("Open Rotation", openRotation.quaternionValue.eulerAngles.ToString("N3"));

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Set Current to Default"))
			{
				defaultPosition.vector3Value = drum.transform.localPosition;
				defaultRotation.quaternionValue = drum.transform.localRotation;
				open = false;
			}
			if (GUILayout.Button("Set Current to Open"))
			{
				openPosition.vector3Value = drum.transform.localPosition;
				openRotation.quaternionValue = drum.transform.localRotation;
				open = true;
			}
			if (GUILayout.Button("Toggle"))
			{
				open = !open;
				drum.transform.localPosition = open ? openPosition.vector3Value : defaultPosition.vector3Value;
				drum.transform.localRotation = open ? openRotation.quaternionValue : defaultRotation.quaternionValue;
			}
			GUILayout.EndHorizontal();

			transitionTime.floatValue = EditorGUILayout.FloatField("Transition Time", transitionTime.floatValue);

			gunHandler.objectReferenceValue = EditorGUILayout.ObjectField("Gun Handler", gunHandler.objectReferenceValue, typeof(VRGunHandler), true);

			if (gunHandler.objectReferenceValue == null)
			{
				EditorGUILayout.HelpBox("Will not interact with weapon unless the gun handler is referenced", MessageType.Warning);
			}

			serializedDrum.ApplyModifiedProperties();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			base.OnInspectorGUI();
		}
	}
}
#endif