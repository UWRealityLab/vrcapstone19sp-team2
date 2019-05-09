//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Primary editor window for the VRWeaponInteractor
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class GunHandlerWindow : EditorWindow 
	{
		public enum WeaponTab
		{
			MAIN,
			TRIGGER,
			SLIDE,
			ATTACHMENTS
		}
		public WeaponTab weaponTab = WeaponTab.MAIN;

		//	Main
		static bool mainHelpBoxFoldout = true;
		public VRGunHandler gunHandler;
		public SerializedObject serializedGunHandler;
		static bool gunPositionFoldout = false;

		static bool raycastLayerFoldout = false;

		static bool fireFxFoldout = true;
		List<GameObject> fireFxInstances = new List<GameObject>();
		GameObject bulletInstance;
		static bool bulletFoldout = false;
		static bool attachmentOverridesFoldout;

		static bool playOnFireFoldout;
		static bool playOnSuccessfulFireFoldout;

		GameObject shootOriginInstance;
		GameObject shootDirectionInstance;

		GameObject ejectionOriginInstance;
		GameObject ejectionDestinationInstance;

		GameObject laserPointerOriginInstance;

		Vector2 scrollPos;

		public VRGunTrigger gunTrigger;
		public SerializedObject serializedTrigger;

		//	Slide
		public VRGunSlide gunSlide;
		public SerializedObject serializedSlide;

		//	Attachments
		List<GameObject> attachmentInstances = new List<GameObject>();

		//	Magazine
		List<GameObject> bulletInstances = new List<GameObject>();
		static bool magBulletFoldout = false;

		public void Init()
		{
			if (gunHandler == null) return;
			serializedGunHandler = new SerializedObject(gunHandler);
			gunTrigger = gunHandler.trigger;
			if (gunTrigger != null) serializedTrigger = new SerializedObject(gunTrigger);
			gunSlide = gunHandler.slide;
			if (gunSlide != null) serializedSlide = new SerializedObject(gunSlide);
		}

		void OnGUI () 
		{
			if (gunHandler == null || serializedGunHandler == null)
			{
				GUILayout.Label("No Gun Handler Referenced");
				gunHandler = (VRGunHandler)EditorGUILayout.ObjectField("Gun Handler", gunHandler, typeof(VRGunHandler), true);
				if (gunHandler != null)
				{
					Init();
					weaponTab = WeaponTab.MAIN;
				} else return;
			}
			GUILayout.BeginHorizontal();
			switch(weaponTab)
			{
			case WeaponTab.MAIN:
				GUILayout.Box("Main", GUILayout.ExpandWidth(true));
				if (GUILayout.Button("Slide"))
					weaponTab = WeaponTab.SLIDE;
				if (GUILayout.Button("Trigger"))
					weaponTab = WeaponTab.TRIGGER;
				if (GUILayout.Button("Attachments"))
					weaponTab = WeaponTab.ATTACHMENTS;
				break;
			case WeaponTab.TRIGGER:
				if (GUILayout.Button("Main"))
					weaponTab = WeaponTab.MAIN;
				if (GUILayout.Button("Slide"))
					weaponTab = WeaponTab.SLIDE;
				GUILayout.Box("Trigger", GUILayout.ExpandWidth(true));
				if (GUILayout.Button("Attachments"))
					weaponTab = WeaponTab.ATTACHMENTS;
				break;
			case WeaponTab.SLIDE:
				if (GUILayout.Button("Main"))
					weaponTab = WeaponTab.MAIN;
				GUILayout.Box("Slide", GUILayout.ExpandWidth(true));
				if (GUILayout.Button("Trigger"))
					weaponTab = WeaponTab.TRIGGER;
				if (GUILayout.Button("Attachments"))
					weaponTab = WeaponTab.ATTACHMENTS;
				break;
			case WeaponTab.ATTACHMENTS:
				if (GUILayout.Button("Main"))
					weaponTab = WeaponTab.MAIN;
				if (GUILayout.Button("Slide"))
					weaponTab = WeaponTab.SLIDE;
				if (GUILayout.Button("Trigger"))
					weaponTab = WeaponTab.TRIGGER;
				GUILayout.Box("Attachments", GUILayout.ExpandWidth(true));
				break;
			}
			GUILayout.EndHorizontal();
			switch(weaponTab)
			{
			case WeaponTab.MAIN:
				EditorGUI.indentLevel++;
				GUILayout.Label("Main", EditorStyles.boldLabel);
				EditorGUI.indentLevel--;
				scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
				serializedGunHandler.Update();
				MainSection();
				PrefabSection();
				SoundsSection();

				serializedGunHandler.ApplyModifiedProperties();

				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

				VRInteractableItemEditor.ItemSection(serializedGunHandler);

				mainHelpBoxFoldout = EditorGUILayout.Foldout(mainHelpBoxFoldout, "Help");
				if (mainHelpBoxFoldout)
				{
					EditorGUILayout.HelpBox("Second Held:\n" +
						"You can add a second held position by clicking the Add second held collider " +
						"button, this will add a new trigger collider as a child of the weapon, move this " +
						"to where you want to grab with the second hand. An example of this is in the " +
						"assault rifle in the example scene, you can set the held position on the second held script to give an offset. " +
						"The second held collider can be used on a rifle to hold with both hands and improve accuracy.\n\n" +
						"Firing Mode:\n" +
						"You can choose how the weapon will fire, semi automatic where the weapon " +
						"will fire once per each pull of the trigger and load the next bullet " +
						"if available, fully automatic where the gun will fire at the given " +
						"fire rate (in seconds the delay between shots) " +
						"for as long as the trigger is held down and there are bullets " +
						"available and pump or bolt action where each bullet has to be " +
						"manually loaded by pulling the slide (only works if you have a slide).\n\n" +
						"Damage:\n" +
						"When you shoot something a message is sent to the object that was hit calling " +
						"the method Damage that takes a damage integer as a parameter on or as a parent of the object hit. " +
						"To modify this you can open the VRGunHandler.cs script and go down to the FireRaycast() " +
						"method. Near the bottom of this method is the line:\n" +
						"'hit.transform.SendMessageUpwards('Damage', damage, SendMessageOptions.DontRequireReceiver); \n" +
						"You can modifiy this line to send a different method name or change the parameter. " +
						"by default the receiving object should have this method to receive:\n" + 
						"Public void Damage(int damage) {}\n\n" +
						"Shoot Direction:\n" +
						"This must be setup to shoot anything, two sphere should be spawned when you click 'Setup Shoot Direction', " +
						"you can select them both at first and move them to the end of the barrel of the gun, right " +
						"where the bullet should come out, once both are there just select the destination sphere " +
						"and drag it away perpedicular to the barrel, mimicking the direction the bullet should go. " +
						"The origin sphere will be use a the origin of the raycast and the difference between the origin " +
						"and destination will be used for the direction of the raycast.\n\n" +
						"Damage Raycast Layers:\n" +
						"You can assign the names of layers you either want to ignore or that you only want to " +
						"hit. Useful if you have trigger colliders you want to be ignored when shooting.\n\n" +
						"Bullet Decal:\n" +
						"The bullet decal prefab will be instantiated on the surface that was shot. You can use the BulletDecalExample " +
						"prefab in the Example folder for reference on how a decal should look, or if you want a bit more control you can " +
						"use the DecalChanger prefab as the reference in the weapon editor then modifiy the references on the DecalChanger " +
						"script to pair decal prefabs with tags.\n\n" +
						"Spawn On Fire Fx's:\n" +
						"Adding fx's like the muzzle flash or smoke, once assigned you can set the fx position. The gun handler " +
						"script will only take care of instantiating the prefab something on " +
						"the object will have to take care of getting rid of it. As an example " +
						"the DestroyIn script can be attached where you just set the time in " +
						"seconds until the object is destroyed.\n\n" +
						"Bullet Prefab:\n" +
						"This is where you should put the prefab saved from the bullet wizard earlier. The bullet doesn't need " +
						"a rigidbody and collider but without one it will just spawn frozen, once it is assigned you will be " +
						"able to set it's loaded position and the ejection direction.\n\n " +
						"Use Chamber Bullet:\n" +
						"With this ticked the fired bullets will be taken straight from the magazine at the time " +
						"of shooting, if set to false the gun will hold a bullet that can still be fired if the magazine is removed.\n " +
						"Eject Casing On Fire: This will eject the casing when the weapon is fired, set this to false for weapons like pump shotgun " +
						" or bolt action rifles where the casing is ejected at a different time.\n" +
						"Eject Casing On Slide Pull: Set this to true for pump shotguns or bolt actions rifles.", MessageType.Info);
				}

				EditorGUILayout.EndScrollView();
				break;
			case WeaponTab.SLIDE:
				EditorGUI.indentLevel++;
				GUILayout.Label("Slide", EditorStyles.boldLabel);
				EditorGUI.indentLevel--;
				scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

				if (serializedSlide != null) serializedSlide.Update();
				if (serializedGunHandler != null) serializedGunHandler.Update();
				var oldGunSlide = gunSlide;
				gunSlide = (VRGunSlide)EditorGUILayout.ObjectField("Gun Slide", gunSlide, typeof(VRGunSlide), true);
				if (gunSlide == null)
				{
					EditorGUILayout.EndScrollView();
					break;
				}

				if (oldGunSlide != gunSlide)
				{
					if (serializedGunHandler == null)
					{
						serializedGunHandler = new SerializedObject(gunHandler);
						serializedGunHandler.Update();
					}
					serializedSlide = new SerializedObject(gunSlide);
					SerializedProperty slideGunHandler = serializedSlide.FindProperty("gunHandler");
					slideGunHandler.objectReferenceValue = gunHandler;
					SerializedProperty gunHandlerSlide = serializedGunHandler.FindProperty("slide");
					gunHandlerSlide.objectReferenceValue = gunSlide;
					weaponTab = WeaponTab.SLIDE;
				}
				SerializedProperty slideItem = serializedSlide.FindProperty("item");
				slideItem.objectReferenceValue = EditorGUILayout.ObjectField("Item", slideItem.objectReferenceValue, typeof(Transform), true);

				if (slideItem.objectReferenceValue == null)
				{
					serializedSlide.ApplyModifiedProperties();
					EditorGUILayout.EndScrollView();
					break;
				}
				serializedSlide.ApplyModifiedProperties();
				GunHandlerWindow.SlideSection(serializedSlide);

				EditorGUILayout.HelpBox("If the slide is left blank the bullet will" +
					" be loaded as soon as the magazine goes in, otherwise you" +
					" will have to pull the slide to load the bullet from the magazine" +
					" (unless 'Add Bullet On Load' is ticked on the magazine). " +
					"It can also be used to eject loaded bullets if pulled when already loaded." +
					" The slide can be pulled when held and is clamped between the default" +
					" and pulled position (and optionally additional positions in the case of" +
					" a bolt action rifle), when not held if 'Slide Spring' is ticked" +
					" it will always move back to the default position. When the weapon" +
					" is fired and 'Animate When Firing' is true it will lerp back and forth" +
					" to give room for the ejected shell. Move it to the default position" +
					" hit set current to default, then move it back to the pulled" +
					" position and set current to pulled</i> use the toggle button to" +
					" move the slide between the two positions until you are happy with it." +
					" You can also set SecondHeldPosition to true that will allow you to" +
					" hold the slide like a shotgun, there is a shotgun example to demonstrate" +
					" this in the example scene", MessageType.Info);

				EditorGUILayout.EndScrollView();
				break;
			case WeaponTab.TRIGGER:
				EditorGUI.indentLevel++;
				GUILayout.Label("Trigger", EditorStyles.boldLabel);
				EditorGUI.indentLevel--;
				var oldGunTrigger = gunTrigger;
				gunTrigger = (VRGunTrigger)EditorGUILayout.ObjectField("Gun Trigger", gunTrigger, typeof(VRGunTrigger), true);
				if (gunTrigger != null && oldGunTrigger != gunTrigger)
				{
					if (serializedGunHandler == null) serializedGunHandler = new SerializedObject(gunHandler);
					serializedGunHandler.Update();
					SerializedProperty gunHandlerTrigger = serializedGunHandler.FindProperty("trigger");
					gunHandlerTrigger.objectReferenceValue = gunTrigger;
					serializedGunHandler.ApplyModifiedProperties();
					serializedTrigger = new SerializedObject(gunTrigger);
					SerializedProperty slideGunHandler = serializedTrigger.FindProperty("gunHandler");
					slideGunHandler.objectReferenceValue = gunHandler;
					weaponTab = WeaponTab.TRIGGER;
				} else if (gunTrigger == null) break;

				GunHandlerWindow.TriggerSection(serializedTrigger);

				EditorGUILayout.HelpBox("Trigger:\n" +
					"If the trigger is left blank it will still shoot but there will be no visual representation " +
					"of the trigger. The trigger will rest in the default position and move to the pulled position as you pull " +
					"the trigger on the controller when the weapon is held. Move it to the default position hit " +
					"'set current to default', then move it back to the pulled position and " +
					"'set current to pulled' use the toggle button to switch between positions until you are happy with it. " +
					"Unlike the slide and magazine where the pivot is used when grabbing them, the trigger only has the option to create " +
					"a pivot object so you can place the pivot in a hinge position just to make it a little easier to set the " +
					"default and pulled positions.", MessageType.Info);

				break;
			case WeaponTab.ATTACHMENTS:
				AttachmentsSection();
				break;
			}
		}

		private void MainSection()
		{
			var oldGunHandler = gunHandler;
			gunHandler = (VRGunHandler)EditorGUILayout.ObjectField("Gun Handler", gunHandler, typeof(VRGunHandler), true);
			if (gunHandler == null)
				return;
			if (oldGunHandler != gunHandler)
			{
				Init();
			}
			gunPositionFoldout = EditorGUILayout.Foldout(gunPositionFoldout, "Weapon Position");
			if (gunPositionFoldout)
			{
				EditorGUI.indentLevel++;
				SerializedProperty defaultPosition = serializedGunHandler.FindProperty("defaultPosition");
				defaultPosition.vector3Value = EditorGUILayout.Vector3Field("Default Position", defaultPosition.vector3Value);
				SerializedProperty defaultRotation = serializedGunHandler.FindProperty("defaultRotation");
				Quaternion tempDefaultRotation = defaultRotation.quaternionValue;
				tempDefaultRotation.eulerAngles = EditorGUILayout.Vector3Field("Default Rotation", tempDefaultRotation.eulerAngles);
				defaultRotation.quaternionValue = tempDefaultRotation;
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Set Current To Default"))
				{
					defaultPosition.vector3Value = gunHandler.transform.localPosition;
					defaultRotation.quaternionValue = gunHandler.transform.localRotation;
				}
				if (GUILayout.Button("Move To Default"))
				{
					Undo.RecordObject(gunHandler.transform, "Move To Default");
					gunHandler.transform.localPosition = defaultPosition.vector3Value;
					gunHandler.transform.localRotation = defaultRotation.quaternionValue;
				}
				GUILayout.EndHorizontal();
				EditorGUILayout.HelpBox("The gun meshes position relative to the gun model parent. Can usually be left at the default", MessageType.Info);
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				EditorGUI.indentLevel--;

			}
			if (GUILayout.Button("Setup Weapon Held Position"))
			{
				HeldPositionWindow newWindow = (HeldPositionWindow)EditorWindow.GetWindow(typeof(HeldPositionWindow), true, "Held Position", true);
				newWindow.interactableItem = gunHandler;
				newWindow.Init();
			}
			SerializedProperty secondHeldCollider = serializedGunHandler.FindProperty("secondHeld");
			secondHeldCollider.objectReferenceValue = EditorGUILayout.ObjectField("Second Held Collider", secondHeldCollider.objectReferenceValue, typeof(VRSecondHeld), true);

			if (secondHeldCollider.objectReferenceValue == null)
			{
				if (GUILayout.Button("Add Second Held Collider"))
				{
					GameObject secondHeldObject = new GameObject("Second Held Collider");
					secondHeldObject.transform.parent = gunHandler.item;
					secondHeldObject.transform.localPosition = Vector3.zero;
					Undo.RegisterCreatedObjectUndo(secondHeldObject, "Create second held collider");
					VRSecondHeld secondHeld = secondHeldObject.AddComponent<VRSecondHeld>();
					secondHeld.item = gunHandler.item;
					secondHeld.parents.Add(gunHandler);
					secondHeldCollider.objectReferenceValue = secondHeld;
					Selection.activeGameObject = secondHeldObject;
				}
			}

			EditorGUI.indentLevel++;
			GUILayout.Label("Weapon Firing", EditorStyles.boldLabel);
			EditorGUI.indentLevel--;

			SerializedProperty fireModeEnum = serializedGunHandler.FindProperty("firingMode");
			fireModeEnum.intValue = (int)(VRGunHandler.FiringMode)EditorGUILayout.EnumPopup("Firing Mode", (VRGunHandler.FiringMode)fireModeEnum.intValue);
			VRGunHandler.FiringMode fireMode = (VRGunHandler.FiringMode)fireModeEnum.intValue;
			switch(fireMode)
			{
			case VRGunHandler.FiringMode.SEMI_AUTOMATIC:
				EditorGUILayout.HelpBox("Single fire, prepares next bullet if available. Weapon example: Pistol or Rifle", MessageType.Info);
				break;
			case VRGunHandler.FiringMode.FULLY_AUTOMATIC:
				EditorGUILayout.HelpBox("Fire bullets until empty for as long as the trigger is held. Weapon example: Assault Rifle", MessageType.Info);
				break;
			case VRGunHandler.FiringMode.BURST:
				EditorGUILayout.HelpBox("Fires three bullets each trigger pull in a burst: Assault Rifle", MessageType.Info);
				break;
			case VRGunHandler.FiringMode.PUMP_OR_BOLT_ACTION:
				EditorGUILayout.HelpBox("Slide needs to be pulled after each shot to load the next bullet/shell. Weapon example: Shotgun or Rifle", MessageType.Info);
				break;
			}
			SerializedProperty fireRate = serializedGunHandler.FindProperty("fireRate");
			fireRate.floatValue = EditorGUILayout.FloatField("Fire Rate", fireRate.floatValue);

			SerializedProperty damageAmount = serializedGunHandler.FindProperty("damage");
			damageAmount.intValue = EditorGUILayout.IntField("Damage", damageAmount.intValue);

			GUIContent bulletForceContent = new GUIContent("Bullet Force", "The force applied to any rigid body the bullet hits");
			SerializedProperty bulletForce = serializedGunHandler.FindProperty("bulletForce");
			bulletForce.floatValue = EditorGUILayout.FloatField(bulletForceContent, bulletForce.floatValue);

			SerializedProperty recoilKick = serializedGunHandler.FindProperty("recoilKick");
			recoilKick.floatValue = EditorGUILayout.FloatField("Recoil Kick", recoilKick.floatValue);

			SerializedProperty angularRecoilKick = serializedGunHandler.FindProperty("angularRecoilKick");
			angularRecoilKick.floatValue = EditorGUILayout.FloatField("Angular Recoil Kick", angularRecoilKick.floatValue);

			SerializedProperty recoilRecovery = serializedGunHandler.FindProperty("recoilRecovery");
			EditorGUILayout.PropertyField(recoilRecovery);

			SerializedProperty angularRecoilMultiStep = serializedGunHandler.FindProperty("angularRecoilMultiStep");
			EditorGUILayout.PropertyField(angularRecoilMultiStep);

			if (secondHeldCollider.objectReferenceValue != null)
			{
				SerializedObject secondHeldObject = new SerializedObject(secondHeldCollider.objectReferenceValue);
				secondHeldObject.Update();
				SerializedProperty secondHeldRecoilKick = secondHeldObject.FindProperty("recoilKick");
				EditorGUILayout.PropertyField(secondHeldRecoilKick, new GUIContent("Second Held Recoil Kick"));
				SerializedProperty secondHeldAngularRecoilKick = secondHeldObject.FindProperty("angularRecoilKick");
				EditorGUILayout.PropertyField(secondHeldAngularRecoilKick, new GUIContent("Second Held Angular Recoil Kick"));
				SerializedProperty secondHeldRecoilRecovery = secondHeldObject.FindProperty("recoilRecovery");
				EditorGUILayout.PropertyField(secondHeldRecoilRecovery, new GUIContent("Second Held Recoil Recovery"));
				SerializedProperty secondHeldAngularRecoilMultiStep = secondHeldObject.FindProperty("angularRecoilMultiStep");
				EditorGUILayout.PropertyField(secondHeldAngularRecoilMultiStep, new GUIContent("Second Held Angular Recoil Multi Step"));
				secondHeldObject.ApplyModifiedProperties();
			}

			SerializedProperty requireMagToShoot = serializedGunHandler.FindProperty("requireMagToShoot");
			EditorGUILayout.PropertyField(requireMagToShoot);

			SerializedProperty shootOrigin = serializedGunHandler.FindProperty("shootOrigin");
			SerializedProperty shootDirection = serializedGunHandler.FindProperty("shootDirection");
			shootOrigin.vector3Value = EditorGUILayout.Vector3Field("Shoot Origin", shootOrigin.vector3Value);
			shootDirection.vector3Value = EditorGUILayout.Vector3Field("Shoot Direction", shootDirection.vector3Value);

			if (shootOriginInstance == null || shootDirectionInstance == null)
			{
				if (GUILayout.Button("Setup Shoot Direction"))
				{
					shootOriginInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					shootOriginInstance.name = "Shoot Origin";
					Undo.RegisterCreatedObjectUndo(shootOriginInstance, "Setup Shoot Direction");
					shootOriginInstance.transform.SetParent(gunHandler.item);
					shootOriginInstance.transform.localPosition = shootOrigin.vector3Value;
					shootOriginInstance.transform.localScale *= gunHandler.item.localScale.magnitude*0.01f;
					shootDirectionInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					shootDirectionInstance.name = "Shoot Direction";
					Undo.RegisterCreatedObjectUndo(shootDirectionInstance, "Setup Shoot Direction");
					shootDirectionInstance.transform.SetParent(gunHandler.item);
					shootDirectionInstance.transform.localPosition = shootOrigin.vector3Value + (shootDirection.vector3Value*0.1f);
					shootDirectionInstance.transform.localScale = shootOriginInstance.transform.localScale;
					Selection.activeGameObject = shootDirectionInstance;
				}
			} else
			{

				SerializedProperty shotModeEnum = serializedGunHandler.FindProperty("shotMode");
				shotModeEnum.intValue = (int)(VRGunHandler.ShotMode)EditorGUILayout.EnumPopup("Shot Mode", (VRGunHandler.ShotMode)shotModeEnum.intValue);
				VRGunHandler.ShotMode shotMode = (VRGunHandler.ShotMode)shotModeEnum.intValue;
				switch(shotMode)
				{
				case VRGunHandler.ShotMode.SINGLE_SHOT:
					break;
				case VRGunHandler.ShotMode.SHOTGUN_SPREAD:
					{
						SerializedProperty bulletsPerShot = serializedGunHandler.FindProperty("bulletsPerShot");
						bulletsPerShot.intValue = EditorGUILayout.IntField("Bullets Per Shot", bulletsPerShot.intValue);
						SerializedProperty coneSize = serializedGunHandler.FindProperty("coneSize");
						coneSize.floatValue = EditorGUILayout.FloatField("Cone Size", coneSize.floatValue);

						if (GUILayout.Button("Show Test Ray"))
						{
							Vector3 direction = (shootDirectionInstance.transform.position - shootOriginInstance.transform.position)*10;
							for(int i=0; i<bulletsPerShot.intValue; i++) Debug.DrawRay(shootOriginInstance.transform.position, VRUtils.GetConeDirection(direction, coneSize.floatValue));
							SceneView.RepaintAll();
						}
					}
					break;
				case VRGunHandler.ShotMode.MACHINE_GUN_SPREAD:
					{
						SerializedProperty coneSize = serializedGunHandler.FindProperty("coneSize");
						coneSize.floatValue = EditorGUILayout.FloatField("Cone Size", coneSize.floatValue);

						if (GUILayout.Button("Show Test Ray"))
						{
							Vector3 direction = (shootDirectionInstance.transform.position - shootOriginInstance.transform.position)*10;
							Debug.DrawRay(shootOriginInstance.transform.position, VRUtils.GetConeDirection(direction, coneSize.floatValue));
							SceneView.RepaintAll();
						}
					}
					break;
				}
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Save"))
				{
					shootOrigin.vector3Value = shootOriginInstance.transform.localPosition;
					shootDirection.vector3Value = (shootDirectionInstance.transform.localPosition - shootOriginInstance.transform.localPosition)*10;
					Undo.DestroyObjectImmediate(shootOriginInstance);
					Undo.DestroyObjectImmediate(shootDirectionInstance);
				}
				if (GUILayout.Button("Select Origin"))
				{
					Selection.activeGameObject = shootOriginInstance;
				}
				if (GUILayout.Button("Select Destination"))
				{
					Selection.activeGameObject = shootDirectionInstance;
				}
				if (GUILayout.Button("Cancel"))
				{
					Undo.DestroyObjectImmediate(shootOriginInstance);
					Undo.DestroyObjectImmediate(shootDirectionInstance);
				}
				GUILayout.EndHorizontal();
				EditorGUILayout.HelpBox("Move the origin sphere to where the bullet will fire from, in a pistol this would be the end of the gun shaft. Then move the destination" +
					" sphere further along in the direction the bullet should go.", MessageType.Info);
			}


			raycastLayerFoldout = EditorGUILayout.Foldout(raycastLayerFoldout, "Damage Raycast Layers");
			if (raycastLayerFoldout)
			{
				EditorGUI.indentLevel++;
				SerializedProperty raycastLayers = serializedGunHandler.FindProperty("shootLayers");
				SerializedProperty ignoreRaycast = serializedGunHandler.FindProperty("ignoreShootLayers");
				raycastLayers.arraySize = EditorGUILayout.IntField("Size", raycastLayers.arraySize);
				for(int i=0 ; i<raycastLayers.arraySize ; i++)
				{
					SerializedProperty raycastLayer = raycastLayers.GetArrayElementAtIndex(i);
					raycastLayer.stringValue = EditorGUILayout.TextField("Element "+i, raycastLayer.stringValue);
				}
				EditorGUILayout.HelpBox("Leave raycast layers empty to collide with everything", MessageType.Info);
				if (raycastLayers.arraySize > 0)
				{
					ignoreRaycast.boolValue = EditorGUILayout.Toggle("Ignore raycast layers", ignoreRaycast.boolValue);
					EditorGUILayout.HelpBox("Ignore raycast layers True: Ignore anything on the layers specified. False: Ignore anything on layers not specified", MessageType.Info);
				}
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				EditorGUI.indentLevel--;
			}

			playOnFireFoldout = EditorGUILayout.Foldout(playOnFireFoldout, "Play Animation On Fire");
			if (playOnFireFoldout)
			{
				EditorGUI.indentLevel++;
				SerializedProperty playOnFireCount = serializedGunHandler.FindProperty("playOnFireCount");
				var oldPlayOnFireCount = playOnFireCount.intValue;
				playOnFireCount.intValue = EditorGUILayout.IntField("Size", playOnFireCount.intValue);

				SerializedProperty playOnFire = serializedGunHandler.FindProperty("playOnFire");

				if (oldPlayOnFireCount != playOnFireCount.intValue || playOnFire.arraySize != playOnFireCount.intValue)
				{
					if (playOnFireCount.intValue < oldPlayOnFireCount)
					{
						playOnFire.ClearArray();
					}
					playOnFire.arraySize = playOnFireCount.intValue;
				}

				for(int i=0; i<playOnFireCount.intValue; i++)
				{
					SerializedProperty playOnFireAnimation = playOnFire.GetArrayElementAtIndex(i);
					playOnFireAnimation.objectReferenceValue = EditorGUILayout.ObjectField("Animation Reference", playOnFireAnimation.objectReferenceValue, typeof(Animation), true);
				}

				EditorGUI.indentLevel--;
			}

			playOnSuccessfulFireFoldout = EditorGUILayout.Foldout(playOnSuccessfulFireFoldout, "Play Animation On Successful Fire");
			if (playOnSuccessfulFireFoldout)
			{
				EditorGUI.indentLevel++;
				SerializedProperty playOnSuccessfulFireCount = serializedGunHandler.FindProperty("playOnSuccessfulFireCount");
				var oldPlayOnFireCount = playOnSuccessfulFireCount.intValue;
				playOnSuccessfulFireCount.intValue = EditorGUILayout.IntField("Size", playOnSuccessfulFireCount.intValue);

				SerializedProperty playOnSuccessfulFire = serializedGunHandler.FindProperty("playOnSuccessfulFire");

				if (oldPlayOnFireCount != playOnSuccessfulFireCount.intValue || playOnSuccessfulFire.arraySize != playOnSuccessfulFireCount.intValue)
				{
					if (playOnSuccessfulFireCount.intValue < oldPlayOnFireCount)
					{
						playOnSuccessfulFire.ClearArray();
					}
					playOnSuccessfulFire.arraySize = playOnSuccessfulFireCount.intValue;
				}

				for(int i=0; i<playOnSuccessfulFireCount.intValue; i++)
				{
					SerializedProperty playOnFireAnimation = playOnSuccessfulFire.GetArrayElementAtIndex(i);
					playOnFireAnimation.objectReferenceValue = EditorGUILayout.ObjectField("Animation Reference", playOnFireAnimation.objectReferenceValue, typeof(Animation), true);
				}

				EditorGUI.indentLevel--;
			}

			VRInteractableItemEditor.DisplayHoverSegment(serializedGunHandler);

			SerializedProperty enterHover = serializedGunHandler.FindProperty("enterHover");
			enterHover.objectReferenceValue = EditorGUILayout.ObjectField("Enter Hover Sound", enterHover.objectReferenceValue, typeof(AudioClip), false);
			SerializedProperty exitHover = serializedGunHandler.FindProperty("exitHover");
			exitHover.objectReferenceValue = EditorGUILayout.ObjectField("Exit Hover Sound", exitHover.objectReferenceValue, typeof(AudioClip), false);

			VRInteractableItemEditor.DisplayTriggerColliderSegment(serializedGunHandler);
		}

		private void PrefabSection()
		{
			EditorGUI.indentLevel++;
			GUILayout.Label("Prefabs", EditorStyles.boldLabel);
			EditorGUI.indentLevel--;

			fireFxFoldout = EditorGUILayout.Foldout(fireFxFoldout, "Spawn On Fire FXs");
			if (fireFxFoldout)
			{
				EditorGUI.indentLevel++;
				SerializedProperty spawnOnFireCount = serializedGunHandler.FindProperty("spawnOnFireCount");
				var oldSpawnOnFireCount = spawnOnFireCount.intValue;
				spawnOnFireCount.intValue = EditorGUILayout.IntField("Size", spawnOnFireCount.intValue);

				SerializedProperty spawnOnFirePrefabs = serializedGunHandler.FindProperty("spawnOnFire");
				SerializedProperty spawnOnFirePositions = serializedGunHandler.FindProperty("spawnOnFirePositions");
				SerializedProperty spawnOnFireRotations = serializedGunHandler.FindProperty("spawnOnFireRotations");
				if (oldSpawnOnFireCount != spawnOnFireCount.intValue || spawnOnFirePrefabs.arraySize != spawnOnFireCount.intValue ||
					spawnOnFirePositions.arraySize !=  spawnOnFireCount.intValue || spawnOnFireRotations.arraySize != spawnOnFireCount.intValue || fireFxInstances.Count != spawnOnFireCount.intValue)
				{
					if (spawnOnFireCount.intValue < oldSpawnOnFireCount)
					{
						spawnOnFirePrefabs.ClearArray();
						spawnOnFirePositions.ClearArray();
						spawnOnFireRotations.ClearArray();
					}
					spawnOnFirePrefabs.arraySize = spawnOnFirePositions.arraySize = spawnOnFireRotations.arraySize = spawnOnFireCount.intValue;
					int countDiff = spawnOnFireCount.intValue - fireFxInstances.Count;
					if (countDiff > 0)
					{
						for(int i=0; i<countDiff;i++) fireFxInstances.Add(null);
					} else if (countDiff < 0)
					{
						for(int i=0; i<countDiff;i++) 
						{
							if (fireFxInstances[i] == null) continue;
							Destroy(fireFxInstances[i]);
						}
						fireFxInstances.RemoveRange(fireFxInstances.Count+countDiff, -countDiff);
					}
				}

				for(int i=0; i<spawnOnFireCount.intValue; i++)
				{
					SerializedProperty prefab = spawnOnFirePrefabs.GetArrayElementAtIndex(i);
					prefab.objectReferenceValue = EditorGUILayout.ObjectField("FX Prefab", prefab.objectReferenceValue, typeof(GameObject), false);
					if (prefab.objectReferenceValue != null)
					{
						SerializedProperty position = spawnOnFirePositions.GetArrayElementAtIndex(i);
						SerializedProperty rotation = spawnOnFireRotations.GetArrayElementAtIndex(i);
						GameObject newInstance = fireFxInstances[i];
						ReferencePositionConfig(serializedGunHandler, position, rotation, (GameObject)prefab.objectReferenceValue, ref newInstance);
						fireFxInstances[i] = newInstance;
					}
				}
				EditorGUI.indentLevel--;
			}

			EditorGUI.indentLevel++;
			GUILayout.Label("Bullets", EditorStyles.boldLabel);
			EditorGUI.indentLevel--;

			SerializedProperty damageMethodName = serializedGunHandler.FindProperty("damageMethodName");
			GUIContent damageMethodNameContent = new GUIContent("Damage Method Name", "This is the method name on the receiving object, " +
																					"it should take an integer paramter called damage. " +
																					"To modify this for raycast go to the FireRaycast method " +
																					"in the VRGunHandler script or for a physical bullet go to " +
																					"the ApplyDamage method of the VRPhysicalBullet script if you " +
																					"are using that script on your physical bullet.");
			EditorGUILayout.PropertyField(damageMethodName, damageMethodNameContent);

			SerializedProperty fireType = serializedGunHandler.FindProperty("fireType");
			EditorGUILayout.PropertyField(fireType);

			VRGunHandler.FireType fireTypeEnum = (VRGunHandler.FireType)fireType.intValue;
			switch(fireTypeEnum)
			{
			case VRGunHandler.FireType.RAYCAST:
				break;
			case VRGunHandler.FireType.PHYSICAL:
				SerializedProperty physicalBullet = serializedGunHandler.FindProperty("physicalBullet");
				GUIContent physicalBulletContent = new GUIContent("Physical Bullet", "The object will be instantiated at the shoot origin facing shoot direction, " +
																					"this is a fire and forgot mode, so the physical bullet must have a script that " +
																					"deals with moving and applying damage, the script VRPhysicalBullet can be used for this.");
				EditorGUILayout.PropertyField(physicalBullet, physicalBulletContent);
				break;
			}

			SerializedProperty bulletDecalPrefab = serializedGunHandler.FindProperty("bulletDecalPrefab");
			bulletDecalPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Bullet Decal Prefab", bulletDecalPrefab.objectReferenceValue, typeof(GameObject), false);

			SerializedProperty useChamberBullet = serializedGunHandler.FindProperty("useChamberBullet");
			useChamberBullet.boolValue = EditorGUILayout.Toggle("Use Chamber Bullet", useChamberBullet.boolValue);
			SerializedProperty ejectCasingOnFire = serializedGunHandler.FindProperty("ejectCasingOnFire");
			ejectCasingOnFire.boolValue = EditorGUILayout.Toggle("Eject Casing On Fire", ejectCasingOnFire.boolValue);
			if (serializedSlide != null)
			{
				SerializedProperty ejectCasingOnSlidePull = serializedGunHandler.FindProperty("ejectCasingOnSlidePull");
				EditorGUILayout.PropertyField(ejectCasingOnSlidePull);

				SerializedProperty ejectBulletOnSlidePull = serializedGunHandler.FindProperty("ejectBulletOnSlidePull");
				EditorGUILayout.PropertyField(ejectBulletOnSlidePull);
			}

			SerializedProperty loadedBullet = serializedGunHandler.FindProperty("loadedBulletPrefab");
			var oldLoadedBullet = loadedBullet.objectReferenceValue;
			loadedBullet.objectReferenceValue = EditorGUILayout.ObjectField("Loadable Bullet Prefab", loadedBullet.objectReferenceValue, typeof(GameObject), false);

			SerializedProperty spentBullet = serializedGunHandler.FindProperty("spentBulletPrefab");
			spentBullet.objectReferenceValue = EditorGUILayout.ObjectField("Spent Bullet Prefab", spentBullet.objectReferenceValue, typeof(GameObject), false);

			GameObject magazineObject = GetMagazineObject();
			VRMagazine magazine = null;

			if (oldLoadedBullet != loadedBullet.objectReferenceValue)
			{
				if (magazineObject != null)
				{
					bool isPrefab = IsPrefab(magazineObject);
					GameObject attachmentObj = null;
					if (isPrefab)
					{
						attachmentObj = (GameObject)Instantiate(magazineObject);
						magazine = attachmentObj.GetComponentInChildren<VRMagazine>();
					} else
					{
						magazine = magazineObject.GetComponentInChildren<VRMagazine>();
					}
					magazine.bulletPrefab = (GameObject)loadedBullet.objectReferenceValue;
					if (isPrefab)
					{
						PrefabUtility.ReplacePrefab(attachmentObj, magazineObject, ReplacePrefabOptions.ConnectToPrefab | ReplacePrefabOptions.ReplaceNameBased);
						DestroyImmediate(attachmentObj);
					} else
					{
						EditorUtility.SetDirty(magazine);
						EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
					}
				}
			}

			if (loadedBullet.objectReferenceValue != null)
			{
				bulletFoldout = EditorGUILayout.Foldout(bulletFoldout, "Bullet Settings");
				if (bulletFoldout)
				{
					Transform bulletParent = null;
					if (magazineObject != null)
					{
						bool isPrefab = IsPrefab(magazineObject);
						if (!isPrefab)
						{
							magazine = magazineObject.GetComponentInChildren<VRMagazine>();
							bulletParent = magazine.bulletParent;
						}
					}

					SerializedProperty bulletEjectionPush = serializedGunHandler.FindProperty("bulletEjectionPush");
					GUIContent ejectionPushContent = new GUIContent("Ejection Push", "The starting position of the" +
						" bullet when ejected as a percentage" +
						" between 0 and 1 from the bullet position" +
						" to the ejection position. This is useful" +
						" if the bullet is getting stuck in the model" +
						" when ejecting");
					bulletEjectionPush.floatValue = EditorGUILayout.FloatField(ejectionPushContent, bulletEjectionPush.floatValue);

					ReferencePositionConfig(serializedGunHandler, serializedGunHandler.FindProperty("loadedBulletPosition"), serializedGunHandler.FindProperty("loadedBulletRotation"), (GameObject)loadedBullet.objectReferenceValue, ref bulletInstance, bulletParent);
					EditorGUILayout.HelpBox("The bullet position is where the bullet prefab will sit when loaded.", MessageType.Info);
					EditorGUI.indentLevel++;
					SerializedProperty bulletPosition = serializedGunHandler.FindProperty("loadedBulletPosition");
					SerializedProperty ejectDirection = serializedGunHandler.FindProperty("bulletEjectionDirection");
					ejectDirection.vector3Value = EditorGUILayout.Vector3Field("Bullet Eject Direction", ejectDirection.vector3Value);
					if (ejectionOriginInstance == null || ejectionDestinationInstance == null)
					{
						if (GUILayout.Button("Setup Ejection Direction"))
						{
							ejectionOriginInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
							ejectionOriginInstance.name = "Bullet Origin";
							Undo.RegisterCreatedObjectUndo(ejectionOriginInstance, "Setup Ejection");
							ejectionOriginInstance.transform.SetParent(gunHandler.item);
							ejectionOriginInstance.transform.localPosition = bulletPosition.vector3Value;
							ejectionOriginInstance.transform.localScale *= gunHandler.item.localScale.magnitude*0.01f;
							ejectionDestinationInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
							ejectionDestinationInstance.name = "Bullet Direction";
							Undo.RegisterCreatedObjectUndo(ejectionDestinationInstance, "Setup Ejection");
							ejectionDestinationInstance.transform.SetParent(gunHandler.item);
							ejectionDestinationInstance.transform.localPosition = bulletPosition.vector3Value + (ejectDirection.vector3Value*0.1f);
							ejectionDestinationInstance.transform.localScale = ejectionOriginInstance.transform.localScale;
							Selection.activeGameObject = ejectionDestinationInstance;
						}
					} else
					{
						EditorGUILayout.HelpBox("If you need to move the origin position cancel this, change the bullet loaded position, then come back and set the ejection direction", MessageType.Info);
						if (GUILayout.Button("Save"))
						{
							ejectDirection.vector3Value = (ejectionDestinationInstance.transform.localPosition - ejectionOriginInstance.transform.localPosition)*10;
							Undo.DestroyObjectImmediate(ejectionOriginInstance);
							Undo.DestroyObjectImmediate(ejectionDestinationInstance);
						}
						if (GUILayout.Button("Select Destination"))
						{
							Selection.activeGameObject = ejectionDestinationInstance;
						}
						if (GUILayout.Button("Cancel"))
						{
							Undo.DestroyObjectImmediate(ejectionOriginInstance);
							Undo.DestroyObjectImmediate(ejectionDestinationInstance);
						}
						EditorGUILayout.HelpBox("The ejection origin will always be the bullet position. Drag the destination sphere in the direction the bullet should eject towards." +
							"The further away you move it the more force will be applied when ejecting.", MessageType.Info);
					}
					EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
					EditorGUI.indentLevel--;
				}
			}

			EditorGUILayout.HelpBox("The bullet receiver is a collider that allows you to load a bullet directly into the gun." +
				"It should be placed around where the bullet will sit in the gun", MessageType.Info);

			BoxCollider bulletReceiver = null;
			int bulletId = gunHandler.bulletId;

			if (magazineObject != null && !IsPrefab(magazineObject))
			{
				magazine = magazineObject.GetComponentInChildren<VRMagazine>();
				if (magazine != null)
				{
					bulletReceiver = magazine.bulletReceiver;
				}
			} else
				bulletReceiver = gunHandler.bulletReceiver;

			var oldBulletReceiver = bulletReceiver;
			bulletReceiver = (BoxCollider)EditorGUILayout.ObjectField("Bullet Receiver", bulletReceiver, typeof(BoxCollider), true);
			if (oldBulletReceiver != bulletReceiver)
			{
				gunHandler.bulletReceiver = bulletReceiver;
				if (magazine != null)
				{
					magazine.bulletReceiver = bulletReceiver;
					EditorUtility.SetDirty(magazine);
				}
				EditorUtility.SetDirty(gunHandler);
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}
			if (bulletReceiver == null)
			{
				if (GUILayout.Button("Create Bullet Receiver"))
				{
					GameObject bulletEntryColliderObj = new GameObject("BulletReceiver");
					Undo.RegisterCreatedObjectUndo(bulletEntryColliderObj, "Create Bullet Receiver");
					bulletEntryColliderObj.transform.SetParent(gunHandler.item);
					bulletEntryColliderObj.transform.localPosition = Vector3.zero;
					bulletEntryColliderObj.transform.localRotation = Quaternion.identity;
					bulletReceiver = bulletEntryColliderObj.AddComponent<BoxCollider>();
					bulletReceiver.size = new Vector3(0.05f*gunHandler.item.localScale.magnitude, 0.001f*gunHandler.item.localScale.magnitude, 0.05f*gunHandler.item.localScale.magnitude);
					bulletReceiver.isTrigger = true;
					VRBulletReceiver bulletReceiverScript = bulletEntryColliderObj.AddComponent<VRBulletReceiver>();
					bulletReceiverScript.gunHandler = gunHandler;
					if (magazineObject != null && !IsPrefab(magazineObject) && magazine != null)
					{
						magazine.bulletReceiver = bulletReceiver;
						EditorUtility.SetDirty(magazine);
					}
					gunHandler.bulletReceiver = bulletReceiver;
					EditorUtility.SetDirty(bulletReceiverScript);
					EditorUtility.SetDirty(gunHandler);
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
					Selection.activeGameObject = bulletEntryColliderObj;
				}
			} else
			{
				VRBulletReceiver bulletReceiverScript = bulletReceiver.GetComponent<VRBulletReceiver>();
				if (bulletReceiverScript == null) bulletReceiverScript = bulletReceiver.gameObject.AddComponent<VRBulletReceiver>();
				if (bulletReceiverScript.gunHandler == null && bulletReceiverScript.magazine == null && bulletReceiverScript.ammoPack == null)
				{
					bulletReceiverScript.gunHandler = gunHandler;
					EditorUtility.SetDirty(bulletReceiverScript);
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
				}

				EditorGUILayout.HelpBox("When a bullet matching the bullet id touches the collider it will attempt to load", MessageType.Info);
				var oldBulletId = bulletId;
				bulletId = EditorGUILayout.IntField("Bullet Id", bulletId);
				if (oldBulletId != bulletId)
				{
					gunHandler.bulletId = bulletId;
					if (magazine != null)
					{
						magazine.bulletId = bulletId;
						gunHandler.bulletId = bulletId;
						EditorUtility.SetDirty(magazine);
					}
					EditorUtility.SetDirty(gunHandler);
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
				}
				if (GUILayout.Button("Select Bullet Receiver"))
				{
					Selection.activeGameObject = bulletReceiver.gameObject;
				}
			}

			SerializedProperty laserPointerOrigin = serializedGunHandler.FindProperty("laserPointerOrigin");
			laserPointerOrigin.vector3Value = ShowLaserPointerSettings(laserPointerOrigin.vector3Value);

			SerializedProperty tracerMat = serializedGunHandler.FindProperty("tracerMat");
			tracerMat.objectReferenceValue = EditorGUILayout.ObjectField("Tracer Material", tracerMat.objectReferenceValue, typeof(Material), false);
			if (tracerMat.objectReferenceValue != null)
			{
				SerializedProperty lineWidth = serializedGunHandler.FindProperty("tracerLineWidth");
				lineWidth.floatValue = EditorGUILayout.FloatField("Tracer Line Width", lineWidth.floatValue);
			}
		}

		private Vector3 ShowLaserPointerSettings(Vector3 laserPointerOrigin)
		{
			SerializedProperty laserPointMat = serializedGunHandler.FindProperty("laserPointerMat");
			laserPointMat.objectReferenceValue = EditorGUILayout.ObjectField("Laser Pointer Material", laserPointMat.objectReferenceValue, typeof(Material), false);
			if (laserPointMat.objectReferenceValue != null)
			{
				SerializedProperty laserPointerEnabled = serializedGunHandler.FindProperty("laserPointerEnabled");
				EditorGUILayout.PropertyField(laserPointerEnabled);
				SerializedProperty lineWidth = serializedGunHandler.FindProperty("laserPointerLineWidth");
				lineWidth.floatValue = EditorGUILayout.FloatField("Line Width", lineWidth.floatValue);
				laserPointerOrigin = EditorGUILayout.Vector3Field("Laser Pointer Origin", laserPointerOrigin);
				if (laserPointerOriginInstance == null)
				{
					if (GUILayout.Button("Setup Laser Origin Position"))
					{
						laserPointerOriginInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
						laserPointerOriginInstance.name = "Laser Pointer Origin";
						Undo.RegisterCreatedObjectUndo(laserPointerOriginInstance, "Setup Laser Pointer");
						laserPointerOriginInstance.transform.SetParent(gunHandler.item);
						laserPointerOriginInstance.transform.localPosition = laserPointerOrigin;
						laserPointerOriginInstance.transform.localScale *= gunHandler.item.localScale.magnitude*0.01f;
						Selection.activeGameObject = laserPointerOriginInstance;
					}
				} else
				{
					if (GUILayout.Button("Save"))
					{
						laserPointerOrigin = laserPointerOriginInstance.transform.localPosition;
						Undo.DestroyObjectImmediate(laserPointerOriginInstance);
					}
					if (GUILayout.Button("Select Destination"))
					{
						Selection.activeGameObject = laserPointerOriginInstance;
					}
					if (GUILayout.Button("Cancel"))
					{
						Undo.DestroyObjectImmediate(laserPointerOriginInstance);
					}
				}
			}
			return laserPointerOrigin;
		}

		private GameObject GetMagazineObject()
		{
			foreach(VRGunHandler.AttachmentPrefabs attachmentPrefabs in gunHandler.attachmentPrefabs)
			{
				if (attachmentPrefabs.attachmentPrefab != null)
				{
					VRMagazine magazine = attachmentPrefabs.attachmentPrefab.GetComponentInChildren<VRMagazine>();
					if (magazine == null) continue;
					return attachmentPrefabs.attachmentPrefab;
				}
			}
			return null;
		}

		private void SoundsSection()
		{
			EditorGUI.indentLevel++;
			GUILayout.Label("Sounds", EditorStyles.boldLabel);
			EditorGUI.indentLevel--;

			SerializedProperty fireSound = serializedGunHandler.FindProperty("fireSound");
			fireSound.objectReferenceValue = EditorGUILayout.ObjectField("Fire", fireSound.objectReferenceValue, typeof(AudioClip), false);
			SerializedProperty dryFireSound = serializedGunHandler.FindProperty("dryFireSound");
			dryFireSound.objectReferenceValue = EditorGUILayout.ObjectField("Dry Fire", dryFireSound.objectReferenceValue, typeof(AudioClip), false);

			if (gunSlide != null)
			{
				SerializedProperty slidePulled = serializedGunHandler.FindProperty("slidePulled");
				slidePulled.objectReferenceValue = EditorGUILayout.ObjectField("Slide Pulled", slidePulled.objectReferenceValue, typeof(AudioClip), false);
				SerializedProperty slideReleased = serializedGunHandler.FindProperty("slideRelease");
				slideReleased.objectReferenceValue = EditorGUILayout.ObjectField("Slide Released", slideReleased.objectReferenceValue, typeof(AudioClip), false);
			}
		}

		static public void TriggerSection(SerializedObject triggerObject)
		{
			if (triggerObject == null) return;
			VRGunTrigger gunTrigger = (VRGunTrigger)triggerObject.targetObject;
			SerializedProperty defaultPosition = triggerObject.FindProperty("defaultTriggerPosition");
			defaultPosition.vector3Value = EditorGUILayout.Vector3Field("Default Position", defaultPosition.vector3Value);
			SerializedProperty defaultRotation = triggerObject.FindProperty("defaultTriggerRotation");
			Quaternion tempDefaultRotation = defaultRotation.quaternionValue;
			tempDefaultRotation.eulerAngles = EditorGUILayout.Vector3Field("Default Rotation", tempDefaultRotation.eulerAngles);
			defaultRotation.quaternionValue = tempDefaultRotation;

			SerializedProperty pulledPosition = triggerObject.FindProperty("pulledTriggerPosition");
			pulledPosition.vector3Value = EditorGUILayout.Vector3Field("Pulled Position", pulledPosition.vector3Value);
			SerializedProperty pulledRotation = triggerObject.FindProperty("pulledTriggerRotation");
			Quaternion tempPulledRotation = pulledRotation.quaternionValue;
			tempPulledRotation.eulerAngles = EditorGUILayout.Vector3Field("Pulled Rotation", tempPulledRotation.eulerAngles);
			pulledRotation.quaternionValue = tempPulledRotation;

			SerializedProperty triggerPulled = triggerObject.FindProperty("triggerPulled");

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Set Current To Default"))
			{
				triggerPulled.boolValue = false;
				defaultPosition.vector3Value = gunTrigger.transform.localPosition;
				defaultRotation.quaternionValue = gunTrigger.transform.localRotation;
			}
			if (GUILayout.Button("Set Current To Pulled"))
			{
				triggerPulled.boolValue = true;
				pulledPosition.vector3Value = gunTrigger.transform.localPosition;
				pulledRotation.quaternionValue = gunTrigger.transform.localRotation;
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.HelpBox("Move the object in the scene and save the positions using the buttons above. Use the toggle button to switch between the two positions", MessageType.Info);

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Toggle Trigger"))
			{
				if (triggerPulled.boolValue)
				{
					gunTrigger.transform.localPosition = gunTrigger.defaultTriggerPosition;
					gunTrigger.transform.localRotation = gunTrigger.defaultTriggerRotation;
				} else
				{
					gunTrigger.transform.localPosition = gunTrigger.pulledTriggerPosition;
					gunTrigger.transform.localRotation = gunTrigger.pulledTriggerRotation;
				}
				triggerPulled.boolValue = !triggerPulled.boolValue;
			}
			if (GUILayout.Button("Select Trigger"))
			{
				Selection.activeGameObject = gunTrigger.gameObject;
			}

			if (GUILayout.Button("Create Pivot Object"))
			{
				//Create pivot
				GunHandlerWindow.AddPivot(gunTrigger.transform);
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.HelpBox("Creating a pivot creates a new object as a parent that can be used to change the pivot point. Once made" +
				"you will be able to move the child of the newly created pivot object. The pivot position is used when finding what the controller" +
				"is closest to, so if all the pivot points are in the same place by default you will need to make you own. Note that the collider has" +
				"to be on the parent object and so will need to be manually adjusted.", MessageType.Info);

			SerializedProperty resetToDefaultOnShoot = triggerObject.FindProperty("resetToDefaultOnShoot");
			GUIContent resetToDefaultOnShootContent = new GUIContent("Reset To Default On Shoot", "Used for the hammer of a pistol");
			EditorGUILayout.PropertyField(resetToDefaultOnShoot, resetToDefaultOnShootContent);

			triggerObject.ApplyModifiedProperties();
		}

		static public void AddPivot(Transform trans)
		{
			Transform[] triggerChildren = trans.GetComponentsInChildren<Transform>();

			GameObject newPivot = new GameObject("Pivot");
			newPivot.transform.SetParent(trans);
			newPivot.transform.localPosition = Vector3.zero;
			newPivot.transform.localRotation = Quaternion.identity;
			newPivot.transform.localScale = Vector3.one;

			foreach(Transform childTrans in triggerChildren)
			{
				if (trans == childTrans) continue;
				childTrans.SetParent(newPivot.transform);
			}
			Selection.activeGameObject = newPivot;
		}

		static public void SlideSection(SerializedObject slideObject)
		{
			slideObject.Update();

			SerializedProperty gunHandlerProperty = slideObject.FindProperty("gunHandler");
			if (gunHandlerProperty.objectReferenceValue == null)
				return;
			SerializedObject gunHandlerObject = new SerializedObject(gunHandlerProperty.objectReferenceValue);
			gunHandlerObject.Update();

			VRGunSlide gunSlide = (VRGunSlide)slideObject.targetObject;
			if (gunSlide == null || gunSlide.item == null)
				return;

			SerializedProperty animateSlide = slideObject.FindProperty("animateSlide");
			animateSlide.boolValue = EditorGUILayout.Toggle("Animate When Firing", animateSlide.boolValue);

			if (animateSlide.boolValue)
			{
				SerializedProperty animateSlideTime = slideObject.FindProperty("slideAnimationTime");
				GUIContent animateSlideTimeTitle = new GUIContent("Slide Animation Time", "In Seconds");
				animateSlideTime.floatValue = EditorGUILayout.FloatField(animateSlideTimeTitle, animateSlideTime.floatValue);
			}

			SerializedProperty slideSpring = slideObject.FindProperty("slideSpring");
			GUIContent slideSpringContent = new GUIContent("Slide Spring", "Auto return the slide to the default position when released");
			slideSpring.boolValue = EditorGUILayout.Toggle(slideSpringContent, slideSpring.boolValue);

			SerializedProperty lockSlideBack = slideObject.FindProperty("lockSlideBack");
			GUIContent lockSlideBackContent = new GUIContent("Lock Slide Back", "If there is no bullet should the slide lock back or not.");
			EditorGUILayout.PropertyField(lockSlideBack, lockSlideBackContent);

			if (lockSlideBack.boolValue)
			{
				SerializedProperty onlyLockSlideWithMag = slideObject.FindProperty("onlyLockSlideWithMag");
				GUIContent onlyLockSlideWithMagContent = new GUIContent("Only Lock Slide With Mag", "Only lock the slide back on the last bullet " +
																									"when there is a magazine loaded. This will also " +
																									"stop you from releasing the slide until the magazine is removed.");
				onlyLockSlideWithMag.boolValue = EditorGUILayout.Toggle(onlyLockSlideWithMagContent, onlyLockSlideWithMag.boolValue);
			}

			SerializedProperty useAsSecondHeld = slideObject.FindProperty("useAsSecondHeld");
			var oldUseAsSecondHeld = useAsSecondHeld.boolValue;
			useAsSecondHeld.boolValue = EditorGUILayout.Toggle("Second Held Position", useAsSecondHeld.boolValue);

			if (useAsSecondHeld.boolValue != oldUseAsSecondHeld)
			{
				SerializedProperty secondHeld = gunHandlerObject.FindProperty("secondHeld");
				secondHeld.objectReferenceValue = useAsSecondHeld.boolValue ? gunSlide : null;
			}

			if (useAsSecondHeld.boolValue)
			{
				SerializedProperty toggleToPickup = slideObject.FindProperty("toggleToPickup");
				toggleToPickup.boolValue = EditorGUILayout.Toggle("Toggle To Pickup", toggleToPickup.boolValue);
			}

			if (animateSlide.boolValue && useAsSecondHeld.boolValue)
			{
				EditorGUILayout.HelpBox("The slide animation will conflict when holding the slide with the second held position and firing", MessageType.Warning);
			}

			SerializedProperty useScaleCalculation = slideObject.FindProperty("useScaleCalculation");
			useScaleCalculation.boolValue = EditorGUILayout.Toggle("Use Scale Calculation", useScaleCalculation.boolValue);

			SerializedProperty defaultPosition = slideObject.FindProperty("defaultPosition");
			defaultPosition.vector3Value = EditorGUILayout.Vector3Field("Default Position", defaultPosition.vector3Value);

			SerializedProperty defaultRotation = slideObject.FindProperty("defaultRotation");
			Quaternion tempDefaultRotation = defaultRotation.quaternionValue;
			tempDefaultRotation.eulerAngles = EditorGUILayout.Vector3Field("Default Rotation", tempDefaultRotation.eulerAngles);
			defaultRotation.quaternionValue = tempDefaultRotation;

			SerializedProperty useExtraPositions = slideObject.FindProperty("useExtraPositions");
			EditorGUILayout.PropertyField(useExtraPositions);
			if (useExtraPositions.boolValue)
			{
				SerializedProperty slidePositions = slideObject.FindProperty("extraPositions");
				SerializedProperty slideRotations = slideObject.FindProperty("extraRotations");
				slidePositions.arraySize = EditorGUILayout.IntField("Size", slidePositions.arraySize);
				if (slideRotations.arraySize != slidePositions.arraySize)
				{
					if (slidePositions.arraySize < slideRotations.arraySize)
					{
						slidePositions.ClearArray();
						slideRotations.ClearArray();
					}
					slideRotations.arraySize = slidePositions.arraySize;
				}

				for(int i=0; i<slidePositions.arraySize; i++)
				{
					SerializedProperty slidePosition = slidePositions.GetArrayElementAtIndex(i);
					SerializedProperty slideRotation = slideRotations.GetArrayElementAtIndex(i);
					slidePosition.vector3Value = EditorGUILayout.Vector3Field("Slide Position", slidePosition.vector3Value);
					Quaternion tempSlideRotation = slideRotation.quaternionValue;
					tempSlideRotation.eulerAngles = EditorGUILayout.Vector3Field("Slide Rotation", tempSlideRotation.eulerAngles);
					slideRotation.quaternionValue = tempSlideRotation;
					if (GUILayout.Button("Set Current To Index"))
					{
						slidePosition.vector3Value = gunSlide.transform.localPosition;
						slideRotation.quaternionValue = gunSlide.transform.localRotation;
					}
					if (GUILayout.Button("Move Slide to Index"))
					{
						gunSlide.transform.localPosition = slidePosition.vector3Value;
						gunSlide.transform.localRotation = slideRotation.quaternionValue;
					}
				}

			}

			SerializedProperty pulledPosition = slideObject.FindProperty("pulledPosition");
			pulledPosition.vector3Value = EditorGUILayout.Vector3Field("Pulled Position", pulledPosition.vector3Value);

			SerializedProperty pulledRotation = slideObject.FindProperty("pulledRotation");
			Quaternion tempPulledRotation = pulledRotation.quaternionValue;
			tempPulledRotation.eulerAngles = EditorGUILayout.Vector3Field("Pulled Rotation", tempPulledRotation.eulerAngles);
			pulledRotation.quaternionValue = tempPulledRotation;

			SerializedProperty slidePulled = slideObject.FindProperty("slidePulled");

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Set Current To Default"))
			{
				slidePulled.boolValue = false;
				defaultPosition.vector3Value = gunSlide.transform.localPosition;
				defaultRotation.quaternionValue = gunSlide.transform.localRotation;
			}
			if (GUILayout.Button("Set Current To Pulled"))
			{
				slidePulled.boolValue = true;
				pulledPosition.vector3Value = gunSlide.transform.localPosition;
				pulledRotation.quaternionValue = gunSlide.transform.localRotation;
			}
			GUILayout.EndHorizontal();

			EditorGUILayout.HelpBox("Move the object in the scene and save the positions using the buttons above. Use the toggle button to switch between the two positions", MessageType.Info);

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Toggle Slide"))
			{
				if (slidePulled.boolValue)
				{
					gunSlide.transform.localPosition = gunSlide.defaultPosition;
					gunSlide.transform.localRotation = gunSlide.defaultRotation;
				} else
				{
					gunSlide.transform.localPosition = gunSlide.pulledPosition;
					gunSlide.transform.localRotation = gunSlide.pulledRotation;
				}
				slidePulled.boolValue = !slidePulled.boolValue;
			}
			if (GUILayout.Button("Select Slide"))
			{
				Selection.activeGameObject = ((VRGunSlide)slideObject.targetObject).gameObject;
			}
			if (GUILayout.Button("Create Pivot Object"))
			{
				//Create pivot
				GunHandlerWindow.AddPivot(gunSlide.transform);
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.HelpBox("Creating a pivot creates a new object as a parent that can be used to change the pivot point. Once made" +
				"you will be able to move the child of the newly created pivot object. The pivot position is used when finding what the controller" +
				"is closest to, so if all the pivot points are in the same place by default you will need to make you own. Note that the collider has" +
				"to be on the parent object and can be fixed with the fix pivot collider button.", MessageType.Info);
			
			if (useAsSecondHeld.boolValue)
			{
				slideObject.ApplyModifiedProperties();
				gunHandlerObject.ApplyModifiedProperties();

				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

				VRSecondHeldEditor.SecondHeldSection(slideObject);

				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

				VRInteractableItemEditor.ItemSection(slideObject, true, true);
			} else
			{
				slideObject.ApplyModifiedProperties();

				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

				VRInteractableItemEditor.ItemSection(slideObject, false, true);
			}
		}

		private void AttachmentsSection()
		{
			if (serializedGunHandler == null) return;

			serializedGunHandler.Update();

			EditorGUI.indentLevel++;
			GUILayout.Label("Attachments", EditorStyles.boldLabel);
			EditorGUI.indentLevel--;

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			SerializedProperty gunHandlerAttachmentId = serializedGunHandler.FindProperty("attachmentId");
			GUIContent gunHandlerAttachmentIdContent = new GUIContent("Gun Handler Attachment Id");
			var oldGunHandlerAttachmentId = gunHandlerAttachmentId.intValue;
			EditorGUILayout.PropertyField(gunHandlerAttachmentId, gunHandlerAttachmentIdContent);
			if (oldGunHandlerAttachmentId != gunHandlerAttachmentId.intValue)
			{
				//Update all attachments with the new id
				for(int i=0 ; i<gunHandler.attachmentPrefabs.Count; i++)
				{
					if (gunHandler.attachmentPrefabs == null || gunHandler.attachmentPrefabs[i].attachmentPrefab == null) continue;
					GameObject attachmentInstance = null;
					bool destroyAfter = false;
					if (attachmentInstances.Count > i && attachmentInstances[i] != null)
					{
						attachmentInstance = attachmentInstances[i];
					} else if (!gunHandler.attachmentPrefabs[i].isPrefab)
					{
						attachmentInstance = gunHandler.attachmentPrefabs[i].attachmentPrefab;
					} else
					{
						attachmentInstance = Instantiate<GameObject>(gunHandler.attachmentPrefabs[i].attachmentPrefab);
						destroyAfter = true;
					}


					VRAttachment attachment = attachmentInstance.GetComponentInChildren<VRAttachment>();
					if (attachment != null)
					{
						foreach(VRAttachment.AttachmentPosition attachmentPosition in attachment.attachmentRefs)
						{
							if (attachmentPosition.gunHandlerAttachmentId == oldGunHandlerAttachmentId)
							{
								attachmentPosition.gunHandlerAttachmentId = gunHandlerAttachmentId.intValue;
							}
						}
					}
					if (gunHandler.attachmentPrefabs[i].isPrefab)
					{
						PrefabUtility.ReplacePrefab(attachmentInstance, gunHandler.attachmentPrefabs[i].attachmentPrefab, ReplacePrefabOptions.ConnectToPrefab | ReplacePrefabOptions.ReplaceNameBased);
						if (destroyAfter) DestroyImmediate(attachmentInstance);
					} else
					{
						EditorUtility.SetDirty(attachmentInstance);
						EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
					}
				}
			}

			if (GUILayout.Button("Add Attachment Receiver"))
			{
				gunHandler.attachmentPrefabs.Add(new VRGunHandler.AttachmentPrefabs());

				GameObject newReceiver = new GameObject("Attachment Receiver");
				Undo.RegisterCreatedObjectUndo(newReceiver, "Created Attachment Receiver");

				newReceiver.transform.parent = gunHandler.item;
				newReceiver.transform.localPosition = Vector3.zero;
				newReceiver.transform.localRotation = Quaternion.identity;
				VRAttachmentReceiver receiver = newReceiver.AddComponent<VRAttachmentReceiver>();
				receiver.gunHandler = gunHandler;
				BoxCollider boxCollider = newReceiver.AddComponent<BoxCollider>();
				boxCollider.isTrigger = true;

				gunHandler.attachmentPrefabs[gunHandler.attachmentPrefabs.Count-1].attachmentReceiver = receiver;

				Selection.activeGameObject = newReceiver;

				EditorUtility.SetDirty(gunHandler);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}

			for(int i=0 ; i<gunHandler.attachmentPrefabs.Count; i++)
			{
				if (attachmentInstances.Count < i+1) attachmentInstances.Add(null);

				ShowAttachmentReceiverSection(gunHandler.attachmentPrefabs[i], i, gunHandlerAttachmentId.intValue);
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			}

			EditorGUILayout.HelpBox("Attachments:\n" +
				"The gun handler attachment id should be a unique integer for this weapon, this will stop attachments that are not " +
				"compatable with this weapon from attaching, even if an attachment can attach to multiple weapons each of those weapons should " +
				"have a unique attachment id.\n" +
				"The most common attachment is a magazine, even if it is an integrated magazine that can't be removed " +
				"it is still considered an attachment but with interaction disabled set to true.\n" +
				"To add a new attachment first click the 'Add Attachment Receiver' button, this will instantiate a new" +
				"trigger collider you can then position and resize to fit the area the collider will be attached to.\n" +
				"The receiver id is local to this weapon and so just has to be unique to the other receivers on this weapon.\n" +
				"Reference the attachment prefab in the prefab slot, you can also reference a child of the weapon like in the case of " +
				"an integrated attachment or an attachment that never needs to be instantiated separately to the weapon. " +
				"Once assign you can click 'Edit Attachment' to instantiate an instance of the attachment you can then position.\n" +
				"If 'Use Slide' is set to true you can save a default position and entry position, you can use this for a magazine that slides " +
				"into position. If it is set to false the attachment will snap into the default position when you drop it over the reciever. \n" +
				"Use the 'Destroy Instance' button to delete the current instance and clear up the editor, this will be called on all active attachments " +
				"when the editor is closed.\n" +
				"'Auto Load' will make the attachment move toward the default position if over the receiver, if set to false it will use gravity\n " +
				"and can slip out if not clicked into the default position.\n" +
				"If the attachment is a laser pointer you can position a unique laser pointer origin.\n" +
				"You can change the shoot direction when this attachment is attached by ticking 'Use New Shoot Direction', this can be useful " +
				"in the case of a silencer that would require the origin to move up.\n" +
				"You can change the fire and dry fire sound, this will play when this attachment is attached, use this for things like silencers that " +
				"will change the fire sound.\n" +
				"If the attachment is a magazine it's section will appear next\n" +
				"The bullet id can be set for the magazine, this should match the id in the loadable bullet script\n" +
				"You can set this magazine to infinite ammo, if you would like one magazine with infinite and another without, create an instance of the magazine " +
				"prefab in the scene and make a new prefab, you can then add a new attachment receiver, set the new magazine the same as the current one. " +
				"Make sure at least one of them has 'Start Loaded' set to false or they will spawn on top of each other.\n" +
				"'Add Bullet On Load' will decide whether a bullet is immediately chambered when the magazine is inserted, if the weapon uses a chambered bullet. " +
				"'Clip Size' is how many bullets the magazine can hold\n" +
				"In the bullet section you can specify the prefab to use and position visible bullets if you would like them to show up.\n" +
				"'Replace Bullets With Spent' is used for the revolver weapon, this is not refereing to bullets in the chamber which are replace with a spent casing " +
				"by default when you shoot\n" +
				"You can add a bullet receiver for VRLoadableBullets, this will be disabled when attached to the weapon.\n" +
				"To change item specific variables drag an instance of the attachment prefab into the scene, make the changes on the VRAttachment/VRMagazine script " +
				"and apply the changes.", MessageType.Info);

			EditorGUILayout.EndScrollView();

			serializedGunHandler.ApplyModifiedProperties();
		}

		private void RemoveAttachmentPrefab(GameObject attachmentPrefab, int gunHandlerAttachmentPrefabIndex, int gunHandlerAttachmentId)
		{
			if (attachmentPrefab == null) return;
			GameObject attachmentObj = null;
			if (IsPrefab(attachmentPrefab))
				attachmentObj = (GameObject)Instantiate(attachmentPrefab);
			else
				attachmentObj = attachmentPrefab;

			VRAttachment attachment = attachmentObj.GetComponentInChildren<VRAttachment>();

			if (attachment == null) return;
			bool found = false;
			for (int j = attachment.attachmentRefs.Count; j-- > 0;)
			{
				if (gunHandler.attachmentPrefabs[gunHandlerAttachmentPrefabIndex].attachmentReceiver == null || attachment.attachmentRefs[j].gunHandlerAttachmentId != gunHandlerAttachmentId || attachment.attachmentRefs[j].receiverAttachmentId != gunHandler.attachmentPrefabs[gunHandlerAttachmentPrefabIndex].attachmentReceiver.attachmentId) continue;
				attachment.attachmentRefs.RemoveAt(j);
				found = true;
			}
			if (IsPrefab(attachmentPrefab))
			{
				if (found) PrefabUtility.ReplacePrefab(attachmentObj, attachmentPrefab, ReplacePrefabOptions.ConnectToPrefab | ReplacePrefabOptions.ReplaceNameBased);
				DestroyImmediate(attachmentObj);
			}
		}

		private void ChangeAttachmentReceiverId(GameObject attachmentPrefab, int gunHandlerAttachmentId, int oldReceiverId, int newReceiverId)
		{
			if (attachmentPrefab == null) return;
			GameObject attachmentObj = null;
			if (IsPrefab(attachmentPrefab))
				attachmentObj = (GameObject)Instantiate(attachmentPrefab);
			else
				attachmentObj = attachmentPrefab;

			VRAttachment attachment = attachmentObj.GetComponentInChildren<VRAttachment>();

			if (attachment == null) return;

			bool found = false;
			for (int j = attachment.attachmentRefs.Count; j-- > 0;)
			{
				if (attachment.attachmentRefs[j].gunHandlerAttachmentId != gunHandlerAttachmentId || attachment.attachmentRefs[j].receiverAttachmentId != oldReceiverId) continue;
				attachment.attachmentRefs[j].receiverAttachmentId = newReceiverId;
				found = true;
			}
			if (IsPrefab(attachmentPrefab))
			{
				if (found) PrefabUtility.ReplacePrefab(attachmentObj, attachmentPrefab, ReplacePrefabOptions.ConnectToPrefab | ReplacePrefabOptions.ReplaceNameBased);
				DestroyImmediate(attachmentObj);
			}
		}

		private bool IsPrefab(GameObject possiblePrefab)
		{
			PrefabType prefabType = PrefabUtility.GetPrefabType(possiblePrefab);
			return prefabType == PrefabType.ModelPrefab || prefabType == PrefabType.Prefab;
		}

		private VRAttachment.AttachmentPosition FindOrMakeVRAttachmentPosition(VRAttachment attachment, int gunHandlerId, int receiverId)
		{
			if (attachment == null)
			{
				Debug.LogError("No VRAttachment script attached to prefab");
				return null;
			}
			VRAttachment.AttachmentPosition returnPosition = null;
			for(int j=0; j<attachment.attachmentRefs.Count;j++)
			{
				VRAttachment.AttachmentPosition attachmentPosition = attachment.attachmentRefs[j];
				if (attachmentPosition.gunHandlerAttachmentId != gunHandlerId
					|| attachmentPosition.receiverAttachmentId != receiverId) continue;
				returnPosition = attachment.attachmentRefs[j];
				break;
			}
			if (returnPosition == null)
			{
				returnPosition = new VRAttachment.AttachmentPosition();
				returnPosition.gunHandlerAttachmentId = gunHandlerId;
				returnPosition.receiverAttachmentId = receiverId;
				Object gunHandlerPrefab = PrefabUtility.GetPrefabParent(((Transform)serializedGunHandler.FindProperty("item").objectReferenceValue).gameObject);
				returnPosition.weaponPrefab = (GameObject)gunHandlerPrefab;
				returnPosition.localPosition = Vector3.zero;
				returnPosition.localRotation = Quaternion.identity;
				attachment.attachmentRefs.Add(returnPosition);
			}
			return returnPosition;
		}

		private void ShowAttachmentReceiverSection(VRGunHandler.AttachmentPrefabs gunHandlerAttachmentPrefab, int gunHandlerAttachmentPrefabIndex, int gunHandlerAttachmentId)
		{
			if (GUILayout.Button("Remove Attachment Receiver"))
			{
				RemoveAttachmentPrefab(gunHandlerAttachmentPrefab.attachmentPrefab, gunHandlerAttachmentPrefabIndex, gunHandlerAttachmentId);
				if (attachmentInstances[gunHandlerAttachmentPrefabIndex] != null && IsPrefab(gunHandlerAttachmentPrefab.attachmentPrefab)) DestroyImmediate(attachmentInstances[gunHandlerAttachmentPrefabIndex]);

				if (gunHandlerAttachmentPrefab.attachmentReceiver != null) DestroyImmediate(gunHandlerAttachmentPrefab.attachmentReceiver.gameObject);

				gunHandler.attachmentPrefabs.RemoveAt(gunHandlerAttachmentPrefabIndex);
				EditorUtility.SetDirty(gunHandler);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				return;
			}
			if (gunHandlerAttachmentPrefab.attachmentReceiver != null)
			{
				if (GUILayout.Button("Select Attachment Receiver"))
				{
					Selection.activeGameObject = gunHandlerAttachmentPrefab.attachmentReceiver.gameObject;
				}
			}

			VRAttachmentReceiver attachmentReceiver = gunHandlerAttachmentPrefab.attachmentReceiver;
			attachmentReceiver = (VRAttachmentReceiver)EditorGUILayout.ObjectField("Receiver", attachmentReceiver, typeof(VRAttachmentReceiver), true);
			if (attachmentReceiver == null) return;
			var oldReceiverId = attachmentReceiver.attachmentId;
			attachmentReceiver.attachmentId = EditorGUILayout.IntField("Receiver Id", attachmentReceiver.attachmentId);

			if (oldReceiverId != attachmentReceiver.attachmentId)
			{
				ChangeAttachmentReceiverId(gunHandlerAttachmentPrefab.attachmentPrefab, gunHandlerAttachmentId, oldReceiverId, attachmentReceiver.attachmentId);
			}

			GameObject attachmentPrefab = gunHandlerAttachmentPrefab.attachmentPrefab;
			attachmentPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", attachmentPrefab, typeof(GameObject), true);
			gunHandlerAttachmentPrefab.attachmentPrefab = attachmentPrefab;

			if (attachmentPrefab != null)
			{
				gunHandlerAttachmentPrefab.isPrefab = IsPrefab(attachmentPrefab);
				if (!gunHandlerAttachmentPrefab.isPrefab) gunHandlerAttachmentPrefab.startLoaded = true;
				GameObject attachmentInstance = null;
				if (attachmentInstances[gunHandlerAttachmentPrefabIndex] == null && !gunHandlerAttachmentPrefab.isPrefab)
				{
					attachmentInstance = attachmentInstances[gunHandlerAttachmentPrefabIndex] = attachmentPrefab;
				} else if (attachmentInstances[gunHandlerAttachmentPrefabIndex] != null)
				{
					attachmentInstance = attachmentInstances[gunHandlerAttachmentPrefabIndex];
				}

				if (gunHandlerAttachmentPrefab.isPrefab)
				{
					var oldStartLoaded = gunHandlerAttachmentPrefab.startLoaded;
					GUIContent startLoadedContent = new GUIContent("Start Loaded", "If the attachment is a prefab you can chose to instantiate and instance " +
						"when the weapon initializes.");
					gunHandlerAttachmentPrefab.startLoaded = EditorGUILayout.Toggle(startLoadedContent, gunHandlerAttachmentPrefab.startLoaded);
					if (oldStartLoaded != gunHandlerAttachmentPrefab.startLoaded)
					{
						EditorUtility.SetDirty(gunHandler);
						EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
					}
				}

				if (attachmentInstance == null)
				{
					if (GUILayout.Button("Edit Attachment"))
					{
						attachmentInstance = attachmentInstances[gunHandlerAttachmentPrefabIndex] = (GameObject)Instantiate(attachmentPrefab, gunHandler.item);

						Selection.activeGameObject = attachmentInstance;
						VRAttachment attachment = attachmentInstance.GetComponentInChildren<VRAttachment>();

						VRAttachment.AttachmentPosition attachmentPosition = FindOrMakeVRAttachmentPosition(attachment, gunHandlerAttachmentId, attachmentReceiver.attachmentId);

						attachmentInstance.transform.localPosition = attachmentPosition.localPosition;
						attachmentInstance.transform.localRotation = attachmentPosition.localRotation;
					}
				} else
				{
					bool changed = false;
					VRAttachment attachment = attachmentInstance.GetComponentInChildren<VRAttachment>();

					if (attachment == null)
					{
						EditorGUILayout.HelpBox("Must have VRAttachment script attached", MessageType.Error);
						return;
					}

					var oldItem = attachment.item;
					attachment.item = (Transform)EditorGUILayout.ObjectField("Item", attachment.item, typeof(Transform), true);
					if (oldItem != attachment.item)
					{
						EditorUtility.SetDirty(attachment);
						EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
						changed = true;
					}
					if (attachment.item == null)
					{
						return;
					}

					VRAttachment.AttachmentPosition attachmentPosition = FindOrMakeVRAttachmentPosition(attachment, gunHandlerAttachmentId, attachmentReceiver.attachmentId);

					if (gunHandler != null && !gunHandlerAttachmentPrefab.isPrefab && !attachment.parents.Contains(gunHandler))
					{
						attachment.parents.Add(gunHandler);
						attachmentPosition.localPosition = attachmentInstance.transform.localPosition;
						attachmentPosition.localRotation = attachmentInstance.transform.localRotation;
						changed = true;
					}

					GUIContent slideAttachmentContent = new GUIContent("Use Slide", "Set to false, attachment will snap into position, true it can slide between default and entry positions");
					attachment.slideAttachment = EditorGUILayout.Toggle(slideAttachmentContent, attachment.slideAttachment);

					EditorGUILayout.LabelField("Position: " + attachmentPosition.localPosition.ToString("N5"));
					EditorGUILayout.LabelField("Rotation: " + attachmentPosition.localRotation.ToString("N5"));
					if (attachment.slideAttachment)
					{
						EditorGUILayout.LabelField("Entry Position: " + attachmentPosition.entryPosition.ToString("N5"));
					}

					if (GUILayout.Button("Save Current As Default Position"))
					{
						attachmentPosition.localPosition = attachmentInstance.transform.localPosition;
						attachmentPosition.localRotation = attachmentInstance.transform.localRotation;
						changed = true;
					}
					if (attachment.slideAttachment)
					{
						if (GUILayout.Button("Save Current As Entry Position"))
						{
							attachmentPosition.entryPosition = attachmentInstance.transform.localPosition;
							changed = true;
						}
					}
					if (GUILayout.Button("Select Receiver"))
					{
						Selection.activeGameObject = attachmentInstance;
					}
					if (gunHandlerAttachmentPrefab.isPrefab)
					{
						if (GUILayout.Button("Destroy Instance"))
						{
							Undo.DestroyObjectImmediate(attachmentInstance);
							return;
						}
					}

					if (attachment.slideAttachment)
					{
						if (GUILayout.Button("Toggle Positions"))
						{
							if (attachmentInstance.transform.localPosition == attachmentPosition.localPosition)
								attachmentInstance.transform.localPosition = attachmentPosition.entryPosition;
							else
								attachmentInstance.transform.localPosition = attachmentPosition.localPosition;
							attachmentInstance.transform.localRotation = attachmentPosition.localRotation;
						}

						GUIContent autoLoadContent = new GUIContent("Auto Load", "If set to true will move at auto load speed towards the default position if dropped while on the slide track" +
							"else will use gravity to determine whether it falls out or not.");
						var oldAutoLoad = attachment.autoLoad;
						attachment.autoLoad = EditorGUILayout.Toggle(autoLoadContent, attachment.autoLoad);
						if (oldAutoLoad != attachment.autoLoad) changed = true;
						if (attachment.autoLoad)
						{
							var oldLoadSpeed = attachment.autoLoadSpeed;
							attachment.autoLoadSpeed = EditorGUILayout.FloatField("Auto Load Speed", attachment.autoLoadSpeed);
							if (oldLoadSpeed != attachment.autoLoadSpeed) changed = true;
						}
					}

					var oldAttachSound = attachment.attachSound;
					attachment.attachSound = (AudioClip)EditorGUILayout.ObjectField("Attach Sound", attachment.attachSound, typeof(AudioClip), false);
					if (oldAttachSound != attachment.attachSound) changed = true;
					var oldDetatchSound = attachment.detatchSound;
					attachment.detatchSound = (AudioClip)EditorGUILayout.ObjectField("Detatch Sound", attachment.detatchSound, typeof(AudioClip), false);
					if (oldDetatchSound != attachment.detatchSound) changed = true;

					attachmentOverridesFoldout = EditorGUILayout.Foldout(attachmentOverridesFoldout, "Attachment Overrides");
					if (attachmentOverridesFoldout)
					{
						EditorGUI.indentLevel++;
						var oldUseLaserPointer = attachmentPosition.useLaserSight;
						attachmentPosition.useLaserSight = EditorGUILayout.Toggle("Use Laser Sight", attachmentPosition.useLaserSight);
						if (oldUseLaserPointer != attachmentPosition.useLaserSight) changed = true;
						if (attachmentPosition.useLaserSight)
						{
							var oldLaserOrigin = attachmentPosition.laserPointOrigin;
							attachmentPosition.laserPointOrigin = ShowLaserPointerSettings(attachmentPosition.laserPointOrigin);
							if (attachmentPosition.laserPointOrigin != oldLaserOrigin) changed = true;
						}

						var oldUseShootDirection = attachmentPosition.useNewShootDirection;
						attachmentPosition.useNewShootDirection = EditorGUILayout.Toggle("Use New Shoot Direction", attachmentPosition.useNewShootDirection);
						if (oldUseShootDirection != attachmentPosition.useNewShootDirection) changed = true;
						if (attachmentPosition.useNewShootDirection)
						{
							EditorGUILayout.LabelField("Shoot Origin: " + attachmentPosition.newShootOrigin.ToString("N5"));
							EditorGUILayout.LabelField("Shoot Direction: " + attachmentPosition.newShootDirection.ToString("N5"));

							if (shootOriginInstance == null || shootDirectionInstance == null)
							{
								if (GUILayout.Button("Setup Shoot Direction"))
								{
									shootOriginInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
									shootOriginInstance.name = "Shoot Origin";
									Undo.RegisterCreatedObjectUndo(shootOriginInstance, "Setup Shoot Direction");
									shootOriginInstance.transform.SetParent(gunHandler.item);
									shootOriginInstance.transform.localPosition = attachmentPosition.newShootOrigin;
									shootOriginInstance.transform.localScale *= gunHandler.item.localScale.magnitude*0.01f;
									shootDirectionInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
									shootDirectionInstance.name = "Shoot Direction";
									Undo.RegisterCreatedObjectUndo(shootDirectionInstance, "Setup Shoot Direction");
									shootDirectionInstance.transform.SetParent(gunHandler.item);
									shootDirectionInstance.transform.localPosition = attachmentPosition.newShootOrigin + (attachmentPosition.newShootDirection*0.1f);
									shootDirectionInstance.transform.localScale = shootOriginInstance.transform.localScale;
									Selection.activeGameObject = shootDirectionInstance;
								}
							} else
							{
								var oldShotMode = attachmentPosition.newShotMode;
								attachmentPosition.newShotMode = (VRGunHandler.ShotMode)EditorGUILayout.EnumPopup("Shot Mode", attachmentPosition.newShotMode);
								if (oldShotMode != attachmentPosition.newShotMode) changed = true;
								switch(attachmentPosition.newShotMode)
								{
								case VRGunHandler.ShotMode.SINGLE_SHOT:
									break;
								case VRGunHandler.ShotMode.SHOTGUN_SPREAD:
									{
										var oldBulletsPerShot = attachmentPosition.newBulletsPerShot;
										attachmentPosition.newBulletsPerShot = EditorGUILayout.IntField("Bullets Per Shot", attachmentPosition.newBulletsPerShot);
										if (oldBulletsPerShot != attachmentPosition.newBulletsPerShot) changed = true;
										var oldConeSize = attachmentPosition.newConeSize;
										attachmentPosition.newConeSize = EditorGUILayout.FloatField("Cone Size", attachmentPosition.newConeSize);
										if (oldConeSize != attachmentPosition.newConeSize) changed = true;

										if (GUILayout.Button("Show Test Ray"))
										{
											Vector3 direction = (shootDirectionInstance.transform.position - shootOriginInstance.transform.position)*10;
											for(int j=0; j<attachmentPosition.newBulletsPerShot; j++) Debug.DrawRay(shootOriginInstance.transform.position, VRUtils.GetConeDirection(direction, attachmentPosition.newConeSize));
											SceneView.RepaintAll();
										}
									}
									break;
								case VRGunHandler.ShotMode.MACHINE_GUN_SPREAD:
									{
										var oldConeSize = attachmentPosition.newConeSize;
										attachmentPosition.newConeSize = EditorGUILayout.FloatField("Cone Size", attachmentPosition.newConeSize);
										if (oldConeSize != attachmentPosition.newConeSize) changed = true;

										if (GUILayout.Button("Show Test Ray"))
										{
											Vector3 direction = (shootDirectionInstance.transform.position - shootOriginInstance.transform.position)*10;
											Debug.DrawRay(shootOriginInstance.transform.position, VRUtils.GetConeDirection(direction, attachmentPosition.newConeSize));
											SceneView.RepaintAll();
										}
									}
									break;
								}
								GUILayout.BeginHorizontal();
								if (GUILayout.Button("Save"))
								{
									attachmentPosition.newShootOrigin = shootOriginInstance.transform.localPosition;
									attachmentPosition.newShootDirection = (shootDirectionInstance.transform.localPosition - shootOriginInstance.transform.localPosition)*10;
									Undo.DestroyObjectImmediate(shootOriginInstance);
									Undo.DestroyObjectImmediate(shootDirectionInstance);
									changed = true;
								}
								if (GUILayout.Button("Select Origin"))
								{
									Selection.activeGameObject = shootOriginInstance;
								}
								if (GUILayout.Button("Select Destination"))
								{
									Selection.activeGameObject = shootDirectionInstance;
								}
								if (GUILayout.Button("Cancel"))
								{
									Undo.DestroyObjectImmediate(shootOriginInstance);
									Undo.DestroyObjectImmediate(shootDirectionInstance);
								}
								GUILayout.EndHorizontal();
								EditorGUILayout.HelpBox("Move the origin sphere to where the bullet will fire from, in a pistol this would be the end of the gun shaft. Then move the destination" +
									" sphere further along in the direction the bullet should go.", MessageType.Info);
							}
						}

						var oldFireSound = attachmentPosition.newFireSound;
						attachmentPosition.newFireSound = (AudioClip)EditorGUILayout.ObjectField("New Fire Sound", attachmentPosition.newFireSound, typeof(AudioClip), false);
						if (oldFireSound != attachmentPosition.newFireSound) changed = true;
						var oldDryFireSound = attachmentPosition.newDryFireSound;
						attachmentPosition.newDryFireSound = (AudioClip)EditorGUILayout.ObjectField("New Dry Fire Sound", attachmentPosition.newDryFireSound, typeof(AudioClip), false);
						if (oldDryFireSound != attachmentPosition.newDryFireSound) changed = true;
						EditorGUI.indentLevel--;
					}

					if (attachment.item != null)
					{
						if (GUILayout.Button("Setup Attachment Held Position"))
						{
							HeldPositionWindow attachHeldWindow = (HeldPositionWindow)EditorWindow.GetWindow(typeof(HeldPositionWindow), true, "Attachment Held Position", true);
							attachHeldWindow.interactableItem = attachment;
							attachHeldWindow.gunHandlerWindow = true;
							if (IsPrefab(attachmentPrefab)) 
							{
								attachHeldWindow.OnSaveEvent += AttachmentWindowDestroy;
								attachHeldWindow.attachmentInstance = attachment.item.gameObject;
								attachHeldWindow.attachmentPrefab = attachmentPrefab;
							}
							attachHeldWindow.Init();
						}
					}

					VRMagazine magazine = attachmentInstance.GetComponentInChildren<VRMagazine>();
					if (magazine != null)
					{
						MagazineSection(magazine, attachmentPrefab);
					}

					if (changed && IsPrefab(attachmentPrefab))
						PrefabUtility.ReplacePrefab(attachmentInstance, attachmentPrefab, ReplacePrefabOptions.ConnectToPrefab | ReplacePrefabOptions.ReplaceNameBased);
				}
			}
		}

		void MagazineSection(VRMagazine magazine, GameObject magazinePrefab)
		{
			if (magazine == null) return;

			EditorGUI.indentLevel++;
			GUILayout.Label("Magazine", EditorStyles.boldLabel);
			EditorGUI.indentLevel--;

			SerializedObject serializedMagazine = new SerializedObject(magazine);
			serializedMagazine.Update();
			bool changed = false;

			bool sectionChanged = VRMagazineEditor.MagazineSection(serializedMagazine);
			if (sectionChanged) changed = true;


			EditorGUILayout.HelpBox("Creating a pivot creates a new object as a parent that can be used to change the pivot point. Once made" +
				"you will be able to move the child of the newly created pivot object. The pivot position is used when finding what the controller" +
				"is closest to, so if all the pivot points are in the same place by default you will need to make you own. Note that the collider has" +
				"to be on the parent object and can be fixed with the fix pivot collider button.", MessageType.Info);

			EditorGUI.indentLevel++;
			GUILayout.Label("Bullets", EditorStyles.boldLabel);
			EditorGUI.indentLevel--;

			SerializedProperty clipSize = serializedMagazine.FindProperty("clipSize");
			SerializedProperty loadedBullet = serializedGunHandler.FindProperty("loadedBulletPrefab");
			loadedBullet.objectReferenceValue = EditorGUILayout.ObjectField("Bullet Prefab", loadedBullet.objectReferenceValue, typeof(GameObject), false);
			SerializedProperty bulletPrefab = serializedMagazine.FindProperty("bulletPrefab");
			var oldBulletPrefab = bulletPrefab.objectReferenceValue;
			bulletPrefab.objectReferenceValue = loadedBullet.objectReferenceValue;
			if (oldBulletPrefab != bulletPrefab.objectReferenceValue) changed = true;

			SerializedProperty autoEjectEmptyMag = serializedGunHandler.FindProperty("autoEjectEmptyMag");
			var oldAutoEjectEmptyMag = autoEjectEmptyMag.boolValue;
			EditorGUILayout.PropertyField(autoEjectEmptyMag);
			if (oldAutoEjectEmptyMag != autoEjectEmptyMag.boolValue) changed = true;

			if (gunHandler.spentBulletPrefab != null)
			{
				SerializedProperty replaceBulletsWithSpentCasings = serializedMagazine.FindProperty("replaceBulletsWithSpentCasings");
				EditorGUILayout.PropertyField(replaceBulletsWithSpentCasings);
			}

			if (bulletPrefab.objectReferenceValue != null)
			{
				magBulletFoldout = EditorGUILayout.Foldout(magBulletFoldout, "Magazine Bullet Settings");
				if (magBulletFoldout)
				{
					EditorGUI.indentLevel++;
					SerializedProperty bulletParentProp = serializedMagazine.FindProperty("bulletParent");
					var oldBulletParent = bulletParentProp.objectReferenceValue;
					EditorGUILayout.PropertyField(bulletParentProp);
					if (oldBulletParent != bulletParentProp.objectReferenceValue) changed = true;
					Transform bulletParent = magazine.item;
					if (bulletParentProp.objectReferenceValue != null) bulletParent = (Transform)bulletParentProp.objectReferenceValue;

					SerializedProperty bulletVisible = serializedMagazine.FindProperty("bulletVisible");
					SerializedProperty bulletPositions = serializedMagazine.FindProperty("bulletPositions");
					SerializedProperty bulletRotations = serializedMagazine.FindProperty("bulletRotations");
					SerializedProperty bulletEjectionPositions = serializedMagazine.FindProperty("bulletEjectionPositions");
					var oldBulletSize = bulletVisible.arraySize;
					bulletVisible.arraySize = bulletPositions.arraySize = bulletRotations.arraySize = bulletEjectionPositions.arraySize = clipSize.intValue;
					if (oldBulletSize != bulletVisible.arraySize)
						changed = true;

					EditorGUILayout.HelpBox("You can go through each bullet, first toggle whether or not it is visible, if it is spawn a reference position and" +
						"save. The reference can be destroyed once the position is saved. Bullet one goes at the top of the clip aka closest to the loaded position", MessageType.Info);

					for(int i=0; i<clipSize.intValue; i++)
					{
						SerializedProperty visible = bulletVisible.GetArrayElementAtIndex(i);
						var oldVisible = visible.boolValue;
						visible.boolValue = EditorGUILayout.Toggle("Bullet "+(i+1)+" Visible", visible.boolValue);
						if (oldVisible != visible.boolValue) changed = true;
						if (visible.boolValue)
						{
							SerializedProperty bulletPosition = bulletPositions.GetArrayElementAtIndex(i);
							var oldBulletPosition = bulletPosition.vector3Value;
							EditorGUILayout.LabelField("Position: " + bulletPosition.vector3Value.ToString("G4"));
							if (oldBulletPosition != bulletPosition.vector3Value) changed = true;
							SerializedProperty bulletRotation = bulletRotations.GetArrayElementAtIndex(i);
							EditorGUILayout.LabelField("Rotation: " + bulletRotation.quaternionValue.eulerAngles.ToString("G4"));
							SerializedProperty bulletEjectionPosition = bulletEjectionPositions.GetArrayElementAtIndex(i);
							EditorGUILayout.LabelField("Ejection: " + bulletEjectionPosition.vector3Value.ToString("G4"));
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
									referenceBullet.transform.SetParent(bulletParent);
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
									changed = true;
								}
								if (GUILayout.Button("Save Ejection Position"))
								{
									bulletEjectionPosition.vector3Value = bulletInstances[i].transform.localPosition;
									changed = true;
								}
								if (GUILayout.Button("Toggle Bullet Positions"))
								{
									if (bulletInstances[i].transform.localPosition != bulletPosition.vector3Value)
									{
										bulletInstances[i].transform.localPosition = bulletPosition.vector3Value;
									} else
									{
										bulletInstances[i].transform.localPosition = bulletEjectionPosition.vector3Value;
									}
									bulletInstances[i].transform.localRotation = bulletRotation.quaternionValue;
								}
								if(GUILayout.Button("Destory Bullet"))
								{
									Undo.DestroyObjectImmediate(bulletInstances[i]);
								}
								GUILayout.EndHorizontal();
							}
						}
					}
					EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
					EditorGUI.indentLevel++;
				}
			}

			SerializedProperty bulletReceiver = serializedMagazine.FindProperty("bulletReceiver");
			var oldBulletReceiver = bulletReceiver.objectReferenceValue;
			bulletReceiver.objectReferenceValue = EditorGUILayout.ObjectField("Bullet Receiver", bulletReceiver.objectReferenceValue, typeof(BoxCollider), true);
			if (oldBulletReceiver != bulletReceiver.objectReferenceValue) changed = true;
			if (bulletReceiver.objectReferenceValue == null)
			{
				if (GUILayout.Button("Create Bullet Receiver"))
				{
					GameObject bulletEntryColliderObj = new GameObject("BulletReceiver");
					Undo.RegisterCreatedObjectUndo(bulletEntryColliderObj, "Create Bullet Receiver");
					bulletEntryColliderObj.transform.SetParent(magazine.item);
					bulletEntryColliderObj.transform.localPosition = Vector3.zero;
					bulletEntryColliderObj.transform.localRotation = Quaternion.identity;
					bulletReceiver.objectReferenceValue = bulletEntryColliderObj.AddComponent<BoxCollider>();
					((BoxCollider)bulletReceiver.objectReferenceValue).size = new Vector3(0.05f*magazine.item.localScale.magnitude, 0.001f*magazine.item.localScale.magnitude, 0.05f*magazine.item.localScale.magnitude);
					((BoxCollider)bulletReceiver.objectReferenceValue).isTrigger = true;
					VRBulletReceiver bulletReceiverScript = bulletEntryColliderObj.AddComponent<VRBulletReceiver>();
					bulletReceiverScript.magazine = magazine;
					Selection.activeGameObject = bulletEntryColliderObj;
				}
			} else
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Select Bullet Receiver"))
				{
					Selection.activeGameObject = ((BoxCollider)bulletReceiver.objectReferenceValue).gameObject;
				}
				if (GUILayout.Button("Save"))
				{
					changed = true;
				}
				GUILayout.EndHorizontal();
			}

			EditorGUI.indentLevel++;
			GUILayout.Label("Hover", EditorStyles.boldLabel);
			EditorGUI.indentLevel--;

			var hoverChanged = VRInteractableItemEditor.DisplayHoverSegment(serializedMagazine);
			if (hoverChanged) changed = true;

			var triggersChanged = VRInteractableItemEditor.DisplayTriggerColliderSegment(serializedGunHandler);
			if (triggersChanged) changed = true;

			serializedMagazine.ApplyModifiedProperties();
			if (changed && IsPrefab(magazinePrefab))
			{
				SaveAttachmentPrefab(magazine.item.gameObject, magazinePrefab);
			} else if (changed)
			{
				if (magazine != null) EditorUtility.SetDirty(magazine);
				EditorUtility.SetDirty(gunHandler);
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}
		}

		void AttachmentWindowDestroy(HeldPositionWindow sender)
		{
			SaveAttachmentPrefab(sender.attachmentInstance, sender.attachmentPrefab);
		}

		public void SaveAttachmentPrefab(GameObject attachmentInstance, GameObject attachmentPrefab)
		{
			for(int i=0; i<bulletInstances.Count;i++)
			{
				if (bulletInstances[i] != null) bulletInstances[i].transform.SetParent(null);
			}
			PrefabUtility.ReplacePrefab(attachmentInstance, attachmentPrefab, ReplacePrefabOptions.ConnectToPrefab | ReplacePrefabOptions.ReplaceNameBased);
			for(int i=0; i<bulletInstances.Count;i++)
			{
				if (bulletInstances[i] != null) bulletInstances[i].transform.SetParent(attachmentInstance.GetComponentInChildren<VRMagazine>().item);
			}
		}

		void OnDestroy()
		{
			if (bulletInstance != null) Undo.DestroyObjectImmediate(bulletInstance);
			foreach(GameObject fireFXInstance in fireFxInstances) if (fireFXInstance != null) Undo.DestroyObjectImmediate(fireFXInstance);
			fireFxInstances.Clear();
			if (ejectionOriginInstance != null) Undo.DestroyObjectImmediate(ejectionOriginInstance);
			if (ejectionDestinationInstance != null) Undo.DestroyObjectImmediate(ejectionDestinationInstance);
			if (shootOriginInstance != null) Undo.DestroyObjectImmediate(shootOriginInstance);
			if (shootDirectionInstance != null) Undo.DestroyObjectImmediate(shootDirectionInstance);
			if (laserPointerOriginInstance != null) Undo.DestroyObjectImmediate(laserPointerOriginInstance);
			if (bulletInstances.Count != 0)
			{
				foreach(GameObject bullet in bulletInstances)
				{
					if (bullet == null) continue;
					Undo.DestroyObjectImmediate(bullet);
				}
			}

			if (gunHandler != null)
			{
				for(int i=0 ; i<gunHandler.attachmentPrefabs.Count; i++)
				{
					if (attachmentInstances.Count <= i) break;
					if (attachmentInstances[i] != null && IsPrefab(gunHandler.attachmentPrefabs[i].attachmentPrefab)) DestroyImmediate(attachmentInstances[i]);
				}
			}
				

			/*foreach(List<GameObject> attachmentPrefabInstances in attachmentInstances)
			{
				foreach(GameObject attachmentInstance in attachmentPrefabInstances)
				{
					if (attachmentInstance == null) continue;
					DestroyImmediate(attachmentInstance);
				}
			}*/
		}

		void ReferencePositionConfig(SerializedObject refObject, SerializedProperty positionProperty, SerializedProperty rotationProperty, GameObject objectPrefab, ref GameObject objectInstance, Transform objectParent = null)
		{
			EditorGUI.indentLevel++;
			//SerializedProperty position = refObject.FindProperty(positionVar);
			positionProperty.vector3Value = EditorGUILayout.Vector3Field("Position", positionProperty.vector3Value);
			//SerializedProperty rotation = refObject.FindProperty(rotationVar);
			Quaternion newRotation = rotationProperty.quaternionValue;
			newRotation.eulerAngles = EditorGUILayout.Vector3Field("Rotation", newRotation.eulerAngles);
			rotationProperty.quaternionValue = newRotation;

			if (objectInstance == null)
			{
				if (GUILayout.Button("Setup Position"))
				{
					objectInstance = (GameObject)Instantiate(objectPrefab);
					Undo.RegisterCreatedObjectUndo(objectInstance, "Spawn Reference");
					if (objectParent == null)
					{
						SerializedProperty item = refObject.FindProperty("item");
						objectParent = (Transform)item.objectReferenceValue;
					}
					objectInstance.transform.SetParent(objectParent);
					objectInstance.transform.localPosition = positionProperty.vector3Value;
					objectInstance.transform.localRotation = rotationProperty.quaternionValue;
					Selection.activeGameObject = objectInstance;
				}
			} else
			{
				EditorGUILayout.HelpBox("Move object into the default position", MessageType.Info);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Save"))
				{
					positionProperty.vector3Value = objectInstance.transform.localPosition;
					rotationProperty.quaternionValue = objectInstance.transform.localRotation;
					Undo.DestroyObjectImmediate(objectInstance);
				}
				if (GUILayout.Button("Select Object"))
				{
					Selection.activeGameObject = objectInstance;
				}
				if (GUILayout.Button("Cancel"))
				{
					Undo.DestroyObjectImmediate(objectInstance);
				}
				GUILayout.EndHorizontal();
			}

			EditorGUI.indentLevel--;
		}
	}
}
#endif