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


    public void AddItemToInventory(Item item, int pickedUpItemQuantity) {
        StaticVariables.mainUI.ShowItemBeingAdded(item, pickedUpItemQuantity);

        // Check if stack has met cap > if it has create new cap.
        for(int i = 0; i < inventorySlot.Count; i++) {
            // if existing and not too large for stack add quantity to max, exclude the rest?
            if(IsItemAlreadyInSlot(i, item) && IsSlotFull(i)) {
                if(WouldAddingQuantityOverfillSlot(i, pickedUpItemQuantity)) { 
                    pickedUpItemQuantity = RemainingItemQuantity(i, pickedUpItemQuantity);
                    SetSlotQuantityToStackLimit(i);
                }
                if(PickedUpQuantityIsLessThanOrEqualToStackLimit(i, pickedUpItemQuantity)) {
                    AddQuantityToSlot(i, pickedUpItemQuantity);
                    return; // ends the code because pickedUpItemQuantity item has been exhausted
                }
            }
        }
        // adds item & quantity to new InventorySlot
        AddItemToNewSlot(item, pickedUpItemQuantity);
    }


    public bool CanAddItemToInventory() {
        if(ItemsInInventoryAreLessThanTheMaximumInventoryCapacity()) {
            return true;
        }
        return false;
    }

    public int GetItemQuantity(int inventorySlotPosition) {
        return inventorySlot[inventorySlotPosition].quantity;
    }

    public int GetTotalItemQuantity(Item item) {
        int totalQuantity = 0;
        for(int i = 0; i < inventorySlot.Count; i++){
            if(IsItemAlreadyInSlot(i, item)) {
                totalQuantity += inventorySlot[i].quantity;
            }
        }
        return totalQuantity;
    }

    public bool WouldAddingQuantityOverfillSlot(int slotNumber, int quantity) {
        return inventorySlot[slotNumber].quantity + quantity > inventorySlot[slotNumber].item.stackLimit;
    }

    public bool PickedUpQuantityIsLessThanOrEqualToStackLimit(int slotNumber, int quantity) {
        return inventorySlot[slotNumber].quantity + quantity <= inventorySlot[slotNumber].item.stackLimit;
    }

    public bool IsSlotFull(int slotNumber) {
        return inventorySlot[slotNumber].quantity < inventorySlot[slotNumber].item.stackLimit;
    }

    public bool ItemsInInventoryAreLessThanTheMaximumInventoryCapacity() {
        return inventorySlot.Count < (int) size;
    }

    public bool IsItemAlreadyInSlot(int slotNumber, Item item) {
        return inventorySlot[slotNumber].item == item;
    }

    public void SetSlotQuantityToStackLimit(int slotNumber) {
        inventorySlot[slotNumber].quantity = inventorySlot[slotNumber].item.stackLimit;
    }

    public void AddQuantityToSlot(int slotNumber, int quantity) {
        inventorySlot[slotNumber].AddQuantity(quantity);
        onItemChangedCallback.Invoke();
    }

    public void AddItemToNewSlot(Item item, int quantity) { 
        inventorySlot.Add(new InventorySlot(item, quantity));
        onItemChangedCallback.Invoke();
        StaticVariables.mainUI.ShowItemBeingAdded(item, quantity);
    }

    public int RemainingItemQuantity(int slotNumber, int quantity) {
        return inventorySlot[slotNumber].quantity + quantity - inventorySlot[slotNumber].item.stackLimit;
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