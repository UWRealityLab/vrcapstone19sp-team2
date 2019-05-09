using UnityEngine;
using System;
using UnityEditor;
/****************************************
	BreakableObject Editor v1.09			
	Copyright 2013 Unluck Software	
 	www.chemicalbliss.com																																				
*****************************************/
[CustomEditor(typeof(BreakableObject))]
[CanEditMultipleObjects]

[System.Serializable]
public class BreakableObjectEditor: Editor {
    public override void OnInspectorGUI() {
    	var target_cs = (BreakableObject)target;
        EditorGUILayout.LabelField("Drag & Drop", EditorStyles.miniLabel);
    	target_cs.fragments = (Transform)EditorGUILayout.ObjectField("Fractured Object Prefab", target_cs.fragments, typeof(Transform) ,false );
    	target_cs.breakParticles = (ParticleSystem)EditorGUILayout.ObjectField("Particle System Prefab", target_cs.breakParticles, typeof(ParticleSystem) ,false);
    	EditorGUILayout.Space();
    	EditorGUILayout.LabelField("Seconds before removing fragment colliders (zero = never)", EditorStyles.miniLabel);   	
    	target_cs.waitForRemoveCollider = EditorGUILayout.FloatField("Remove Collider Delay" , target_cs.waitForRemoveCollider);
    	EditorGUILayout.Space();
    	EditorGUILayout.LabelField("Seconds before removing fragment rigidbodies (zero = never)", EditorStyles.miniLabel);   	
    	target_cs.waitForRemoveRigid = EditorGUILayout.FloatField("Remove Rigidbody Delay" , target_cs.waitForRemoveRigid);	
    	EditorGUILayout.Space();
  		EditorGUILayout.LabelField("Seconds before removing fragments (zero = never)", EditorStyles.miniLabel);   	
    	target_cs.waitForDestroy = EditorGUILayout.FloatField("Destroy Fragments Delay" , target_cs.waitForDestroy);	
    	EditorGUILayout.Space();
    	EditorGUILayout.LabelField("Force applied to fragments after object is broken", EditorStyles.miniLabel);   
    	target_cs.explosiveForce = EditorGUILayout.FloatField("Fragment Force" , target_cs.explosiveForce);
    	EditorGUILayout.Space();
    	EditorGUILayout.LabelField("How hard must object be hit before it breaks", EditorStyles.miniLabel);   	
    	target_cs.durability = EditorGUILayout.FloatField("Object Durability" , target_cs.durability);	
    	target_cs.mouseClickDestroy = EditorGUILayout.Toggle("Click To Break Object" , target_cs.mouseClickDestroy);
        if (GUI.changed)
            EditorUtility.SetDirty (target_cs);
    }
}