//========= Copyright 2019, Sam Tague, All rights reserved. ===================
//
// Inventory storage can be add to an object that has an ItemSlot script attached.
// This will allow the slot to store multiple of the same item. The prefab is used
// to instantiate initial items.
//
//=============================================================================
#if VRInteraction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class InventoryStorage : MonoBehaviour 
	{
		public GameObject prefab;
		public bool infinte;
		public int storageCapacity = 10;
		public int initialStoredItems;

		private int _storedItems;
		private ItemSlot _itemSlot;
		private Queue<GameObject> _disabledStoredItems = new Queue<GameObject>();
		private string acceptedItemId;
		public int storedItems
		{
			get { return _storedItems; }
			set { _storedItems = value; }
		}

		void Start()
		{
			_itemSlot = GetComponent<ItemSlot>();
			_storedItems = initialStoredItems;
			if (_itemSlot == null)
			{
				Debug.LogError("Inventory Storage requires an item slot: " + name, gameObject);
				return;
			}
			if (prefab == null)
			{
				Debug.LogError("No prefab on Inventory Storage: " + name, gameObject);
				return;
			}
			VRInteractableItem item = prefab.GetComponentInChildren<VRInteractableItem>();
			if (item == null)
			{
				Debug.LogError("No VRInteractbleItem on prefab object: " + name, gameObject);
				return;
			}
			if (_itemSlot.GetSlotPosition(item) == null)
			{
				Debug.LogError("Prefab in Inventory Storage doesn't have an inventory slot accepted id: " + name, gameObject);
				return;
			}
			acceptedItemId = item.itemId;
		}

		void Update()
		{
			if((!infinte && storedItems == 0) || _itemSlot.hasItem) return;

			if (!infinte) storedItems--;
			GameObject itemObject;
			if (_disabledStoredItems.Count == 0)
				itemObject = Instantiate<GameObject>(prefab);
			else itemObject = _disabledStoredItems.Dequeue();
			
			VRInteractableItem item = itemObject.GetComponentInChildren<VRInteractableItem>();
			_itemSlot.AddItemToSlot(item);
		}

		public bool TryAddItem(VRInteractableItem item)
		{
			if (item.itemId != acceptedItemId) return false;
			if ((infinte || storedItems < storageCapacity) && !_disabledStoredItems.Contains(item.item.gameObject))
			{
				//Can store
				if (!infinte) storedItems++;
				item.item.gameObject.SetActive(false);
				_disabledStoredItems.Enqueue(item.item.gameObject);
				return true;
			}
			return false;
		}
	}
}
#endif