#if VRInteraction
using UnityEngine;
using UnityEditor;
using System.Collections;
using VRInteraction;

namespace VRWeaponInteractor
{
	[CustomEditor(typeof(VRBow))]
	public class VRBowEditor : VRInteractableItemEditor
	{
		/*public GameObject arrowPrefab;
		public Collider arrowReceiver;

		public Transform arrowDefaultPoint;
		public Transform arrowPulledPoint;*/

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			SerializedProperty arrowPrefab = serializedObject.FindProperty("arrowPrefab");
			EditorGUILayout.PropertyField(arrowPrefab);
			SerializedProperty animator = serializedObject.FindProperty("animator");
			EditorGUILayout.PropertyField(animator);
			SerializedProperty bowPullAnimName = serializedObject.FindProperty("bowPullAnimName");
			EditorGUILayout.PropertyField(bowPullAnimName);
			SerializedProperty bowShootAnimName = serializedObject.FindProperty("bowShootAnimName");
			EditorGUILayout.PropertyField(bowShootAnimName);
			SerializedProperty autoLoadNextArrow = serializedObject.FindProperty("autoLoadNextArrow");
			EditorGUILayout.PropertyField(autoLoadNextArrow);
			SerializedProperty distanceMultiplier = serializedObject.FindProperty("distanceMultiplier");
			EditorGUILayout.PropertyField(distanceMultiplier);
			SerializedProperty arrowPower = serializedObject.FindProperty("arrowPower");
			EditorGUILayout.PropertyField(arrowPower);
			SerializedProperty arrowDefaultPoint = serializedObject.FindProperty("arrowDefaultPoint");
			EditorGUILayout.PropertyField(arrowDefaultPoint);
			SerializedProperty arrowPulledPoint = serializedObject.FindProperty("arrowPulledPoint");
			EditorGUILayout.PropertyField(arrowPulledPoint);
			serializedObject.ApplyModifiedProperties();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			base.OnInspectorGUI();
		}
	}
}
#endif