using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepositHandler : InteractionHandler
{
    public bool gameIsStarted;
    public bool gameIsOver;
    public Item depositItem;
    public GameObject depositObject;
    public Inventory depositInventory;
    #region Inherited Functions

    public override void ProcessInteractAction() {
        if (!StaticVariables.interactScript.currentlyInteracting) {
            StaticVariables.SetupPlayerInteractionWithHighlightedObject();
            StaticVariables.PlayAnimation("Sharpening Axe", 1);
            ActivePlayerIsDeposittingWood();
        }
    }

    public override void ProcessInteractAnimationEnding() {
        StaticVariables.currentInteractionHandler = null;
        StaticVariables.interactScript.PutPreviousItemBackInHand();
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if(StaticVariables.interactScript.itemInHand != null) {
            if(StaticVariables.interactScript.itemInHand.name == "Wood") {
                return true;
            }
        }
        return false;
    }
    #endregion


    public void Start() {
        gameIsStarted = false;
        gameIsOver = false;
    }
    public void Update() {
        if(StaticVariables.woodcuttingHandler.inWoodcuttingScene) {
            if(!gameIsStarted) {
                ResetWoodcuttingLocalVariables();
            }
            if(gameIsOver) {
                depositInventory.ClearInventory();
            }
        }
    }
    
    private void ActivePlayerIsDeposittingWood() {
        // ya ya player deposited wood, ding dong
        StaticVariables.interactScript.closestInteractable.inventory.AddItemToInventory(StaticVariables.interactScript.itemInHand, 1);
        StaticVariables.controller.Carry();
    }

    public void ResetWoodcuttingLocalVariables() {
        depositObject = GameObject.FindGameObjectWithTag("Interact (Depositing)");
        depositInventory = depositObject.GetComponent<Interactable>().inventory;
        depositItem = depositObject.GetComponent<Interactable>().item;
        gameIsStarted = true;
        gameIsOver = false;
    }

    public int GetQuantityOfWoodCollected() {
        return depositInventory.GetTotalItemQuantity(depositObject.GetComponent<Interactable>().item);
    }
}