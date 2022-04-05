using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public enum InventorySize 
    { 
        Massive = 50, 
        Large = 40,
        Medium = 30,
        Small = 20,
        JustOne = 1
    }

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class Inventory : ScriptableObject
{
    public InventorySize size;
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    private List<InventorySlot> emptySlots = new List<InventorySlot>();

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    void Start() {
    }


    public void AddItemToInventory(Item item, int pickedUpItemQuantity) {
        ShowItemQuantityChangeInUI(item, pickedUpItemQuantity);

        // Check if stack has met cap > if it has create new cap.
        for(int i = 0; i < inventorySlots.Count; i++) {
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
        AddItemToNewSlot(item, pickedUpItemQuantity, item.model);
    }
    
    public bool CanAddItemToInventory(Item item, int quantity) {
        //do you have open inventory space?
        if(!IsInventoryFull())
            return true;
        //do you have an unfilled stack?
        int preexistingQuant = GetQuantityOfSpecificItem(item);
        int partialStackQuant = preexistingQuant % item.stackLimit;
        if (partialStackQuant == 0)
            return false;
        //is there enough room in the unfilled stack to hold the new amount?
        if ((partialStackQuant + quantity) <= item.stackLimit)
            return true;
        //otherwise you dont got no room sun
        return false;
    }

    private int GetQuantityInSlot(int inventorySlotPosition) {
        return inventorySlots[inventorySlotPosition].quantity;
    }

    public int GetTotalItemQuantity(Item item) {
        int totalQuantity = 0;
        for(int i = 0; i < inventorySlots.Count; i++){
            if(IsItemAlreadyInSlot(i, item)) {
                totalQuantity += inventorySlots[i].quantity;
            }
        }
        return totalQuantity;
    }

    private bool WouldAddingQuantityOverfillSlot(int slotNumber, int quantity) {
        return inventorySlots[slotNumber].quantity + quantity > inventorySlots[slotNumber].item.stackLimit;
    }

    private bool PickedUpQuantityIsLessThanOrEqualToStackLimit(int slotNumber, int quantity) {
        return inventorySlots[slotNumber].quantity + quantity <= inventorySlots[slotNumber].item.stackLimit;
    }

    private bool IsSlotFull(int slotNumber) {
        return inventorySlots[slotNumber].quantity < inventorySlots[slotNumber].item.stackLimit;
    }

    public bool IsInventoryFull() {
        return (!(inventorySlots.Count < (int) size));
    }

    private bool IsItemAlreadyInSlot(int slotNumber, Item item) {
        return inventorySlots[slotNumber].item == item;
    }

    private void SetSlotQuantityToStackLimit(int slotNumber) {
        inventorySlots[slotNumber].quantity = inventorySlots[slotNumber].item.stackLimit;
    }

    private void AddQuantityToSlot(int slotNumber, int quantity) {
        inventorySlots[slotNumber].AddQuantity(quantity);
        OnItemChangedCheckIfCallbackNeeded();
    }

    private void OnItemChangedCheckIfCallbackNeeded() {
        if(CheckIfInventoryIsPlayerInventory()) {
            onItemChangedCallback.Invoke();
        }
    }

    private void ShowItemQuantityChangeInUI(Item item, int quantity) {
        if(CheckIfInventoryIsPlayerInventory()) {
            StaticVariables.commonUI.ShowItemQuantityChange(item, quantity);
        }
    }

    private bool CheckIfInventoryIsPlayerInventory() {
        return inventorySlots == StaticVariables.playerInventory.inventorySlots;
    }

    private void AddItemToNewSlot(Item item, int quantity, GameObject newGameObject) { 
        inventorySlots.Add(new InventorySlot(item, quantity, newGameObject));
        OnItemChangedCheckIfCallbackNeeded();
        ShowItemQuantityChangeInUI(item, quantity);
    }

    private int RemainingItemQuantity(int slotNumber, int quantity) {
        return inventorySlots[slotNumber].quantity + quantity - inventorySlots[slotNumber].item.stackLimit;
    }

    public int GetQuantityOfSpecificItem(Item item) {
        int totalOfItems = 0;
        for(int i = 0; i < inventorySlots.Count; i++) {
            // if existing and not too large for stack add quantity to max, exclude the rest?
            if(IsItemAlreadyInSlot(i, item)) {
                totalOfItems += GetQuantityInSlot(i);
            }
        }
        return totalOfItems;
    }

    public List<(Item, int)> GetListOfItemsWithType(ItemType searchType) {
        List<(Item, int)> result = new List<(Item, int)>();
        for (int i = 0; i < inventorySlots.Count; i++) {
            Item itemInSlot = inventorySlots[i].item;
            int quantityInSlot = inventorySlots[i].quantity;
            
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

    public void RemoveItemFromInventory(Item item, int quantity) {
        ShowItemQuantityChangeInUI(item, -quantity);

        int totalQuantity = GetTotalItemQuantity(item);
        int diff = totalQuantity - quantity;
        if (diff < 0) {
            Debug.Log("trying to remove too many items.");
            return;
        }

        //remove all of item
        RemoveAllOfItem(item);

        //add in the new amount you want
        if (diff > 0)
            AddItemToInventory(item, diff);
    }
    
    public void RemoveAllOfItem(Item item) {
        List<InventorySlot> newSlots = new List<InventorySlot>();
        foreach (InventorySlot slot in inventorySlots) {
            bool keepSlot = true;
            if (slot.item == item) {
                slot.item = null;
                slot.itemGameObject = null;
                slot.quantity = 0;
                keepSlot = false;
            }
            if (keepSlot)
                newSlots.Add(slot);
        }
        inventorySlots = newSlots;
        OnItemChangedCheckIfCallbackNeeded();
    }

    public void ClearInventory() {
        inventorySlots = emptySlots;
    }

    // This may be useful later, but it likely isn't

    // public void UpdateGameObjectForItem(Item item, GameObject updatedGameObject) {
    //     foreach (InventorySlot slot in inventorySlots) {
    //         if (slot.item == item) {
    //             slot.itemGameObject = updatedGameObject;
    //             break;
    //         }
    //     }
    // }

    // public bool GameObjectExistsForItem(Item item) {
    //     foreach (InventorySlot slot in inventorySlots) {
    //         if (slot.item == item && slot.itemGameObject != null) {
    //             return true;
    //         }
    //     }
    //     return false;
    // }

    public ToolStats GetToolScriptFromItem(Item item) {
        ToolStats toolStats;
        foreach (InventorySlot slot in inventorySlots) {
            if (slot.item == item) {
                toolStats = slot.itemGameObject.GetComponentInChildren<ToolStats>();
                return toolStats;
            }
        }
        return null;
    }

    public bool DoesInventoryContainToolWithType(Tool.ToolTypes toolType) {
        if (GetFirstToolWithType(toolType) != null)
            return true;
        return false;
    }

    public Item GetFirstToolWithType(Tool.ToolTypes toolType) {
        foreach (InventorySlot slot in inventorySlots) {
            if (slot.item.type == ItemType.Tool) {
                if (((Tool)slot.item).toolType == toolType)
                    return slot.item;
            }
        }
        return null;
    }
}

[System.Serializable]
public class InventorySlot {
    public Item item;
    public int quantity;
    public GameObject itemGameObject;
    public InventorySlot(Item item, int quantity, GameObject itemGameobject) {
        this.item = item;
        this.quantity = quantity;
        this.itemGameObject = itemGameobject;
    }

    public void AddQuantity(int value) {
        quantity += value;
    }

    public void RemoveQuantity(int value) {
        quantity -= value;
    }
}