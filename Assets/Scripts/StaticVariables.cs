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
    static public SceneHandler sceneHandler;
    static public Invector.vCharacterController.vThirdPersonController controller;
    static public Animator playerAnimator;
    static public Essentials essentials;
    static public Text interactButtonText;
    static public Transform tweenDummy;
    static public MainUI mainUI;
    static public Inventory playerInventory;
    static public ItemDetails itemDetails;
    static public ToolHandler ToolHandler;
    static public CommonUI commonUI;

    //interaction handlers
    static public InteractionHandler currentInteractionHandler;
    static public FishingHandler fishingHandler;
    static public CookingHandler cookingHandler;
    static public PickupHandler pickupHandler;
    static public WoodcuttingHandler woodcuttingHandler;
    static public MiningHandler miningHandler;
    static public ForgeHandler forgeHandler;
    static public SharpeningHandler sharpeningHandler;
    static public Timer timer;

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

    static public IEnumerator AnimateChildObjectsAppearing(Transform transform) {
        if (transform.childCount == 0)
            yield break;

        foreach (Transform t in transform) {
            t.gameObject.AddComponent<AnimatedUIAppearing>();
            t.gameObject.SetActive(true);
        }

        //delay the animation start for each child object
        foreach (Transform t in transform) {
            yield return new WaitForSeconds(0.1f);
            t.gameObject.GetComponent<AnimatedUIAppearing>().StartGrowth();
        }

        //wait until the final child has finished animating
        yield return new WaitForSeconds(transform.GetChild(transform.childCount - 1).GetComponent<AnimatedUIAppearing>().GetTotalAnimationTime());
        yield return null;
    }

    static public IEnumerator AnimateChildObjectsDisappearing(Transform transform, bool SetParentInactiveAfterwards=false) {
        if (transform.childCount == 0)
            yield break;

        foreach (Transform t in transform)
            t.gameObject.AddComponent<AnimatedUIRemoval>();

        //delay the animation start for each child object
        foreach (Transform t in transform) {
            yield return new WaitForSeconds(0.1f);
            t.gameObject.GetComponent<AnimatedUIRemoval>().StartSquash();
        }

        //wait until the final child has finished animating
        yield return new WaitForSeconds(transform.GetChild(transform.childCount - 1).GetComponent<AnimatedUIRemoval>().GetTotalAnimationTime());

        if (SetParentInactiveAfterwards)
            transform.gameObject.SetActive(false);
        yield return null;
    }
}