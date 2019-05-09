#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRInteraction;

namespace VRWeaponInteractor
{
	[CustomEditor(typeof(VRRevolverSpeedLoader))]
	public class VRRevolverSpeedLoaderEditor : VRInteractableItemEditor
	{
		public VRRevolverSpeedLoader speedLoader = null;
		SerializedObject serializedSpeedLoader;

		SerializedProperty bulletPrefab;


		override public void OnEnable()
		{
			base.OnEnable();
			speedLoader = (VRRevolverSpeedLoader)target;
			serializedSpeedLoader = new SerializedObject(speedLoader);

			bulletPrefab = serializedSpeedLoader.FindProperty("bulletPrefab");
		}

		public override void OnInspectorGUI()
		{
			serializedSpeedLoader.Update();

			bulletPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Bullet Prefab", bulletPrefab.objectReferenceValue, typeof(GameObject), false);
			if (bulletPrefab.objectReferenceValue != null)
			{
				if (GUILayout.Button("Open Bullet Window"))
				{
					SpeedLoaderWindow newWindow = (SpeedLoaderWindow)EditorWindow.GetWindow(typeof(SpeedLoaderWindow), true, "Speed Loader", true);
					newWindow.speedLoader = speedLoader;
					newWindow.Init();
				}


			}
			serializedSpeedLoader.ApplyModifiedProperties();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			base.OnInspectorGUI();
		}
	}
}
#endif