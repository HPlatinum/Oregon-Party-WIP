using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    static public void SetInteractButtonText(string newText) {
        interactButtonText.text = newText;
    }

}