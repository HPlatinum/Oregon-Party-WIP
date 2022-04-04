using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WoodcuttingHandler : ToolHandler
{
    #region Beaver Variables
    public GameObject beaverPrefab;
    List<bool> possibleSpawnLocations;
    bool continueSpawningBeavers;
    public Item wood;
    System.Random rand;
    List<Transform> beaverSpawns;
    public List<GameObject> woodpile;
    public List<GameObject> depositWoodPile;
    #endregion
    public float scaleUP;
    public int totalGameTime;
    public bool gameIsStarted;
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
    public GameObject newObj;
    Timer timeForReward;
    Timer gameTimer;
    public Transform uiParent;
    public Transform finishScreen;
    public Transform messageUI;
    public Transform startGameUI;
    public Transform timerUI;
    public GameObject woodCuttingStation;
    public GameObject storageArea;
    public GameObject sharpeningStation;
    public GameObject woodDestination;
    public GameObject cutWoodModel;
    public GameObject log;
    public ToolStats toolStats;


    #region Inherited Functions

    public override void ProcessInteractAction() {
        if (!StaticVariables.interactScript.currentlyInteracting) {
            if(!gameIsStarted) {
                print("here 1");
                StartCoroutine(PromptUserToStartWoodcuttingGame());
                return;
            }
            if(LogInPlayersHand()) {
                log = InstantiatePrefabAsChild(StaticVariables.interactScript.itemInHand.model, woodDestination.transform, StaticVariables.interactScript.itemInHand);
                StaticVariables.interactScript.RemoveItemFromHand();
                StaticVariables.controller.Carry();
            }
            else {
                StaticVariables.interactScript.SetPreviousItemInHand();
                StaticVariables.interactScript.PutFirstToolOfTypeInHand(Tool.ToolTypes.axe);
                // StaticVariables.controller.MoveCharacter(StaticVariables.interactScript.closestInteractable.transform.Find("Standing Location").position); if I could move it to a specific position
                StaticVariables.SetupPlayerInteractionWithHighlightedObject();
                if(toolStats == null) {
                    GetToolStats();
                    ResetToolStats();
                }
                ActivePlayerCuttingWood();
            }
        }
        
    }

    public override void ProcessInteractAnimationEnding() {
        StaticVariables.currentInteractionHandler = null;
        StaticVariables.interactScript.PutPreviousItemBackInHand();
        StaticVariables.WaitTimeThenCallFunction(.5f,StaticVariables.controller.Carry);
        StaticVariables.WaitTimeThenCallFunction(.75f, PlaceWoodInPlayersHand);
        StaticVariables.interactScript.closestInteractable = null;
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        if(StaticVariables.playerInventory.DoesInventoryContainToolWithType(Tool.ToolTypes.axe) && !GameIsStarted() && !interactable.resourceMined) {
            return true;
        }
        if (LogInPlayersHand() && !LogIsOnChoppingBlock() || LogIsOnChoppingBlock()) {
            if (StaticVariables.playerInventory.CanAddItemToInventory(interactable.item, 1)) {
                if(inWoodcuttingScene && toolStats != null) {
                    if(toolStats.isBroken) {
                        Debug.Log("You need to sharpen your axe, home-skillet");
                        return false;
                    }
                }
                return true;
            }
        }
        return false;
    }

    public override void AssignLocalVariables() {
    }
    #endregion

    public void Start() {
        inWoodcuttingScene = false;
        gameOver = false;
        gameIsStarted = false;
    }

    public void Update() {
        if(GameIsStarted()) {
            if(ShouldWoodcuttingUIBeShown()) {
                showWoodcuttingUI = false;
                StartCoroutine(BeginWoodcutting());
                
            }
            if(GameISOver()) {
                gameIsStarted = false;
                gameOver = false;
                gameTimer.SetTimerEndedBackToFalse();
                StaticVariables.playerInventory.AddItemToInventory(StaticVariables.depositHandler.depositItem, StaticVariables.depositHandler.GetQuantityOfWoodCollected());
                print("Game is over. You collected " +StaticVariables.depositHandler.GetQuantityOfWoodCollected() + " total wood.");
                StaticVariables.depositHandler.gameIsOver = true;
                woodCuttingStation.GetComponent<Interactable>().resourceMined = true;
            }
            if(ShouldFinishUIBeShown()) {
                showFinishUI = false;
                // StartCoroutine(CloseMiningUIAndOpenFinishUI());
            }
            if(StaticVariables.timer.TimerIsRunning() && timerUIText.text != StaticVariables.timer.GetTimeForDisplaying()) {
                if(!timerUI.gameObject.activeSelf) {
                    timerUI.gameObject.SetActive(true);
                }
                timerUIText.text = StaticVariables.timer.GetTimeForDisplaying();
                SetTimerTextColor();
                ResizeTimer();
            }
            if(PlayerIsCuttingWood()) {
                StaticVariables.controller.FaceTo(woodCuttingStation,0.2f);
                if(Mathf.Round(timeForReward.GetTimeForChangingDisplayColor()) == 1 && !playerIsInFinalAnimationState) {
                    playerIsInFinalAnimationState = true;
                    StaticVariables.controller.SwingAxe();
                    toolStats.SubtractFromWear(20);
                    if(toolStats.wear == 0) {
                        toolStats.SetToolToBroken();
                    }
                    StaticVariables.WaitTimeThenCallFunction(1f, PlayWoodChopAnimation);
                }
                if(timeForReward.GetTimeForChangingDisplayColor() == 0) {
                    playerInWoodcuttingState = false;
                    playerIsInFinalAnimationState = false;
                }
            }
            if(PlayerIsSharpeningAxe()) {
                if(timeForReward.GetTimeForChangingDisplayColor() == 0) {
                    playerInSharpeningState = false;
                }
            }
            if(!GameISOver() && continueSpawningBeavers) {
                DetermineIfBeaverShouldBeReleased();
            }
            if(StaticVariables.interactScript.itemInHand != null && StaticVariables.interactScript.itemInHand.name == "Log" && !StaticVariables.controller._isCarrying) {
                StaticVariables.controller.Carry();
            }
        }
    }

    private IEnumerator PromptUserToStartWoodcuttingGame() {
        yield return HideMainUI();
        yield return DisplayStartGameUI();
    }

    private IEnumerator DisplayStartGameUI() {
        startGameUI.gameObject.SetActive(true);
        yield return StaticVariables.AnimateChildObjectsAppearing(startGameUI);
    }

    private IEnumerator HideMainUI() {
        print("here UI");
        yield return StaticVariables.mainUI.HideUI2();
    }
    private IEnumerator ShowMainUI() {
        yield return StaticVariables.mainUI.ShowUI2();
    }
    public void CloseStartGameUI() {
        StartCoroutine(CloseStartGameUIAnimate());
    }

    public IEnumerator CloseStartGameUIAnimate() {
        yield return StaticVariables.AnimateChildObjectsDisappearing(startGameUI);
        startGameUI.gameObject.SetActive(false);
        yield return ShowMainUI();
    }

    private bool GameIsStarted() {
        return gameIsStarted;
    }

    private GameObject InstantiatePrefabAsChild(GameObject prefab, Transform parent, Item item) {
        GameObject newObj = Instantiate(prefab);
        Destroy(newObj.transform.Find("Log").GetComponent<BoxCollider>());
        newObj.transform.SetParent(parent);
        newObj.transform.localPosition = new Vector3 (0,0,0); // positions object in woodDestination
        newObj.transform.rotation = Quaternion.Euler(0,0,0);
        return newObj;
    }
    public void StartWoodcuttingGame() {
        showWoodcuttingUI = true;
        gameIsStarted = true;
        AssignLocalVariables();
        ResetLocalVariables();
        SetWoodcuttingObjectsInteractable();
    }

    private void SetWoodcuttingObjectsInteractable() {
        storageArea.GetComponent<Interactable>().enabled = true;
        sharpeningStation.GetComponent<Interactable>().enabled = true;
    }

    private void PlayWoodChopAnimation() {
        log.GetComponent<AnimatedWoodcutting>().AnimateWoodChop();
    }

    private void PlaceWoodInPlayersHand() {
        StaticVariables.interactScript.PutItemInPlayerHandButChangeModel(StaticVariables.interactScript.GetClosestInteractable().GetItem(), cutWoodModel);
    }

    private void ActivePlayerCuttingWood() {
        if(StaticVariables.interactScript.GetClosestInteractable().GetComponent<Interactable>().storedItemCount > 0) {
            if(StaticVariables.controller._doneSwingingAxe) {
                StaticVariables.controller.SwingAxe();
            }
            StaticVariables.PlayAnimation("Swing Axe Loop", 1);
            playerInWoodcuttingState = true;
            timeForReward = StaticVariables.interactScript.GetClosestInteractable().GetComponentInChildren<Timer>();
            if(!timeForReward.TimerIsRunning()) {
                timeForReward.StartGameTimer(5f);
            }
        }
    }

    private bool ShouldWoodcuttingUIBeShown() {
        return showWoodcuttingUI; 
    }

    public bool InWoodcuttingScene() {
        return (StaticVariables.sceneHandler.GetSceneName() == "Woodcutting Minigame");
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
        toolStats = StaticVariables.playerInventory.GetToolScriptFromItem(StaticVariables.interactScript.itemInHand);
        print("wear = " + toolStats.wear);
    }

    private void ResetToolStats() {
        toolStats.wear = 100;
    }

    private void RealeaseBeaver(int beaverSpawn) {
        GameObject newBeaver = Instantiate(beaverPrefab);
        newBeaver.transform.position = beaverSpawns[beaverSpawn].position;
    }

    private void DetermineIfBeaverShouldBeReleased() {
        if(storageArea.GetComponent<Interactable>().inventory.GetTotalItemQuantity(wood) >= 1) {
            possibleSpawnLocations = new List<bool>();
            for(int i = 0; i < 5; i++) {
                possibleSpawnLocations.Add(beaverSpawns[i].GetComponent<BeaverSpawned>().BeaverHasBeenSpawned());
            }
            int index = rand.Next(0, 4);
            if(!possibleSpawnLocations[index]) {
                RealeaseBeaver(index);
                beaverSpawns[index].GetComponent<BeaverSpawned>().SpawnBeaver();
            }
            continueSpawningBeavers = false;
            StaticVariables.WaitTimeThenCallFunction(5f, ContinueSpawningBeaversSetTrue);
        }
    }

    public void ContinueSpawningBeaversSetTrue() {
        continueSpawningBeavers = true;
    }
    public void ResetLocalVariables() { 
        woodCuttingStation = StaticVariables.interactScript.closestInteractable.GetComponent<WoodcuttingGameObjects>().tree;
        storageArea = StaticVariables.interactScript.closestInteractable.GetComponent<WoodcuttingGameObjects>().storage;
        sharpeningStation = StaticVariables.interactScript.closestInteractable.GetComponent<WoodcuttingGameObjects>().sharepeningStation;
        woodDestination = StaticVariables.interactScript.closestInteractable.GetComponent<WoodcuttingGameObjects>().woodDestination;
        beaverSpawns = StaticVariables.interactScript.closestInteractable.GetComponent<WoodcuttingGameObjects>().beaverSpawns;
        woodpile = StaticVariables.interactScript.closestInteractable.GetComponent<WoodcuttingGameObjects>().woodpile;
        depositWoodPile = StaticVariables.interactScript.closestInteractable.GetComponent<WoodcuttingGameObjects>().depositWoodPile;
        totalGameTime = 150;
        gameTimer = StaticVariables.timer;
        tweenRunning = false;
        scaleUP = 1f;
        playerIsInFinalAnimationState = false;
        toolStats = null;
        continueSpawningBeavers = true;
        rand = new System.Random();
    }

    public bool GameISOver() {
        return gameOver = gameTimer.TimerWasStartedAndIsNowStopped();
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

    public bool LogInPlayersHand() {
        if(StaticVariables.interactScript.itemInHand != null) {
            return StaticVariables.interactScript.itemInHand.name == "Log";
        }
        return false;
    }

    public bool LogIsOnChoppingBlock() {
        if(log == null) {
            return false;
        }
        return true;
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