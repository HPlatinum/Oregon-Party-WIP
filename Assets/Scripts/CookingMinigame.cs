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

    private int cookAmount = 1;
    private Item cookableItem;
    private int cookableItemTotalQuantity = -1;

    private Text cookAmountText;

    List<(Item, int)> allRawFood;

    List<int> allowedCookableQuantities;


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

        cookAmountText = cookInterface.Find("Cook X").Find("Text").GetComponent<Text>();
    }

    private void DisplayRawFoodFromInventory() {
        ItemType itemType = ItemType.RawFood;
        Inventory inventory = StaticVariables.playerInventory;
        InventorySlotUI.OnClickEffect onClick = InventorySlotUI.OnClickEffect.CookingInterface;
        string inventoryTitle = "Raw Food";

        compactInventory.SetupValues(onClick, inventoryTitle);
        compactInventory.ClearAllItemDisplay();
        allRawFood = compactInventory.DisplayAllItemsOfTypeFromInventory(itemType, inventory);
    }

    public void ClickedRawFood(Item item, int quantity) {
        cookableItem = item;
        cookableItemTotalQuantity = quantity;
        allowedCookableQuantities = GetQuantitiesThatPlayerEnoughRoomToCook();
        SetCurrentCookQuantityToMinAllowed();
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
        DisplayCookAmount();
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
        cookableItem = null;
        cookableItemTotalQuantity = -1;
        allowedCookableQuantities = null;
    }
    
    private void DisplayItemInCookingInterface(Item item, int quantity) {

        cookItemName.text = item.name;
        cookItemQuantity.text = quantity + " in full Inventory";

        displayItem.AddItemAsChild(item, 0.8f);
        displayItem.shouldRotate = true;

    }
    
    public void CookAllOfItem() {
        print("cooking all copies of " + cookableItem.name + " from your inventory");
        ReplaceRawItemWithCooked(cookableItem, cookableItemTotalQuantity);
        QuitCookingUI();

    }

    public void CookAllOfEverything() {
        print("cooking every cookable item in your inventory");
        foreach ((Item, int) tuple in allRawFood)
            ReplaceRawItemWithCooked(tuple.Item1, tuple.Item2);
        QuitSelectionUI();
    }

    public void CookXOfItem() {
        print("cooking " + cookAmount + " of " + cookableItem.name + " from your inventory");
        ReplaceRawItemWithCooked(cookableItem, cookAmount);
        QuitCookingUI();
    }

    public void AddCookAmount() {
        CalculateNextHigherCookAmount();
        DisplayCookAmount();
    }

    private void CalculateNextHigherCookAmount() {
        int currentAmount = cookAmount;
        while (true) {
            cookAmount++;
            if (allowedCookableQuantities.Contains(cookAmount))
                return;
            if (cookAmount > cookableItemTotalQuantity)
                cookAmount = currentAmount; return;
        }
    }

    public void SubtractCookAmount() {
        CalculateNextLowerCookAmount();
        DisplayCookAmount();
    }

    private void CalculateNextLowerCookAmount() {
        int currentAmount = cookAmount;
        while (true) {
            cookAmount--;
            if (allowedCookableQuantities.Contains(cookAmount))
                return;
            if (cookAmount == 0)
                cookAmount = currentAmount; return;
        }
    }

    private void DisplayCookAmount() {
        cookAmountText.text = "Cook " + cookAmount;
    }

    private void ReplaceRawItemWithCooked(Item item, int quantity) {
        Item newItem = ((RawFood)item).cookedVariant;
        print("turning " + quantity + " " + item.name + " into " + newItem.name);
        Inventory inventory = StaticVariables.playerInventory;
        inventory.RemoveItemFromInventory(item, quantity);
        inventory.AddItemToInventory(newItem, quantity);
    }

    private bool DoesPlayerHaveEnoughRoomToCook(int quantity) {
        int maxStackSize = cookableItem.stackLimit;
        int partialStackSize = cookableItemTotalQuantity % maxStackSize;
        if (quantity == maxStackSize) {
            print("quantity is max size");
            return true;
        }
        //otherwise, there is 1 stack that is partially full
        if (quantity == partialStackSize) {
            print("quantity is partial size");
            return true;
        }
            
        if (!StaticVariables.playerInventory.IsInventoryFull()) {
            print("full");
            return true;
        }
            
        return false;
    }

    private List<int> GetQuantitiesThatPlayerEnoughRoomToCook() {
        List<int> allowedQuantities = new List<int>();
        for (int i = 1; i<cookableItemTotalQuantity + 1; i++) {
            if (DoesPlayerHaveEnoughRoomToCook(i))
                allowedQuantities.Add(i);
        }
        foreach (int i in allowedQuantities)
            print(i);
        return allowedQuantities;
    }

    private void SetCurrentCookQuantityToMinAllowed() {
        cookAmount = allowedCookableQuantities[0];
    }

}
