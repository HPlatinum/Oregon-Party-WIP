using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlots : MonoBehaviour
{
    public Image icon;
    public Button removeButton;
    public Inventory inventory;
    Item item;

    public void AddItem(Item newItem){
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        removeButton.interactable = true;
    }

    public void ClearSlot(){
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
    }

    public void OnRemoveButton(){
        inventory.RemoveItem(item, inventory.ItemQuantity(item), inventory);
    }

    public void UseItem (){
        if(item != null){
            item.Use();
        }
    }
}
