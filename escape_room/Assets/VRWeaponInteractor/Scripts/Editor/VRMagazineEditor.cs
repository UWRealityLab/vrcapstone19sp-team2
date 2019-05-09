//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Editor for VRMagazine. Inherits from VRInteractableItemEditor
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace VRWeaponInteractor
{
	[CustomEditor(typeof(VRMagazine))]
	public class VRMagazineEditor : VRAttachmentEditor
	{
		// target component
		public VRMagazine magazine = null;

		override public void OnEnable()
		{
			base.OnEnable();
			magazine = (VRMagazine)target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			MagazineSection(serializedObject);
			serializedObject.ApplyModifiedProperties();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			base.OnInspectorGUI();
		}

		/// <summary>
		/// Magazines the section. Returns true if something was changed
		/// </summary>
		/// <returns><c>true</c>, if section was magazined, <c>false</c> otherwise.</returns>
		/// <param name="serializedMagazine">Serialized magazine.</param>
		/// <param name="serializedGunHandler">Serialized gun handler.</param>
		static public bool MagazineSection(SerializedObject serializedMagazine)
		{
			bool changed = false;

			SerializedProperty bulletId = serializedMagazine.FindProperty("bulletId");
			var oldBulletId = bulletId.intValue;
			EditorGUILayout.PropertyField(bulletId);
			if (oldBulletId != bulletId.intValue) changed = true;

			SerializedProperty infiniteAmmo = serializedMagazine.FindProperty("infiniteAmmo");
			var oldInfiniteAmmo = infiniteAmmo.boolValue;
			EditorGUILayout.PropertyField(infiniteAmmo);
			if (oldInfiniteAmmo != infiniteAmmo.boolValue) changed = true;

			SerializedProperty addBulletOnLoad = serializedMagazine.FindProperty("addBulletOnLoad");
			var oldAutoLoad = addBulletOnLoad.boolValue;
			EditorGUILayout.PropertyField(addBulletOnLoad);
			if (oldAutoLoad != addBulletOnLoad.boolValue) changed = true;

			SerializedProperty clipSize = serializedMagazine.FindProperty("clipSize");
			var oldClipSize = clipSize.intValue;
			EditorGUILayout.PropertyField(clipSize);
			if (oldClipSize != clipSize.intValue) changed = true;

			SerializedProperty startFull = serializedMagazine.FindProperty("startFull");
			var oldstartFull = startFull.boolValue;
			EditorGUILayout.PropertyField(startFull);
			if (oldstartFull != startFull.boolValue) changed = true;

			SerializedProperty loadBulletSound = serializedMagazine.FindProperty("loadBulletSound");
			var oldLoadBulletSound = loadBulletSound.objectReferenceValue;
			EditorGUILayout.PropertyField(loadBulletSound);
			if (oldLoadBulletSound != loadBulletSound.objectReferenceValue) changed = true;

			return changed;
		}
	}
}
#endif