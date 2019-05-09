#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VRWeaponInteractor
{
	public class SpeedLoaderWindow : EditorWindow
	{
		public VRRevolverSpeedLoader speedLoader;

		SerializedObject serializedSpeedLoader;

		SerializedProperty bulletPrefab;
		List<GameObject> bulletInstances = new List<GameObject>();

		public void Init()
		{
			if (speedLoader == null) return;
			serializedSpeedLoader = new SerializedObject(speedLoader);
			bulletPrefab = serializedSpeedLoader.FindProperty("bulletPrefab");
		}

		void OnGUI () 
		{
			var oldSpeedLoader = speedLoader;
			speedLoader = (VRRevolverSpeedLoader)EditorGUILayout.ObjectField("Speed Loader", speedLoader, typeof(VRRevolverSpeedLoader), true);
			if (speedLoader != null && oldSpeedLoader != speedLoader) Init();
			else if (speedLoader == null) return;

			serializedSpeedLoader.Update();

			bulletPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Bullet Prefab", bulletPrefab.objectReferenceValue, typeof(GameObject), false);

			if (bulletPrefab.objectReferenceValue == null)
			{
				EditorGUILayout.HelpBox("Need bullet prefab", MessageType.Warning);
				serializedSpeedLoader.ApplyModifiedProperties();
				return;
			}

			SerializedProperty clipSize = serializedSpeedLoader.FindProperty("clipSize");
			var oldClipSize = clipSize.intValue;
			clipSize.intValue = EditorGUILayout.IntField("Clip Size", clipSize.intValue);

			SerializedProperty bulletPositions = serializedSpeedLoader.FindProperty("bulletPositions");
			SerializedProperty bulletRotations = serializedSpeedLoader.FindProperty("bulletRotations");
			if (oldClipSize != clipSize.intValue || bulletPositions.arraySize !=  clipSize.intValue || bulletRotations.arraySize != clipSize.intValue)
			{
				if (clipSize.intValue < oldClipSize)
				{
					bulletPositions.ClearArray();
					bulletRotations.ClearArray();
				}
				bulletPositions.arraySize = bulletRotations.arraySize = clipSize.intValue;
			}

			EditorGUILayout.HelpBox("Bullet one goes at the top of the clip aka closest to the loaded position", MessageType.Info);

			for(int i=0; i<clipSize.intValue; i++)
			{
				SerializedProperty bulletPosition = bulletPositions.GetArrayElementAtIndex(i);
				EditorGUILayout.LabelField("Position: " + bulletPosition.vector3Value.ToString("G4"));
				SerializedProperty bulletRotation = bulletRotations.GetArrayElementAtIndex(i);
				EditorGUILayout.LabelField("Rotation: " + bulletRotation.quaternionValue.eulerAngles.ToString("G4"));
				if (bulletInstances.Count < clipSize.intValue)
				{
					int extraNeeded = clipSize.intValue-bulletInstances.Count;
					for(int j=0; j<extraNeeded; j++)
						bulletInstances.Add(null);
				} else if (bulletInstances.Count > clipSize.intValue)
				{
					int toRemove = bulletInstances.Count - clipSize.intValue;
					for(int j=bulletInstances.Count-toRemove; j<toRemove; j++)
					{
						Undo.DestroyObjectImmediate(bulletInstances[i]);
					}
					bulletInstances.RemoveRange(bulletInstances.Count-toRemove, toRemove);
				}
				if (bulletInstances[i] == null)
				{
					if (GUILayout.Button("Spawn Reference"))
					{
						GameObject referenceBullet = (GameObject)Instantiate(bulletPrefab.objectReferenceValue, Vector3.zero, Quaternion.identity);
						Undo.RegisterCreatedObjectUndo(referenceBullet, "Create reference bullet");
						referenceBullet.transform.SetParent(speedLoader.item != null ? speedLoader.item : speedLoader.transform);
						referenceBullet.transform.localPosition = bulletPosition.vector3Value;
						referenceBullet.transform.localRotation = bulletRotation.quaternionValue;
						bulletInstances.Insert(i, referenceBullet);
						Selection.activeGameObject = referenceBullet;
					}
				} else
				{
					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Select Bullet"))
					{
						Selection.activeGameObject = bulletInstances[i];
					}
					if (GUILayout.Button("Save Current Position"))
					{
						bulletPosition.vector3Value = bulletInstances[i].transform.localPosition;
						bulletRotation.quaternionValue = bulletInstances[i].transform.localRotation;
					}
					if(GUILayout.Button("Destory Bullet"))
					{
						Undo.DestroyObjectImmediate(bulletInstances[i]);
					}
					GUILayout.EndHorizontal();
				}
			}

			serializedSpeedLoader.ApplyModifiedProperties();
		}

		void OnDestroy()
		{
			if (bulletInstances.Count != 0)
			{
				foreach(GameObject bullet in bulletInstances)
				{
					if (bullet == null) continue;
					Undo.DestroyObjectImmediate(bullet);
				}
			}
		}
	}
}
#endif