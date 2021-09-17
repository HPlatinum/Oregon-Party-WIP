using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PickupHandler : InteractionHandler {
    
    #region Inherited Functions

    public override void ProcessInteractAction() {
        StaticVariables.SetupPlayerInteractionWithHighlightedObject();
        StaticVariables.PlayAnimation("Lifting");
    }

    public override void ProcessInteractAnimationEnding() {
        StaticVariables.interactScript.AddCurrentInteractableItemToInventory();
        StaticVariables.currentInteractionHandler = null;
        StaticVariables.interactScript.DestroyCurrentInteractable();
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        return StaticVariables.playerInventory.CanAddItemToInventory(interactable.item, 1);
    }
    
    #endregion

}
