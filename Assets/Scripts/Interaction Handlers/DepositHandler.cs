using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepositHandler : InteractionHandler
{
    #region Inherited Functions

    public override void ProcessInteractAction() {
        if (!StaticVariables.interactScript.currentlyInteracting) {
            StaticVariables.interactScript.SetPreviousItemInHand();
            StaticVariables.interactScript.PutFirstToolOfTypeInHand(Tool.ToolTypes.axe);
            StaticVariables.SetupPlayerInteractionWithHighlightedObject();
            StaticVariables.PlayAnimation("Sharpening Axe Idle Loop", 1);
            ActivePlayerIsDeposittingWood();
        }
    }

    public override void ProcessInteractAnimationEnding() {
        StaticVariables.currentInteractionHandler = null;
        StaticVariables.interactScript.PutPreviousItemBackInHand();
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if(StaticVariables.interactScript.itemInHand.name == "Wood") {
            return true;
        }
        return false;
    }
    #endregion

    private void Start() {
        
    }
    private void Update() {

    }

    private void ActivePlayerIsDeposittingWood() {
        // ya ya player deposited wood, ding dong
    }

}