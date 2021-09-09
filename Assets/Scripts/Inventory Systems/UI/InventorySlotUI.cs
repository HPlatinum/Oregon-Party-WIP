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
    private Transform itemModelParent;
    private Vector2 originalModelParentPos;
    public enum OnClickEffect { ItemDetails, CookingInterface }
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
        itemModelParent = transform.Find("Object Parent");
        originalModelParentPos = new Vector2(itemModelParent.localPosition.x, itemModelParent.localPosition.y);
    }

    // adds an item to a slot and sets the quantity active if there is a 
    public void AddItemToInventorySlot(Item newItem){
        item = newItem;


        //add the object 3d model
        //create the 3d model instance and position it correctly
        GameObject newModel = GameObject.Instantiate(item.model, itemModelParent);
        newModel.transform.localPosition = Vector3.zero;
        SetLayerRecursively(newModel, 5); //assumes UI layer is #5
        newModel.transform.localScale = newModel.transform.localScale * item.modelScale;
        newModel.transform.Rotate(item.modelRotation);
        //set the position of the 3d model. position offset is scaled down to 20% of the offset used in the item details screen
        itemModelParent.localPosition = new Vector3(originalModelParentPos.x + (item.modelPosition.x * .2f), originalModelParentPos.y + (item.modelPosition.y * .2f), itemModelParent.localPosition.z);
    }
    public void DisplayItemQuantity(int quantity) {
        this.quantity = quantity;
        //print(quantity);
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
        foreach (Transform t in itemModelParent)
            GameObject.Destroy(t.gameObject);
    }

    public Item GetItem() {
        return item;
    }

    public void OnClick() {
        if (item == null)
            return;
        if (clickEffect == OnClickEffect.ItemDetails) {
            FindObjectOfType<ItemDetails>().DisplayItem(item, quantity);
            return;
        }
        if (clickEffect == OnClickEffect.CookingInterface) {
            print("cooking time");
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
