using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WoodcuttingHandler : ToolHandler
{
    public bool showWoodcuttingUI;
    public bool inWoodcuttingScene;
    public bool gameOver;
    public bool showFinishUI;
    public Text timerText;
    public Text timerUIText;
    public float counter;
    Transform uiParent;
    Transform finishScreen;
    Transform messageUI;
    Transform timerUI;

    #region Inherited Functions

    public override void ProcessInteractAction() {
        if (!StaticVariables.interactScript.currentlyInteracting) {
            StaticVariables.interactScript.SetPreviousItemInHand();
            StaticVariables.interactScript.PutFirstToolOfTypeInHand(Tool.ToolTypes.axe);
            StaticVariables.SetupPlayerInteractionWithHighlightedObject();
            StaticVariables.interactScript.currentlyInteracting = true;
            StaticVariables.PlayAnimation("Swing Axe", 1);
            FindBlade();
            StaticVariables.WaitTimeThenCallFunction(.6f, blade.EnableBlade);
            StaticVariables.WaitTimeThenCallFunction(2.5f,StartWoodcuttingGame);
        }
    }

    public override void ProcessInteractAnimationEnding() {
        StaticVariables.currentInteractionHandler = null;
        StaticVariables.interactScript.PutPreviousItemBackInHand();
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
    }
    #endregion

    public void Start() {
        AssignLocalVariables();
        inWoodcuttingScene = false;
    }

    public void Update() {
        if(ShouldWoodcuttingUIBeShown()) {
            StartCoroutine(BeginWoodcutting());
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

    }

    public void StartWoodcuttingGame() {
        StaticVariables.sceneHandler.LoadScene(1);
    }
    private bool ShouldWoodcuttingUIBeShown() {
        if(StaticVariables.sceneHandler.GetSceneName() == "Woodcutting Minigame" && !inWoodcuttingScene){
            inWoodcuttingScene = true;
            showWoodcuttingUI = true;
        }
        return showWoodcuttingUI;
    }
    private void FindBlade() {
        blade = StaticVariables.interactScript.objectInHand.transform.GetChild(0).GetComponent<BladeInteraction>();
    }

    // public IEnumerator ShowTimer() {
    //     yield return StaticVariables.AnimateChildObjectsAppearing(timerUI);
    // }

    // public IEnumerator restartTimer(float timerTimeInSeconds) {
    //     counter = 0;
    //     while(counter < timerTimeInSeconds) {
    //         counter += Time.deltaTime;
    //         timerText.text = counter.ToString();
    //         SetTimerText();
    //         yield return null;
    //     }
    // }

    // public void SetTimerText() {
    //     timerUIText = timerText;
    // }

    public bool GameISOver() {
        return gameOver;
    }

    public bool ShouldFinishUIBeShown() {
        return showFinishUI;
    }

    public IEnumerator BeginWoodcutting() {
        yield return ShowWoodcuttingUI();
        // StartCoroutine(restartTimer(30));
    }

    public IEnumerator ShowWoodcuttingUI() {
        yield return StaticVariables.mainUI.HideUI2();
        uiParent.gameObject.SetActive(true);
        yield return StaticVariables.AnimateChildObjectsAppearing(uiParent);
        yield return null;
    }

}