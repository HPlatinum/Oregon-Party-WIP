using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticVariables
{
    //contains all the variables that need to be retained in between scenes

    static public Interact interactScript;
    static public Invector.vCharacterController.vThirdPersonController controller;
    static public Minigame currentMinigame;
    static public Animator playerAnimator;
    static public FishingMinigame fishingMinigame;
    static public Essentials essentials;

}