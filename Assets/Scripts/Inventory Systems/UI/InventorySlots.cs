using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlots : MonoBehaviour
{
    public Image icon;
    public Inventory inventory;
    public GameObject quantityUI;
    Item item;
    private Text quantity;

    // sets the quantity UI active in order to get the text component from it. Then deactivates. If you know a better way, feel free to tell me.
    void Start() {
        quantityUI.SetActive(true);
        quantity = transform.Find("Quantity").Find("Text").GetComponent<Text>();
        quantityUI.SetActive(false);
    }

    // adds an item to a slot and sets the quantity active if there is a 
    public void AddItem(Item newItem){
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        if(inventory.ItemQuantity(item) == 1){
            return;
        }
        else {
            quantity.text = "" + inventory.ItemQuantity(item);
            quantityUI.SetActive(true);
        }
    }

    // clears the slot and removes the quantity UI
    public void ClearSlot(){
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        quantityUI.SetActive(false);
        quantity.text = "";
    }


    public void UseItem (){
        if(item != null){
            item.Use();
        }
    }
}
