using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpeningHandler : InteractionHandler
{
    public bool playerIsSharpening;
    public Timer timeForReward;
    #region Inherited Functions

    public override void ProcessInteractAction() {
        if (!StaticVariables.interactScript.currentlyInteracting) {
            StaticVariables.interactScript.SetPreviousItemInHand();
            StaticVariables.interactScript.PutFirstToolOfTypeInHand(Tool.ToolTypes.axe);
            StaticVariables.SetupPlayerInteractionWithHighlightedObject();
            StaticVariables.PlayAnimation("Sharpening Axe Idle Loop", 1);
            ActivePlayerSharpeningAxe();
        }
    }

    public override void ProcessInteractAnimationEnding() {
        StaticVariables.currentInteractionHandler = null;
        StaticVariables.interactScript.PutPreviousItemBackInHand();
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if(StaticVariables.woodcuttingHandler.toolStats != null) {
            if(StaticVariables.woodcuttingHandler.toolStats.wear < 100) {
                return true;
            }
        }
        return false;
    }
    #endregion

    private void Start() {
        playerIsSharpening = false;
    }
    private void Update() {
        if(PlayerStartedSharpening()) {
            if(Mathf.Round(timeForReward.GetTimeForChangingDisplayColor()) == 1) {
                StaticVariables.PlayAnimation("Sharpening Axe", 0);
                playerIsSharpening = false;
            }
        }

        if(timeForReward != null) {
            if(timeForReward.TimerWasStartedAndIsNowStopped()) {
                timeForReward.SetTimerEndedBackToFalse();
                StaticVariables.woodcuttingHandler.toolStats.RepairTool();
            }
        }
        
    }

    private bool PlayerStartedSharpening() {
        return playerIsSharpening;
    }

    private void ActivePlayerSharpeningAxe() {
        playerIsSharpening = true;
        timeForReward = StaticVariables.interactScript.GetClosestInteractable().GetComponentInChildren<Timer>();
        if(!timeForReward.TimerIsRunning()) {
            timeForReward.StartGameTimer(7f);
        }
    }

}