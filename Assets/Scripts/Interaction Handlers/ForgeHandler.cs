using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ForgeHandler : InteractionHandler {
    
    /*
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
    */
    private Transform interactableObject;
    /*
    private GameObject flameObject;
    private GameObject woodObject;
    */
    private GameObject litForge;
    private GameObject unlitForge;

    private bool showForgeUIWhenAnimatorIsIdle = false;

    //public Item requiredWoodItem;
    private bool currentlyLightingForge = false;

    //public Material unlitMaterial;
    //public Material litMaterial;

    #region Inherited Functions

    public override void ProcessInteractAction() {
        if (!StaticVariables.interactScript.currentlyInteracting) {
            StaticVariables.SetupPlayerInteractionWithHighlightedObject();
            StaticVariables.interactScript.currentlyInteracting = true;
            SetForgeInteractable();
            if (!IsForgeLit()) {
                LightForge();
            }
            else {
                StaticVariables.SetupPlayerInteractionWithHighlightedObject();
                StaticVariables.PlayAnimation("Cooking - Down");
                showForgeUIWhenAnimatorIsIdle = true;
            }
        }
    }

    public override void ProcessInteractAnimationEnding() {
        if (currentlyLightingForge)
            currentlyLightingForge = false;
        StaticVariables.currentInteractionHandler = null;
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        SetForgeInteractable();
        if (!IsForgeLit())
            return CanPlayerLightForge();
        else
            return true;
    }

    #endregion

    #region Show/Hide UI Functions

    /*
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
        StaticVariables.currentInteractionHandler = null;
        StaticVariables.PlayAnimation("Cooking - Stand");
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

        displayItem.AddItemAsChild(item, 1.2f);
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
    */
    #endregion

    void Start() {
        AssignLocalVariables();
        /*
        background.SetActive(false);
        selectionInterface.gameObject.SetActive(false);
        cookInterface.gameObject.SetActive(false);
        */
    }

    private void Update() {
        if (ShouldForgeUIBeShown()) {
            //StartCoroutine(ShowSelectionUI());
            print("show UI now");
            showForgeUIWhenAnimatorIsIdle = false;
        }
    }

    private bool CanPlayerLightForge() {
        return StaticVariables.playerInventory.DoesInventoryContainToolWithType(Tool.ToolTypes.firelighter);
    }

    private bool ShouldForgeUIBeShown() {
    return (showForgeUIWhenAnimatorIsIdle && StaticVariables.IsPlayerAnimatorInState("Cooking - Idle Part 1"));
}

    private void AssignLocalVariables() {
        /*
        selectionInterface = transform.Find("Selection");
        cookInterface = transform.Find("Cook Item");
        background = transform.Find("Background").gameObject;
        compactInventory = selectionInterface.Find("Compact Inventory").GetComponent<CompactInventory>();
        displayItem = cookInterface.Find("Item Model").Find("Display Item").GetComponent<DisplayItem>();


        cookItemName = cookInterface.Find("Name").Find("Text").GetComponent<Text>();
        cookItemQuantity = cookInterface.Find("Quantity").Find("Text").GetComponent<Text>();

        cookAmountText = cookInterface.Find("Cook X").Find("Text").GetComponent<Text>();
        */
    }

    private void SetForgeInteractable() {
        interactableObject = StaticVariables.interactScript.closestInteractable.transform;

        litForge = interactableObject.Find("Lit Forge").gameObject;
        unlitForge = interactableObject.Find("Unlit Forge").gameObject;
    }

    private bool IsForgeLit() {
        return litForge.activeSelf;
    }

    private void LightForge() {
        StaticVariables.PlayAnimation("Standing To Kneeling");
        currentlyLightingForge = true;

        float timeUntilForgeLights = 3f;
        
        StaticVariables.WaitTimeThenCallFunction(timeUntilForgeLights, ShowLitForge);
    }
    

    private void ShowLitForge() {
        unlitForge.SetActive(false);
        litForge.SetActive(true);
    }
}
