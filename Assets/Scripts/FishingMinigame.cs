using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FishingMinigame : Minigame {

    private int bobsRemainingUntilFishBites;
    private Transform fish1;
    private Transform fish2;
    private Transform fish3;
    private Vector3 fish1Origin;
    private Vector3 fish2Origin;
    private Vector3 fish3Origin;
    private Vector3 fish1Destination;
    private Vector3 fish2Destination;
    private Vector3 fish3Destination;
    public float moveInTime = 0.5f;
    public float moveOutTime = 0.5f;
    public float startDelay = 1f;
    public float availableCatchTime = 1f;
    [Range(1, 4)]
    public int minBobs = 2;
    [Range(5, 30)]
    public int maxBobs = 15;

    private bool fishHooked = false;
    private bool playerReeled = false;
    [HideInInspector]
    public bool playerGotFish = false;

    public bool showPopupWhenIdle = false;
    private ParticleSystem bobberSplash;
    private ParticleSystem bobberBigSplash;

    private enum FishMovement { fish1In, fish1Out, fish2In, fish2Out, fish3In, fish3Out};
    private FishMovement currentFishMovementStep = FishMovement.fish1In;



    void Start() {
        //hide the fishing minigame ui when the scene starts
        transform.Find("Fish Circle").gameObject.SetActive(false);
    }

    private void Update() {
        CheckIfUIShouldBeShown();
    }

    private void CheckIfUIShouldBeShown() {
        if (showPopupWhenIdle) {
            if (StaticVariables.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fishing - Idle")) {
                BeginFishing();
                showPopupWhenIdle = false;
            }
        }
    }


    public void BeginFishing() {
        //reset end condition values
        fishHooked = false;
        playerReeled = false;
        playerGotFish = false;

        transform.Find("Fish Circle").gameObject.SetActive(true);
        StaticVariables.SetInteractButtonText("Reel In");

        RandomlyChooseStartingBobAmount();
        RandomlyChooseFirstFish();

        //set up variables
        fish1 = transform.Find("Fish Circle").Find("Fish 1");
        fish1Origin = fish1.localPosition;
        fish1Destination = new Vector3(0, 90, fish1.localPosition.z);
        fish2 = transform.Find("Fish Circle").Find("Fish 2");
        fish2Origin = fish2.localPosition;
        fish2Destination = new Vector3(77.95229f, -45, fish2.localPosition.z);
        fish3 = transform.Find("Fish Circle").Find("Fish 3");
        fish3Origin = fish3.localPosition;
        fish3Destination = new Vector3(-77.95229f, -45, fish3.localPosition.z);
        bobberSplash = transform.Find("Fish Circle").Find("Bobber").Find("Splash").GetComponent<ParticleSystem>();
        bobberBigSplash = transform.Find("Fish Circle").Find("Bobber").Find("Big Splash").GetComponent<ParticleSystem>();
        
        //start a pointless tween to delay a function call
        //then start the fish-moving process
        bobberSplash.transform.DOLocalMove(bobberSplash.transform.localPosition, startDelay, false).OnComplete(StartFishMovement);
    }

    private void RandomlyChooseStartingBobAmount() {
        bobsRemainingUntilFishBites = new System.Random().Next(minBobs, maxBobs);
    }

    private void RandomlyChooseFirstFish() {
        int startingFishNum = new System.Random().Next(1, 4);
        if (startingFishNum == 1) 
            currentFishMovementStep = FishMovement.fish1In;
        if (startingFishNum == 2)
            currentFishMovementStep = FishMovement.fish2In;
        if (startingFishNum == 3)
            currentFishMovementStep = FishMovement.fish3In;
    }

    private void FishFinishedMoving() {
        if (playerReeled) 
            return;

        CountDownBobber();

        //if no bobs remain, the fish bites the rod
        if (bobsRemainingUntilFishBites == 0) {
            fishHooked = true;
            bobberBigSplash.Play();
            //start a pointless tween to delay a function call
            bobberSplash.transform.DOLocalMove(bobberSplash.transform.localPosition, availableCatchTime, false).OnComplete(EndMinigameFromNotReelingInTime);
            //return so no more fish move
            return;
        }

        PlayBobberSplashAnimationIfFishIsIn();
        DetermineNextFishMovement();
        StartFishMovement();
    }

    private void PlayBobberSplashAnimationIfFishIsIn() {
        if (currentFishMovementStep == FishMovement.fish1In)
            bobberSplash.Play();
        if (currentFishMovementStep == FishMovement.fish2In)
            bobberSplash.Play();
        if (currentFishMovementStep == FishMovement.fish3In)
            bobberSplash.Play();
    }

    private void CountDownBobber() {
        if (currentFishMovementStep == FishMovement.fish1In)
            bobsRemainingUntilFishBites--;
        if (currentFishMovementStep == FishMovement.fish2In)
            bobsRemainingUntilFishBites--;
        if (currentFishMovementStep == FishMovement.fish3In)
            bobsRemainingUntilFishBites--;
    }

    private void DetermineNextFishMovement() {
        if (currentFishMovementStep == FishMovement.fish1In)
            currentFishMovementStep = FishMovement.fish1Out;
        else if (currentFishMovementStep == FishMovement.fish1Out)
            currentFishMovementStep = FishMovement.fish2In;
        else if (currentFishMovementStep == FishMovement.fish2In)
            currentFishMovementStep = FishMovement.fish2Out;
        else if (currentFishMovementStep == FishMovement.fish2Out)
            currentFishMovementStep = FishMovement.fish3In;
        else if (currentFishMovementStep == FishMovement.fish3In)
            currentFishMovementStep = FishMovement.fish3Out;
        else if (currentFishMovementStep == FishMovement.fish3Out)
            currentFishMovementStep = FishMovement.fish1In;
    }

    private void StartFishMovement() {
        switch (currentFishMovementStep) {
            case (FishMovement.fish1In):
                fish1.DOLocalMove(fish1Destination, moveInTime, false).OnComplete(FishFinishedMoving);
                break;
            case (FishMovement.fish1Out):
                fish1.DOLocalMove(fish1Origin, moveInTime, false).OnComplete(FishFinishedMoving);
                break;
            case (FishMovement.fish2In):
                fish2.DOLocalMove(fish2Destination, moveInTime, false).OnComplete(FishFinishedMoving);
                break;
            case (FishMovement.fish2Out):
                fish2.DOLocalMove(fish2Origin, moveInTime, false).OnComplete(FishFinishedMoving);
                break;
            case (FishMovement.fish3In):
                fish3.DOLocalMove(fish3Destination, moveInTime, false).OnComplete(FishFinishedMoving);
                break;
            case (FishMovement.fish3Out):
                fish3.DOLocalMove(fish3Origin, moveInTime, false).OnComplete(FishFinishedMoving);
                break;
        }
    }

    private void EndMinigameFromNotReelingInTime() {
        //if the player does not reel in the fish in time, end the minigame
        if (!playerReeled) {
            fishHooked = false;
            StaticVariables.playerAnimator.CrossFadeInFixedTime("Shake Fist", 0.2f);
            EndFishingUI();
        }
    }

    public void ReelIn() {
        //handle player interaction during the minigame

        //set booleans for reference by other functions
        playerReeled = true;

        //catch a fish
        if (fishHooked) {
            playerGotFish = true;
            StaticVariables.playerAnimator.CrossFadeInFixedTime("Fishing - Reeling", 0.2f);
            EndFishingUI();
        }

        //reel in too early
        else {
            StaticVariables.playerAnimator.CrossFadeInFixedTime("Shake Fist", 0.2f);
            EndFishingUI();
        }

    }

    public void EndFishingUI() {
        //reset all the fish back to their starting positions and end the fishing minigame
        transform.Find("Fish Circle").gameObject.SetActive(false);
        fish1.localPosition = fish1Origin;
        fish2.localPosition = fish2Origin;
        fish3.localPosition = fish3Origin;


        StaticVariables.SetInteractButtonText("Interact");
    }



    //inherited functions

    public override void InteractAction() {
        
        //if you are in the idle state, reel in
        if (StaticVariables.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fishing - Idle")) {
            ReelIn();
            return;
        }
        //if you are not already interacting, start fishing
        else if (!StaticVariables.interactScript.currentlyInteracting) {
            //start the fishing animation
            StaticVariables.controller.StartAnimation("Fishing - Cast", 0.2f);

            //when the player hits the idle state, show the fishing UI popup
            showPopupWhenIdle = true;

            //set interactscript values
            StaticVariables.interactScript.currentlyInteracting = true;

            //put the fishing rod in the hand
            StaticVariables.interactScript.removeItemWhenFinished = true;
            if (StaticVariables.interactScript.itemInHand == StaticVariables.interactScript.fishingRod)
                StaticVariables.interactScript.removeItemWhenFinished = false; //if the player already has the fishing rod in their hand, do not remove it at the end of the fishing minigame
            StaticVariables.interactScript.PutObjectInHand(StaticVariables.interactScript.fishingRod, false);
        }
    }

    public override void EndInteractAnimation() {
        //pickup the fish, or don't
        if (playerGotFish) 
            StaticVariables.interactScript.Pickup();
        else 
            StaticVariables.interactScript.DestroyInteractable();

        //remove the fishing rod from the hand, if it wasn't in the player's hand before the minigame start
        if (StaticVariables.interactScript.removeItemWhenFinished)
            StaticVariables.interactScript.RemoveObjectFromHand();

        StaticVariables.currentMinigame = null;
    }


}
