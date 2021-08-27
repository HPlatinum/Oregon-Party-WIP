using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FishingMinigame : Minigame {

    //fish and fish position references
    private Transform fish1;
    private Transform fish2;
    private Transform fish3;
    private Vector3 fish1Origin;
    private Vector3 fish2Origin;
    private Vector3 fish3Origin;
    private Vector3 fish1Destination;
    private Vector3 fish2Destination;
    private Vector3 fish3Destination;

    //which fish is currently moving, and in what direction
    private enum FishMovement { fish1In, fish1Out, fish2In, fish2Out, fish3In, fish3Out };
    private FishMovement currentFishMovementStep = FishMovement.fish1In;

    //timings
    public float moveInTime = 0.5f;
    public float moveOutTime = 0.5f;
    public float timeBetweenUIOpeningAndMinigameStart = 1f;
    public float availableCatchTime = 1f;

    //bobs before fish bites
    [Range(1, 4)]
    public int minBobs = 2;
    [Range(5, 30)]
    public int maxBobs = 15;
    private int bobsRemainingUntilFishBites;

    //minigame end conditions
    private bool fishHooked = false;
    private bool playerReeled = false;
    private bool playerGotFish = false;

    //bobbers to animate
    private ParticleSystem bobberSplash;
    private ParticleSystem bobberBigSplash;

    //misc
    private bool showFishingUIWhenAnimatorIsIdle = false;

    #region Inherited Functions

    public override void ProcessInteractAction() {
        if (StaticVariables.IsPlayerAnimatorInState("Fishing - Idle")) {
            ReelIn();
            return;
        }
        else if (!StaticVariables.interactScript.currentlyInteracting) {
            StaticVariables.SetupPlayerInteractionWithHighlightedObject();
            StaticVariables.PlayAnimation("Fishing - Cast");
            showFishingUIWhenAnimatorIsIdle = true;

            //set interactscript values
            StaticVariables.interactScript.currentlyInteracting = true;
            //StaticVariables.interactScript.isInteracting = true;

            //put the fishing rod in the hand
            StaticVariables.interactScript.removeItemWhenFinishedWithInteraction = true;
            if (StaticVariables.interactScript.itemInHand == StaticVariables.interactScript.fishingRod)
                StaticVariables.interactScript.removeItemWhenFinishedWithInteraction = false; //if the player already has the fishing rod in their hand, do not remove it at the end of the fishing minigame
            StaticVariables.interactScript.PutItemInPlayerHand(StaticVariables.interactScript.fishingRod, false);
        }
    }

    public override void ProcessInteractAnimationEnding() {
        if (playerGotFish)
            StaticVariables.interactScript.AddCurrentInteractableItemToInventory();

        if (StaticVariables.interactScript.removeItemWhenFinishedWithInteraction)
            StaticVariables.interactScript.RemoveItemFromHand();

        StaticVariables.currentMinigame = null;
        StaticVariables.interactScript.DestroyCurrentInteractable();
    }

    #endregion

    void Start() {
        AssignLocalVariables();
        HideFishingUI();
    }

    private void Update() {
        if (ShouldFishingUIBeShown()) {
            BeginFishing();
            showFishingUIWhenAnimatorIsIdle = false;
        }
    }


    private bool ShouldFishingUIBeShown() {
        return (showFishingUIWhenAnimatorIsIdle && StaticVariables.IsPlayerAnimatorInState("Fishing - Idle"));
    }

    public void BeginFishing() {
        ResetLocalVariables();
        ShowFishingUI();
        StaticVariables.SetInteractButtonText("Reel In");
        RandomlyChooseStartingBobAmount();
        RandomlyChooseFirstFish();
        StaticVariables.WaitTimeThenCallFunction(timeBetweenUIOpeningAndMinigameStart, StartFishMovement);
    }

    private void AssignLocalVariables() {
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
    }

    private void ShowFishingUI() {
        transform.Find("Fish Circle").gameObject.SetActive(true);
    }

    private void HideFishingUI() {
        transform.Find("Fish Circle").gameObject.SetActive(false);
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
            return; //return so no more fish move

        CountDownBobber();

        if (bobsRemainingUntilFishBites == 0) {
            FishBitesRod();
            return; //return so no more fish move
        }

        PlayBobberSplashAnimationIfFishIsIn();
        DetermineNextFishMovement();
        StartFishMovement();
    }


    private void FishBitesRod() {
        fishHooked = true;
        bobberBigSplash.Play();
        StaticVariables.WaitTimeThenCallFunction(availableCatchTime, EndMinigameFromNotReelingInTime);
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
        if (!playerReeled) {
            fishHooked = false;
            StaticVariables.PlayAnimation("Shake Fist");
            CloseFishingUI();
        }
    }

    public void ReelIn() {
        playerReeled = true;

        if (fishHooked) {
            playerGotFish = true;
            StaticVariables.PlayAnimation("Fishing - Reeling");
            CloseFishingUI();
        }

        else { //reel in too early
            StaticVariables.PlayAnimation("Shake Fist");
            CloseFishingUI();
        }
    }

    public void CloseFishingUI() {
        HideFishingUI();
        StaticVariables.SetInteractButtonText("Interact");
    }

    private void ResetLocalVariables() {
        //set some values to what they were at the start of the minigame, to prep for next time
        fish1.localPosition = fish1Origin;
        fish2.localPosition = fish2Origin;
        fish3.localPosition = fish3Origin;
        fishHooked = false;
        playerReeled = false;
        playerGotFish = false;
    }
}
