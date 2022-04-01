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
            StaticVariables.PlayAnimation("Deposit Wood", 1);
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
            if(gameIsStarted) {
                if(currentObjectCount != GetQuantityOfWoodCollected()) {
                    currentObjectCount = GetQuantityOfWoodCollected();
                    SetWoodGameObjectsActive();
                }
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
        currentObjectCount = GetQuantityOfWoodCollected();
        print("Current Object Count" + currentObjectCount);
        gameIsStarted = true;
        gameIsOver = false;
        CreateListOfWoodGameObjects();
        FillListOfWoodGameObjects();
        SetWoodGameObjectsActive();
    }



    public int GetQuantityOfWoodCollected() {
        return depositInventory.GetQuantityOfSpecificItem(depositItem);
    }

    private void CreateListOfWoodGameObjects() {
        depositWoodPile = new List<GameObject>();
    }

    private void FillListOfWoodGameObjects() {
        GameObject newObj;
        for(int i = 0; i < 18; i++) {
            if(i == 0) {
                newObj = depositObject.transform.Find("Wood Slot").gameObject;
            }
            else {
                newObj = depositObject.transform.Find("Wood Slot (" + i + ")").gameObject;
            }
            depositWoodPile.Add(newObj);
        }
    }

    private void SetWoodGameObjectsActive() {
        for(int i = 0; i < 18; i++) {
            if(currentObjectCount >= i && !depositWoodPile[i].activeSelf) {
                depositWoodPile[i].SetActive(true);
            }
            if(currentObjectCount <= i && depositWoodPile[i].activeSelf) {
                depositWoodPile[i].SetActive(false);
            }
        }
    }
}