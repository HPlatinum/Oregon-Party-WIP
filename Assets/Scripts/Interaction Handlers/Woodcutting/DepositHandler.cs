using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepositHandler : InteractionHandler
{
    int currentObjectCount;
    public bool gameIsStarted;
    public bool gameIsOver;
    public Item depositItem;
    public GameObject depositObject;
    public Inventory depositInventory;
    List<GameObject> depositWoodPile;
    #region Inherited Functions

    public override void ProcessInteractAction() {
        if (!StaticVariables.interactScript.currentlyInteracting) {
            StaticVariables.SetupPlayerInteractionWithHighlightedObject();
            StaticVariables.PlayAnimation("Deposit Wood", .2f);
            ActivePlayerIsDeposittingWood();
        }
    }

    public override void ProcessInteractAnimationEnding() {
        StaticVariables.currentInteractionHandler = null;
        StaticVariables.interactScript.PutPreviousItemBackInHand();
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if(StaticVariables.interactScript.itemInHand != null && StaticVariables.woodcuttingHandler.gameIsStarted) {
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
        if(StaticVariables.woodcuttingHandler.gameIsStarted) {
            if(!gameIsStarted) {
                ResetLocalVariables();
            }
            if(gameIsStarted) {
                if(currentObjectCount != GetQuantityOfWoodCollected()) {
                    currentObjectCount = GetQuantityOfWoodCollected();
                    SetWoodGameObjectsActive();
                }
            }
        }
        if(gameIsOver) {
            depositInventory.ClearInventory();
            currentObjectCount = 0;
            SetWoodGameObjectsActive();
            gameIsOver = false;
        }
    }
    
    private void ActivePlayerIsDeposittingWood() {
        // ya ya player deposited wood, ding dong
        StaticVariables.interactScript.closestInteractable.inventory.AddItemToInventory(StaticVariables.interactScript.itemInHand, 2);
        StaticVariables.controller.Carry();
    }

    public void ResetLocalVariables() {
        depositObject = StaticVariables.woodcuttingHandler.storageArea;
        depositInventory = depositObject.GetComponent<Interactable>().inventory;
        depositItem = depositObject.GetComponent<Interactable>().item;
        currentObjectCount = GetQuantityOfWoodCollected();
        gameIsStarted = true;
        gameIsOver = false;
        FillListOfWoodGameObjects();
        SetWoodGameObjectsActive();
    }



    public int GetQuantityOfWoodCollected() {
        return depositInventory.GetQuantityOfSpecificItem(depositItem);
    }

    public void FillListOfWoodGameObjects() {
        depositWoodPile = StaticVariables.woodcuttingHandler.depositWoodPile;
    }


    private void SetWoodGameObjectsActive() {
        for(int i = 0; i < 36; i++) {
            if(currentObjectCount >= i && !depositWoodPile[i].activeSelf) {
                depositWoodPile[i].SetActive(true);
            }
            if(currentObjectCount <= i && depositWoodPile[i].activeSelf) {
                depositWoodPile[i].SetActive(false);
            }
        }
    }
}