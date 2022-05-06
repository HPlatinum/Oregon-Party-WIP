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
            beaver.GetComponentInChildren<NPCInteractionManager>().enabled = false;
            beaver.GetComponentInChildren<BeaverController>().MoveBeaver();
            beaver.GetComponentInChildren<BeaverController>().FreezeBeaver();
            beaver.GetComponent<Outline>().enabled = false;
            StaticVariables.interactScript.PutFirstToolOfTypeInHand(Tool.ToolTypes.axe);
            StaticVariables.interactScript.SetupPlayerInteractionWithClosestInteractable(.1f);
            StaticVariables.PlayAnimation("Swing Pickaxe");
            StaticVariables.WaitTimeThenCallFunction(1.55f, beaver.GetComponentInChildren<ParticleSystem>().Play);
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
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if(StaticVariables.woodcuttingHandler.gameIsStarted) {
            return true;
        }
        return false;
    }
    
    #endregion
}
