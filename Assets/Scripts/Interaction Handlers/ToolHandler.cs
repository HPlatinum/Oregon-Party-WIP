using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolHandler : InteractionHandler
{
    public BladeInteraction blade;
    
    public override void ProcessBladeHittingObject(ParticleSystem particleEffect) {
        particleEffect.Play();
        CountInteractSubjectHits();
        if(StaticVariables.interactScript.closestInteractable.hitsCurrentlyAppliedToResource == StaticVariables.interactScript.closestInteractable.hitsRequiredToGatherResource) {
            StaticVariables.interactScript.AddCurrentInteractableItemToInventory();
            StaticVariables.interactScript.DestroyCurrentInteractable();
        }
    }

    private void CountInteractSubjectHits() {
        StaticVariables.interactScript.closestInteractable.hitsCurrentlyAppliedToResource ++;
    }

    public virtual void AssignLocalVariables() {
        blade = StaticVariables.interactScript.objectInHand.transform.GetChild(0).GetComponent<BladeInteraction>();
    }

}
