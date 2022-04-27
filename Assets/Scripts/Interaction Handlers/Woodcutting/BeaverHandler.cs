using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaverHandler : InteractionHandler
{
    public override void ProcessInteractAction() {
        StaticVariables.PlayAnimation("Swing Pickaxe");       
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if(StaticVariables.woodcuttingHandler.gameIsStarted) {
            return true;
        }
        return false;
    }
}
