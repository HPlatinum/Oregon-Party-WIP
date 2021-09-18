using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetails : MonoBehaviour { 
    private Item item;
    private Vector2 originalModelParentPos;
    private enum ItemAction { Equip, Eat, Unequip };
    private ItemAction mainAction;
    private InteractionManager interactScript;

    //gameobject references
    private Text descriptionText;
    private Text nameText;
    private Text mainActionText;
    private PauseMenu pauseMenu;
    private GameObject quantityGO;
    private Text quantityText;
    private DisplayItem displayItem;

    void Start(){
        //declare the gameobject references
        displayItem = transform.Find("Item Model").Find("Display Item").GetComponent<DisplayItem>();
        descriptionText = transform.Find("Description").Find("Text").GetComponent<Text>();
        nameText = transform.Find("Name").Find("Text").GetComponent<Text>();
        mainActionText = transform.Find("Main Action").Find("Text").GetComponent<Text>();
        interactScript = FindObjectOfType<InteractionManager>();
        pauseMenu = FindObjectOfType<PauseMenu>();
        quantityGO = transform.Find("Quantity").gameObject;
        quantityText = quantityGO.transform.Find("Text").GetComponent<Text>();

        //hide the item details popup
        ShowContents(false);
    }

    private void Update() {
        //itemModelParent.Rotate(0, (rotationSpeed * Time.unscaledDeltaTime), 0);
    }

    private void ShowContents(bool show) {
        //if true, sets all child objects to active. if false, sets to inactive
        foreach (Transform t in transform) {
            t.gameObject.SetActive(show);
        }
    }

    public void DisplayItem(Item item, int quantity) {
        //called when the player taps an item in the inventory
        //shows the item info and a close-up of the 3d model

        //set variables
        this.item = item;

        displayItem.AddItemAsChild(item);
        displayItem.shouldRotate = true;

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

        //display the item quantity - has to go after showing all UI
        quantityGO.SetActive(true);
        quantityText.text = "x" + quantity;
        if (quantity <= 1) quantityGO.SetActive(false);

        //hide the pause menu UI
        pauseMenu.ShowMenu(false);

    }

    public void Close() {
        //closes the window
        ShowContents(false);

        //clear the UI elements
        displayItem.ClearDisplay();
        //foreach (Transform t in itemModelParent)
        //    GameObject.Destroy(t.gameObject);
        descriptionText.text = "";
        nameText.text = "";

        //clear the item reference
        item = null;

        //show the pause menu UI again
        pauseMenu.ShowMenu(true);
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
            interactScript.PutItemInPlayerHand(item);
        }
        else if (mainActionText.text == "Unequip") {
            interactScript.RemoveItemFromHand();
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
