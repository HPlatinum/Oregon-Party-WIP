using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetails : MonoBehaviour { 
    private Item item;
    public float rotationSpeed = 50f;
    private Vector2 originalModelParentPos;
    private enum ItemAction { Equip, Eat, Unequip };
    private ItemAction mainAction;
    private Interact interactScript;

    //gameobject references
    private Transform itemModelParent;
    private Text descriptionText;
    private Text nameText;
    private Text mainActionText;

    void Start(){
        //declare the gameobject references
        itemModelParent = transform.Find("Item Model").Find("Object Parent");
        descriptionText = transform.Find("Description").Find("Text").GetComponent<Text>();
        nameText = transform.Find("Name").Find("Text").GetComponent<Text>();
        mainActionText = transform.Find("Main Action").Find("Text").GetComponent<Text>();
        interactScript = FindObjectOfType<Interact>();


        //set the starting position of the 3d model UI element
        originalModelParentPos = new Vector2(itemModelParent.localPosition.x, itemModelParent.localPosition.y);

        //hide the item details popup
        ShowContents(false);
    }

    private void Update() {
        itemModelParent.Rotate(0, (rotationSpeed * Time.deltaTime), 0);
    }

    private void ShowContents(bool show) {
        //if true, sets all child objects to active. if false, sets to inactive
        foreach (Transform t in transform) {
            t.gameObject.SetActive(show);
        }
    }

    public void DisplayItem(Item item) {
        //called when the player taps an item in the inventory
        //shows the item info and a close-up of the 3d model

        //set variables
        this.item = item;

        //create the 3d model instance and position it correctly
        GameObject newModel = GameObject.Instantiate(item.model, itemModelParent);
        newModel.transform.localPosition = Vector3.zero;
        SetLayerRecursively(newModel, 5); //assumes UI layer is #5
        newModel.transform.localScale = newModel.transform.localScale * item.modelScale;
        newModel.transform.Rotate(item.modelRotation);
        itemModelParent.localPosition = new Vector3 (originalModelParentPos.x + item.modelPosition.x, originalModelParentPos.y + item.modelPosition.y, itemModelParent.localPosition.z);

        //add the description text
        descriptionText.text = item.description;

        //add the item name
        nameText.text = item.name;

        //set the main action function
        if (item.type == ItemType.Tool) mainAction = ItemAction.Equip;
        if (item.type == ItemType.Food) mainAction = ItemAction.Eat;

        //set the main action text
        SetMainActionText();


        //show the ItemDetails UI
        ShowContents(true);

    }

    public void Close() {
        //closes the window
        ShowContents(false);

        //clear the UI elements
        foreach (Transform t in itemModelParent)
            GameObject.Destroy(t.gameObject);
        descriptionText.text = "";
        nameText.text = "";

        //clear the item reference

        item = null;
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

    public  void Trash() {
        //prompt the user if they want to destroy the item
        print("trash item");
    }

    public void BreakDown() {
        //prompt the user if they want to break down the item for components
        print("break down item");
    }

    public void MainAction() {
        //use the item's main action
        if (mainActionText.text == "Equip") {
            interactScript.PutObjectInHand(item, false);
        }
        else if (mainActionText.text == "Unequip") {
            interactScript.RemoveObjectFromHand();
        }
        else if (mainActionText.text == "") {
            //do nothing
        }
        else if (mainActionText.text == "Eat") {
            //do nothing
        }

        SetMainActionText();
    }

    private void SetMainActionText() {
        //set the interact button text for the currently-examined item
        if (mainAction == ItemAction.Equip) {
            Item equippedItem = interactScript.itemInHand;
            bool isEquippedItemInUse = interactScript.currentlyInteracting;

            //if there is no equipped item
            if (equippedItem == null) {
                mainActionText.text = "Equip";
            }
            //if there is an equipped item and it is in use
            else if (isEquippedItemInUse) {
                //grey out the interact box?
                mainActionText.text = "";
            }
            //if the equipped item is the same as the selected item, and it is not in use
            else if (equippedItem == item) {
                mainActionText.text = "Unequip";
            }
            //if the eqipped item is not the same as the selected item, and it is not in use
            else {
                mainActionText.text = "Equip";
            }
        }

        else if (mainAction == ItemAction.Eat) {
            mainActionText.text = "Eat";
        }
    }

}
