using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrappingHandler : InteractionHandler
{
    private Transform selectionInterface;
    private Transform selectedInterface;
    private Transform quantitySliderTransform;
    private Transform quantitySelectionDetails;
    private Transform scrapReturnQuantityTransform;
    private Transform quantitySelectionHolder;
    private Transform closeButton;
    private Item currentlySelectedItem;
    private Text quantitySliderText;
    private Text quantitySelectionDetailsText;
    private int currentlySelectedItemQuantity;
    private Slider quantitySlider;
    private GameObject background;
    private GameObject quantitySelectBackground;
    private GameObject scrapButton;
    private RectTransform scrapButtonRectTransform;
    private Transform commitButton;
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

        yield return StaticVariables.mainUI.HideUIWithAnimation();
        yield return HideAllUI();
        background.SetActive(true);
        selectionInterface.gameObject.SetActive(true);
        yield return StaticVariables.AnimateChildObjectsAppearing(selectionInterface);
        selectedInterface.gameObject.SetActive(true);
        yield return StaticVariables.AnimateChildObjectsAppearing(selectedInterface);
        closeButton.gameObject.SetActive(true);
        
        DisplayScrappableItemsFromInventory();

        yield return null;
    }

    private IEnumerator ShowQuantityUI() {
        yield return AnimateQuantityPanelAppearing();
        yield return null;
    }

    private IEnumerator HideQuantityUI() {
        yield return AnimateQuantityPanelDisappearing();
        yield return StaticVariables.AnimateChildObjectsDisappearing(transform.Find("Scrap Button Container"));
        yield return null;
    }


    private IEnumerator ReturnToMainUI() {
        yield return HideAllUI();
        background.SetActive(false);
        yield return StaticVariables.mainUI.ShowUIWithAnimation();
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
        closeButton.gameObject.SetActive(false);
        // yield return AnimateScrapAllButtonDisappearing();
        scrapButton.SetActive(false);
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
            if(quantitySliderTransform.gameObject.activeSelf) {
                StartCoroutine(AnimateQuantitySelectionDisappearing());
            }            
        }
        if(quantitySlider.gameObject.activeSelf && quantitySliderText.text != quantitySlider.value.ToString()) {
            quantitySliderText.text = quantitySlider.value.ToString();
            UpdateQuantityDetails();
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
        quantitySelectBackground = transform.Find("Background (1)").gameObject;
        closeButton = transform.Find("Close");
        scrapButton = transform.Find("Scrap Button Container").Find("Scrap Contents").gameObject;
        scrapButtonRectTransform = scrapButton.GetComponent<RectTransform>();
        selectionInterface = transform.Find("Selection");
        selectionCompactInventory = selectionInterface.Find("Compact Inventory").GetComponent<CompactInventory>();
        selectedInterface = transform.Find("Selected");
        selectedCompactInventory = selectedInterface.Find("Compact Inventory").GetComponent<CompactInventory>();
        quantitySelectionHolder = transform.Find("Quantity Selection UI");
        quantitySliderTransform = quantitySelectionHolder.Find("Quantity Slider");
        commitButton = quantitySelectionHolder.Find("Commit Selected");
        scrapReturnQuantityTransform = transform.Find("Scrap Return Quantity");
        quantitySlider = quantitySliderTransform.GetComponent<Slider>();
        quantitySliderText = quantitySlider.transform.Find("Handle Slide Area").Find("Handle").Find("Quantity").GetComponent<Text>();
        quantitySelectionDetailsText = quantitySelectionHolder.Find("Quantity Selection Details").GetComponentInChildren<Text>();
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

    public void ClickedScrappableItem(Item item, int quantity) {
        if(currentlySelectedItem == item) {
            CommitAllSelectionToSelected(quantity);
            return;
        }
        currentlySelectedItem = item;
        currentlySelectedItemQuantity = quantity;
        if(quantity > 1) {
            //pop up quantity UI;
            StartCoroutine(AnimateQuantitySelectionAppearing());
            quantitySlider.value = 1;
            quantitySlider.maxValue = quantity;
        }
        else {
            CommitSelectionToSelected();
        }
    }

    public void CommitSelectionToSelected() {
        int quantity;
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
        int endingXValue = 800;
        Vector3 endPosition = new Vector3(endingXValue, 48, 0);
        if(scrapReturnQuantityTransform.localPosition != endPosition && !animateQuantityPanelIsRunning) {
            StartCoroutine(ShowQuantityUI());
        }
        currentlySelectedItem = null;
    }

    public void CommitAllSelectionToSelected(int quantity) {
        scrapInventory.AddItemToInventory(currentlySelectedItem, quantity);
        StaticVariables.playerInventory.RemoveItemFromInventorySilently(currentlySelectedItem, quantity);
        ClearBothItemDisplays();
        StaticVariables.WaitTimeThenCallFunction(.25f, CallSelecttedInventoryUpdate);
        int endingXValue = 800;
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
            if(child.name == "Metal Scrap Container") {
                child.GetComponentInChildren<Text>().text = (scrapReturns[metalScrapQuantity].ToString()); 
            }
            if(child.name == "Wood Scrap Container") {
                child.GetComponentInChildren<Text>().text = (scrapReturns[woodScrapQuantity].ToString()); 
            }
            if(child.name == "Food Scrap Container") {
                child.GetComponentInChildren<Text>().text = (scrapReturns[foodScrapQuantity].ToString()); 
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
        StaticVariables.WaitTimeThenCallFunction(.25f, CallScrapAllButton);
        scrapReturnQuantityTransform.gameObject.SetActive(true);
        float timeSinceStarted = 0f;
        int endingXValue = 800;
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

    private IEnumerator AnimateQuantitySelectionAppearing() {
        UpdateQuantityDetails();
        quantitySelectionHolder.gameObject.SetActive(true);
        commitButton.gameObject.SetActive(true);
        quantitySelectBackground.SetActive(true);
        quantitySliderTransform.gameObject.SetActive(true);
        yield return StaticVariables.AnimateChildObjectsAppearing(quantitySelectionHolder);
    }

    private IEnumerator AnimateQuantitySelectionDisappearing() {
        quantitySelectBackground.SetActive(false);
        yield return StaticVariables.AnimateChildObjectsDisappearing(quantitySelectionHolder, true);
    }

    public void ClickToEscapeQuantitySelection() {
        currentlySelectedItem = null;
        StartCoroutine(AnimateQuantitySelectionDisappearing());
    }

    private void UpdateQuantityDetails() {
        string quantityDetails = "Scrapping " + quantitySlider.value.ToString() + " " + currentlySelectedItem.name + "(s) will return";
        if(currentlySelectedItem.foodScrapReturn > 0) {
            quantityDetails = quantityDetails + " " +  (currentlySelectedItem.foodScrapReturn * quantitySlider.value).ToString() + " food scrap(s)";
        }
        if(currentlySelectedItem.metalScrapReturn > 0) {
            quantityDetails = quantityDetails + " " +  (currentlySelectedItem.metalScrapReturn * quantitySlider.value).ToString() + " metal scrap(s)";
        }
        if(currentlySelectedItem.woodScrapReturn > 0) {
            quantityDetails = quantityDetails + " " + (currentlySelectedItem.woodScrapReturn * quantitySlider.value).ToString()+ " wood scrap(s)";
        }
        quantityDetails = quantityDetails + ".";
        quantitySelectionDetailsText.text = quantityDetails;
    }

    private void CallScrapAllButton() {
        StartCoroutine(AnimateScrapAllButtonAppearing());
    }
    private IEnumerator AnimateScrapAllButtonAppearing() {
        scrapButton.SetActive(true);
        yield return StaticVariables.AnimateChildObjectsAppearing(transform.Find("Scrap Button Container"));
    }

    // private IEnumerator AnimateScrapAllButtonDisappearing() {
    //     scrapButton.SetActive(false);
    //     bool hasFinishedRun = true;
    //     float timeSinceStarted = 0f;
    //     Vector3 startSize = new Vector3 ( 1, 1, 1);
    //     Vector3 midSize = new Vector3 (2,2,2);
    //     Vector3 endSize = new Vector3(3, 3, 3);
    //     Vector3 currentSize = new Vector3(1, 1, 1);
    //     while(true) {
    //         if(hasFinishedRun) {
    //             timeSinceStarted += 1.2f *  Time.deltaTime;
    //             int currentTime = 0;
    //             currentSize.z = timeSinceStarted
    //         scrapButtonRectTransform.localScale = currentSize.z = 
    //         if (0 + timeSinceStarted > 3)
    //         {
    //             yield break;
    //         }
    //         yield return null;
    //         }
    //         timeSinceStarted += 1.2f *  Time.deltaTime;
    //         int currentTime = 0;
            
    //         scrapButtonRectTransform.localScale = 
    //         if (0 + timeSinceStarted > 3)
    //         {
    //             yield break;
    //         }
            
    //     }
    // }
}