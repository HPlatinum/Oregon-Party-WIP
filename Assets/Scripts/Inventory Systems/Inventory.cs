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
        if(!IsInventoryFull()) {
            return true;
        }
        //todo, check to see if a provided item can fit in an already-occupied slot
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

    public bool IsInventoryFull() {
        return (!(inventorySlot.Count < (int) size));
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

    public int GetQuantityOfSpecificItem(Item item) {
        int totalOfItems = 0;
        for(int i = 0; i < inventorySlot.Count; i++) {
            // if existing and not too large for stack add quantity to max, exclude the rest?
            if(IsItemAlreadyInSlot(i, item)) {
                totalOfItems += GetItemQuantity(i);
            }
        }
        return totalOfItems;
    }

    public List<(Item, int)> GetListOfItemsWithType(ItemType searchType) {
        List<(Item, int)> result = new List<(Item, int)>();
        for (int i = 0; i < inventorySlot.Count; i++) {
            Item itemInSlot = inventorySlot[i].item;
            int quantityInSlot = inventorySlot[i].quantity;
            
            //look for all items with the specified type
            if (itemInSlot.type == searchType) {

                //if the item is already in the list, increase the item quantity
                bool foundInListAlready = false;
                for (int j =0; j<result.Count; j++) {
                    if (result[j].Item1 == itemInSlot) {
                        int newQuantity = result[j].Item2 + quantityInSlot;
                        result[j] = (result[j].Item1, newQuantity);
                        foundInListAlready = true;
                    }
                }

                //otherwise, add the item to the end of the list
                if (!foundInListAlready) {
                    result.Add((itemInSlot, quantityInSlot));
                }
            }
        }
        return result;
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