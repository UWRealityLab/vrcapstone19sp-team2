//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Window used to set positions for inventory slot items
//
//===================Contact Email: Sam@MassGames.co.uk===========================
#if VRInteraction
using UnityEngine;
using UnityEditor;
using System.Collections;
using VRInteraction;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace VRWeaponInteractor
{
	public class InventorySlotPositionWindow : EditorWindow 
	{
		public ItemSlot itemSlot;

		private GameObject itemInstance;
		private int _slotInstanceIndex;

		public int slotInstanceIndex
		{
			get { return _slotInstanceIndex; }
			set { _slotInstanceIndex = value; }
		}

		void OnGUI()
		{
			if (itemSlot == null) return;

			bool changed = false;

			if (itemSlot.slotPositions.Count == 0)
			{
				EditorGUILayout.HelpBox("Item Slot has no slot positions", MessageType.Warning);
				return;
			} else if (slotInstanceIndex >= itemSlot.slotPositions.Count) slotInstanceIndex = itemSlot.slotPositions.Count-1;

			Undo.RecordObject(itemSlot, "Change Item Slot");

			ItemSlot.SlotPosition slotPosition = itemSlot.slotPositions[slotInstanceIndex];

			var oldItemPrefab = slotPosition.itemPrefab;
			slotPosition.itemPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", slotPosition.itemPrefab, typeof(GameObject), false);
			if (oldItemPrefab != slotPosition.itemPrefab) changed = true;

			if (itemInstance == null)
			{
				if (GUILayout.Button("Spawn Instance"))
				{
					itemInstance = Instantiate<GameObject>(slotPosition.itemPrefab);
					Undo.RegisterCreatedObjectUndo(itemInstance, "Spawn Inventory Slot Item");
					if (slotPosition.parentTarget == null)
					{
						changed = true;
						slotPosition.parentTarget = itemSlot.transform;
					}
					itemInstance.transform.parent = slotPosition.parentTarget;
					itemInstance.transform.localPosition = slotPosition.localPosition;
					itemInstance.transform.localRotation = slotPosition.localRotation;
					Selection.activeGameObject = itemInstance;
				}
			} else
			{
				GUILayout.Label(slotPosition.localPosition.ToString("N3"));
				GUILayout.Label(slotPosition.localRotation.eulerAngles.ToString("N3"));
				if (GUILayout.Button("Save Current Position"))
				{
					slotPosition.localPosition = itemInstance.transform.localPosition;
					slotPosition.localRotation = itemInstance.transform.localRotation;
					changed = true;
				}
				if (GUILayout.Button("Select Instance"))
				{
					Selection.activeGameObject = itemInstance;
				}
				if (GUILayout.Button("Clear Reference"))
				{
					Undo.DestroyObjectImmediate(itemInstance);
				}
			}

			if (changed)
			{
				EditorUtility.SetDirty(itemSlot);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}

		void OnDestroy()
		{
			if (itemInstance != null) Undo.DestroyObjectImmediate(itemInstance);
		}
	}
}
#endif