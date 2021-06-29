using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class Inventory : ScriptableObject
{
    // Event which we can subscribe different methods to -- Trigger calls all attached events
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;
    public float maxWeight;
    public float currentWeight;

    public List<InventorySlot> inventorySlot = new List<InventorySlot>();

    // Adds an item to the List<InventorySlot>
    public bool AddItem(Item _item, int _quantity, Inventory _inventory) {
        if(_quantity < _item.stackLimit && CanAdd(_inventory, _item)) {
                bool containsItem = false;
            for(int i = 0;i < inventorySlot.Count; i++) {
                if(inventorySlot[i].item == _item) {
                    containsItem = true;
                    inventorySlot[i].AddQuantity(_quantity);
                    AddWeight(_item);
                    return true;
                }
            }
            if(!containsItem){
                inventorySlot.Add(new InventorySlot(_item, _quantity));
                AddWeight(_item);
                return true;
            }
        }
        return false;
    }

    public bool RemoveItem(Item _item, int _quantity, Inventory _inventory) {
        return true;
    }

    // Checks to see if the current weight plus the new item's weight is within the 
    // inventory scope
    public bool CanAdd(Inventory _inventory, Item _item) { 
        if(_inventory.currentWeight + _item.weight <= _inventory.maxWeight) {
            Debug.Log("We here");
            return true;
        }
        Debug.Log("This weighs too much for you to lift. Weaksauce.");
        return false;
    }

    // adds to inventory weight total
    public void AddWeight(Item _item){
        currentWeight += _item.weight;
    }

    // subtracts from inventory weight total
    public void SubtractWeight(Item _item){
        currentWeight -= _item.weight;
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
        quantity -= value;
    }
}