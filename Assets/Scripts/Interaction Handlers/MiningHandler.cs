using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MiningHandler : ToolHandler
{
    //metal and metal position references
    private int hitCount;
    private int currentUIHitCount;
    private int maximumHits;
    private Transform collectable1;
    private Transform collectable2;
    private Transform collectable3;
    private Transform collectable4;
    private Transform collectable5;
    private Transform collectable6;
    private Vector3 metalPos;
    private List<int> possibleXNumbers;
    private List<int> possibleYNumbers;
    private List<int> listXNumbers;
    private List<int> listYNumbers;
    private List<int> listLeftBorderMineable;
    private List<int> listRightBorderMineable;
    private List<int> listTopBorderMineable;
    private List<int> listBottomBorderMineable;
    private List<Item> collectedItems;
    private List<GameObject> mineableLayerGOList;
    private List<GameObject> hitUIGOList;
    
    public Item collectableObject1;
    public Item collectableObject2;
    public Item collectableObject3;
    public Item obstructionObject1;
    public Item obstructionObject2;
    public Item obstructionObject3;
    public Item obstructionObject4;
    public GameObject collectableGameObject1;
    public GameObject collectableGameObject2;
    public GameObject collectableGameObject3;
    public GameObject collectableGameObject4;
    public GameObject collectableGameObject5;
    public GameObject collectableGameObject6;
    private int randomlyChosenObstructionObject;
    private DisplayItem displayItem;
    CollectableColliderTest colliderTest;
    private bool largeDestroy;
    private bool gameOver;
    public bool gameOverFR;
    private bool showMiningUI;
    private bool showFinishUI;
    Transform miningUI;
    Transform uiParent;
    Transform finishScreen;
    Transform messageUI;
    Transform hitCountUI;
    GameObject background;
    private string warning;
    int fontSize;

    #region Inherited Functions
    public override void ProcessInteractAction() {
        if (!StaticVariables.interactScript.currentlyInteracting) {
            StaticVariables.interactScript.SetPreviousItemInHand();
            StaticVariables.interactScript.PutFirstToolOfTypeInHand(Tool.ToolTypes.pickaxe);
            StaticVariables.SetupPlayerInteractionWithHighlightedObject();
            StaticVariables.interactScript.currentlyInteracting = true;
            StaticVariables.PlayAnimation("Swing Pickaxe", 1);
            FindBlade();
            StaticVariables.WaitTimeThenCallFunction(.6f, blade.EnableBlade);
            StaticVariables.WaitTimeThenCallFunction(2.5f, SetShowMiningUIToTrue);
            StaticVariables.mainUI.HideUI();
        }
    }

    public override void ProcessInteractAnimationEnding() {
        if(gameOverFR) {
            StaticVariables.currentInteractionHandler = null;
            StaticVariables.interactScript.PutPreviousItemBackInHand();
        }
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if (StaticVariables.playerInventory.DoesInventoryContainToolWithType(Tool.ToolTypes.pickaxe) && !StaticVariables.interactScript.closestInteractable.resourceMined) {
            if (StaticVariables.playerInventory.CanAddItemToInventory(interactable.item, 1)) {
                return true;
            }
        }
        return false;
    }

    public override void AssignLocalVariables() {
        uiParent = transform.Find("Mining Game Parent");
        finishScreen = transform.Find("Finish Screen");
        messageUI = transform.Find("Message UI");
        background = transform.Find("Background").gameObject;
        collectable1 = uiParent.Find("Items Parent").Find("Collectable");
        collectable2 = uiParent.Find("Items Parent").Find("Collectable (1)");
        collectable3 = uiParent.Find("Items Parent").Find("Collectable (2)");
        collectable4 = uiParent.Find("Items Parent").Find("Collectable (3)");
        collectable5 = uiParent.Find("Items Parent").Find("Collectable (4)");
        collectable6 = uiParent.Find("Items Parent").Find("Collectable (5)");
    }

    #endregion

    public void Start(){
        AssignLocalVariables();
        CreateListOfMineableAreaObjects();
        CreateListsOfBorderMineableAreaObjects();
        AddMineableSlotsToListOfMineableAreaObjects();
    }
    public void Update() {
        if(ShouldMiningUIBeShown()) {
            StartCoroutine(BeginMining());
            showMiningUI = false;
        }
        if(GameISOver()) {
            gameOver = false;
            gameOverFR = true;
        }
        if(ShouldFinishUIBeShown()) {
            showFinishUI = false;
            StartCoroutine(CloseMiningUIAndOpenFinishUI());
        }
        if(hitCount != currentUIHitCount) {
            currentUIHitCount++;
            print("here");
            StartCoroutine(UpdateHitUI());
        }
    }

    private void FindBlade() {
        blade = StaticVariables.interactScript.objectInHand.transform.GetChild(0).GetComponent<BladeInteraction>();
    }
    private bool ShouldMiningUIBeShown() {
        return showMiningUI;
    }

    private bool ShouldFinishUIBeShown() {
        return showFinishUI;
    }

    private bool GameISOver() {
        return gameOver;
    }

    private void SetShowMiningUIToTrue() {
        showMiningUI = true;
    }

    private void SetShowFinishUIToTrue() {
        showFinishUI = true;
    }

    public IEnumerator BeginMining() {
        ResetLocalVariables();
        ShowHitUI();
        yield return ShowMiningUI();
        DetermineNumberOfCollectables();
        PositionMiningCollectables();
        DetermineNumberOfCollectables();
    }


    public IEnumerator ShowMiningUI() {
        yield return StaticVariables.mainUI.HideUI2();
        uiParent.gameObject.SetActive(true);
        background.SetActive(true);
        yield return StaticVariables.AnimateChildObjectsAppearing(uiParent);
        yield return null;
    }

    private IEnumerator CloseMiningUIAndOpenFinishUI() {
        yield return StaticVariables.AnimateChildObjectsDisappearing(uiParent);
        uiParent.gameObject.SetActive(false);
        finishScreen.gameObject.SetActive(true);
        yield return StaticVariables.AnimateChildObjectsAppearing(finishScreen);
        yield return null;
    }


    private IEnumerator CloseFinishUI() {
        yield return StaticVariables.AnimateChildObjectsDisappearing(finishScreen);
        finishScreen.gameObject.SetActive(false);
    }

    private IEnumerator ReturnToMainUI() {
        yield return CloseFinishUI();
        background.SetActive(false);
        yield return StaticVariables.mainUI.ShowUI2();
    }

    private IEnumerator ShowHitUI() {
        hitCountUI.gameObject.SetActive(true);
        return null;
    }

    private IEnumerator UpdateHitUI() {
        int removeGameObjectAtPosition = maximumHits - currentUIHitCount;
        Transform hitBar = hitUIGOList[removeGameObjectAtPosition].transform;
        yield return StaticVariables.AnimateChildObjectsDisappearing(hitBar);
        hitBar.Find("Image").gameObject.SetActive(false);
        yield return null;
    }

    private IEnumerator ShowAlertMessage(string message, int size) {
        messageUI.Find("Message").Find("Text").GetComponent<Text>().text = message;
        messageUI.Find("Message").Find("Text").GetComponent<Text>().fontSize = size;
        messageUI.Find("Message").gameObject.SetActive(true);
        yield return StaticVariables.AnimateChildObjectsAppearing(messageUI);
        StaticVariables.WaitTimeThenCallFunction(1.5f, RemoveAlertMessage);
    }

    private void RemoveAlertMessage () {
        StartCoroutine(HideAlertMessage());
    }

    private IEnumerator HideAlertMessage() {
        yield return StaticVariables.AnimateChildObjectsDisappearing(messageUI);
        messageUI.Find("Message").gameObject.SetActive(false);
    }

    private void CreateMiningObstructions() {
        System.Random rand = new System.Random();
        foreach (Transform child in uiParent.Find("Mining Parent")) {
            randomlyChosenObstructionObject = rand.Next(1, 4);
            displayItem = child.Find("Mine Area").Find("Display Obstruction").GetComponent<DisplayItem>();
            if(randomlyChosenObstructionObject == 1)
                displayItem.AddItemAsChild(obstructionObject1, 3);
            if(randomlyChosenObstructionObject == 2)
                displayItem.AddItemAsChild(obstructionObject2, 3);
            if(randomlyChosenObstructionObject == 3)
                displayItem.AddItemAsChild(obstructionObject3, 3);
            if(randomlyChosenObstructionObject == 4)
                displayItem.AddItemAsChild(obstructionObject4, 3);
        }
    }

    private void DetermineNumberOfCollectables() {
        if(StaticVariables.interactScript.GetToolTier() >= 1){
            collectableGameObject1.SetActive(true);
            collectableGameObject2.SetActive(true);
            collectableGameObject3.SetActive(true);

        }
        if(StaticVariables.interactScript.GetToolTier() >= 2){
            collectableGameObject4.SetActive(true);
            collectableGameObject5.SetActive(true);
        }
        if(StaticVariables.interactScript.GetToolTier() == 3){
            collectableGameObject6.SetActive(true);
        }
    }

    private void DetermineNumberOfHits() {
        print("Tool Tier " + StaticVariables.interactScript.GetToolTier());
        if(StaticVariables.interactScript.GetToolTier() == 1){
            maximumHits = 20;
        }
        if(StaticVariables.interactScript.GetToolTier() == 2){
            maximumHits = 30;
        }
        if(StaticVariables.interactScript.GetToolTier() == 3){
            maximumHits = 40;
        }
    }

    private void DisableMeshes() {
        collectableGameObject1.GetComponentInChildren<MeshCollider>().enabled = false;
        collectableGameObject2.GetComponentInChildren<MeshCollider>().enabled = false;
        collectableGameObject3.GetComponentInChildren<MeshCollider>().enabled = false;
        collectableGameObject4.GetComponentInChildren<MeshCollider>().enabled = false;
        collectableGameObject5.GetComponentInChildren<MeshCollider>().enabled = false;
        collectableGameObject6.GetComponentInChildren<MeshCollider>().enabled = false;
    }
    private void CreateMiningCollectables() {
        System.Random rand = new System.Random();
        foreach (Transform child in uiParent.Find("Items Parent")) {
            displayItem = child.Find("Item Holder").GetComponent<DisplayItem>();
            randomlyChosenObstructionObject = rand.Next(1,3);
            if(randomlyChosenObstructionObject == 1)
                displayItem.AddItemAsChild(collectableObject1, .1f);
            if(randomlyChosenObstructionObject == 2)
                displayItem.AddItemAsChild(collectableObject2, .1f);
            if(randomlyChosenObstructionObject == 3)
                displayItem.AddItemAsChild(collectableObject3, .1f);
            displayItem.transform.localPosition = new Vector3(0, 0, -12);
        }
    }

    public void CreateListOfMineableAreaObjects() {
        mineableLayerGOList = new List<GameObject>();
    }    
    public void AddMineableSlotsToListOfMineableAreaObjects() {
        GameObject gameObject;
        string str;
        for(int i = 0; i < 130; i++) {
            if(i == 0) {
                str = "Mineable Slot";
                gameObject = uiParent.Find("Mining Parent").Find(str).GetChild(0).gameObject;
            }
            else {
                str = "Mineable Slot ("+ i.ToString() +")";
                gameObject = uiParent.Find("Mining Parent").Find(str).GetChild(0).gameObject;
            }
            mineableLayerGOList.Add(gameObject);
        }
    }

    public void CreateHitUIGameObjectList() {
        hitUIGOList = new List<GameObject>();
    }
    private void AddHitUIGameObjectsToList() {
        CreateHitUIGameObjectList();
        GameObject gameObject;
        string str;
        for(int i = 0; i < maximumHits; i++) {
            if(i == 0) {
                str = "Hit";
                gameObject = hitCountUI.Find(str).gameObject;
            }
            else {
                str = "Hit ("+ i.ToString() +")";
                gameObject = hitCountUI.Find(str).gameObject;
            }
            hitUIGOList.Add(gameObject);
        }
    }

    public void CreateListsOfBorderMineableAreaObjects() {
        int[] left = {0, 13, 26, 39, 52, 65, 78, 91, 104, 117};
        listLeftBorderMineable = new List<int>(left);
        int[] right = {12, 25, 38, 51, 64, 77, 90, 103, 116, 129};
        listRightBorderMineable = new List<int>(right);
        listTopBorderMineable = new List<int>();
        listTopBorderMineable = Enumerable.Range(0,13).ToList();
        listBottomBorderMineable = new List<int>();
        listBottomBorderMineable = Enumerable.Range(117,13).ToList();
    }

    public bool CheckIfNumberIsInList(List<int> list, int number) {
        foreach(int listNumber in list) {
            if(listNumber == number) {
                return true;
            }
        }
        return false;
    }

    public void CreateListsForRandomNumberGeneration() {
        possibleXNumbers = Enumerable.Range(-450, 900 + 1).ToList();
        possibleYNumbers = Enumerable.Range(-325, 650 + 1).ToList();
        listXNumbers = new List<int>();
        listYNumbers = new List<int>();
    }

    public void CreateListForCollectablesGathered() {
        collectedItems = new List<Item>();
    }

    private void GeneratePositionXYValues(){
        System.Random rand = new System.Random();
        for(int i = 0; i < 6; i++){
            int indexX = rand.Next(0, possibleXNumbers.Count);
            int indexY = rand.Next(0, possibleYNumbers.Count);
            listXNumbers.Add(possibleXNumbers[indexX]);
            listYNumbers.Add(possibleYNumbers[indexY]);
            RemoveNumbersFromPossibleNumbersList(indexX, possibleXNumbers);
            RemoveNumbersFromPossibleNumbersList(indexY, possibleYNumbers);
        }
    }

    #region index functions
    public bool IndexPlusEightyIsLessThanMaximumIndexOfPossibleNumbers(int index, List<int> possibleNumbers){
        return(index + 80 < possibleNumbers.Count);
    }

    public bool NumbersAreContinuousFromIndexToIndexPlusEightyForward(int index, List<int> possibleNumbers){
        return possibleNumbers[index] + 80 == possibleNumbers[index + 80];
    }
    public bool NumbersAreContinuousFromIndexToIndexMinusEightyBackward(int index, List<int> possibleNumbers){
        return possibleNumbers[index] - 80 == possibleNumbers[index - 80];
    }
    public bool IndexDoesntExceedMaximumLengthOfList(int index, List<int> possibleNumbers){
        return possibleNumbers.Count > index + 2;
    }
    public bool IndexMinusEightyIsZeroOrHigher(int index, List<int> possibleNumbers){
        return index - 80 > -1;
    }
    public bool NumbersAtIndexMinusXEqualsTheSameAsOriginalIndexNumberMinusX(int index, int x, List<int> possibleNumbers){
        return possibleNumbers[index]-x == possibleNumbers[index-x];
    }
    public int GetDifferenceBetweenListMaximumAndIndex(int index, List<int> possibleNumbers){
        return possibleNumbers.Count - 2 - index;
    }

    public void RemoveAtIndex(int index, List<int> possibleNumbers){
        possibleNumbers.RemoveAt(index);
    }

    #endregion
    public void RemoveNumbersFromPossibleNumbersList(int index, List<int> possibleNumbers) {
        int removeIndex;
        // remove from right
        if(IndexPlusEightyIsLessThanMaximumIndexOfPossibleNumbers(index, possibleNumbers)) { //bugged doesn't except if the numbers don't match
            removeIndex = index + 1;
            if(NumbersAreContinuousFromIndexToIndexPlusEightyForward(index, possibleNumbers)){
                for(int x = 0; x < 80; x++) {
                    if(IndexDoesntExceedMaximumLengthOfList(index, possibleNumbers)){
                        RemoveAtIndex(removeIndex,possibleNumbers);
                    }
                }
            }
            else { 
                int difference = GetDifferenceBetweenListMaximumAndIndex(index, possibleNumbers);
                for(int x = 0; x < difference; x++) {
                    RemoveAtIndex(removeIndex,possibleNumbers);
                }
            }
        }
        // remove from left
        if(IndexMinusEightyIsZeroOrHigher(index, possibleNumbers)) {
            if(NumbersAreContinuousFromIndexToIndexMinusEightyBackward(index, possibleNumbers)) {
                removeIndex = index-80;
                for(int x = 0; x < 81; x++) {
                    RemoveAtIndex(removeIndex,possibleNumbers);
                }
            }
            else {
            //count the difference
            int difference = 0;
            for(int x = 0; x < 80; x++) {
                if(NumbersAtIndexMinusXEqualsTheSameAsOriginalIndexNumberMinusX(index, x, possibleNumbers)){
                    difference++;
                    continue;
                }
                else {
                    break;
                }
            }
            removeIndex = index-difference;
            for(int x = 0; x < difference; x++) {
                RemoveAtIndex(removeIndex,possibleNumbers);
                }
            }
        }
        else {
            removeIndex = 0;
            for(int x = 0; x < index; x++) {
                RemoveAtIndex(removeIndex,possibleNumbers);
            }
        }
    }

    public void PositionMiningCollectables() {
        CreateListsForRandomNumberGeneration();
        GeneratePositionXYValues();
        MoveCollectables(collectable1, 0);
        MoveCollectables(collectable2, 1);
        MoveCollectables(collectable3, 2);
        MoveCollectables(collectable4, 3);
        MoveCollectables(collectable5, 4);
        MoveCollectables(collectable6, 5);
    }

    private void MoveCollectables(Transform collectable, int metalNumber) {
        metalPos = new Vector3 (listXNumbers[metalNumber], listYNumbers[metalNumber], 0);
        collectable.localPosition = metalPos;
    }

    private void MoveMinedCollectablesOut(Transform collectable) {
        collectable.transform.position = new Vector3 (collectable.position.x, collectable.position.y, 5.9f);
    }

    public void SetToolToLargeDestroy() {
        Debug.Log("Large Destroy active");
        largeDestroy = true;
    }

    public void SetToolToSingleDestory() {
        Debug.Log("Large Destroy active false");
        largeDestroy = false;
    }

    public void DestroyMineableLayer(GameObject go) {
        if(hitCount < maximumHits) {
            string goString = go.name;
            string numberString = string.Empty;
            int goPosition = 0;
            for (int i = 0; i < goString.Length; i++) {
                if(char.IsDigit(goString[i])) {
                    numberString += goString[i];
                }
                if(numberString.Length > 0) {
                    goPosition = int.Parse(numberString);
                }
            }
            if(largeDestroy == false) {
                SetMineableSlotInactive(goPosition);
            }
            else {
                if(hitCount + 5 <= maximumHits){
                    int goAbove = goPosition - 13;
                    int goBelow = goPosition + 13;
                    int goLeft = goPosition - 1;
                    int goRight = goPosition + 1;
                    bool isOnTop = false;
                    bool isOnRight = false;
                    SetMineableSlotInactive(goPosition);
                    if(!CheckIfNumberIsInList(listTopBorderMineable, goPosition)) {
                        SetMineableSlotInactive(goAbove);
                    }
                    else {
                        isOnTop = true;
                    }
                    if(isOnTop || !CheckIfNumberIsInList(listBottomBorderMineable, goPosition)) {
                        SetMineableSlotInactive(goBelow);
                    }
                    if(!CheckIfNumberIsInList(listRightBorderMineable, goPosition)) {
                        SetMineableSlotInactive(goRight);
                    }
                    else {
                        isOnRight = true;
                    }
                    if(isOnRight || !CheckIfNumberIsInList(listLeftBorderMineable, goPosition)){
                        SetMineableSlotInactive(goLeft);
                    }
                }
                else{
                    fontSize = 55;
                    warning = "You have fewer than 5 remaining hits. Setting tool to Single Destroy.";
                    StartCoroutine(ShowAlertMessage(warning, fontSize));
                    SetToolToSingleDestory();
                }
            }
        }
        else {
            print("You've exceeded the maximum number of allowed hits. That number is " + maximumHits);
        }
        
    }

    private void SetMineableSlotInactive(int goPosition) {
        if(mineableLayerGOList[goPosition].activeSelf && goPosition >= 0 && goPosition <= 129) {
            mineableLayerGOList[goPosition].GetComponentInChildren<MiningButtonClicks>().PlayParticleEffect();
            StaticVariables.WaitTimeThenCallFunction(.15f,mineableLayerGOList[goPosition].GetComponentInChildren<MiningButtonClicks>().SetButtonInactive);
            hitCount++;
        }
        if(hitCount == maximumHits) {
            SetGameOverToTrue();
            EndMiningGameMode();
        }
    }

    private void SetGameOverToTrue() {
        gameOver = true;
    }
    private void EndMiningGameMode() {
        StaticVariables.WaitTimeThenCallFunction(.5f,ShowRevealedCollectableItems);
        StaticVariables.WaitTimeThenCallFunction(2.5f, SetMiningParentInactive);
        StaticVariables.WaitTimeThenCallFunction(2.7f, AddRevealedCollectablesToFinishScreen);
        StaticVariables.WaitTimeThenCallFunction(4.5f, SetGameOverToTrue);
        StaticVariables.WaitTimeThenCallFunction(4.5f, SetShowFinishUIToTrue);
    }

    private void ShowRevealedCollectableItems() { 
        MoveMinedCollectablesOut(collectable1);
        MoveMinedCollectablesOut(collectable2);
        MoveMinedCollectablesOut(collectable3);
        MoveMinedCollectablesOut(collectable4);
        MoveMinedCollectablesOut(collectable5);
        MoveMinedCollectablesOut(collectable6);
    }

    private void AddRevealedCollectablesToFinishScreen() {
        AddItemToCollectedItemList(collectableGameObject1, collectable1);
        AddItemToCollectedItemList(collectableGameObject2, collectable2);
        AddItemToCollectedItemList(collectableGameObject3, collectable3);
        AddItemToCollectedItemList(collectableGameObject4, collectable4);
        AddItemToCollectedItemList(collectableGameObject5, collectable5);
        AddItemToCollectedItemList(collectableGameObject6, collectable6);
        AddMinedCollectablesToFinishScreen();
        SizeFinishScreen();
    }

    private void AddItemToCollectedItemList(GameObject collectableGO, Transform collectable) {
        if(collectableGO.activeSelf){
            collectedItems.Add(GetItemFromCollectable(collectable));
        }
    }

    private void AddMinedCollectablesToFinishScreen() {
        int i = 0;
        string str;
        foreach(Item item in collectedItems) {
            if(i == 0) {
                str = "Collectable";
            }
            else {
                str = "Collectable (" + i.ToString() +")";
            }
            transform.Find("Finish Screen").Find("Grid").Find(str).GetComponentInChildren<DisplayItem>().AddItemAsChild(item, .6f);
            transform.Find("Finish Screen").Find("Grid").Find(str).gameObject.SetActive(true);
            i++;
        }
    }

    private void SizeFinishScreen() {
        int width = collectedItems.Count * 325;
        Vector2 size = new Vector2 (width, 350);
        transform.Find("Finish Screen").Find("Grid").GetComponent<RectTransform>().sizeDelta = size;
    }
    private void SetMiningParentInactive() {
        foreach(GameObject go in mineableLayerGOList) {
            if(go.activeSelf) {
                go.GetComponentInChildren<MiningButtonClicks>().PlayParticleEffect();
                go.GetComponentInChildren<MiningButtonClicks>().SetButtonInactive();
            }
        }
    }

    public void GrantCollectedItem() {
        foreach(Item item in collectedItems) {
            StaticVariables.playerInventory.AddItemToInventory(item, 1);
        }
        StaticVariables.interactScript.closestInteractable.SetResourceMinedTrue();
        StaticVariables.WaitTimeThenCallFunction(2f, StaticVariables.interactScript.DestroyCurrentInteractable);
        StartCoroutine(ReturnToMainUI());
        ProcessInteractAnimationEnding();
    }

    private Item GetItemFromCollectable(Transform collectable) {
        return collectable.Find("Item Holder").GetComponentInChildren<Interactable>().GetItem();
    }

    public void ResetLocalVariables() {
        largeDestroy = false;
        gameOverFR = false;
        hitCount = 0;
        currentUIHitCount = 0;
        DetermineNumberOfHits();
        hitCountUI = uiParent.Find("Hit Bars").Find("Hit Bar " + maximumHits);
        AddHitUIGameObjectsToList();
        ResetHitUI();
        CreateListForCollectablesGathered();
        ResetMiningParentSlots();
        ClearItemsFromCollectables();
        ClearCollectablesFromFinishScreen(); 
        CreateMiningObstructions();
        CreateMiningCollectables();
        DisableMeshes();
        print("resetting variables..");
    }

    private void ResetHitUI() {
        foreach(GameObject go in hitUIGOList) {
            go.transform.Find("Image").gameObject.SetActive(true);
        }
    }

    public void ClearItemsFromCollectables() {
        ClearDisplayCollectableItems(collectable1);
        ClearDisplayCollectableItems(collectable2);
        ClearDisplayCollectableItems(collectable3);
        ClearDisplayCollectableItems(collectable4);
        ClearDisplayCollectableItems(collectable5);
        ClearDisplayCollectableItems(collectable6);
    }

    private void ClearCollectablesFromFinishScreen() {
        string str;
        for(int i = 0; i < 6; i++) {
        if(i == 0) {
            str = "Collectable";
        }
        else {
            str = "Collectable (" + i.ToString() +")";
        }
        transform.Find("Finish Screen").Find("Grid").Find(str).GetComponentInChildren<DisplayItem>().ClearDisplay();
        transform.Find("Finish Screen").Find("Grid").Find(str).gameObject.SetActive(false);
        }
        
    }
    public void ClearDisplayCollectableItems(Transform collectable) {
        collectable.GetComponentInChildren<DisplayItem>().ClearDisplay();
        collectable.gameObject.SetActive(false);
    }
    private void ResetMiningParentSlots() {
        print("Length of mineableLayerGOList " + mineableLayerGOList.Count);
        foreach(GameObject go in mineableLayerGOList) {
            if(!go.activeSelf){
                go.SetActive(true);
            }
            go.GetComponentInChildren<DisplayItem>().ClearDisplay();
        }
    }
    
}