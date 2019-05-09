#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRInteraction;

namespace VRWeaponInteractor
{
	[CustomEditor(typeof(VRAttachment))]
	public class VRAttachmentEditor : VRInteractableItemEditor
	{
		override public void OnEnable()
		{
			base.OnEnable();

			if (interactableItem == null) return;

			Collider col = interactableItem.GetComponent<Collider>();
			if (col == null) col = interactableItem.gameObject.AddComponent<BoxCollider>();
			if (!col.isTrigger) col.isTrigger = true; 
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			SerializedProperty showHighlight = serializedObject.FindProperty("showHighlight");
			EditorGUILayout.PropertyField(showHighlight);
			if (showHighlight.boolValue)
			{
				SerializedProperty highlightMaterial = serializedObject.FindProperty("highlightMaterial");
				EditorGUILayout.PropertyField(highlightMaterial);

				SerializedProperty readyToAttachMaterial = serializedObject.FindProperty("readyToAttachMaterial");
				EditorGUILayout.PropertyField(readyToAttachMaterial);
			}
			SerializedProperty attachSound = serializedObject.FindProperty("attachSound");
			EditorGUILayout.PropertyField(attachSound);

			SerializedProperty detatchSound = serializedObject.FindProperty("detatchSound");
			EditorGUILayout.PropertyField(detatchSound);

			SerializedProperty attachEvent = serializedObject.FindProperty("attachEvent");
			EditorGUILayout.PropertyField(attachEvent);

			SerializedProperty detatchEvent = serializedObject.FindProperty("detatchEvent");
			EditorGUILayout.PropertyField(detatchEvent);

			SerializedProperty attachmentRefs = serializedObject.FindProperty("attachmentRefs");

			if (attachmentRefs.arraySize != 0)
			{
				if (GUILayout.Button("Clear References"))
				{
					attachmentRefs.ClearArray();
				}
			}

			serializedObject.ApplyModifiedProperties();


			EditorGUILayout.PropertyField(attachmentRefs, true);

			EditorGUILayout.HelpBox("References to all compatible weapons, to add a new weapon drag weapon into scene open the editor and assign this attachments prefab" +
				" to a VRAttachmentReceiver in the attahments tab. You cannot modify this list from here, this is just to make it easy to see what weapons this attachment are setup to be compatable with.", MessageType.Info);

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			base.OnInspectorGUI();
		}
	}
}
#endif