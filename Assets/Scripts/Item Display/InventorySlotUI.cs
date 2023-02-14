using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour {
    // todo remove inventory dependency
    // todo change display item quantity to just take a new value
    // todo set the tap function at runtime


    //public Inventory inventory;
    private GameObject quantityUI;
    private GameObject quantityMask;
    [HideInInspector]
    public Item item;
    [HideInInspector]
    public int quantity;
    private Text quantityText;
    //private Transform itemModelParent;
    private DisplayItem displayItem;
    private Vector2 originalModelParentPos;
    public enum OnClickEffect { ItemDetails, CookingInterface, ScrappingInterface, ScrappingInterfaceReturnItem }
    public OnClickEffect clickEffect = OnClickEffect.ItemDetails;
    
    //set the method to call when clicked         private 

    public void Setup() {
        AssignLocalVariables();
        quantityUI.SetActive(false);
    }


    public void AssignLocalVariables() {
        quantityUI = transform.Find("Quantity").gameObject;
        quantityMask = transform.Find("Masks").Find("Quantity").gameObject;
        quantityText = quantityUI.transform.Find("Text").GetComponent<Text>();
        displayItem = transform.Find("Display Item").GetComponent<DisplayItem>();
    }

    // adds an item to a slot and sets the quantity active if there is a 
    public void AddItemToInventorySlot(Item newItem){
        item = newItem;

        displayItem.AddItemAsChild(item, true, 0.2f);

    }
    public void DisplayItemQuantity(int quantity) {
        this.quantity = quantity;
        quantityText.text = quantity + "";
        quantityUI.SetActive(true);
        quantityMask.SetActive(true);
        if (quantity <= 1) {
            quantityUI.SetActive(false);
            quantityMask.SetActive(false);
        }
            
        
    }
    
    public void ClearSlot(){
        item = null;
        quantity = 0;
        quantityUI.SetActive(false);
        quantityText.text = "";
        displayItem.ClearDisplay();
    }

    public Item GetItem() {
        return item;
    }

    public void OnClick() {
        if (item == null)
            return;
        if (clickEffect == OnClickEffect.ItemDetails) {
            StaticVariables.itemDetails.DisplayItem(item, quantity);
            return;
        }
        if (clickEffect == OnClickEffect.CookingInterface) {
            StaticVariables.cookingHandler.ClickedRawFood(item, quantity);
            return;
        }
        if (clickEffect == OnClickEffect.ScrappingInterface) {
            StaticVariables.scrappingHandler.ClickedScrappableItem(item, quantity);
            return;
        }
        if(clickEffect == OnClickEffect.ScrappingInterfaceReturnItem) {
            StaticVariables.scrappingHandler.CommitSelectedBackToSelection(item, quantity);
            return;
        }
    }
}
