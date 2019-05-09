//========= Copyright 2019, Sam Tague, All rights reserved. ===================
//
// Editor for the Item slot inventory script
//
//=============================================================================
#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRInteraction;

namespace VRWeaponInteractor
{
	[CustomEditor(typeof(ItemSlot))]
	public class ItemSlotEditor : Editor
	{
		private ItemSlot itemSlot;
		private SerializedProperty slotPositions;
		private int	slotPositionSize;
		private bool positionsFoldout = true;
		private bool eventsFoldout = true;
		private List<bool> acceptedIdsFoldouts = new List<bool>();

		virtual public void OnEnable()
		{
			itemSlot = (ItemSlot)target;
			slotPositions = serializedObject.FindProperty("slotPositions");
			slotPositionSize = slotPositions.arraySize;
			acceptedIdsFoldouts = new List<bool>();
			for (int i=0; i<slotPositionSize; i++) acceptedIdsFoldouts.Add(true);
		}

		public override void OnInspectorGUI()
		{
			if (itemSlot == null) return;

			serializedObject.Update();

			eventsFoldout = EditorGUILayout.Foldout(eventsFoldout, "Slot Events");
			if (eventsFoldout)
			{
				EditorGUI.indentLevel++;
				SerializedProperty addItemSound = serializedObject.FindProperty("addItemSound");
				SerializedProperty removeItemSound = serializedObject.FindProperty("removeItemSound");
				SerializedProperty enterHover = serializedObject.FindProperty("enterHover");
				SerializedProperty exitHover = serializedObject.FindProperty("exitHover");

				SerializedProperty addItemEvent = serializedObject.FindProperty("addItemEvent");
				SerializedProperty removeItemEvent = serializedObject.FindProperty("removeItemEvent");
				SerializedProperty enterHoverEvent = serializedObject.FindProperty("enterHoverEvent");
				SerializedProperty exitHoverEvent = serializedObject.FindProperty("exitHoverEvent");

				EditorGUILayout.PropertyField(addItemSound);
				EditorGUILayout.PropertyField(removeItemSound);
				EditorGUILayout.PropertyField(enterHover);
				EditorGUILayout.PropertyField(exitHover);

				EditorGUILayout.PropertyField(addItemEvent);
				EditorGUILayout.PropertyField(removeItemEvent);
				EditorGUILayout.PropertyField(enterHoverEvent);
				EditorGUILayout.PropertyField(exitHoverEvent);

				ItemSlotEditor.DisplayHoverSegment(serializedObject);

				EditorGUI.indentLevel--;
			}

			positionsFoldout = EditorGUILayout.Foldout(positionsFoldout, "Slot Positions");
			if (positionsFoldout)
			{
				EditorGUI.indentLevel++;
				GUI.SetNextControlName("SlotSize");
				slotPositionSize = EditorGUILayout.IntField("Size", slotPositionSize);
				if (AreMatchingIds()) EditorGUILayout.HelpBox("One or more of the slot positions have the same accepted ids", MessageType.Warning);

				for (int i=0; i < slotPositions.arraySize; i++)
				{
					SerializedProperty slotPosition = slotPositions.GetArrayElementAtIndex(i);
					SerializedProperty acceptedIds = slotPosition.FindPropertyRelative("acceptedIds");
					SerializedProperty parentTarget = slotPosition.FindPropertyRelative("parentTarget");
					SerializedProperty localPosition = slotPosition.FindPropertyRelative("localPosition");
					SerializedProperty localRotation = slotPosition.FindPropertyRelative("localRotation");
					SerializedProperty distance = slotPosition.FindPropertyRelative("distance");
					SerializedProperty watchItemsFor = slotPosition.FindPropertyRelative("watchItemsFor");
					SerializedProperty triggerColliders = slotPosition.FindPropertyRelative("triggerColliders");
					SerializedProperty itemPrefab = slotPosition.FindPropertyRelative("itemPrefab");

					acceptedIdsFoldouts[i] = EditorGUILayout.Foldout(acceptedIdsFoldouts[i], "Accepted IDs");
					if (acceptedIdsFoldouts[i])
					{
						EditorGUI.indentLevel++;
						acceptedIds.arraySize = EditorGUILayout.IntField("Size", acceptedIds.arraySize);
						for(int j=0; j<acceptedIds.arraySize; j++)
						{
							
							SerializedProperty acceptedId = acceptedIds.GetArrayElementAtIndex(j);
							GUI.SetNextControlName("acceptedId"+i+j);
							acceptedId.stringValue = EditorGUILayout.TextField("Id "+j, acceptedId.stringValue);
						}
						EditorGUI.indentLevel--;
					}

					EditorGUILayout.PropertyField(parentTarget);
					EditorGUILayout.PropertyField(localPosition);
					Vector3 localRotationEuler = localRotation.quaternionValue.eulerAngles;
					localRotationEuler = EditorGUILayout.Vector3Field("Local Rotation", localRotationEuler);
					localRotation.quaternionValue = Quaternion.Euler(localRotationEuler);
					EditorGUILayout.PropertyField(distance);
					EditorGUILayout.PropertyField(watchItemsFor);

					EditorGUILayout.PropertyField(triggerColliders, true);

					EditorGUILayout.HelpBox("Item prefab is just for setting up the item position in the editor.", MessageType.Info);
					EditorGUILayout.PropertyField(itemPrefab);

					if (itemPrefab != null)
					{
						if (GUILayout.Button("Setup Slot Item Position"))
						{
							InventorySlotPositionWindow newWindow = (InventorySlotPositionWindow)EditorWindow.GetWindow(typeof(InventorySlotPositionWindow), true, "Slot Position", true);
							newWindow.itemSlot = (ItemSlot)target;
							newWindow.slotInstanceIndex = i;
						}
					}
					EditorGUI.indentLevel--;
					EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
					EditorGUI.indentLevel++;
				}
				EditorGUI.indentLevel--;
			}

			if (GUI.GetNameOfFocusedControl() == "SlotSize" && (Event.current.type == EventType.KeyUp) && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
			{
				slotPositions.arraySize = slotPositionSize;
				acceptedIdsFoldouts.Clear();
				for (int i=0; i<slotPositionSize; i++) acceptedIdsFoldouts.Add(true);
			}

			serializedObject.ApplyModifiedProperties();
		}

		private bool AreMatchingIds()
		{
			if (GUI.GetNameOfFocusedControl() != "") return false;

			for (int i=0; i < itemSlot.slotPositions.Count; i++)
			{
				foreach(string firstId in itemSlot.slotPositions[i].acceptedIds)
				{
					for (int j=0; j < itemSlot.slotPositions.Count; j++)
					{
						if (i == j) continue;
						foreach(string secondId in itemSlot.slotPositions[j].acceptedIds)
						{
							if (firstId == secondId) return true;
						}
					}
				}
			}
			return false;
		}

		public static bool DisplayHoverSegment(SerializedObject itemSlot)
		{
			bool changed = false;

			SerializedProperty hovers = itemSlot.FindProperty("hovers");
			SerializedProperty defaultShaders = itemSlot.FindProperty("defaultShaders");
			SerializedProperty hoverShaders = itemSlot.FindProperty("hoverShaders");
			SerializedProperty defaultMats = itemSlot.FindProperty("defaultMats");
			SerializedProperty hoverMats = itemSlot.FindProperty("hoverMats");
			SerializedProperty hoverModes = itemSlot.FindProperty("hoverModes");
			var oldHoverModeArraySize = hoverModes.arraySize;
			var olddefaultShadersArraySize = hoverModes.arraySize;
			var oldhoverShadersArraySize = hoverModes.arraySize;
			var olddefaultMatsArraySize = hoverModes.arraySize;
			var oldhoverMatsArraySize = hoverModes.arraySize;
			hoverModes.arraySize = defaultShaders.arraySize = hoverShaders.arraySize = defaultMats.arraySize = hoverMats.arraySize = hovers.arraySize;
			if (oldHoverModeArraySize != hoverModes.arraySize ||
				olddefaultShadersArraySize != defaultShaders.arraySize ||
				oldhoverShadersArraySize != hoverShaders.arraySize ||
				olddefaultMatsArraySize != defaultMats.arraySize ||
				oldhoverMatsArraySize != hoverMats.arraySize) changed = true;

			SerializedProperty hoverFoldout = itemSlot.FindProperty("hoverFoldout");
			hoverFoldout.boolValue = EditorGUILayout.Foldout(hoverFoldout.boolValue, "Hovers");
			if (!hoverFoldout.boolValue) return changed;

			EditorGUILayout.HelpBox("Here you can choose exactly how you want hover " +
				"to work, by default it will use an unlit shader for the hover. You " +
				"can chose a different shader or you can switch to two materials. " +
				"Leaving the default shader blank will have it use the shader or " +
				"material the mesh renderer has at startup.", MessageType.Info);

			var oldArraySize = hovers.arraySize;
			hoverModes.arraySize = defaultShaders.arraySize = hoverShaders.arraySize = defaultMats.arraySize = hoverMats.arraySize = hovers.arraySize = EditorGUILayout.IntField("Size", hovers.arraySize);
			if (oldArraySize != hovers.arraySize) changed = true;
			for(int i=0; i<hovers.arraySize; i++)
			{
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				SerializedProperty hover = hovers.GetArrayElementAtIndex(i);
				var oldHover = hover.objectReferenceValue;
				hover.objectReferenceValue = EditorGUILayout.ObjectField("Hover", hover.objectReferenceValue, typeof(Renderer), true);
				if (oldHover != hover.objectReferenceValue) changed = true;
				if (hover.objectReferenceValue != null)
				{
					SerializedProperty hoverMode = hoverModes.GetArrayElementAtIndex(i);
					var oldHoverMode = hoverMode.intValue;
					hoverMode.intValue = (int)(VRInteractableItem.HoverMode)EditorGUILayout.EnumPopup("Hover Mode", (VRInteractableItem.HoverMode)hoverMode.intValue);
					if (oldHoverMode != hoverMode.intValue) changed = true;
					VRInteractableItem.HoverMode hoverModeEnum = (VRInteractableItem.HoverMode)hoverMode.intValue;
					switch(hoverModeEnum)
					{
					case VRInteractableItem.HoverMode.SHADER:
						EditorGUILayout.HelpBox("Leave null to use current materials shader", MessageType.Info);
						SerializedProperty defaultShader = defaultShaders.GetArrayElementAtIndex(i);
						var oldDefaultShader = defaultShader.objectReferenceValue;
						defaultShader.objectReferenceValue = EditorGUILayout.ObjectField("Default Shader", defaultShader.objectReferenceValue, typeof(Shader), false);
						if (oldDefaultShader != defaultShader.objectReferenceValue) changed = true;
						EditorGUILayout.HelpBox("Hover Default is Unlit/Texture", MessageType.Info);
						SerializedProperty hoverShader = hoverShaders.GetArrayElementAtIndex(i);
						var oldHoverShader = hoverShader.objectReferenceValue;
						hoverShader.objectReferenceValue = EditorGUILayout.ObjectField("Hover Shader", hoverShader.objectReferenceValue, typeof(Shader), false);
						if (oldHoverShader != hoverShader.objectReferenceValue) changed = true;
						break;
					case VRInteractableItem.HoverMode.MATERIAL:
						EditorGUILayout.HelpBox("Leave null to use current material", MessageType.Info);
						SerializedProperty defaultMat = defaultMats.GetArrayElementAtIndex(i);
						var oldDefaultMat = defaultMat.objectReferenceValue;
						defaultMat.objectReferenceValue = EditorGUILayout.ObjectField("Default Material", defaultMat.objectReferenceValue, typeof(Material), false);
						if (oldDefaultMat != defaultMat.objectReferenceValue) changed = true;
						EditorGUILayout.HelpBox("Hover Default is Unlit/Texture", MessageType.Info);
						SerializedProperty hoverMat = hoverMats.GetArrayElementAtIndex(i);
						var oldHoverMat = hoverMat.objectReferenceValue;
						hoverMat.objectReferenceValue = EditorGUILayout.ObjectField("Hover Material", hoverMat.objectReferenceValue, typeof(Material), false);
						if (oldHoverMat != hoverMat.objectReferenceValue) changed = true;
						break;
					}
				}
			}
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			return changed;
		}
	}
}
#endif