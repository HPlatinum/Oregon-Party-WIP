using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MiningHandler : ToolHandler
{
    //metal and metal position references
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

    #region Inherited Functions

    public override void ProcessInteractAction() {
        if (!StaticVariables.interactScript.currentlyInteracting) {
            StaticVariables.interactScript.SetPreviousItemInHand();
            StaticVariables.interactScript.PutFirstToolOfTypeInHand(Tool.ToolTypes.pickaxe);
            AssignLocalVariables();
            DetermineNumberOfCollectables();
            StaticVariables.SetupPlayerInteractionWithHighlightedObject();
            StaticVariables.interactScript.currentlyInteracting = true;
            StaticVariables.PlayAnimation("Swing Pickaxe", 1);
            StaticVariables.WaitTimeThenCallFunction(.6f, blade.EnableBlade);
            StaticVariables.WaitTimeThenCallFunction(2.5f, ShowMiningUI);
            StaticVariables.mainUI.HideUI();
        }
    }

    public override void ProcessInteractAnimationEnding() {
        StaticVariables.currentInteractionHandler = null;
        StaticVariables.interactScript.PutPreviousItemBackInHand();
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if (StaticVariables.playerInventory.DoesInventoryContainToolWithType(Tool.ToolTypes.pickaxe)) {
            if (StaticVariables.playerInventory.CanAddItemToInventory(interactable.item, 1)) {
                return true;
            }
        }
        return false;
    }

    public override void AssignLocalVariables() {
        collectable1 = transform.Find("Background").Find("Items Parent").Find("Collectable");
        collectable2 = transform.Find("Background").Find("Items Parent").Find("Collectable (1)");
        collectable3 = transform.Find("Background").Find("Items Parent").Find("Collectable (2)");
        collectable4 = transform.Find("Background").Find("Items Parent").Find("Collectable (3)");
        collectable5 = transform.Find("Background").Find("Items Parent").Find("Collectable (4)");
        collectable6 = transform.Find("Background").Find("Items Parent").Find("Collectable (5)");
        blade = StaticVariables.interactScript.objectInHand.transform.GetChild(0).GetComponent<BladeInteraction>();
    }

    #endregion

    private void ShowMiningUI() {
        transform.Find("Background").gameObject.SetActive(true);
        CreateMiningObstructions();
        CreateMiningCollectables();
        DisableMeshes();
        PositionMiningCollectables();

    }

    private void CreateMiningObstructions() {
        System.Random rand = new System.Random();
        foreach (Transform child in transform.Find("Background").Find("Mining Parent")) {
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
        if(StaticVariables.interactScript.GetToolTier() == 1){
            Debug.Log("Here 11111");
            collectableGameObject4.SetActive(false);
            collectableGameObject5.SetActive(false);
            collectableGameObject6.SetActive(false);
        }
        if(StaticVariables.interactScript.GetToolTier() == 2){
            collectableGameObject6.SetActive(false);
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
        foreach (Transform child in transform.Find("Background").Find("Items Parent")) {
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

    public void CreateLists() {
        possibleXNumbers = Enumerable.Range(-450, 900 + 1).ToList();
        possibleYNumbers = Enumerable.Range(-325, 650 + 1).ToList();
        listXNumbers = new List<int>();
        listYNumbers = new List<int>();
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
        CreateLists();
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

    public void DestroyMineableLayer(GameObject go) {
        if(true){
            go.SetActive(false);
        }
    }

}