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
    static public Minigame currentMinigame;
    static public Animator playerAnimator;
    static public FishingMinigame fishingMinigame;
    static public Essentials essentials;
    static public Text interactButtonText;
    static public Transform tweenDummy;
    static public MainUI mainUI;
    static public Inventory playerInventory;
    static public CookingMinigame cookingMinigame;
    static public ItemDetails itemDetails;
    static public Woodcutting woodcutting;

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

    /*
    static public void DisplayItemInsideParentRectTransform(Item item, RectTransform parent) {
        //add the object 3d model
        //create the 3d model instance and position it correctly
        GameObject newModel = GameObject.Instantiate(item.model, parent);
        newModel.transform.localPosition = Vector3.zero;
        itemDetails.SetLayerRecursively(newModel, 5); //assumes UI layer is #5
        newModel.transform.localScale = newModel.transform.localScale * item.modelScale;
        newModel.transform.Rotate(item.modelRotation);
        //set the position of the 3d model. position offset is scaled down to 20% of the offset used in the item details screen
        //parent.localPosition = new Vector3(originalModelParentPos.x + (item.modelPosition.x * .2f), originalModelParentPos.y + (item.modelPosition.y * .2f), itemModelParent.localPosition.z);
    }
    */
}