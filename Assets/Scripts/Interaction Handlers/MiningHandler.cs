using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningHandler : ToolResourceCollectionHandler
{
    #region Inherited Functions

    public override void ProcessInteractAction() {
        StaticVariables.SetupPlayerInteractionWithHighlightedObject();
        StaticVariables.PlayAnimation("Swing Pickaxe", 1);
        StaticVariables.WaitTimeThenCallFunction(.6f,StaticVariables.toolResourceCollection.EnableBlade);
    }

    public override void ProcessInteractAnimationEnding() {
        StaticVariables.currentInteractionHandler = null;
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if (StaticVariables.playerInventory.GetQuantityOfSpecificItem(interactable.requiredItem) > 0)
            return StaticVariables.playerInventory.CanAddItemToInventory(interactable.item, 1);
        else
            return false;
    }


    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
