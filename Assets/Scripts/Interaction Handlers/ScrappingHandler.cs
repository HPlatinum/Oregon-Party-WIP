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
    private Text quantitySliderText;
    private int currentlySelectedItemQuantity;
    private Slider quantitySlider;
    private GameObject background;
    private GameObject commitButton;
    private CompactInventory selectionCompactInventory;
    private CompactInventory selectedCompactInventory;
    private List<(Item, int)> allScrappableItems;
    private List<(Item, int)> allSelectedItems;
    private bool showUI;
    private bool updateSelectedInventory;
    // private List<(Item, int)> selectedItems;
    public Inventory scrapInventory;
    public Item foodScraps;
    public Item metalScraps;
    public Item woodScraps;

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
        yield return StaticVariables.AnimateChildObjectsAppearing(selectionInterface);
        selectedInterface.gameObject.SetActive(true);
        yield return StaticVariables.AnimateChildObjectsAppearing(selectedInterface);
        
        DisplayScrappableItemsFromInventory();

        yield return null;
    }


    private IEnumerator ReturnToMainUI() {
        yield return HideAllUI();
        background.SetActive(false);
        yield return StaticVariables.mainUI.ShowUI2();
    }

    private IEnumerator HideAllUI() {

        if (selectionInterface.gameObject.activeSelf) {
            yield return StaticVariables.AnimateChildObjectsDisappearing(selectionInterface, false);
        }
        if (selectedInterface.gameObject.activeSelf) {
            yield return StaticVariables.AnimateChildObjectsDisappearing(selectedInterface, true);
        }        
        yield return null;
    }

    // public void QuitSelectionUI() {
    //     StaticVariables.currentInteractionHandler = null;
    //     StaticVariables.PlayAnimation("Cooking - Stand");
    //     StartCoroutine(ReturnToMainUI());
    // }

    public void QuitScrappingUI() {
        TransferAnyRemainingItemsBackToPlayerInventory();
        StartCoroutine(ReturnToMainUI());
        ResetLocalVariables();
    }

    private void DisplayScrappableItemsFromInventory() {
        Inventory inventory = StaticVariables.playerInventory;
        InventorySlotUI.OnClickEffect onClick = InventorySlotUI.OnClickEffect.ScrappingInterface;
        string inventoryTitle = "Scrappable Items";
        selectionCompactInventory.SetupValues(onClick, inventoryTitle);
        allScrappableItems = selectionCompactInventory.DisplayAllItemsFromInventory(inventory);
    }

    private void DisplaySelectedItemsFromInventory() {
        Inventory inventory = scrapInventory;
        InventorySlotUI.OnClickEffect onClick = InventorySlotUI.OnClickEffect.ScrappingInterfaceReturnItem;
        string inventoryTitle = "Selected Items";
        selectedCompactInventory.SetupValues(onClick, inventoryTitle);
        allSelectedItems = selectedCompactInventory.DisplayAllItemsFromInventory(inventory);
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
        if(quantitySlider.gameObject.activeSelf && quantitySliderText.text != quantitySlider.value.ToString()) {
            quantitySliderText.text = quantitySlider.value.ToString();
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

        commitButton = transform.Find("MoveMultiple").Find("Commit Selected").gameObject;
        selectionInterface = transform.Find("Selection");
        selectionCompactInventory = selectionInterface.Find("Compact Inventory").GetComponent<CompactInventory>();
        selectedInterface = transform.Find("Selected");
        selectedCompactInventory = selectedInterface.Find("Compact Inventory").GetComponent<CompactInventory>();
        quantitySliderTransform = transform.Find("MoveMultiple").Find("Quantity Slider");
        quantitySlider = quantitySliderTransform.GetComponent<Slider>();
        quantitySliderText = quantitySlider.transform.Find("Handle Slide Area").Find("Handle").Find("Quantity").GetComponent<Text>();
        allScrappableItems = null;
        // selectedItems = new List();
    }

    private void ResetLocalVariables() {
        
        scrapInventory.ClearInventory();
        allScrappableItems = null;
        allSelectedItems = null;
    }

    public void ClickedScrappableItem(Item item, int quantity) {
        currentlySelectedItem = item;
        currentlySelectedItemQuantity = quantity;
        if(quantity > 1) {
            //pop up quantity UI;
            quantitySliderTransform.gameObject.SetActive(true);
            commitButton.SetActive(true);
            quantitySlider.maxValue = quantity;
        }
        else {
            CommitSelectionToSelected();
        }
    }

    public void CommitSelectionToSelected () {
        int quantity;
        commitButton.SetActive(false);
        quantitySliderTransform.gameObject.SetActive(false);
        if(currentlySelectedItemQuantity > 1) {
            quantity = (int)quantitySlider.value;
        }
        else{
            quantity = currentlySelectedItemQuantity;
        }
        scrapInventory.AddItemToInventory(currentlySelectedItem, quantity);
        StaticVariables.playerInventory.RemoveItemFromInventory(currentlySelectedItem, quantity);
        ClearBothItemDisplays();
        StaticVariables.WaitTimeThenCallFunction(.25f, CallSelecttedInventoryUpdate);
    }
    
    public void CommitSelectedBackToSelection (Item item, int quantity) {
        currentlySelectedItem = item;
        currentlySelectedItemQuantity = quantity;
        StaticVariables.playerInventory.AddItemToInventory(currentlySelectedItem, quantity);
        scrapInventory.RemoveItemFromInventory(currentlySelectedItem, quantity);
        ClearBothItemDisplays();
        StaticVariables.WaitTimeThenCallFunction(.25f, CallSelecttedInventoryUpdate);
    }

    private void ClearBothItemDisplays() {
        selectionCompactInventory.ClearAllItemDisplay();
        selectedCompactInventory.ClearAllItemDisplay();
    }
    public void CallSelecttedInventoryUpdate() {
        updateSelectedInventory = true;
    }

    public void TransferAnyRemainingItemsBackToPlayerInventory() {
        List <(Item, int)> remainingItems = scrapInventory.GetListOfAllItemsAndTheirQuantity();
        if(remainingItems.Count > 0) {
            foreach((Item, int) items in remainingItems) {
                StaticVariables.playerInventory.AddItemToInventory(items.Item1, items.Item2);
            }
        }
    }

    public void ScrapAllItems() {
        List <(Item, int)> remainingItems = scrapInventory.GetListOfAllItemsAndTheirQuantity();
        if(remainingItems.Count > 0) {
            foreach((Item, int) items in remainingItems) {
                if(items.Item1.type == ItemType.Food || items.Item1.type == ItemType.RawFood) {
                    StaticVariables.playerInventory.AddItemToInventory(foodScraps, items.Item2);
                }
                else {
                    if(items.Item1.metalScrapReturn > 0) {
                        StaticVariables.playerInventory.AddItemToInventory(metalScraps, items.Item2);
                    }
                    if(items.Item1.woodScrapReturn > 0) {
                        StaticVariables.playerInventory.AddItemToInventory(woodScraps, items.Item2);
                    }
                }
            }
        }
        QuitScrappingUI();
    }
}
