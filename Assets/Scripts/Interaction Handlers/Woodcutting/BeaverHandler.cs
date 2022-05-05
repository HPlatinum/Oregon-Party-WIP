using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaverHandler : InteractionHandler
{
    GameObject beaver;
    #region  overrides
    public override void ProcessInteractAction() {
        if(!StaticVariables.interactScript.currentlyInteracting) {
            StaticVariables.interactScript.currentlyInteracting = true;
            beaver = StaticVariables.interactScript.GetClosestInteractable().gameObject;
            StaticVariables.interactScript.PutFirstToolOfTypeInHand(Tool.ToolTypes.axe);
            StaticVariables.PlayAnimation("Swing Pickaxe");
        }
        
    }
    public override void ProcessInteractAnimationEnding() {
        // not working for some reason
        StaticVariables.interactScript.RemoveItemFromHand();
        print("The currently interacting variable is " + StaticVariables.interactScript.currentlyInteracting);
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if(StaticVariables.woodcuttingHandler.gameIsStarted && StaticVariables.interactScript.itemInHand == null) {
            return true;
        }
        return false;
    }
    
    #endregion
}
