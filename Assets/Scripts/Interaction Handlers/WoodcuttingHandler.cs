using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodcuttingHandler : ToolHandler
{
    #region Inherited Functions

    public override void ProcessInteractAction() {
        // //put the fishing rod in the hand
        StaticVariables.interactScript.SetPreviousItemInHand();
        StaticVariables.interactScript.PutFirstToolOfTypeInHand(Tool.ToolTypes.pickaxe);

        AssignLocalVariables();
        StaticVariables.SetupPlayerInteractionWithHighlightedObject();
        StaticVariables.PlayAnimation("Swing Axe", 1);
        StaticVariables.WaitTimeThenCallFunction(.6f, blade.EnableBlade);
    }

    public override void ProcessInteractAnimationEnding() {
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if (StaticVariables.playerInventory.DoesInventoryContainToolWithType(Tool.ToolTypes.axe)) {
            if (StaticVariables.playerInventory.CanAddItemToInventory(interactable.item, 1)) {
                return true;
            }
        }
        return false;
    }

    #endregion
}