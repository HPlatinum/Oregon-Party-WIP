using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CookingMinigame : Minigame {

    private int cookingTier = 0;

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
        List<(Item, int)> rawFood = StaticVariables.playerInventory.GetListOfItemsWithType(ItemType.RawFood);
        PrintTupleListContents(rawFood);
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

}
