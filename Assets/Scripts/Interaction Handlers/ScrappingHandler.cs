using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrappingHandler : InteractionHandler
{
    private Transform selectionInterface;
    private Transform selectedInterface;
    private Transform quantitySliderTransform;
    private Transform scrapReturnQuantityTransform;
    private Item currentlySelectedItem;
    private Text quantitySliderText;
    private int currentlySelectedItemQuantity;
    private Slider quantitySlider;
    private GameObject background;
    private GameObject scrapButton;
    private GameObject commitButton;
    private CompactInventory selectionCompactInventory;
    private CompactInventory selectedCompactInventory;
    private List<(Item, int)> allScrappableItems;
    private List<(Item, int)> allSelectedItems;
    private List<Item> itemExclusions;
    private Vector3 startPositionOfQuantityUI;
    private bool showUI;
    private bool updateSelectedInventory;
    private bool animateQuantityPanelIsRunning;
    // private List<(Item, int)> selectedItems;
    public Inventory scrapInventory;
    public Item foodScraps;
    public Item metalScraps;
    public Item woodScraps;

    #region Overrides
    public override void ProcessInteractAction() {
        StaticVariables.SetupPlayerInteractionWithHighlightedObject();
        StaticVariables.interactScript.currentlyInteracting = true;
        StaticVariables.PlayAnimation("Scrapping - Approach");
        //run when the player pushes the interact button in range of an interactable
        showUI = true;
    }


    public override void ProcessInteractAnimationEnding() {
        //run when the interact animation ends - when the animator is no longer in state tagged "Interact"
        StaticVariables.currentInteractionHandler = null;
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

    private IEnumerator ShowQuantityUI() {
        yield return AnimateQuantityPanelAppearing();
        yield return null;
    }

    private IEnumerator HideQuantityUI() {
        yield return AnimateQuantityPanelDisappearing();
        yield return null;
    }


    private IEnumerator ReturnToMainUI() {
        yield return HideAllUI();
        background.SetActive(false);
        yield return StaticVariables.mainUI.ShowUI2();
    }

    private IEnumerator HideAllUI() {

        if (selectionInterface.gameObject.activeSelf) {
            yield return StaticVariables.AnimateChildObjectsDisappearing(selectionInterface, true);
        }
        if(scrapReturnQuantityTransform.gameObject.activeSelf) {
            yield return HideQuantityUI();
        }
        if (selectedInterface.gameObject.activeSelf) {
            yield return StaticVariables.AnimateChildObjectsDisappearing(selectedInterface, true);
        }
        if(quantitySliderTransform.gameObject.activeSelf) {
            quantitySliderTransform.gameObject.SetActive(false);
        }
        if(commitButton.gameObject.activeSelf) {
            commitButton.gameObject.SetActive(false);
        }
        yield return null;
    }

    public void QuitScrappingUI() {
        StaticVariables.currentInteractionHandler = null;
        StaticVariables.PlayAnimation("Scrapping - Exit");
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
            showUI = false;
            StartCoroutine(ShowSelectionUI());
            ResetLocalVariables();
        }
        if(ShouldSelectedInventoryBeUpdated()){
            updateSelectedInventory = false;
            DisplayScrappableItemsFromInventory();
            DisplaySelectedItemsFromInventory();
            DisplayScrapReturnQuantities();
            
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
        animateQuantityPanelIsRunning = false;
        background = transform.Find("Background").gameObject;
        scrapButton = transform.Find("Scrap Contents").gameObject;
        commitButton = transform.Find("MoveMultiple").Find("Commit Selected").gameObject;
        selectionInterface = transform.Find("Selection");
        selectionCompactInventory = selectionInterface.Find("Compact Inventory").GetComponent<CompactInventory>();
        selectedInterface = transform.Find("Selected");
        selectedCompactInventory = selectedInterface.Find("Compact Inventory").GetComponent<CompactInventory>();
        quantitySliderTransform = transform.Find("MoveMultiple").Find("Quantity Slider");
        scrapReturnQuantityTransform = transform.Find("Scrap Return Quantity");
        quantitySlider = quantitySliderTransform.GetComponent<Slider>();
        quantitySliderText = quantitySlider.transform.Find("Handle Slide Area").Find("Handle").Find("Quantity").GetComponent<Text>();
        allScrappableItems = null;
        itemExclusions = new List<Item>();
        itemExclusions.Add(woodScraps);
        itemExclusions.Add(metalScraps);
        itemExclusions.Add(foodScraps);
        startPositionOfQuantityUI = scrapReturnQuantityTransform.transform.localPosition;
    }

    private void ResetLocalVariables() {
        
        scrapInventory.ClearInventory();
        allScrappableItems = null;
        allSelectedItems = null;
    }

    public void ClickedScrappableItem(Item item, int quantity, GameObject slot) {
        print(slot.name);
        if(currentlySelectedItem == item) {
            CommitAllSelectionToSelected(quantity);
            return;
        }
        currentlySelectedItem = item;
        currentlySelectedItemQuantity = quantity;
        if(quantity > 1) {
            //pop up quantity UI;
            quantitySliderTransform.gameObject.SetActive(true);
            commitButton.SetActive(true);
            quantitySlider.value = 1;
            quantitySlider.maxValue = quantity;
        }
        else {
            CommitSelectionToSelected();
        }
    }

    public void CommitSelectionToSelected() {
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
        StaticVariables.playerInventory.RemoveItemFromInventorySilently(currentlySelectedItem, quantity);
        ClearBothItemDisplays();
        StaticVariables.WaitTimeThenCallFunction(.25f, CallSelecttedInventoryUpdate);
        int endingXValue = 743;
        Vector3 endPosition = new Vector3(endingXValue, 48, 0);
        if(scrapReturnQuantityTransform.localPosition != endPosition && !animateQuantityPanelIsRunning) {
            StartCoroutine(ShowQuantityUI());
        }
        currentlySelectedItem = null;
    }

    public void CommitAllSelectionToSelected(int quantity) {
        commitButton.SetActive(false);
        quantitySliderTransform.gameObject.SetActive(false);
        scrapInventory.AddItemToInventory(currentlySelectedItem, quantity);
        StaticVariables.playerInventory.RemoveItemFromInventorySilently(currentlySelectedItem, quantity);
        ClearBothItemDisplays();
        StaticVariables.WaitTimeThenCallFunction(.25f, CallSelecttedInventoryUpdate);
        int endingXValue = 743;
        Vector3 endPosition = new Vector3(endingXValue, 48, 0);
        if(scrapReturnQuantityTransform.localPosition != endPosition && !animateQuantityPanelIsRunning) {
            StartCoroutine(ShowQuantityUI());
        }
        currentlySelectedItem = null;
    }
    
    public void CommitSelectedBackToSelection (Item item, int quantity) {
        currentlySelectedItem = item;
        currentlySelectedItemQuantity = quantity;
        StaticVariables.playerInventory.AddItemToInventory(currentlySelectedItem, quantity, false);
        scrapInventory.RemoveItemFromInventory(currentlySelectedItem, quantity);
        ClearBothItemDisplays();
        StaticVariables.WaitTimeThenCallFunction(.25f, CallSelecttedInventoryUpdate);
        if(scrapInventory.GetListOfAllItemsAndTheirQuantity().Count == 0) {
            StartCoroutine(HideQuantityUI());
        }
        currentlySelectedItem = null;
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
                StaticVariables.playerInventory.AddItemToInventory(items.Item1, items.Item2, false);
            }
        }
    }

    public void ScrapAllItems() {
        List<int> scrapReturns = GetQuantityOfScrapReturn();
        int metalScrapQuantity = 0;
        int woodScrapQuantity = 1;
        int foodScrapQuantity = 2;
        if(scrapReturns[foodScrapQuantity] > 0) {
            StaticVariables.playerInventory.AddItemToInventory(foodScraps, scrapReturns[foodScrapQuantity]);
        }
        if(scrapReturns[woodScrapQuantity] > 0) {
            StaticVariables.playerInventory.AddItemToInventory(woodScraps, scrapReturns[woodScrapQuantity]);
        }
        if(scrapReturns[metalScrapQuantity] > 0) {
            StaticVariables.playerInventory.AddItemToInventory(metalScraps, scrapReturns[metalScrapQuantity]);
        }
        scrapInventory.ClearInventory();
        QuitScrappingUI();
    }

    private void DisplayScrapReturnQuantities() {
        List<int> scrapReturns = GetQuantityOfScrapReturn();
        int metalScrapQuantity = 0;
        int woodScrapQuantity = 1;
        int foodScrapQuantity = 2;
        foreach(Transform child in scrapReturnQuantityTransform) {
            if(child.name == "Scrap Quantity") {
                child.GetComponent<Text>().text = ("M: " + scrapReturns[metalScrapQuantity]); 
            }
            if(child.name == "Scrap Quantity (1)") {
                child.GetComponent<Text>().text = ("W: " + scrapReturns[woodScrapQuantity]); 
            }
            if(child.name == "Scrap Quantity (2)") {
                child.GetComponent<Text>().text = ("F: " + scrapReturns[foodScrapQuantity]); 
            }
        }
    }

    private List<int> GetQuantityOfScrapReturn() {
        int metalScrapQuantity = 0;
        int woodScrapQuantity = 0;
        int foodScrapQuantity = 0;
        List<int> quantityOfScrapReturns = new List<int>();
        List <(Item, int)> remainingItems = scrapInventory.GetListOfAllItemsAndTheirQuantity();
        if(remainingItems.Count > 0) {
            foreach((Item, int) tuple in remainingItems) {
                if(tuple.Item1.foodScrapReturn > 0) {
                    foodScrapQuantity += tuple.Item2 * tuple.Item1.foodScrapReturn;
                }
                if(tuple.Item1.metalScrapReturn > 0) {
                    metalScrapQuantity += tuple.Item2 * tuple.Item1.metalScrapReturn;
                }
                if(tuple.Item1.woodScrapReturn > 0) {
                    woodScrapQuantity += tuple.Item2 * tuple.Item1.woodScrapReturn;
                }
            }
        }
        quantityOfScrapReturns.Add(metalScrapQuantity);
        quantityOfScrapReturns.Add(woodScrapQuantity);
        quantityOfScrapReturns.Add(foodScrapQuantity);
        return quantityOfScrapReturns;
    }

    private IEnumerator AnimateQuantityPanelAppearing() {
        animateQuantityPanelIsRunning = true;
        scrapButton.SetActive(true);
        scrapReturnQuantityTransform.gameObject.SetActive(true);
        float timeSinceStarted = 0f;
        int endingXValue = 743;
        Vector3 endPosition = new Vector3(endingXValue, 48, 0);
        while(true) {
            timeSinceStarted += Time.deltaTime;
            scrapReturnQuantityTransform.transform.localPosition = Vector3.Lerp(startPositionOfQuantityUI, endPosition, timeSinceStarted);
            if (scrapReturnQuantityTransform.transform.localPosition == endPosition)
            {
                animateQuantityPanelIsRunning = false;
                yield break;
            }
            yield return null;
        }
    }
    
    private IEnumerator AnimateQuantityPanelDisappearing() {
        scrapButton.SetActive(false);
        float timeSinceStarted = 0f;
        while(true && !animateQuantityPanelIsRunning) {
            timeSinceStarted += Time.deltaTime;
            scrapReturnQuantityTransform.transform.localPosition = Vector3.Lerp(scrapReturnQuantityTransform.transform.localPosition, startPositionOfQuantityUI, timeSinceStarted);
            if (scrapReturnQuantityTransform.transform.localPosition == startPositionOfQuantityUI)
            {
                scrapReturnQuantityTransform.gameObject.SetActive(false);
                yield break;
            }
            yield return null;
        }
    }
}