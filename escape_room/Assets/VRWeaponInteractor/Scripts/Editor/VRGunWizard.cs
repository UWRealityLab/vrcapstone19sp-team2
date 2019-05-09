//========= Copyright 2017, Sam Tague, All rights reserved. ===================
//
// Wizard window to walk through the initial settings for setting up a weapon
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using UnityEditor;
using System.Collections;
using VRWeaponInteractor;

namespace VRInteraction
{
	public class VRGunWizard : EditorWindow 
	{
		public enum WizardType
		{
			Weapon,
			Bullet
		}

		public WizardType wizardType = WizardType.Weapon;

		//References
		GameObject rootObject;
		GameObject slideMesh;
		GameObject triggerMesh;
		GameObject magMesh;

		bool isCasing;
		int bulletId;
		bool destroyIn = true;
		float destroyInSeconds = 5f;

		[MenuItem("VR Weapon Interactor/Weapon Wizard", false, 0)]
		public static void MenuInitWeapon()
		{
			VRGunWizard gunWizard = (VRGunWizard)EditorWindow.GetWindow(typeof(VRGunWizard), true, "Weapon Wizard", true);
			gunWizard.wizardType = WizardType.Weapon;
		}

		[MenuItem("VR Weapon Interactor/Bullet Wizard", false, 0)]
		public static void MenuInitBullet()
		{
			VRGunWizard gunWizard = (VRGunWizard)EditorWindow.GetWindow(typeof(VRGunWizard), true, "Weapon Wizard", true);
			gunWizard.wizardType = WizardType.Bullet;
		}

		void OnGUI () 
		{
			switch(wizardType)
			{
			case WizardType.Weapon:
				ShowWeaponPreSetup();
				break;
			case WizardType.Bullet:
				ShowBulletPreSetup();
				break;
			}
		}

