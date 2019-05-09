//========= Copyright 2017, Sam Tague, All rights reserved. ===================
//
// Wizard window to walk through the initial settings for setting up a bullet
//
//=============================================================================

/*using UnityEngine;
using UnityEditor;
using System.Collections;

namespace VRInteraction
{

	public class VRBulletWizard : EditorWindow 
	{
		[MenuItem("VR Weapon Interactor/Bullet Wizard", false, 0)]
		public static void MenuInitAbout()
		{
			EditorWindow.GetWindow(typeof(VRBulletWizard), true, "BUllet Wizard", true);
		}

		public GameObject bulletReference;
		bool noMeshRenderer = false;
		public int bulletId;

		void OnGUI () 
		{
			EditorGUILayout.HelpBox("Welcome to VR Bullet Wizard. Drop the bullet object and click" +
				" setup. Set the held position and save the result as a prefab, you can then use that " +
				"as a reference in the weapon editor. Once setup make sure to check the collider looks" +
				" right and adjust as necessary.", MessageType.Info);

			if (noMeshRenderer)
			{
				EditorGUILayout.HelpBox("No mesh renderer found", MessageType.Error);
			}

			var oldBulletReference = bulletReference;
			bulletReference = (GameObject)EditorGUILayout.ObjectField("Bullet Object", bulletReference, typeof(GameObject), true);
			if (noMeshRenderer && oldBulletReference != bulletReference) noMeshRenderer = false;
			if (bulletReference != null)
			{
				PrefabType prefabType = PrefabUtility.GetPrefabType(bulletReference);
				if (prefabType == PrefabType.ModelPrefab || prefabType == PrefabType.Prefab)
				{
					EditorGUILayout.HelpBox("Bullet reference must be a gameobject in the scene.", MessageType.Warning);
					return;
				}
				bulletId = EditorGUILayout.IntField("Bullet Id", bulletId);
				if (GUILayout.Button("Setup"))
				{
					MeshRenderer bulletRenderer = bulletReference.GetComponentInChildren<MeshRenderer>();
					if (bulletRenderer == null)
					{
						noMeshRenderer = true;
						return;
					}
					GameObject parentObject = null;
					if (bulletRenderer.gameObject == bulletReference)
					{
						parentObject = new GameObject("Bullet");
						Undo.RegisterCreatedObjectUndo(parentObject, "Setup Bullet");
						parentObject.transform.SetParent(bulletReference.transform.parent);
						parentObject.transform.localPosition = bulletReference.transform.localPosition;
						parentObject.transform.localRotation = bulletReference.transform.localRotation;
						parentObject.transform.localScale = bulletReference.transform.localScale;
						Undo.SetTransformParent(bulletReference.transform, parentObject.transform, "Setup Bullet");
					} else
						parentObject = bulletReference;

					VRLoadableBullet loadableBullet = Undo.AddComponent<VRLoadableBullet>(bulletRenderer.gameObject);
					loadableBullet.bulletId = bulletId;
					BoxCollider triggerCollider = Undo.AddComponent<BoxCollider>(bulletRenderer.gameObject);
					triggerCollider.isTrigger = true;
					Rigidbody parentBody = Undo.AddComponent<Rigidbody>(parentObject);
					parentBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
					VRGunWizard.ResizedCollider(parentObject, triggerCollider);
					loadableBullet.item = parentObject.transform;
					loadableBullet.hovers.Add(bulletRenderer);

					HeldPositionWindow weaponHeldWindow = (HeldPositionWindow)EditorWindow.GetWindow(typeof(HeldPositionWindow), true, "Bullet Held Position", true);
					weaponHeldWindow.interactableItem = loadableBullet;
					weaponHeldWindow.Init();

					Close();
				}
			}
		}
	}

}*/