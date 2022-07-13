using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrappingHandler : InteractionHandler
{
    private Transform selectionInterface;
    private Transform selectedInterface;
    private Transform quantitySliderTransform;
    private Item currentlySelectedItem;
    private Slider quantitySlider;
    private GameObject background;
    private CompactInventory selectionCompactInventory;
    private CompactInventory selectedCompactInventory;
    private List<(Item, int)> allScrappableItems;
    private bool showUI;
    private bool updateSelectedInventory;
    // private List<(Item, int)> selectedItems;
    public Inventory scrapInventory;

    #region Overrides
    public override void ProcessInteractAction() {
        //run when the player pushes the interact button in range of an interactable
        showUI = true;
    }


    public override void ProcessInteractAnimationEnding() {
        //run when the interact animation ends - when the animator is no longer in state tagged "Interact"
        print("interact animation ended, no response to execute");
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        return true;
    }
    #endregion

    #region Show/Hide UI Functions

    private IEnumerator ShowSelectionUI() {

        yield return StaticVariables.mainUI.HideUI2();
        yield return HideAllUI();
        background.SetActive(true);
        selectionInterface.gameObject.SetActive(true);
        selectedInterface.gameObject.SetActive(true);
        yield return StaticVariables.AnimateChildObjectsAppearing(selectionInterface);
        
        DisplayScrappableItemsFromInventory();

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

        else if (selectedInterface.gameObject.activeSelf)
            yield return StaticVariables.AnimateChildObjectsDisappearing(selectedInterface, true);
        
        yield return null;
    }

    public void QuitScrappingUI() {
        StartCoroutine(ShowSelectionUI());
        ResetLocalVariables();
        // cookableItem = null;
        // cookableItemTotalQuantity = -1;
        // allowedCookableQuantities = null;
    }

    private void DisplayScrappableItemsFromInventory() {
        Inventory inventory = StaticVariables.playerInventory;
        InventorySlotUI.OnClickEffect onClick = InventorySlotUI.OnClickEffect.ScrappingInterface;
        string inventoryTitle = "Scrappable Items";
        selectionCompactInventory.SetupValues(onClick, inventoryTitle);
        selectionCompactInventory.ClearAllItemDisplay();
        allScrappableItems = selectionCompactInventory.DisplayAllItemsFromInventory(inventory);
    }

    private void DisplaySelectedItemsFromInventory() {
        Inventory inventory = scrapInventory;
        InventorySlotUI.OnClickEffect onClick = InventorySlotUI.OnClickEffect.ScrappingInterface;
        string inventoryTitle = "Selected Items";
        selectedCompactInventory.SetupValues(onClick, inventoryTitle);
        selectedCompactInventory.ClearAllItemDisplay();
        allScrappableItems = selectionCompactInventory.DisplayAllItemsFromInventory(inventory);
    }

    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        AssignLocalVariables();
        background.SetActive(false);
        selectionInterface.gameObject.SetActive(false);
        selectedInterface.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if(ShouldScrappingUIBeShown()) {
            StartCoroutine(ShowSelectionUI());
            showUI = false;
        }
        if(ShouldSelectedInventoryBeUpdated()){
            DisplayScrappableItemsFromInventory();
            DisplaySelectedItemsFromInventory();
            updateSelectedInventory = false;
        }
    }

    private bool ShouldScrappingUIBeShown() {
        return showUI;
    }

    private bool ShouldSelectedInventoryBeUpdated() {
        return updateSelectedInventory;
    }

    private void AssignLocalVariables() {
        showUI = false;
        updateSelectedInventory = false;
        background = transform.Find("Background").gameObject;

        selectionInterface = transform.Find("Selection");
        selectionCompactInventory = selectionInterface.Find("Compact Inventory").GetComponent<CompactInventory>();
        selectedInterface = transform.Find("Selected");
        selectedCompactInventory = selectedInterface.Find("Compact Inventory").GetComponent<CompactInventory>();
        quantitySliderTransform = selectionInterface.Find("Quantity Slider");
        quantitySlider = quantitySliderTransform.GetComponent<Slider>();
        allScrappableItems = null;
        // selectedItems = new List();
    }

    private void ResetLocalVariables() {
        scrapInventory = new Inventory();
        allScrappableItems = null;
        // selectedItems = new List();
    }

    public void ClickedScrappableItem(Item item, int quantity) {
        currentlySelectedItem = item;
        if(quantity > 1) {
            //pop up quantity UI;
            quantitySliderTransform.gameObject.SetActive(true);
            quantitySlider.maxValue = quantity;
        }
        //
        print("Item " + item + " Quantity " + quantity);
    }

    public void CommitSelection () {
        int quantity = (int)quantitySlider.value;
        scrapInventory.AddItemToInventory(currentlySelectedItem, quantity);
        StaticVariables.playerInventory.RemoveItemFromInventory(currentlySelectedItem, quantity);
        updateSelectedInventory = true;
    }
}
