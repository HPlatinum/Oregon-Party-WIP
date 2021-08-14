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

    void Start() {
        quantity = transform.Find("Quantity").Find("Text").GetComponent<Text>();
        quantityUI.SetActive(false);
    }

    public void AddItem(Item newItem){
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        quantityUI.SetActive(true);
        quantity.text = "" + inventory.ItemQuantity(item);
    }

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
