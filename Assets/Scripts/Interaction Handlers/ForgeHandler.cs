using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ForgeHandler : InteractionHandler {

    //add a timer to the minigame. if the last scrap dropped X seconds ago, the game ends
    //X should be maybe 10?

    private Transform forgeUI;
    private GameObject background;
    private Text scrapRemainingText;
    private Text metalForgedText;
    private Transform scrapMovingInterface;
    private Transform scrapSpawnArea;

    private Transform interactableObject;

    private GameObject forgeObject;
    private GameObject lightSource;
    private GameObject smoke;

    private bool showForgeUIWhenAnimatorIsIdle = false;
    
    private bool currentlyLightingForge = false;

    private int scrapRemainingInBag = 0;
    private int metalForged = 0;
    public float timeBetweenUIOpeningAndMinigameStart = 1f;
    public float timeBetweenScrapDrops = 0.5f;

    public Material litForgeMaterial;

    public GameObject metalGO;
    public GameObject dirtGO;
    private int scrapOnScreenCount = 0;

    public Item metalScrapItem;
    public Item refinedMetalItem;

    [Range(1, 100)]
    public int percentScrapIsMetal = 80;

    #region Inherited Functions

    public override void ProcessInteractAction() {
        StaticVariables.SetupPlayerInteractionWithHighlightedObject();
        StaticVariables.interactScript.currentlyInteracting = true;
        SetForgeInteractable();
        if (!IsForgeLit()) {
            LightForge();
        }
        else {
            StaticVariables.SetupPlayerInteractionWithHighlightedObject();
            StaticVariables.PlayAnimation("Cooking - Down");
            showForgeUIWhenAnimatorIsIdle = true;
        }
    }

    public override void ProcessInteractAnimationEnding() {
        
        if (currentlyLightingForge)
            currentlyLightingForge = false;

            
        StaticVariables.currentInteractionHandler = null;
    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        SetForgeInteractable();
        if (!IsForgeLit())
            return CanPlayerLightForge();
        else
            if (StaticVariables.playerInventory.GetTotalItemQuantity(metalScrapItem) > 0)
                return true;
        return false;
    }

    #endregion

    #region Show/Hide UI Functions

    private IEnumerator ShowForgeUI() {

        yield return StaticVariables.mainUI.HideUI2();
        background.SetActive(true);
        forgeUI.gameObject.SetActive(true);

        scrapRemainingInBag = StaticVariables.playerInventory.GetQuantityOfSpecificItem(metalScrapItem);
        scrapOnScreenCount = 0;
        metalForged = 0;
        UpdateScrapRemainingText();
        UpdateMetalForgedText();

        yield return StaticVariables.AnimateChildObjectsAppearing(forgeUI);

        StartForging();

        yield return null;
    }

    private void QuitForgeUI() {
        StaticVariables.currentInteractionHandler = null;
        StaticVariables.PlayAnimation("Cooking - Stand");
        StartCoroutine(ReturnToMainUI());
    }

    private IEnumerator ReturnToMainUI() {
        yield return HideAllUI();
        background.SetActive(false);
        yield return StaticVariables.mainUI.ShowUI2();
        if (metalForged > 0) {
            StaticVariables.playerInventory.AddItemToInventory(refinedMetalItem, metalForged);
            StaticVariables.playerInventory.RemoveAllOfItem(metalScrapItem);
        }
    }

    private IEnumerator HideAllUI() {

        if (forgeUI.gameObject.activeSelf)
            yield return StaticVariables.AnimateChildObjectsDisappearing(forgeUI, true);

        yield return null;
    }

    private void StartForging() {
        
        StaticVariables.WaitTimeThenCallFunction(timeBetweenUIOpeningAndMinigameStart, DropOneScrap);

    }

    private void DropOneScrap() {
        CreateRandomScrap();
        scrapRemainingInBag--;
        scrapOnScreenCount++;
        UpdateScrapRemainingText();
        if (scrapRemainingInBag > 0)
            StaticVariables.WaitTimeThenCallFunction(timeBetweenScrapDrops, DropOneScrap);
    }

    private void CreateRandomScrap() {
        //randomly determine if the scrap is metal or dirt
        int r = Random.Range(1, 101);
        bool isMetal = r <= percentScrapIsMetal;
        GameObject prefab = metalGO;
        if (!isMetal)
            prefab = dirtGO;

        //create the new object
        GameObject newItem = GameObject.Instantiate(prefab);

        //set the transforms
        newItem.transform.SetParent(scrapMovingInterface);
        newItem.transform.localPosition = scrapSpawnArea.localPosition;
        //newItem.layer = 5;
        foreach (Transform t in newItem.transform)
            t.gameObject.layer = 5;
        newItem.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 359));
        newItem.transform.localScale *= 1;


        //start the object moving down the interface
        float horiz = 0.2f;
        horiz *= (Random.Range(6, 15) / 10f); //multiply horiz by 0.6-1.4
        if (Random.Range(0, 2) == 1) //set horiz direction to be left or right
            horiz *= -1;
        newItem.GetComponent<Rigidbody2D>().velocity = new Vector3(horiz, -1, 0);
        newItem.GetComponent<ForgeSwipable>().forgeHandler = this;
    }

    public void ItemEnteredCollectionArea(bool isMetal, bool isMetalArea, GameObject go) {
        if (isMetal && isMetalArea) {
            metalForged++;
            UpdateMetalForgedText();
        }
        if (!isMetal && isMetalArea) {
            metalForged--;
            UpdateMetalForgedText();
        }

        Destroy(go);
        scrapOnScreenCount--;
        if ((scrapRemainingInBag == 0) && (scrapOnScreenCount == 0)) {
            EndForging();
        }
    }
    
    private void UpdateScrapRemainingText() {
        scrapRemainingText.text = scrapRemainingInBag + " Scrap Remaining";
    }

    private void UpdateMetalForgedText() {
        metalForgedText.text = metalForged + " Metal Forged";
    }

    private void EndForging() {
        print("the last scrap made it to the bottom, ending forging");
        //give player the amount of metal they are owed
        //remove the scrap from their inventory
        QuitForgeUI();
    }

    #endregion

    void Start() {
        AssignLocalVariables();
        background.SetActive(false);
        forgeUI.gameObject.SetActive(false);
    }

    private void Update() {
        if (ShouldForgeUIBeShown()) {
            StartCoroutine(ShowForgeUI());
            print("show UI now");
            showForgeUIWhenAnimatorIsIdle = false;
        }
    }

    private bool CanPlayerLightForge() {
        return StaticVariables.playerInventory.DoesInventoryContainToolWithType(Tool.ToolTypes.firelighter);
    }

    private bool ShouldForgeUIBeShown() {
    return (showForgeUIWhenAnimatorIsIdle && StaticVariables.IsPlayerAnimatorInState("Cooking - Idle Part 1"));
}

    private void AssignLocalVariables() {
        forgeUI = transform.Find("UI");
        background = transform.Find("Background").gameObject;
        scrapRemainingText = forgeUI.Find("Scrap Remaining").Find("Text").GetComponent<Text>();
        metalForgedText = forgeUI.Find("Metal Forged").Find("Text").GetComponent<Text>();
        scrapMovingInterface = forgeUI.Find("Scrap Moving Interface");
        scrapSpawnArea = scrapMovingInterface.Find("Spawn Area");
    }

    private void SetForgeInteractable() {
        interactableObject = StaticVariables.interactScript.closestInteractable.transform;

        forgeObject = interactableObject.Find("Forge Object").gameObject;
        lightSource = forgeObject.transform.Find("Lightsource").gameObject;
        smoke = forgeObject.transform.Find("Smoke").gameObject;
    }

    private bool IsForgeLit() {
        return lightSource.activeSelf;
    }

    private void LightForge() {
        StaticVariables.PlayAnimation("Standing To Kneeling");
        currentlyLightingForge = true;

        float timeUntilForgeLights = 3f;
        
        StaticVariables.WaitTimeThenCallFunction(timeUntilForgeLights, ShowLitForge);
    }
    

    private void ShowLitForge() {
        lightSource.SetActive(true);
        smoke.SetActive(true);

        //if the forge is highlighted, unhighlight it
        bool isHighlighted = interactableObject.GetComponent<Outline>().enabled;
        if (isHighlighted)
            interactableObject.GetComponent<Outline>().enabled = false;

        //change the material of the forge
        Material[] newMaterials = forgeObject.GetComponent<MeshRenderer>().materials;
        newMaterials[1] = litForgeMaterial;
        forgeObject.GetComponent<MeshRenderer>().materials = newMaterials;

        //if the forge was highlighted, re-highlight it
        if (isHighlighted)
            interactableObject.GetComponent<Outline>().enabled = true;
    }
}
