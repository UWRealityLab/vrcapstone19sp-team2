#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRInteraction;

namespace VRWeaponInteractor
{
[CustomEditor(typeof(VRAmmoPack))]
	public class VRAmmoPackEditor : VRInteractableItemEditor
	{
		SerializedObject serializedItem;
		SerializedProperty selectedAmmoType;
		SerializedProperty ammoTypes;
		SerializedProperty ammoTypeNames;
		SerializedProperty slots;
		SerializedProperty placedSound;
		SerializedProperty removedSound;

		override public void OnEnable()
		{
			base.OnEnable();
			serializedItem = new SerializedObject(interactableItem);
			selectedAmmoType = serializedItem.FindProperty("selectedAmmoType");
			ammoTypes = serializedItem.FindProperty("ammoTypes");
			ammoTypeNames = serializedItem.FindProperty("ammoTypeNames");
			slots = serializedItem.FindProperty("slots");
			placedSound = serializedItem.FindProperty("placedSound");
			removedSound = serializedItem.FindProperty("removedSound");
		}

		public override void OnInspectorGUI()
		{
			serializedItem.Update();

			selectedAmmoType.intValue = EditorGUILayout.Popup("Selected Ammo Type", selectedAmmoType.intValue, ((VRAmmoPack)target).ammoTypeNames.ToArray());
			if (GUILayout.Button("Add Ammo Type"))
			{
				int index = ammoTypes.arraySize;
				ammoTypes.InsertArrayElementAtIndex(index);
				ammoTypeNames.InsertArrayElementAtIndex(index);
			}
			for(int i=0; i<ammoTypes.arraySize; i++)
			{
				SerializedProperty ammoType = ammoTypes.GetArrayElementAtIndex(i);
				EditorGUILayout.PropertyField(ammoType, true);
				ammoType.Next(true);
				ammoTypeNames.GetArrayElementAtIndex(i).stringValue = ammoType.stringValue;
				if (GUILayout.Button("Remove Ammo Type"))
				{
					ammoTypes.DeleteArrayElementAtIndex(i);
					ammoTypeNames.DeleteArrayElementAtIndex(i);
					serializedItem.ApplyModifiedProperties();
					return;
				}
			}
				
			EditorGUILayout.PropertyField(slots, true);
			EditorGUILayout.PropertyField(placedSound);
			EditorGUILayout.PropertyField(removedSound);

			serializedItem.ApplyModifiedProperties();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			base.OnInspectorGUI();
		}
	}
}
#endif