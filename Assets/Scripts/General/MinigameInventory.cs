using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New  Minigame Inventory", menuName = "Inventory System/Minigame Inventory")]
public class MinigameInventory : Inventory
{
    // Start is called before the first frame update
    #region Overrides
    public override void AddItemToInventory(Item item, int pickedUpItemQuantity, bool showChangeAmount = true) {

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

    public override void RemoveItemFromInventory(Item item, int quantity) {
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
            AddItemToInventory(item, diff, false);
    }
    #endregion
}
