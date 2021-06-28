using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class Inventory : ScriptableObject
{
    // Event which we can subscribe different methods to -- Trigger calls all attached events
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public List<InventorySlot> inventorySlot = new List<InventorySlot>();

    public bool AddItem(Item _item,int _quantity) {
        if(_quantity < _item.stackLimit) {
                bool containsItem = false;
            for(int i = 0;i < inventorySlot.Count; i++) {
                if(inventorySlot[i].item == _item) {
                    containsItem = true;
                    inventorySlot[i].AddQuantity(_quantity);
                    return true;
                }
            }
            if(!containsItem){
                inventorySlot.Add(new InventorySlot(_item, _quantity));
                return true;
            }
        }
        return false;
    }
}

// Allows us to add or remove items to our inventory.
[System.Serializable]
public class InventorySlot {
    public Item item;
    public int quantity;
    public InventorySlot(Item _item, int _quantity) {
        item = _item;
        quantity = _quantity;
    }

    public void AddQuantity(int value) {
        quantity += value;
    }

    public void RemoveQuantity(int value) {
        quantity += value;
    }
}