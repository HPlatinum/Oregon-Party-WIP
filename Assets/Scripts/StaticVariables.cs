using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StaticVariables
{
    //contains all the variables that need to be retained in between scenes

    static public Interact interactScript;
    static public Invector.vCharacterController.vThirdPersonController controller;
    static public Minigame currentMinigame;
    static public Animator playerAnimator;
    static public FishingMinigame fishingMinigame;
    static public Essentials essentials;
    static public Text interactButtonText;
    static public Transform tweenDummy;

    static public void SetInteractButtonText(string newText) {
        interactButtonText.text = newText;
    }

    static public bool IsPlayerInInteractState(string stateName) {
        return playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    static public void WaitTimeThenCallFunction(float delay, TweenCallback function) {
        tweenDummy.DOLocalMove(tweenDummy.transform.localPosition, delay, false).OnComplete(function);
    }

    static public void SetupPlayerInteractionWithHighlightedObject(float transitionDuration = 0.2f) {
        controller.SetupPlayerInteractionWithHighlightedObject(transitionDuration);
    }

    static public void PlayAnimation(string animationName, float transitionDuration = 0.2f) {
        playerAnimator.CrossFadeInFixedTime(animationName, transitionDuration);
    }
}