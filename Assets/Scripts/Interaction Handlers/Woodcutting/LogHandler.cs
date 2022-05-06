using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogHandler : InteractionHandler {
    #region Overrides

    public override void ProcessInteractAction() {
        if (!StaticVariables.interactScript.currentlyInteracting) {
            StaticVariables.interactScript.PutItemInPlayerHand(StaticVariables.interactScript.GetClosestInteractable().GetItem()); 
            StaticVariables.interactScript.DestroyCurrentInteractable();
            StaticVariables.woodcuttingHandler.woodInHand = 2;
        }
    }
    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if(StaticVariables.interactScript.itemInHand == null && StaticVariables.woodcuttingHandler.log == null && StaticVariables.woodcuttingHandler.gameIsStarted) {
            return true;
        }
        return false;
    }

    #endregion

    
}