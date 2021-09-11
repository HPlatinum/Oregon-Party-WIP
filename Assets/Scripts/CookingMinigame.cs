using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CookingMinigame : Minigame {

    public int cookingTier = 0;
    private CompactInventory compactInventory;

    private Transform selectionInterface;
    private Transform cookInterface;
    private GameObject background;
    private DisplayItem displayItem;

    private Text cookItemName;
    private Text cookItemQuantity;


    #region Inherited Functions

    public override void ProcessInteractAction() {

    }

    public override void ProcessInteractAnimationEnding() {

    }

    #endregion

    void Start() {
        AssignLocalVariables();
        HideAllUI();
    }

    private void AssignLocalVariables() {
        selectionInterface = transform.Find("Selection");
        cookInterface = transform.Find("Cook Item");
        background = transform.Find("Background").gameObject;
        compactInventory = selectionInterface.Find("Compact Inventory").GetComponent<CompactInventory>();
        displayItem = cookInterface.Find("Item Model").Find("Display Item").GetComponent<DisplayItem>();


        cookItemName = cookInterface.Find("Name").Find("Text").GetComponent<Text>();
        cookItemQuantity = cookInterface.Find("Quantity").Find("Text").GetComponent<Text>();
    }

    private void DisplayRawFoodFromInventory() {
        ItemType itemType = ItemType.RawFood;
        Inventory inventory = StaticVariables.playerInventory;
        InventorySlotUI.OnClickEffect onClick = InventorySlotUI.OnClickEffect.CookingInterface;
        string inventoryTitle = "Raw Food";

        compactInventory.SetupValues(onClick, inventoryTitle);
        compactInventory.ClearAllItemDisplay();
        compactInventory.DisplayAllItemsOfTypeFromInventory(itemType, inventory);
    }

    public void ClickedRawFood(Item item, int quantity) {
        ShowCookingUI(item, quantity);
    }

    public void ShowSelectionUI() {
        background.SetActive(true);
        selectionInterface.gameObject.SetActive(true);
        cookInterface.gameObject.SetActive(false);

        StaticVariables.mainUI.HideUI();

        DisplayRawFoodFromInventory();
    }

    private void ShowCookingUI(Item item, int quantity) {
        background.SetActive(true);
        selectionInterface.gameObject.SetActive(false);
        cookInterface.gameObject.SetActive(true);

        StaticVariables.mainUI.HideUI();

        DisplayItemInCookingInterface(item, quantity);
    }

    private void HideAllUI() {
        background.SetActive(false);
        selectionInterface.gameObject.SetActive(false);
        cookInterface.gameObject.SetActive(false);
    }

    public void QuitSelectionUI() {
        HideAllUI();
        StaticVariables.mainUI.ShowUI();
    }

    public void QuitCookingUI() {
        ShowSelectionUI();
    }

    private void DisplayItemInCookingInterface(Item item, int quantity) {

        cookItemName.text = item.name;
        cookItemQuantity.text = quantity + " in full Inventory";

        displayItem.AddItemAsChild(item, 0.8f);
        displayItem.shouldRotate = true;

    }


    public void CookAllOfItem() {
        print("cooking all copies of the item from your inventory");

    }

    public void CookAllOfEverything() {
        print("cooking every cookable item in your inventory");
    }

}
