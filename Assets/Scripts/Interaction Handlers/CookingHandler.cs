using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class CookingHandler : InteractionHandler {

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
    
    private Transform interactableObject;
    private GameObject flameObject;
    private GameObject woodObject;

    private bool showCookingUIWhenAnimatorIsIdle = false;

    public Item requiredWoodItem;
    private bool currentlyLightingFire = false;

    #region Inherited Functions

    public override void ProcessInteractAction() {
        StaticVariables.SetupPlayerInteractionWithHighlightedObject();
        StaticVariables.interactScript.currentlyInteracting = true;
        SetCookingInteractable();
        if (!IsCookingObjectLit()) {
            LightFire();
        }
        else {
            SetCookingTier();
            StaticVariables.SetupPlayerInteractionWithHighlightedObject();
            StaticVariables.PlayAnimation("Cooking - Down");
            showCookingUIWhenAnimatorIsIdle = true;
        }
    }

    public override void ProcessInteractAnimationEnding() {
        if (currentlyLightingFire) {
            currentlyLightingFire = false;
        }
        StaticVariables.currentInteractionHandler = null;
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        SetCookingInteractable();
        if (!IsCookingObjectLit())
            return CanPlayerLightFire();
        else
            return true;
    }

    #endregion

    #region Show/Hide UI Functions

    private IEnumerator ShowSelectionUI() {

        yield return StaticVariables.mainUI.HideUI2();
        yield return HideAllUI();
        background.SetActive(true);
        selectionInterface.gameObject.SetActive(true);
       
        yield return StaticVariables.AnimateChildObjectsAppearing(selectionInterface);
        
        DisplayRawFoodFromInventory();

        yield return null;
    }

    private IEnumerator ShowCookingUI(Item item, int quantity) {
        yield return StaticVariables.mainUI.HideUI2();
        yield return HideAllUI();
        background.SetActive(true);
        cookInterface.gameObject.SetActive(true);

        DisplayItemInCookingInterface(item, quantity);
        DisplayCookAmount();

        yield return StaticVariables.AnimateChildObjectsAppearing(cookInterface);
        
        yield return null;
    }

    public void QuitSelectionUI() {
        StaticVariables.PlayAnimation("Scrapping - Exit");
        StartCoroutine(ReturnToMainUI());
    }

    private IEnumerator ReturnToMainUI() {
        yield return HideAllUI();
        background.SetActive(false);
        yield return StaticVariables.mainUI.ShowUI2();
    }

    private IEnumerator HideAllUI() {

        if (selectionInterface.gameObject.activeSelf)
            yield return StaticVariables.AnimateChildObjectsDisappearing(selectionInterface, true);

        else if (cookInterface.gameObject.activeSelf)
            yield return StaticVariables.AnimateChildObjectsDisappearing(cookInterface, true);
        
        yield return null;
    }

    public void QuitCookingUI() {
        StartCoroutine(ShowSelectionUI());
        cookableItem = null;
        cookableItemTotalQuantity = -1;
        allowedCookableQuantities = null;
    }

    private void DisplayItemInCookingInterface(Item item, int quantity) {

        cookItemName.text = item.name;
        cookItemQuantity.text = quantity + " in full Inventory";

        displayItem.AddItemAsChild(item, false, 1.2f);
        displayItem.shouldRotate = true;

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

    #endregion

    void Start() {
        AssignLocalVariables();
        background.SetActive(false);
        selectionInterface.gameObject.SetActive(false);
        cookInterface.gameObject.SetActive(false);
    }

    private void Update() {
        if (ShouldCookingUIBeShown()) {
            StartCoroutine(ShowSelectionUI());
            showCookingUIWhenAnimatorIsIdle = false;
        }
    }

    private bool CanPlayerLightFire() {
        if (StaticVariables.playerInventory.DoesInventoryContainToolWithType(Tool.ToolTypes.firelighter)) {
            if (StaticVariables.playerInventory.GetQuantityOfSpecificItem(requiredWoodItem) > 0) {
                return true;
            }
        }
        return false;
    }

    private bool ShouldCookingUIBeShown() {
    return (showCookingUIWhenAnimatorIsIdle && StaticVariables.IsPlayerAnimatorInState("Cooking - Idle Part 1"));
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

    private void SetCookingInteractable() {
        interactableObject = StaticVariables.interactScript.closestInteractable.transform;

        flameObject = interactableObject.parent.Find("Campfire Flame").gameObject;
        woodObject = interactableObject.Find("Campfire Wood").gameObject;
    }

    private bool IsCookingObjectLit() {
        return flameObject.activeSelf;
    }

    private void LightFire() {
        StaticVariables.PlayAnimation("Standing To Kneeling");
        currentlyLightingFire = true;

        float timeUntilWoodAppears = 2.5f;
        float timeUntilFireLights = .5f;

        StaticVariables.WaitTimeThenCallFunction(timeUntilWoodAppears, ShowWood);
        StaticVariables.WaitTimeThenCallFunction(timeUntilWoodAppears + timeUntilFireLights, ShowFlame);
    }

    private void ShowWood() {
        woodObject.SetActive(true);
        woodObject.AddComponent<AnimatedObjectAppearing>();
        StaticVariables.playerInventory.RemoveItemFromInventory(requiredWoodItem, 1);
    }

    private void ShowFlame() {
        flameObject.SetActive(true);
    }

    public void ClickedRawFood(Item item, int quantity) {
        cookableItem = item;
        cookableItemTotalQuantity = quantity;
        allowedCookableQuantities = GetQuantitiesThatPlayerEnoughRoomToCook();
        SetCurrentCookQuantityToMinAllowed();
        StartCoroutine(ShowCookingUI(item, quantity));
    }
    
    public void CookAllOfItem() {
        ReplaceRawItemWithCooked(cookableItem, cookableItemTotalQuantity);
        QuitCookingUI();

    }

    public void CookAllOfEverything() {
        foreach ((Item, int) tuple in allRawFood)
            ReplaceRawItemWithCooked(tuple.Item1, tuple.Item2);
        QuitSelectionUI();
    }

    public void CookXOfItem() {
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
        Inventory inventory = StaticVariables.playerInventory;
        inventory.RemoveItemFromInventory(item, quantity);
        inventory.AddItemToInventory(newItem, quantity);

        //todo do something with the cooking tier?
    }

    private bool DoesPlayerHaveEnoughRoomToCook(int quantity) {
        int maxStackSize = cookableItem.stackLimit;
        int partialStackSize = cookableItemTotalQuantity % maxStackSize;
        if (quantity == maxStackSize)
            return true;
        //otherwise, there is 1 stack that is partially full
        if (quantity == partialStackSize)
            return true;
        if (!StaticVariables.playerInventory.IsInventoryFull())
            return true;
        return false;
    }

    private List<int> GetQuantitiesThatPlayerEnoughRoomToCook() {
        List<int> allowedQuantities = new List<int>();
        for (int i = 1; i<cookableItemTotalQuantity + 1; i++) {
            if (DoesPlayerHaveEnoughRoomToCook(i))
                allowedQuantities.Add(i);
        }
        return allowedQuantities;
    }

    private void SetCurrentCookQuantityToMinAllowed() {
        cookAmount = allowedCookableQuantities[0];
    }

    private void SetCookingTier() {
        Interactable.InteractTypes type = StaticVariables.interactScript.closestInteractable.interactType;
        cookingTier = 0;
        if (type == Interactable.InteractTypes.CookingTier1)
            cookingTier = 1;
        if (type == Interactable.InteractTypes.CookingTier2)
            cookingTier = 2;
    }
}
