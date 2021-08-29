using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlots : MonoBehaviour
{
    public Inventory inventory;
    public GameObject quantityUI;
    Item item;
    private Text quantity;
    private Transform itemModelParent;
    private Vector2 originalModelParentPos;

    // Deactivates the quantity UI
    void Start() {
        quantity = transform.Find("Quantity").Find("Text").GetComponent<Text>();
        quantityUI.SetActive(false);
        itemModelParent = transform.Find("Object Parent");

        //set the starting position of the 3d model UI element
        originalModelParentPos = new Vector2(itemModelParent.localPosition.x, itemModelParent.localPosition.y);

    }

    // adds an item to a slot and sets the quantity active if there is a 
    public void AddItem(Item newItem){
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


        // if (inventory.ItemQuantity(item) == 1){
        //     return;
        // }
        // else {
        //     quantity.text = "" + inventory.ItemQuantity(item);
        //     quantityUI.SetActive(true);
        // }
    }

    // clears the slot and removes the quantity UI
    public void ClearSlot(){
        item = null;
        quantityUI.SetActive(false);
        quantity.text = "";
        foreach (Transform t in itemModelParent)
            GameObject.Destroy(t.gameObject);
    }

    public Item GetItem() {
        return item;
    }

    public void TapItem() {
        if (item != null) {
            FindObjectOfType<ItemDetails>().DisplayItem(item, inventory.GetItemQuantity());
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
