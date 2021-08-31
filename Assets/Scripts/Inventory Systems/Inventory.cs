using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public enum InventorySize 
    { 
        Massive = 50, 
        Large = 40,
        Medium = 30,
        Small = 20
    }

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class Inventory : ScriptableObject
{
    public InventorySize size;
    public List<InventorySlot> inventorySlot = new List<InventorySlot>();

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    void Start() {
    }


    public void AddItemToInventory(Item item, int quantity) {
        // Check if stack has met cap > if it has create new cap.
        for(int i = 0; i < inventorySlot.Count; i++) {
            // if existing and not too large for stack add quantity to max, exclude the rest?
            if(inventorySlot[i].item == item && inventorySlot[i].quantity < item.stackLimit) {
                if(inventorySlot[i].quantity + quantity > item.stackLimit) { // if the picked up quantity exceeds the
                // stackLimit, the quantity in the inventory slot[i] where the item is contained
                // is set to the maximum and the quantity of the item being picked up is set to the
                // remainder (quantity + quantity - stackLimit). Code then continues until the return statement 
                // (if other stacks exists or if no stack exists, adds a new stack in line 32 
                    quantity = inventorySlot[i].quantity + quantity - item.stackLimit;
                    inventorySlot[i].quantity = item.stackLimit;
                }
                if(inventorySlot[i].quantity + quantity <= item.stackLimit) {
                    inventorySlot[i].AddQuantity(quantity);
                    onItemChangedCallback.Invoke();
                    return; // ends the code because quantity of picked up item has been exhausted
                }
            }
        }
        // adds item & quantity to new InventorySlot
        inventorySlot.Add(new InventorySlot(item, quantity));
        onItemChangedCallback.Invoke();
    }


    public bool CanAddItemToInventory() {
        if(inventorySlot.Count < (int) size) {
            return true;
        }
        return false;
    }

    public int GetItemQuantity(int inventorySlotPosition) {
        return inventorySlot[inventorySlotPosition].quantity;
    }
}

[System.Serializable]
public class InventorySlot {
    public Item item;
    public int quantity;
    public InventorySlot(Item item, int quantity) {
        this.item = item;
        this.quantity = quantity;
    }

    public void AddQuantity(int value) {
        quantity += value;
    }

    public void RemoveQuantity(int value) {
        quantity -= value;
    }
}