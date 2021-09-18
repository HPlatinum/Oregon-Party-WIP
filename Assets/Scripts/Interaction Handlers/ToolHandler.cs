using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolHandler : InteractionHandler
{
    public BladeInteraction blade;

    #region Inherited Functions
    public override void ProcessBladeHittingObject(ParticleSystem particleEffect) {
        particleEffect.Play();
        CountInteractSubjectHits();
        if(StaticVariables.interactScript.closestInteractable.hitsCurrentlyAppliedToResource == StaticVariables.interactScript.closestInteractable.hitsRequiredToGatherResource) {
            StaticVariables.interactScript.DestroyCurrentInteractable();
            StaticVariables.interactScript.AddCurrentInteractableItemToInventory();
        }
    }
    #endregion

    private void CountInteractSubjectHits() {
        StaticVariables.interactScript.closestInteractable.hitsCurrentlyAppliedToResource ++;
    }

    public virtual void AssignLocalVariables() {
        blade = StaticVariables.interactScript.objectInHand.transform.GetChild(0).GetComponent<BladeInteraction>();
    }

}
