//========= Copyright 2019, Sam Tague, All rights reserved. ===================
//
// The item slot can be used to make inventory, specify a list of compatable items.
// You can specify the local position and rotation of each type of item, use the itemID
// variable.
//
//=============================================================================
#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRInteraction;
using UnityEngine.Events;

namespace VRWeaponInteractor
{
	public class ItemSlot : MonoBehaviour 
	{
		private class WatchItem
		{
			public VRInteractableItem item;

			private float _dropTime;
			public float dropTime
			{
				get { return _dropTime; }
				set { _dropTime = value; }
			}

			public WatchItem(VRInteractableItem item, float dropTime)
			{
				this.item = item;
				this._dropTime = dropTime;
			}
		}

		[System.Serializable]
		public class SlotPosition
		{
			public List<string> acceptedIds = new List<string>();
			public Transform parentTarget;
			public Vector3 localPosition;
			public Quaternion localRotation;
			public float distance = 0.05f;
			public float watchItemsFor = 2f;
			public List<Collider> triggerColliders = new List<Collider>();

			//For Editor
			public GameObject itemPrefab;
		}
			
		public List<SlotPosition> slotPositions = new List<SlotPosition>();
		public AudioClip addItemSound;
		public AudioClip removeItemSound;
		public AudioClip enterHover;
		public AudioClip exitHover;
		public UnityEvent addItemEvent;
		public UnityEvent removeItemEvent;
		public UnityEvent enterHoverEvent;
		public UnityEvent exitHoverEvent;
		public List<Renderer> hovers = new List<Renderer>();
		public List<VRInteractableItem.HoverMode> hoverModes = new List<VRInteractableItem.HoverMode>();
		public List<Shader> defaultShaders = new List<Shader>();
		public List<Shader> hoverShaders = new List<Shader>();
		public List<Material> defaultMats = new List<Material>();
		public List<Material> hoverMats = new List<Material>();

		//	Editor Var
		public bool hoverFoldout;

		private VRInteractableItem _currentItem;
		private SlotPosition _currentSlotPosition;
		private List<WatchItem> _justDropped = new List<WatchItem>();
		private bool _hasItem;
		private InventoryStorage _storage;
		private List<VRInteractableItem> _heldItems = new List<VRInteractableItem>();
		private bool _hovering;

		public bool hasItem
		{
			get { return _hasItem; }
		}

		void Start () 
		{
			_storage = GetComponent<InventoryStorage>();

			for(int i=0; i<hovers.Count; i++)
			{
				Renderer hover = hovers[i];
				if (hover == null)
				{
					Debug.LogError(name + " has a missing renderer. Check the Hover section of the editor", gameObject);
					continue;
				}
				switch(hoverModes[i])
				{
				case VRInteractableItem.HoverMode.SHADER:
					if (hover.material == null) break;
					if (hoverShaders[i] == null) hoverShaders[i] = Shader.Find("Unlit/Texture");
					if (defaultShaders[i] == null) defaultShaders[i] = hover.material.shader;
					else hover.material.shader = defaultShaders[i];
					break;
				case VRInteractableItem.HoverMode.MATERIAL:
					if (hoverMats[i] == null)
					{
						hoverMats[i] = new Material(hover.material);
						hoverMats[i].shader = Shader.Find("Unlit/Texture");
					}
					if (defaultMats[i] == null) defaultMats[i] = hover.material;
					else hover.material = defaultMats[i];
					break;
				}
			}

			VREvent.Listen("Drop", OnDrop);
			VREvent.Listen("Pickup", OnPickup);
		}

		void OnDestroy()
		{
			VREvent.Remove("Drop", OnDrop);
			VREvent.Remove("Pickup", OnPickup);
		}

		void Update()
		{
			if (_hasItem)
			{
				if (_currentItem == null || _currentSlotPosition == null || _currentItem.item.parent != _currentSlotPosition.parentTarget) RemoveItemFromSlot();
				else if (_storage == null) return;
			}

			CheckHeldHover();

			if (_justDropped.Count == 0) return;

			for (int i=_justDropped.Count; i-- > 0;)
			{
				SlotPosition slotPosition = GetSlotPosition(_justDropped[i].item);
				float watchTime = 2f;
				if (slotPosition != null) watchTime = slotPosition.watchItemsFor;
				if (_justDropped[i].dropTime+watchTime < Time.time)
				{
					_justDropped.RemoveAt(i);
					continue;
				}
				if (slotPosition == null) continue;
				if (WithinDistance(_justDropped[i].item, slotPosition))
				{
					AddItemToSlot(_justDropped[i].item);
					break;
				}
			}
		}

		bool WithinDistance(VRInteractableItem item, SlotPosition slotPosition = null)
		{
			if (slotPosition == null)
			{
				slotPosition = GetSlotPosition(item);
				if (slotPosition == null) return false;
			}
			float distance = Vector3.Distance(slotPosition.parentTarget.position, item.item.position);
			bool within = distance < slotPosition.distance;
			if (within) return true;

			//Check within trigger collider bounds
			foreach(Collider col in slotPosition.triggerColliders)
			{
				if (col == null) continue;
				if (col.bounds.Contains(item.item.position)) return true;
			}
			return false;
		}

