using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StaticVariables
{
    //contains all the variables that need to be retained in between scenes

    static public InteractionManager interactScript;
    static public Invector.vCharacterController.vThirdPersonController controller;
    static public Animator playerAnimator;
    static public Essentials essentials;
    static public Text interactButtonText;
    static public Transform tweenDummy;
    static public MainUI mainUI;
    static public Inventory playerInventory;
    static public ItemDetails itemDetails;
    static public ToolResourceCollection toolResourceCollection;

    //interaction handlers
    static public InteractionHandler currentInteractionHandler;
    static public FishingHandler fishingHandler;
    static public CookingHandler cookingHandler;
    static public PickupHandler pickupHandler;

    static public void SetInteractButtonText(string newText) {
        interactButtonText.text = newText;
    }

    static public bool IsPlayerAnimatorInState(string stateName) {
        return playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
    
    static public bool DoesPlayerAnimatorStateHaveInteractTag() {
        return playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Interact");
    }

    static public void WaitTimeThenCallFunction(float delay, TweenCallback function) {
        tweenDummy.DOLocalMove(tweenDummy.transform.localPosition, delay, false).OnComplete(function);
    }

    static public void SetupPlayerInteractionWithHighlightedObject(float transitionDuration = 0.2f) {
        interactScript.SetupPlayerInteractionWithClosestInteractable(transitionDuration);
    }

    static public void PlayAnimation(string animationName, float transitionDuration = 0.2f) {
        playerAnimator.CrossFadeInFixedTime(animationName, transitionDuration);
    }
}