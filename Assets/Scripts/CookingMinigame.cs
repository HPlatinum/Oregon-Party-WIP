using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CookingMinigame : Minigame {

    private int cookingTier = 0;
    private CompactInventory compactInventory;

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

    private void AssignLocalVariables() {
        compactInventory = transform.Find("Compact Inventory").GetComponent<CompactInventory>();
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
        ItemType itemType = ItemType.Tool;
        Inventory inventory = StaticVariables.playerInventory;
        InventorySlotUI.OnClickEffect onClick = InventorySlotUI.OnClickEffect.CookingInterface;
        string inventoryTitle = "Raw Food";

        compactInventory.SetupValues(onClick, inventoryTitle);
        compactInventory.DisplayAllItemsOfTypeFromInventory(itemType, inventory);
    }

    public void SetTier(int tier) {
        cookingTier = tier;
    }

    public void ClickedRawFood(Item item, int quantity) {
        print("clicked " + item.name + ", quantity " + quantity);
    }

}