		private void ShowWeaponPreSetup()
		{
			GUILayout.Label ("Welcome To The VR Weapon Setup Wizard", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox("It should have minimum one mesh," +
				" the body and optional meshes for the trigger, the slide and magazine. If the model isn't split into these seperate meshes " +
				"you can still make a working gun, it just won't have any moveable parts. See the example weapons for reference.", MessageType.Info);

			bool badSettings = false;

			GUILayout.Label("Assigning models", EditorStyles.boldLabel);
			rootObject = (GameObject)EditorGUILayout.ObjectField("Gun Root", rootObject, typeof(GameObject), true);
			slideMesh = (GameObject)EditorGUILayout.ObjectField("Slide mesh", slideMesh, typeof(GameObject), true);
			if (slideMesh != null)
			{
				if (rootObject != null && slideMesh == rootObject)
				{
					EditorGUILayout.HelpBox("Slide cannot also be the root", MessageType.Warning);
					badSettings = true;
				}
				Renderer slideMeshRenderer = slideMesh.GetComponentInChildren<Renderer>();
				if (slideMeshRenderer == null)
				{
					EditorGUILayout.HelpBox("Slide mesh should contain a Renderer", MessageType.Warning);
					badSettings = true;
				}
			}
			triggerMesh = (GameObject)EditorGUILayout.ObjectField("Trigger mesh", triggerMesh, typeof(GameObject), true);
			if (triggerMesh != null)
			{
				if (rootObject != null && triggerMesh == rootObject)
				{
					EditorGUILayout.HelpBox("Trigger cannot also be the root", MessageType.Warning);
					badSettings = true;
				}
				Renderer triggerMeshRenderer = triggerMesh.GetComponentInChildren<Renderer>();
				if (triggerMeshRenderer == null)
				{
					EditorGUILayout.HelpBox("Trigger mesh should contain a Renderer", MessageType.Warning);
					badSettings = true;
				}
			}
			magMesh = (GameObject)EditorGUILayout.ObjectField("Mag mesh", magMesh, typeof(GameObject), true);
			if (magMesh != null)
			{
				EditorGUILayout.HelpBox("After setup is complete the magazine will be in the scene with no parent. " +
					"To finish drag the magazine into the project to turn it into a prefab then delete the " +
					"instance, in the attachments tab in the gun handler click the 'Add Attachment Receiver' button, " +
					"drag the new magazine prefab into the slot and resize the receiver to the approprate size. " +
					"Once the reference is set you can make sure it is positions correctly by clicking 'Edit Attachment'", MessageType.Info);
				if (rootObject != null && magMesh == rootObject)
				{
					EditorGUILayout.HelpBox("Magazine cannot also be the root", MessageType.Warning);
					badSettings = true;
				}
				Renderer magMeshRenderer = magMesh.GetComponentInChildren<Renderer>();
				if (magMeshRenderer == null)
				{
					EditorGUILayout.HelpBox("Magazine mesh should contain a Renderer", MessageType.Warning);
					badSettings = true;
				}
			}
			if (rootObject != null && !badSettings)
			{
				if (GUILayout.Button("Setup"))
					CompleteSetup();
			}
		}

		private void ShowBulletPreSetup()
		{
			GUILayout.Label ("Welcome To The VR Bullet Setup Wizard", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox("Once the bullet is setup you should make it a prefab by dragging the instance into the project," +
				" then deleting the instance in the scene. You can then drag the bullet prefab into the gun editor.", MessageType.Info);

			bool badSettings = false;

			GUILayout.Label("Assigning models", EditorStyles.boldLabel);
			rootObject = (GameObject)EditorGUILayout.ObjectField("Bullet Root", rootObject, typeof(GameObject), true);

			if (rootObject != null)
			{
				Renderer bulletRenderer = rootObject.GetComponentInChildren<Renderer>();
				if (bulletRenderer == null)
				{
					badSettings = true;
					EditorGUILayout.HelpBox("Bullet needs at least one renderer or it will be invisible", MessageType.Warning);
				}

				isCasing = EditorGUILayout.Toggle("Is Casing", isCasing);
				if (isCasing)
				{
					destroyIn = EditorGUILayout.Toggle("Destroy In", destroyIn);
					if (destroyIn)
					{
						destroyInSeconds = EditorGUILayout.FloatField("Destroy In Seconds", destroyInSeconds);
					}
				} else
				{
					bulletId = EditorGUILayout.IntField("Bullet Id", bulletId);
				}
			}

			if (rootObject != null && !badSettings)
			{
				if (GUILayout.Button("Setup"))
					CompleteSetupBullet();
			}
		}

		private void CompleteSetup()
		{
			DeleteUnessentials(rootObject);

			Renderer rootRenderer = rootObject.GetComponent<Renderer>();
			if (rootRenderer != null)
			{
				GameObject newGunRoot = new GameObject(rootObject.name);
				newGunRoot.transform.position = rootObject.transform.position;
				newGunRoot.transform.rotation = rootObject.transform.rotation;
				rootObject.transform.SetParent(newGunRoot.transform);
				rootObject = newGunRoot;
			}

			Undo.AddComponent<Rigidbody>(rootObject);

			//	Add gun handler script
			GameObject gunHandlerObject = new GameObject("VRGunHandler");
			gunHandlerObject.transform.SetParent(rootObject.transform);
			gunHandlerObject.transform.localPosition = Vector3.zero;
			gunHandlerObject.transform.localRotation = Quaternion.identity;
			gunHandlerObject.transform.localScale = Vector3.one;

			VRGunHandler gunHandler = Undo.AddComponent<VRGunHandler>(gunHandlerObject);
			gunHandler.item = rootObject.transform;
			gunHandler.defaultPosition = gunHandler.transform.localPosition;
			gunHandler.defaultRotation = gunHandler.transform.localRotation;
			gunHandler.toggleToPickup = true;

			VRGunHandlerRef gunHandlerRef = Undo.AddComponent<VRGunHandlerRef>(rootObject);
			gunHandlerRef.gunHandler = gunHandler;

			if (triggerMesh != null)
			{
				GameObject triggerObject = new GameObject("Trigger");
				triggerObject.transform.SetParent(triggerMesh.transform.parent);
				triggerObject.transform.localPosition = triggerMesh.transform.localPosition;
				triggerObject.transform.localRotation = triggerMesh.transform.localRotation;

				triggerMesh.transform.SetParent(triggerObject.transform);
				triggerMesh.transform.localPosition = Vector3.zero;
				triggerMesh.transform.localRotation = Quaternion.identity;

				VRGunTrigger trigger = Undo.AddComponent<VRGunTrigger>(triggerObject);
				gunHandler.trigger = trigger;
				trigger.gunHandler = gunHandler;
				trigger.defaultTriggerPosition = trigger.pulledTriggerPosition = trigger.transform.localPosition;
				trigger.defaultTriggerRotation = trigger.pulledTriggerRotation = trigger.transform.localRotation;
			}

			if (slideMesh != null)
			{
				//Add slide script
				GameObject slideObject = new GameObject("Slide");
				slideObject.transform.SetParent(slideMesh.transform.parent);
				slideObject.transform.localPosition = slideMesh.transform.localPosition;
				slideObject.transform.localRotation = slideMesh.transform.localRotation;

				slideMesh.transform.SetParent(slideObject.transform);
				slideMesh.transform.localPosition = Vector3.zero;
				slideMesh.transform.localRotation = Quaternion.identity;

				VRGunSlide gunSlide = Undo.AddComponent<VRGunSlide>(slideObject);
				gunHandler.slide = gunSlide;
				gunSlide.gunHandler = gunHandler;
				gunSlide.item = slideObject.transform;
				gunSlide.parents.Add(gunHandler);
				gunSlide.defaultPosition = gunSlide.pulledPosition = gunSlide.transform.localPosition;
				gunSlide.defaultRotation = gunSlide.pulledRotation = gunSlide.transform.localRotation;

				Renderer[] gunSlideMeshRenderers = slideObject.GetComponentsInChildren<Renderer>();
				foreach(Renderer gunSlideMeshRenderer in gunSlideMeshRenderers)
				{
					gunSlide.hovers.Add(gunSlideMeshRenderer);
					gunSlide.hoverModes.Add(VRInteractableItem.HoverMode.SHADER);
					gunSlide.defaultShaders.Add(null);
					gunSlide.defaultMats.Add(null);
					gunSlide.hoverShaders.Add(null);
					gunSlide.hoverMats.Add(null);
				}
			}
				
			if (magMesh != null)
			{
				GameObject magRoot = new GameObject(magMesh.name);
				magRoot.transform.position = magMesh.transform.position;
				magRoot.transform.rotation = magMesh.transform.rotation;

				magMesh.transform.SetParent(magRoot.transform);
				magMesh.transform.localPosition = Vector3.zero;
				magMesh.transform.localRotation = Quaternion.identity;

				Rigidbody magBody = Undo.AddComponent<Rigidbody>(magRoot);
				magBody.collisionDetectionMode = CollisionDetectionMode.Discrete;

				GameObject magObject = new GameObject("VRMagazine");
				magObject.transform.SetParent(magRoot.transform);
				magObject.transform.localPosition = Vector3.zero;
				magObject.transform.localRotation = Quaternion.identity;
				magObject.transform.localScale = Vector3.one;

				VRMagazine magazine = Undo.AddComponent<VRMagazine>(magObject);
				magazine.item = magRoot.transform;

				GameObject magCollisionObject = new GameObject("Collision");
				magCollisionObject.transform.SetParent(magRoot.transform);
				magCollisionObject.transform.localPosition = Vector3.zero;
				magCollisionObject.transform.localRotation = Quaternion.identity;

				BoxCollider magazineReceiverCollider = null;

				Renderer[] magMeshRenderers = magMesh.GetComponentsInChildren<Renderer>();
				foreach(Renderer magMeshRenderer in magMeshRenderers)
				{
					magazine.hovers.Add(magMeshRenderer);
					magazine.hoverModes.Add(VRInteractableItem.HoverMode.SHADER);
					magazine.defaultShaders.Add(null);
					magazine.defaultMats.Add(null);
					magazine.hoverShaders.Add(null);
					magazine.hoverMats.Add(null);

					GameObject magColliderObject = new GameObject("Collider");
					magColliderObject.transform.SetParent(magCollisionObject.transform);
					magColliderObject.transform.position = magMeshRenderer.transform.position;
					magColliderObject.transform.rotation = magMeshRenderer.transform.rotation;
					ResizedCollider(magColliderObject, magMeshRenderer);

					VRItemCollider magItemCollider = Undo.AddComponent<VRItemCollider>(magColliderObject);
					magItemCollider.item = magazine;

					if (magazineReceiverCollider == null)
					{
						magazineReceiverCollider = ResizedCollider(magObject, magMeshRenderer);
						magazineReceiverCollider.isTrigger = true;
					}
				}
			} else
			{
				GameObject magObject = new GameObject("VRMagazine");
				magObject.transform.SetParent(rootObject.transform);
				magObject.transform.localPosition = Vector3.zero;
				magObject.transform.localRotation = Quaternion.identity;

				VRMagazine magazine = Undo.AddComponent<VRMagazine>(magObject);
				magazine.item = magObject.transform;
				magazine.interactionDisabled = true;
				magazine.parents.Add(gunHandler);

				BoxCollider magazineCollider = Undo.AddComponent<BoxCollider>(magObject);
				magazineCollider.size = new Vector3(0.01f, 0.01f, 0.01f);
				magazineCollider.isTrigger = true;

				GameObject magazineReceiver = new GameObject("Magazine Receiver");
				magazineReceiver.transform.SetParent(rootObject.transform);
				magazineReceiver.transform.localPosition = Vector3.zero;
				magazineReceiver.transform.localRotation = Quaternion.identity;
				VRAttachmentReceiver attachmentReceiver = Undo.AddComponent<VRAttachmentReceiver>(magazineReceiver);
				attachmentReceiver.gunHandler = gunHandler;
				BoxCollider magazineReceiverCollider = Undo.AddComponent<BoxCollider>(magazineReceiver);
				magazineReceiverCollider.size = new Vector3(0.01f, 0.01f, 0.01f);
				magazineReceiverCollider.isTrigger = true;

				VRGunHandler.AttachmentPrefabs attachmentPrefabs = new VRGunHandler.AttachmentPrefabs();
				attachmentPrefabs.attachmentPrefab = magObject;
				attachmentPrefabs.isPrefab = false;
				attachmentPrefabs.startLoaded = true;
				attachmentPrefabs.attachmentReceiver = attachmentReceiver;
				gunHandler.attachmentPrefabs.Add(attachmentPrefabs);
			}

			//Magazine is gone so add all remaining renderers to the hover highlight list
			//and create basic collision boxes
			GameObject collisionObject = new GameObject("Collision");
			collisionObject.transform.SetParent(rootObject.transform);
			collisionObject.transform.localPosition = Vector3.zero;
			collisionObject.transform.localRotation = Quaternion.identity;

			Renderer[] renderers = rootObject.GetComponentsInChildren<Renderer>();
			foreach(Renderer renderer in renderers)
			{
				gunHandler.hovers.Add(renderer);
				gunHandler.hoverModes.Add(VRInteractableItem.HoverMode.SHADER);
				gunHandler.defaultShaders.Add(null);
				gunHandler.defaultMats.Add(null);
				gunHandler.hoverShaders.Add(null);
				gunHandler.hoverMats.Add(null);

				GameObject colliderObject = new GameObject("Collider");
				colliderObject.transform.SetParent(collisionObject.transform);
				colliderObject.transform.localPosition = Vector3.zero; // renderer.transform.position;
				colliderObject.transform.localRotation = Quaternion.identity; // renderer.transform.rotation;
				ResizedCollider(colliderObject, renderer);

				VRItemCollider itemCollider = Undo.AddComponent<VRItemCollider>(colliderObject);
				itemCollider.item = gunHandler;
			}

			GunHandlerWindow newWindow = (GunHandlerWindow)EditorWindow.GetWindow(typeof(GunHandlerWindow), true, "Gun Handler", true);
			newWindow.gunHandler = gunHandler;
			newWindow.Init();
			newWindow.weaponTab = GunHandlerWindow.WeaponTab.MAIN;

			Selection.activeGameObject = rootObject;

			Close();
		}

		private void CompleteSetupBullet()
		{
			DeleteUnessentials(rootObject);

			Renderer rootRenderer = rootObject.GetComponent<Renderer>();
			if (rootRenderer != null)
			{
				GameObject newBulletRoot = new GameObject(rootObject.name);
				newBulletRoot.transform.position = rootObject.transform.position;
				newBulletRoot.transform.rotation = rootObject.transform.rotation;
				rootObject.transform.SetParent(newBulletRoot.transform);
				rootObject = newBulletRoot;
			}

			Undo.AddComponent<Rigidbody>(rootObject);

			GameObject bulletObject = new GameObject("VRBullet");
			bulletObject.transform.SetParent(rootObject.transform);
			bulletObject.transform.localPosition = Vector3.zero;
			bulletObject.transform.localRotation = Quaternion.identity;

			VRInteractableItem bulletItem = null;
			if (isCasing)
			{
				bulletItem = Undo.AddComponent<VRInteractableItem>(bulletObject);
				bulletItem.canBeHeld = false;
				bulletItem.interactionDisabled = true;
				if (destroyIn)
				{
					DestroyIn destroyInScript = Undo.AddComponent<DestroyIn>(rootObject);
					destroyInScript.seconds = destroyInSeconds;
				}
			} else
			{
				VRLoadableBullet bullet = Undo.AddComponent<VRLoadableBullet>(bulletObject);
				bullet.bulletId = bulletId;
				bulletItem = bullet;
			}

			bulletItem.item = rootObject.transform;

			GameObject collisionObject = new GameObject("Collision");
			collisionObject.transform.SetParent(rootObject.transform);
			collisionObject.transform.localPosition = Vector3.zero;
			collisionObject.transform.localRotation = Quaternion.identity;

			BoxCollider bulletReceiver = null;

			Renderer[] renderers = rootObject.GetComponentsInChildren<Renderer>();
			foreach(Renderer renderer in renderers)
			{
				bulletItem.hovers.Add(renderer);
				bulletItem.hoverModes.Add(VRInteractableItem.HoverMode.SHADER);
				bulletItem.defaultShaders.Add(null);
				bulletItem.defaultMats.Add(null);
				bulletItem.hoverShaders.Add(null);
				bulletItem.hoverMats.Add(null);

				GameObject colliderObject = new GameObject("Collider");
				colliderObject.transform.SetParent(collisionObject.transform);
				colliderObject.transform.position = renderer.transform.position;
				colliderObject.transform.rotation = renderer.transform.rotation;
				ResizedCollider(colliderObject, renderer);

				VRItemCollider itemCollider = Undo.AddComponent<VRItemCollider>(colliderObject);
				itemCollider.item = bulletItem;

				if (bulletReceiver == null && !isCasing)
				{
					bulletReceiver = ResizedCollider(bulletObject, renderer);
					bulletReceiver.isTrigger = true;
				}
			}

			Selection.activeGameObject = rootObject;

			Close();
		}

		private void DeleteUnessentials(GameObject target)
		{
			Component[] originalComponents = target.GetComponentsInChildren<Component>();
			foreach(Component originalComponent in originalComponents)
			{
				if (originalComponent.GetType() == typeof(Transform) ||
					originalComponent.GetType() == typeof(MeshFilter) ||
					originalComponent.GetType() == typeof(MeshRenderer))
					continue;
				Undo.DestroyObjectImmediate(originalComponent);
			}
		}

		static public BoxCollider ResizedCollider(GameObject targetObject, Renderer rendererReference)
		{
			BoxCollider tempCollider = Undo.AddComponent<BoxCollider>(rendererReference.gameObject);
			BoxCollider newCollider = ResizedCollider(targetObject, tempCollider);
			Undo.DestroyObjectImmediate(tempCollider);
			return newCollider;
		}

		static public BoxCollider ResizedCollider(GameObject targetObject, BoxCollider colliderReference)
		{
			BoxCollider newCollider = Undo.AddComponent<BoxCollider>(targetObject);
			newCollider.center = (colliderReference.transform.localRotation * Vector3.Scale(colliderReference.center, colliderReference.transform.localScale)) + colliderReference.transform.localPosition;
			Vector3 newSize = colliderReference.transform.localRotation * Vector3.Scale(colliderReference.size, colliderReference.transform.localScale);
			if (newSize.x < 0) newSize.x = -newSize.x;
			if (newSize.y < 0) newSize.y = -newSize.y;
			if (newSize.z < 0) newSize.z = -newSize.z;
			newCollider.size = newSize;
			return newCollider;
		}
	}
}
#endif