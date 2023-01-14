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
        //print(quantityUI);
        quantityText = quantityUI.transform.Find("Text").GetComponent<Text>();
        displayItem = transform.Find("Display Item").GetComponent<DisplayItem>();
        //itemModelParent = transform.Find("Object Parent");
       // originalModelParentPos = new Vector2(itemModelParent.localPosition.x, itemModelParent.localPosition.y);
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
        if (quantity <= 1) 
            quantityUI.SetActive(false);
        
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

    private void SetLayerRecursively(GameObject obj, int newLayer) {
        //sets the object and all children to be in the specified layer
        if (obj == null) {
            return;
        }
        obj.layer = newLayer;
        foreach (Transform child in obj.transform) {
            if (child == null) {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
