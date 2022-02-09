using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodcuttingHandler : ToolHandler
{
    Transform uiParent;
    Transform finishScreen;
    Transform messageUI;
    Transform timerUI;

    #region Inherited Functions

    public override void ProcessInteractAction() {
        if (!StaticVariables.interactScript.currentlyInteracting) {
            StaticVariables.interactScript.SetPreviousItemInHand();
            StaticVariables.interactScript.PutFirstToolOfTypeInHand(Tool.ToolTypes.axe);
            StaticVariables.SetupPlayerInteractionWithHighlightedObject();
            StaticVariables.interactScript.currentlyInteracting = true;
            StaticVariables.PlayAnimation("Swing Axe", 1);
            FindBlade();
            StaticVariables.WaitTimeThenCallFunction(.6f, blade.EnableBlade);
        }
    }

    public override void ProcessInteractAnimationEnding() {
        StaticVariables.currentInteractionHandler = null;
        StaticVariables.interactScript.PutPreviousItemBackInHand();
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if (StaticVariables.playerInventory.DoesInventoryContainToolWithType(Tool.ToolTypes.axe)) {
            if (StaticVariables.playerInventory.CanAddItemToInventory(interactable.item, 1)) {
                return true;
            }
        }
        return false;
    }

    public override void AssignLocalVariables() {
        uiParent = transform.Find("Woodcutting Game Parent");
        finishScreen = transform.Find("Finish Screen");
        messageUI = transform.Find("Message UI");
        timerUI = uiParent.Find("Timer");
    }
    #endregion

    public void Start() {
        AssignLocalVariables();
    }

    private void FindBlade() {
        blade = StaticVariables.interactScript.objectInHand.transform.GetChild(0).GetComponent<BladeInteraction>();
    }

    public IEnumerator ShowTimer() {
        yield return StaticVariables.AnimateChildObjectsAppearing(timerUI);
    }

    
}