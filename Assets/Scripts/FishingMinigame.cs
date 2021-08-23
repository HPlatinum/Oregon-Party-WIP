using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FishingMinigame : Minigame {

    private int bobsRemaining;
    private Transform fish1;
    private Transform fish2;
    private Transform fish3;
    private Vector3 fish1Origin;
    private Vector3 fish2Origin;
    private Vector3 fish3Origin;
    private Vector3 fish1Destination;
    private Vector3 fish2Destination;
    private Vector3 fish3Destination;
    private int nextStep = 0;
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



    void Start() {
        transform.Find("Fish Circle").gameObject.SetActive(false);
    }

    private void Update() {
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

        //set child active
        transform.Find("Fish Circle").gameObject.SetActive(true);

        //update main UI
        FindObjectOfType<MainUI>().transform.Find("Interact").Find("Text").GetComponent<Text>().text = "Reel In";

        //set starting fish and minigame duration
        System.Random rand = new System.Random();
        bobsRemaining = rand.Next(minBobs, maxBobs);
        nextStep = (rand.Next(0, 3)) * 2; //start at a random fish

        //set nextStep based on startingFish#

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

        //wait startdelay time
        fish1.DOLocalMove(fish1.localPosition, startDelay, false).OnComplete(MoveNextFish);
    }

    private void MoveNextFish() {
        //0 is moving fish 1 in
        //1 is moving fish 1 out
        //2 is moving fish 2 in
        //3 is moving fish 2 out
        //4 is moving fish 3 in
        //5 is moving fish 3 out

        if (playerReeled) {
            return;
        }

        if (bobsRemaining == 0) {
            fishHooked = true;
            bobberBigSplash.Play();
            fish1.DOLocalMove(fish1.localPosition, availableCatchTime, false).OnComplete(EndMinigame);
            return;
        }

        if (nextStep == 0) {
            fish1.DOLocalMove(fish1Destination, moveInTime, false).OnComplete(MoveNextFish);
            bobsRemaining--;
        }
        else if (nextStep == 1) {
            //make the bobber bob
            bobberSplash.Play();
            fish1.DOLocalMove(fish1Origin, moveOutTime, false).OnComplete(MoveNextFish);
        }
        else if (nextStep == 2) {
            fish2.DOLocalMove(fish2Destination, moveInTime, false).OnComplete(MoveNextFish);
            bobsRemaining--;
        }
        else if (nextStep == 3) {
            //make the bobber bob
            bobberSplash.Play();
            fish2.DOLocalMove(fish2Origin, moveOutTime, false).OnComplete(MoveNextFish);
        }
        else if (nextStep == 4) {
            fish3.DOLocalMove(fish3Destination, moveInTime, false).OnComplete(MoveNextFish);
            bobsRemaining--;
        }
        else if (nextStep == 5) {
            //make the bobber bob
            bobberSplash.Play();
            fish3.DOLocalMove(fish3Origin, moveOutTime, false).OnComplete(MoveNextFish);
        }

        nextStep++;
        if (nextStep == 6) {
            nextStep = 0;
        }
    }

    private void EndMinigame() {
        //if the player does not reel in the fish in time, end the minigame
        if (!playerReeled) {
            fishHooked = false;
            StaticVariables.playerAnimator.CrossFadeInFixedTime("Shrugging", 0.2f);
            EndFishingUI();
            //FindObjectOfType<Interact>().DestroyInteractable();
            //gameObject.SetActive(false);
        }

    }

    public void ReelIn() {
        playerReeled = true;
        if (fishHooked) {
            playerGotFish = true;
            //FindObjectOfType<Interact>().Pickup();
            StaticVariables.playerAnimator.CrossFadeInFixedTime("Fishing - Reeling", 0.2f);
            EndFishingUI();
            //gameObject.SetActive(false);
        }
        else {
            //FindObjectOfType<Interact>().DestroyInteractable();
            StaticVariables.playerAnimator.CrossFadeInFixedTime("Shrugging", 0.2f);
            EndFishingUI();
            //gameObject.SetActive(false);
        }

    }

    public void EndFishingUI() {
        transform.Find("Fish Circle").gameObject.SetActive(false);
        fish1.localPosition = fish1Origin;
        fish2.localPosition = fish2Origin;
        fish3.localPosition = fish3Origin;


        FindObjectOfType<MainUI>().transform.Find("Interact").Find("Text").GetComponent<Text>().text = "Interact";
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

        //remove the fishing rod from the hand
        if (StaticVariables.interactScript.removeItemWhenFinished)
            StaticVariables.interactScript.RemoveObjectFromHand();

        StaticVariables.currentMinigame = null;

        //close the fishing UI
        //EndFishingUI();
    }


}
