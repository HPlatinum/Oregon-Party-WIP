using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FishingHandler : InteractionHandler {

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

    //interface elements to show/hide
    GameObject background;
    Transform uiParent;

    //misc
    private bool showFishingUIWhenAnimatorIsIdle = false;

    #region Inherited Functions

    public override void ProcessInteractAction() {
        if (!StaticVariables.interactScript.currentlyInteracting) {
            StaticVariables.SetupPlayerInteractionWithHighlightedObject();
            StaticVariables.PlayAnimation("Fishing - Cast");
            showFishingUIWhenAnimatorIsIdle = true;

            //set interactscript values
            StaticVariables.interactScript.currentlyInteracting = true;

            StaticVariables.interactScript.SetPreviousItemInHand();
            StaticVariables.interactScript.PutFirstToolOfTypeInHand(Tool.ToolTypes.rod);

        }
    }

    public override void ProcessInteractAnimationEnding() {
        if (playerGotFish)
            StaticVariables.interactScript.AddCurrentInteractableItemToInventory();

        StaticVariables.interactScript.PutPreviousItemBackInHand();

        StaticVariables.currentInteractionHandler = null;
        StaticVariables.interactScript.DestroyCurrentInteractable();
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if (StaticVariables.playerInventory.DoesInventoryContainToolWithType(Tool.ToolTypes.rod)) {
            if (StaticVariables.playerInventory.CanAddItemToInventory(interactable.item, 1)) {
                return true;
            }
        }
        return false;
    }

    #endregion

    void Start() {
        AssignLocalVariables();

        uiParent.gameObject.SetActive(false);
        background.SetActive(false);
    }

    private void Update() {
        if (ShouldFishingUIBeShown()) {
            StartCoroutine(BeginFishing());
            showFishingUIWhenAnimatorIsIdle = false;
        }
    }


    private bool ShouldFishingUIBeShown() {
        return (showFishingUIWhenAnimatorIsIdle && StaticVariables.IsPlayerAnimatorInState("Fishing - Idle"));
    }

    public IEnumerator BeginFishing() {
        ResetLocalVariables();
        yield return ShowFishingUI();
        RandomlyChooseStartingBobAmount();
        RandomlyChooseFirstFish();
        StaticVariables.WaitTimeThenCallFunction(timeBetweenUIOpeningAndMinigameStart, StartFishMovement);
    }

    private void AssignLocalVariables() {
        uiParent = transform.Find("UI");
        background = transform.Find("Background").gameObject;


        Transform fishCircle = uiParent.Find("Fish Circle");
        fish1 = fishCircle.Find("Fish 1");
        fish1Origin = fish1.localPosition;
        fish1Destination = new Vector3(0, 90, fish1.localPosition.z);
        fish2 = fishCircle.Find("Fish 2");
        fish2Origin = fish2.localPosition;
        fish2Destination = new Vector3(77.95229f, -45, fish2.localPosition.z);
        fish3 = fishCircle.Find("Fish 3");
        fish3Origin = fish3.localPosition;
        fish3Destination = new Vector3(-77.95229f, -45, fish3.localPosition.z);
        bobberSplash = fishCircle.Find("Bobber").Find("Splash").GetComponent<ParticleSystem>();
        bobberBigSplash = fishCircle.Find("Bobber").Find("Big Splash").GetComponent<ParticleSystem>();


    }

    private IEnumerator ShowFishingUI() {
        yield return StaticVariables.mainUI.HideUI2();
        uiParent.gameObject.SetActive(true);
        background.SetActive(true);
        yield return StaticVariables.AnimateChildObjectsAppearing(uiParent);

        yield return null;
    }

    private IEnumerator ReturnToMainUI() {
        yield return StaticVariables.AnimateChildObjectsDisappearing(uiParent);
        background.SetActive(false);
        yield return StaticVariables.mainUI.ShowUI2();
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
            StartCoroutine(ReturnToMainUI());
        }
    }

    public void ReelIn() {
        playerReeled = true;

        if (fishHooked) {
            playerGotFish = true;
            StaticVariables.PlayAnimation("Fishing - Reeling");
            StartCoroutine(ReturnToMainUI());
        }

        else { //reel in too early
            StaticVariables.PlayAnimation("Shake Fist");
            StartCoroutine(ReturnToMainUI());
        }
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
