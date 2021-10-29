using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningHandler : ToolHandler
{
    //metal and metal position references
    private Transform collectable1;
    private Transform collectable2;
    private Transform collectable3;
    private Transform collectable4;
    private Transform collectable5;
    private Transform collectable6;
    private Vector3 metal1Pos;
    private Vector3 metal2Pos;
    private Vector3 metal3Pos;
    private Vector3 metal4Pos;
    private Vector3 metal5Pos;
    private Vector3 metal6Pos;
    
    public Item collectableObject1;
    public Item collectableObject2;
    public Item collectableObject3;
    public Item obstructionObject1;
    public Item obstructionObject2;
    public Item obstructionObject3;
    public Item obstructionObject4;
    public GameObject collectableObject4;
    public GameObject collectableObject5;
    public GameObject collectableObject6;

    private int randomlyChosenObstructionObject;
    private DisplayItem displayItem;

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
            collectableObject4.SetActive(false);
            collectableObject5.SetActive(false);
            collectableObject6.SetActive(false);
        }
        if(StaticVariables.interactScript.GetToolTier() == 2){
            collectableObject6.SetActive(false);
        }
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

    public void PositionMiningCollectables() {
        System.Random rand = new System.Random();
        while(Vector3.Distance(metal1Pos, metal2Pos) <= 150 || Vector3.Distance(metal3Pos, metal1Pos) <= 150 || Vector3.Distance(metal3Pos, metal2Pos) <= 150) {
            metal1Pos = new Vector3 (rand.Next(0,400), rand.Next(0,350), 0);
            metal2Pos = new Vector3 (rand.Next(0,400), rand.Next(0,350), 0);
            metal3Pos = new Vector3 (rand.Next(0,400), rand.Next(0,350), 0);
            metal4Pos = new Vector3 (rand.Next(0,400), rand.Next(0,350), 0);
            metal5Pos = new Vector3 (rand.Next(0,400), rand.Next(0,350), 0);
            metal6Pos = new Vector3 (rand.Next(0,400), rand.Next(0,350), 0);
        }
        collectable1.localPosition = metal1Pos;
        collectable2.localPosition = metal2Pos;
        collectable3.localPosition = metal3Pos;
        collectable4.localPosition = metal4Pos;
        collectable5.localPosition = metal5Pos;
        collectable6.localPosition = metal6Pos;
    }

    public void DestroyMineableLayer(GameObject go) {
        if(true){
            go.SetActive(false);
        }
    }
}