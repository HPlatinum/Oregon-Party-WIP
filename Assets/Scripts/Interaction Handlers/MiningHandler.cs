using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningHandler : ToolHandler
{
    #region Inherited Functions

    public override void ProcessInteractAction() {
        // //put the fishing rod in the hand
        StaticVariables.interactScript.removeItemWhenFinishedWithInteraction = true;
        if (StaticVariables.interactScript.itemInHand == StaticVariables.interactScript.pickaxe)
            StaticVariables.interactScript.removeItemWhenFinishedWithInteraction = false; //if the player already has the fishing rod in their hand, do not remove it at the end of the fishing minigame
        StaticVariables.interactScript.PutItemInPlayerHand(StaticVariables.interactScript.pickaxe, StaticVariables.interactScript.pickaxe.useRightHand);

        AssignLocalVariables();
        StaticVariables.SetupPlayerInteractionWithHighlightedObject();
        StaticVariables.PlayAnimation("Swing Pickaxe", 1);
        StaticVariables.WaitTimeThenCallFunction(.6f, blade.EnableBlade);
    }

    public override void ProcessInteractAnimationEnding() {
        StaticVariables.currentInteractionHandler = null;
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if (StaticVariables.playerInventory.DoesInventoryContainToolWithType(Tool.ToolTypes.pickaxe)) {
            if (StaticVariables.playerInventory.CanAddItemToInventory(interactable.item, 1)) {
                return true;
            }
        }
        return false;
    }

    #endregion


}