		void CheckHeldHover()
		{
			bool foundHover = false;
			for (int i=_heldItems.Count; i-- > 0;)
			{
				VRInteractableItem heldItem = _heldItems[i];
				if (heldItem == null || heldItem.heldBy == null) _heldItems.RemoveAt(i);

				bool within = WithinDistance(heldItem);
				if (within) 
				{
					foundHover = true;
					break;
				}
			}
				
			if (!_hovering && foundHover)
			{
				EnableHover();
			}
			else if (_hovering && !foundHover)
			{
				DisableHover();
			}
		}

		virtual public void EnableHover()
		{
			if (_hovering) return;
			_hovering = true;
			if (enterHover != null) AudioSource.PlayClipAtPoint(enterHover, transform.position);
			if (enterHoverEvent != null) enterHoverEvent.Invoke();
			if (hovers.Count == 0) return;
			for(int i=0; i<hovers.Count; i++)
			{
				Renderer hover = hovers[i];
				if (hover == null) continue;
				switch(hoverModes[i])
				{
				case VRInteractableItem.HoverMode.SHADER:
					if (hover.material != null)
						hover.material.shader = hoverShaders[i];
					break;
				case VRInteractableItem.HoverMode.MATERIAL:
					hover.material = hoverMats[i];
					break;
				}
			}
		}

		virtual public void DisableHover()
		{
			if (!_hovering) return;
			_hovering = false;
			if (exitHover != null) AudioSource.PlayClipAtPoint(exitHover, transform.position);
			if (exitHoverEvent != null) exitHoverEvent.Invoke();
			if (hovers.Count == 0) return;
			for(int i=0; i<hovers.Count; i++)
			{
				Renderer hover = hovers[i];
				if (hover == null) continue;
				switch(hoverModes[i])
				{
				case VRInteractableItem.HoverMode.SHADER:
					if (hover.material != null)
						hover.material.shader = defaultShaders[i];
					break;
				case VRInteractableItem.HoverMode.MATERIAL:
					hover.material = defaultMats[i];
					break;
				}
			}
		}

		void OnPickup(params object[] args)
		{
			VRInteractableItem item = (VRInteractableItem)args[0];
			if (item == null) return;
			if (!_heldItems.Contains(item)) _heldItems.Add(item);
		}

		void OnDrop(params object[] args)
		{
			VRInteractableItem item = (VRInteractableItem)args[0];
			if (item == null) return;
			if (_heldItems.Contains(item)) _heldItems.Remove(item);
			SlotPosition slotPosition = GetSlotPosition(item);
			if (slotPosition == null) return;
			if (slotPosition.parentTarget == null) slotPosition.parentTarget = transform;

			foreach(WatchItem watchItem in _justDropped)
			{
				if (watchItem.item == item)
				{
					watchItem.dropTime = Time.time;
					return;
				}
			}
				
			WatchItem newWatchItem = new WatchItem(item, Time.time);
			_justDropped.Add(newWatchItem);
		}

		public SlotPosition GetSlotPosition(VRInteractableItem item)
		{
			if (item == null) return null;
			foreach(SlotPosition slotPosition in slotPositions)
			{
				foreach(string acceptedItemId in slotPosition.acceptedIds)
				{
					if (item.itemId == acceptedItemId) return slotPosition;
				}
			}
			return null;
		}

		public void AddItemToSlot(VRInteractableItem item)
		{
			if (item == null || item.heldBy != null || (item.GetType().IsSubclassOf(typeof(VRAttachment)) && ((VRAttachment)item).currentReceiver != null)) return;
			SlotPosition slotPosition = GetSlotPosition(item);
			if (slotPositions == null) return;
			_justDropped.Clear();
			if (_hasItem && _storage != null)
			{
				if (_storage.TryAddItem(item))
				{
					if (addItemSound != null) item.PlaySound(addItemSound);
				}
				return;
			}

			if (addItemSound != null) item.PlaySound(addItemSound);
			if (addItemEvent != null) addItemEvent.Invoke();
			DisableHover();
			_hasItem = true;
			if (slotPosition.parentTarget == null) slotPosition.parentTarget = transform;
			item.item.gameObject.SetActive(true);
			item.item.parent = slotPosition.parentTarget;
			item.item.localPosition = slotPosition.localPosition;
			item.item.localRotation = slotPosition.localRotation;
			VRInteractableItem.FreezeItem(item.item.gameObject, false, false, true);
			_currentItem = item;
			_currentSlotPosition = slotPosition;
		}

		public void RemoveItemFromSlot()
		{
			if (removeItemSound != null) AudioSource.PlayClipAtPoint(removeItemSound, transform.position);
			if (removeItemEvent != null) removeItemEvent.Invoke();
			_hasItem = false;
			_currentItem = null;
			_currentSlotPosition = null;
		}
	}
}
#endif