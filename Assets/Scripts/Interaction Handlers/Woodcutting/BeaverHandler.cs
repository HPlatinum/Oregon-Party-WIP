using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaverHandler : InteractionHandler
{
    GameObject beaver;
    public GameObject woodLeft;
    GameObject woodClone;

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
        if(beaver.GetComponent<BeaverController>().hasWoodInHand) {
            woodClone = Instantiate(woodLeft, beaver.transform.position, beaver.transform.rotation);
            woodClone.GetComponent<Rigidbody>().AddForce(new Vector3(0,15, 0));
        }
        StaticVariables.woodcuttingHandler.SetBeaverHasBeenSpawnedBackToFalseAtSpawnLocation(beaver.GetComponent<BeaverController>().beaverSpawnInt);
        StaticVariables.interactScript.DestroyCurrentInteractable();
        StaticVariables.interactScript.RemoveItemFromHand();
        StaticVariables.currentInteractionHandler = null;
        StaticVariables.interactScript.closestInteractable = null;
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if(StaticVariables.woodcuttingHandler.gameIsStarted) {
            return true;
        }
        return false;
    }
    
    #endregion
}
