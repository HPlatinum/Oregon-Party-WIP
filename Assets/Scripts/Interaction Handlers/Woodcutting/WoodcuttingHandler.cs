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
    float timeToWaitBeforeSpawningNextBeaver;
    bool continueSpawningBeavers;
    public Item wood;
    System.Random rand;
    List<Transform> beaverSpawns;
    public List<GameObject> woodpile;
    public List<GameObject> depositWoodPile;
    #endregion
    public float scaleUP;
    public int totalGameTime;
    public int woodInHand;
    public bool gameIsStarted;
    public bool showWoodcuttingUI;
    public bool inWoodcuttingScene;
    public bool gameOver;
    public bool showFinishUI;
    public bool tweenRunning;
    public bool playerInWoodcuttingState;
    public bool playerInSharpeningState;
    public bool playerIsInFinalAnimationState;
    bool inPregame;
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
    public Transform pregameTimerUI;
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
                DisableBeaverSpawns();
                StaticVariables.playerInventory.AddItemToInventory(StaticVariables.depositHandler.depositItem, StaticVariables.depositHandler.GetQuantityOfWoodCollected());
                print("Game is over. You collected " +StaticVariables.depositHandler.GetQuantityOfWoodCollected() + " total wood.");
                StaticVariables.depositHandler.gameIsOver = true;
                woodCuttingStation.GetComponent<Interactable>().resourceMined = true;
                StaticVariables.woodpileHandler.listIsFilled = false;
                StaticVariables.depositHandler.gameIsStarted = false;
            }
            if(ShouldFinishUIBeShown()) {
                showFinishUI = false;
                // StartCoroutine(CloseMiningUIAndOpenFinishUI());
            }
            if(gameTimer.TimerIsRunning() && timerUIText.text != gameTimer.GetTimeForDisplaying()) {
                if(!timerUI.gameObject.activeSelf) {
                    timerUI.gameObject.SetActive(true);
                }
                timerUIText.text = gameTimer.GetTimeForDisplaying();
                SetTimerTextColor(30, 10, timerUIText);
                ResizeTimer();
            }
            if(PlayerIsCuttingWood()) {
                StaticVariables.controller.lockMovement = true;
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
                    StaticVariables.controller.lockMovement = false;
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
        if(inPregame) {
            if(gameTimer.TimerIsRunning() && pregameTimerUI.GetChild(0).GetComponent<Text>().text != gameTimer.GetTimeForDisplaying() && pregameTimerUI.gameObject.activeSelf) {
                if(!pregameTimerUI.gameObject.activeSelf) {
                    pregameTimerUI.gameObject.SetActive(true);
                }
                pregameTimerUI.GetChild(0).GetComponent<Text>().text = gameTimer.GetTimeForDisplayingInSeconds();
                // SetTimerTextColor(4,2, pregameTimerUI.GetChild(0).GetComponent<Text>());
                // ResizeTimer();
                if(gameTimer.GetTimeForDisplayingInSeconds() == "0") {
                    StartCoroutine(ClosePregameTimerUI());
                    EndPregame();
                }
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
        yield return StaticVariables.mainUI.HideUI2();
    }
    private IEnumerator ShowMainUI() {
        yield return StaticVariables.mainUI.ShowUI2();
    }
    public void CloseStartGameUINo() {
        StartCoroutine(CloseStartGameUIAnimate());
        StartCoroutine(ShowMainUI());
    }

    public void CloseStartGameUIYes() {
        StartCoroutine(CloseStartGameUIAnimate());
        StaticVariables.WaitTimeThenCallFunction(.5f, StartPregame);
    }

    private IEnumerator CloseStartGameUIAnimate() {
        yield return StaticVariables.AnimateChildObjectsDisappearing(startGameUI);
        startGameUI.gameObject.SetActive(false);
    }
    private IEnumerator ShowPregameTimerUI() {
        pregameTimerUI.gameObject.SetActive(true);
        yield return StaticVariables.AnimateChildObjectsAppearing(pregameTimerUI);
    }
    private IEnumerator ClosePregameTimerUI() {
        yield return StaticVariables.AnimateChildObjectsDisappearing(pregameTimerUI);
        pregameTimerUI.gameObject.SetActive(false);
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

    private void StartPregame() {
        StartCoroutine(ShowPregameTimerUI());
        inPregame = true;
        AssignLocalVariables();
        ResetLocalVariables();
        gameTimer.StartGameTimer(6f);
    }

    private void EndPregame() {
        inPregame = false;
        gameTimer.SetTimerEndedBackToFalse();
        StartCoroutine(ShowMainUI());
        StartWoodcuttingGame();
    }
    private void StartWoodcuttingGame() {
        showWoodcuttingUI = true;
        gameIsStarted = true;
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
        newBeaver.GetComponent<BeaverController>().beaverSpawnInt = beaverSpawn;
        newBeaver.transform.position = beaverSpawns[beaverSpawn].position;
    }

    private void DetermineIfBeaverShouldBeReleased() {
        if(WoodWithinRange(1,5) && BeaverCanBeSpawned(1)) {
            int index = rand.Next(0, 4);
            if(BeaverCanBeSpawnedAtIndex(index)) {
                Debug.Log("releasing beaver  " + index);
                RealeaseBeaver(index);
                SetBeaverHasBeenSpawnedToTrueAtSpawnLocation(index);
            }
            ContinueSpawningBeaversSetFalse();
            SetTimeToWaitBeforeNextBeaverSpawns(5,20);
            Debug.Log(timeToWaitBeforeSpawningNextBeaver);
            StaticVariables.WaitTimeThenCallFunction(timeToWaitBeforeSpawningNextBeaver, ContinueSpawningBeaversSetTrue);
            return;
        }
        if(WoodWithinRange(5,10) && BeaverCanBeSpawned(2)) {
            int index = rand.Next(0, 4);
            if(BeaverCanBeSpawnedAtIndex(index)) {
                Debug.Log("releasing beaver  " + index);
                RealeaseBeaver(index);
                SetBeaverHasBeenSpawnedToTrueAtSpawnLocation(index);
            }
            ContinueSpawningBeaversSetFalse();
            SetTimeToWaitBeforeNextBeaverSpawns(5,15);
            Debug.Log(timeToWaitBeforeSpawningNextBeaver);
            StaticVariables.WaitTimeThenCallFunction(timeToWaitBeforeSpawningNextBeaver, ContinueSpawningBeaversSetTrue);
            return;
        }
    }

    private bool WoodWithinRange(int low, int high) {
        return storageArea.GetComponent<Interactable>().inventory.GetTotalItemQuantity(wood) >= low && storageArea.GetComponent<Interactable>().inventory.GetTotalItemQuantity(wood) < high;
    }

    private bool BeaverCanBeSpawnedAtIndex(int index) {
        return !possibleSpawnLocations[index];
    }

    private void SetTimeToWaitBeforeNextBeaverSpawns(int low, int high) {
        timeToWaitBeforeSpawningNextBeaver = rand.Next(low, high);
    }

    private void SetBeaverSpawnAllowedTagToFalse(int index) {
        beaverSpawns[index].GetComponent<BeaverSpawn>().SpawnBeaver();
    }

    private bool BeaverCanBeSpawned(int level) {
        if(level != 3) {
            if(level == 1) {
                if(ReturnNumberOfBeaversCurrentlySpawned() > 0) {
                    SetTimeToWaitBeforeNextBeaverSpawns(5,20);
                    StaticVariables.WaitTimeThenCallFunction(timeToWaitBeforeSpawningNextBeaver, ContinueSpawningBeaversSetTrue);
                    ContinueSpawningBeaversSetFalse();
                    Debug.Log("Spawning another beaver is currently not allowed");
                }
            }
            else if(level == 2) {
                if(ReturnNumberOfBeaversCurrentlySpawned() > 2) {
                    SetTimeToWaitBeforeNextBeaverSpawns(5,15);
                    Debug.Log(timeToWaitBeforeSpawningNextBeaver);
                    StaticVariables.WaitTimeThenCallFunction(timeToWaitBeforeSpawningNextBeaver, ContinueSpawningBeaversSetTrue);
                    ContinueSpawningBeaversSetFalse();
                }
            }
        }
        return continueSpawningBeavers;
    }

    public void DestroyBeaver(int index, GameObject beaver) {
        Destroy(beaver);
        SetBeaverHasBeenSpawnedBackToFalseAtSpawnLocation(index);
    }

    private void ContinueSpawningBeaversSetFalse() {
        continueSpawningBeavers = false;
    }

    private void EnableBeaverSpawns() {
        foreach(Transform spawn in beaverSpawns) {
            spawn.gameObject.SetActive(true);
        }
    }

    private void DisableBeaverSpawns() {
        foreach(Transform spawn in beaverSpawns) {
            spawn.gameObject.SetActive(false);
        }
    }
    private void CreateListOfAvailableSpawns() {
        possibleSpawnLocations = new List<bool>();
        for(int i = 0; i < 5; i++) {
            possibleSpawnLocations.Add(beaverSpawns[i].GetComponent<BeaverSpawn>().BeaverHasBeenSpawned());
        }
    }

    public void SetBeaverHasBeenSpawnedBackToFalseAtSpawnLocation(int index) {
        possibleSpawnLocations[index] = false;
    }

    private void SetBeaverHasBeenSpawnedToTrueAtSpawnLocation(int index) {
        possibleSpawnLocations[index] = true;
    }

    private int ReturnNumberOfBeaversCurrentlySpawned() {
        int currentlySpawnedCount = 0;
        for(int i = 0; i < 5; i++) {
            if(possibleSpawnLocations[i] == true){
                currentlySpawnedCount ++;
            }
        }
        return currentlySpawnedCount;
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
        EnableBeaverSpawns();
        CreateListOfAvailableSpawns();
    }

    private bool GameISOver() {
        return gameOver = gameTimer.TimerWasStartedAndIsNowStopped();
    }

    private bool ShouldFinishUIBeShown() {
        return showFinishUI;
    }

    private IEnumerator BeginWoodcutting() {
        StartWoodcuttingGameTimer();
        yield return ShowWoodcuttingUI();
    }

    private void StartWoodcuttingGameTimer() {
        gameTimer.StartGameTimer(totalGameTime);
    }

    private IEnumerator ShowWoodcuttingUI() {
        // yield return StaticVariables.mainUI.HideUI2();
        uiParent.gameObject.SetActive(true);
        yield return StaticVariables.AnimateChildObjectsAppearing(uiParent);
        yield return null;
    }

    public void SetTimerTextColor(int high, int low, Text timerText) {
        if (GetCurrentTimerTime() >= high && timerText.color != Color.green) {
            timerText.color = Color.green;
        }
        else if(GetCurrentTimerTime() < high && GetCurrentTimerTime() > low && timerUIText.color != Color.yellow) {
            timerText.color = Color.yellow;
        }
        else if(GetCurrentTimerTime() < low && timerText.color != Color.red) {
            timerText.color = Color.red;
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
        return gameTimer.GetTimeForChangingDisplayColor();
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