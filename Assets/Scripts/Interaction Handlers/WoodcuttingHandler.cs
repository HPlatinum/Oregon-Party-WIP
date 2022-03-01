using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WoodcuttingHandler : ToolHandler
{
    public float scaleUP;
    public int totalGameTime;
    public bool showWoodcuttingUI;
    public bool inWoodcuttingScene;
    public bool gameOver;
    public bool showFinishUI;
    public bool tweenRunning;
    public bool playerInWoodcuttingState;
    public bool playerInSharpeningState;
    public bool playerIsInFinalAnimationState;
    public Text timerUIText;
    public float counter;
    Timer timeForReward;
    Transform uiParent;
    Transform finishScreen;
    Transform messageUI;
    Transform timerUI;
    GameObject treeMiningSpot;
    GameObject storageArea;
    GameObject sharpeningStation;
    ToolStats toolStats;

    #region Inherited Functions

    public override void ProcessInteractAction() {
        if (!StaticVariables.interactScript.currentlyInteracting) {
            if(StaticVariables.sceneHandler.GetSceneName() != "Woodcutting Minigame") {
                StaticVariables.interactScript.SetPreviousItemInHand();
                StaticVariables.interactScript.PutFirstToolOfTypeInHand(Tool.ToolTypes.axe);
                StaticVariables.SetupPlayerInteractionWithHighlightedObject();
                StaticVariables.PlayAnimation("Swing Axe", 1);
                FindBlade();
                StaticVariables.WaitTimeThenCallFunction(.6f, blade.EnableBlade);
                StaticVariables.WaitTimeThenCallFunction(2.5f,StartWoodcuttingGame);
            }
            else {
                StaticVariables.interactScript.SetPreviousItemInHand();
                StaticVariables.interactScript.PutFirstToolOfTypeInHand(Tool.ToolTypes.axe);
                GetToolStats();
                StaticVariables.SetupPlayerInteractionWithHighlightedObject();
                ActivePlayerCuttingWood();
            }
        }
        
    }

    public override void ProcessInteractAnimationEnding() {
        StaticVariables.currentInteractionHandler = null;
        // StaticVariables.interactScript.PutPreviousItemBackInHand();
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if (StaticVariables.playerInventory.DoesInventoryContainToolWithType(Tool.ToolTypes.axe)) {
            if (StaticVariables.playerInventory.CanAddItemToInventory(interactable.item, 1)) {
                return true;
            }
        }
        return false;
    }

    public override void AssignLocalVariables() {
        uiParent = transform.Find("Woodcutting Game Parent");
        finishScreen = transform.Find("Finish Screen");
        messageUI = transform.Find("Message UI");
        timerUI = uiParent.Find("Timer");
        timerUIText = timerUI.GetComponentInChildren<Text>();
        totalGameTime = 150;
    }
    #endregion

    public void Start() {
        AssignLocalVariables();
        inWoodcuttingScene = false;
    }

    public void Update() {
        if(ShouldWoodcuttingUIBeShown()) {
            StartCoroutine(BeginWoodcutting());
            ResetLocalVariables();
            showWoodcuttingUI = false;
        }
        if(GameISOver()) {
            gameOver = false;
            // gameOverFR = true;
        }
        if(ShouldFinishUIBeShown()) {
            showFinishUI = false;
            // StartCoroutine(CloseMiningUIAndOpenFinishUI());
        }
        if(StaticVariables.timer.TimerIsRunning() && timerUIText.text != StaticVariables.timer.GetTimeForDisplaying()) {
            timerUIText.text = StaticVariables.timer.GetTimeForDisplaying();
            SetTimerTextColor();
            ResizeTimer();
        }

        if(PlayerIsCuttingWood()) {
            if(Mathf.Round(timeForReward.GetTimeForChangingDisplayColor()) == 2 && !playerIsInFinalAnimationState) {
                playerIsInFinalAnimationState = true;
                StaticVariables.PlayAnimation("Swing Axe", 1);
            }
            if(timeForReward.GetTimeForChangingDisplayColor() == 0) {
                StaticVariables.playerInventory.AddItemToInventory(StaticVariables.interactScript.GetClosestInteractable().GetItem(), 1);
                playerInWoodcuttingState = false;
                playerIsInFinalAnimationState = false;
                toolStats.SubtractFromWear(20);
            }
        }
        if(PlayerIsSharpeningAxe()) {
            if(timeForReward.GetTimeForChangingDisplayColor() == 0) {
                //check wear. Set wear
                playerInSharpeningState = false;
            }
        }
        
    }
    public void StartWoodcuttingGame() {
        StaticVariables.sceneHandler.LoadScene(1);
    }

    private void ActivePlayerCuttingWood() {
        if(StaticVariables.interactScript.GetClosestInteractable().GetComponent<Interactable>().storedItemCount > 0 && !toolStats.isBroken) {
            StaticVariables.PlayAnimation("Swing Axe Loop", 1);
            playerInWoodcuttingState = true;
            timeForReward = StaticVariables.interactScript.GetClosestInteractable().GetComponentInChildren<Timer>();
            if(!timeForReward.TimerIsRunning()) {
                print("Weeeeeeeee");
                timeForReward.StartGameTimer(7f);
                print(timeForReward);
            }
        }
        if(toolStats.isBroken) {
            print("You need to sharpen your tool");
        }
    }

    private bool ShouldWoodcuttingUIBeShown() {
        if(StaticVariables.sceneHandler.GetSceneName() == "Woodcutting Minigame" && !inWoodcuttingScene){
            inWoodcuttingScene = true;
            showWoodcuttingUI = true;
        }
        return showWoodcuttingUI;
    }

    public bool PlayerIsCuttingWood() {
        return playerInWoodcuttingState;
    }

    public bool PlayerIsSharpeningAxe() {
        return playerInSharpeningState;
    }
    private void FindBlade() {
        blade = StaticVariables.interactScript.objectInHand.transform.GetChild(0).GetComponent<BladeInteraction>();
    }

    private void GetToolStats() {
        toolStats = StaticVariables.interactScript.objectInHand.transform.GetComponentInChildren<ToolStats>();
    }

    
    public void ResetLocalVariables() { 
        treeMiningSpot = GameObject.Find("Tree");
        storageArea = GameObject.Find("Storage");
        sharpeningStation = GameObject.Find("Sharpening Station");
        print(treeMiningSpot);
        print(storageArea);
        print(sharpeningStation);
        tweenRunning = false;
        scaleUP = 1f;
        playerIsInFinalAnimationState = false;
    }

    public bool GameISOver() {
        return gameOver;
    }

    public bool ShouldFinishUIBeShown() {
        return showFinishUI;
    }

    public IEnumerator BeginWoodcutting() {
        StaticVariables.timer.StartGameTimer(totalGameTime);
        yield return ShowWoodcuttingUI();
    }

    public IEnumerator ShowWoodcuttingUI() {
        // yield return StaticVariables.mainUI.HideUI2();
        uiParent.gameObject.SetActive(true);
        yield return StaticVariables.AnimateChildObjectsAppearing(uiParent);
        yield return null;
    }

    public void SetTimerTextColor() {
        if (GetCurrentTimerTime() >= 30 && timerUIText.color != Color.green) {
            timerUIText.color = Color.green;
        }
        else if(GetCurrentTimerTime() < 30 && GetCurrentTimerTime() > 10 && timerUIText.color != Color.yellow) {
            timerUIText.color = Color.yellow;
        }
        else if(GetCurrentTimerTime() < 10 && timerUIText.color != Color.red) {
            timerUIText.color = Color.red;
        }
    }

    public void ResizeTimer() {
        if(IsTimeWithinSecondsRangeForScaling(9)){
            float scaleUP = 1.5f;
            float duration = .2f;
            ScaleUpTimer(scaleUP, duration);
            StaticVariables.WaitTimeThenCallFunction(.2f,ScaleDownTimer);
        }
        if(IsTimeWithinTensOfSecondsRangeForScaling()) {
            float scaleUP = 1.2f;
            float duration = .2f;
            ScaleUpTimer(scaleUP, duration);
            StaticVariables.WaitTimeThenCallFunction(.2f,ScaleDownTimer);
        }

    }

    public bool IsTimeWithinSecondsRangeForScaling(int time) {
        bool withinRange = false;
        for(int i = time; i > 0; i--) {
            withinRange = GetCurrentTimerTime() < i + .05 && GetCurrentTimerTime() > i && !tweenRunning;
            if(withinRange == true) {
                break;
            }
        }
        return withinRange;
    }

    public bool IsTimeWithinTensOfSecondsRangeForScaling() {
        bool withinRange = false;
        if(GetCurrentTimerTime() >= 10) {
            for(int i = totalGameTime / 10; i > 0; i--) {
                if(GetCurrentTimerTime() - Mathf.Round(GetCurrentTimerTime()) < .05 && Mathf.Round(GetCurrentTimerTime()) / 10 == i && !tweenRunning) {
                    withinRange = true;
                    break;
                }
            }
        }
        return withinRange;
    }

    public float GetCurrentTimerTime() {
        return StaticVariables.timer.GetTimeForChangingDisplayColor();
    }

    public void ScaleUpTimer(float scaleUP, float duration) {
        ChangeTweenRunningStatus();
        timerUI.transform.DOScale(scaleUP, duration);
    }
    public void ScaleDownTimer() {
        StaticVariables.WaitTimeThenCallFunction(.7f, ChangeTweenRunningStatus);
        float endScale = 1f;
        float duration = .2f;
        timerUI.transform.DOScale(endScale, duration);
    }

    public void ChangeTweenRunningStatus() {
        tweenRunning = !tweenRunning;
    }

    //drop twist timer, then drop it

    //pop up end screen

}