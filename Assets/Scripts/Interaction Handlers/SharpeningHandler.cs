using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpeningHandler : InteractionHandler
{
     #region Inherited Functions

    public override void ProcessInteractAction() {
        if (!StaticVariables.interactScript.currentlyInteracting) {
            if(StaticVariables.sceneHandler.GetSceneName() != "Woodcutting Minigame") {
                StaticVariables.interactScript.SetPreviousItemInHand();
                StaticVariables.interactScript.PutFirstToolOfTypeInHand(Tool.ToolTypes.axe);
                StaticVariables.SetupPlayerInteractionWithHighlightedObject();
                StaticVariables.PlayAnimation("Swing Axe", 1);
            }
            else {
                StaticVariables.interactScript.SetPreviousItemInHand();
                StaticVariables.interactScript.PutFirstToolOfTypeInHand(Tool.ToolTypes.axe);
                StaticVariables.SetupPlayerInteractionWithHighlightedObject();
            }
        }
        
    }

    public override void ProcessInteractAnimationEnding() {
        StaticVariables.currentInteractionHandler = null;
        StaticVariables.interactScript.PutPreviousItemBackInHand();
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        return true;
    }
    #endregion

}