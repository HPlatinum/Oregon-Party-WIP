using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CookingMinigame : Minigame {

    private int cookingTier = 0;
    public GameObject inventorySpacePrefab;
    private Transform inventoryParent;

    #region Inherited Functions

    public override void ProcessInteractAction() {

    }

    public override void ProcessInteractAnimationEnding() {

    }

    #endregion

    void Start() {
        AssignLocalVariables();
        HideUI();
    }

    private void Update() {

    }


    private void AssignLocalVariables() {
        inventoryParent = transform.Find("Inventory").Find("ItemsParent");
    }

    public void ShowUI() {
        foreach (Transform t in transform)
            t.gameObject.SetActive(true);
        DisplayRawFoodFromInventory();
    }

    public void HideUI() {
        foreach (Transform t in transform) {
            t.gameObject.SetActive(false);
        }
    }

    private void DisplayRawFoodFromInventory() {
        List<(Item, int)> rawFood = StaticVariables.playerInventory.GetListOfItemsWithType(ItemType.Tool);
        PrintTupleListContents(rawFood);

        foreach ((Item, int) tuple in rawFood) {
            Item item = tuple.Item1;
            int quantity = tuple.Item2;

            //instatntiate the inventory space prefab
            GameObject newObj = Instantiate(inventorySpacePrefab);
            newObj.transform.SetParent(inventoryParent);
            newObj.transform.localScale = new Vector3(1, 1, 1);
            //newObj.transform.localPosition = Vector3.zero;

            //set image quantity for inventory space prefab
            InventorySlotUI slot = newObj.GetComponent<InventorySlotUI>();
            slot.Setup();
            slot.AddItemToInventorySlot(item);
            slot.DisplayItemQuantity(quantity);

            //set click interaction
            slot.clickEffect = InventorySlotUI.OnClickEffect.CookingInterface;
        }

    }

    public void SetTier(int tier) {
        cookingTier = tier;
    }

    private void PrintTupleListContents(List<(Item, int)> list) {
        foreach ((Item, int) tuple in list) {
            print("Raw Food item found: " + tuple.Item1 + ", with quantity " + tuple.Item2);
        }

        if (list.Count == 0) {
            print("No Raw Food found in inventory");
        }
    }

    public void ClickedRawFood(Item item, int quantity) {

    }

}
