using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ForgeHandler : InteractionHandler {

    private Transform forgeUI;
    private GameObject background;
    private Text scrapRemainingText;
    private Text metalForgedText;
    private Transform scrapMovingInterface;
    private Transform scrapSpawnArea;
    private Transform metalDestination;
    private Transform dirtDestination;

    private Transform interactableObject;

    private GameObject forgeObject;
    private GameObject lightSource;
    private GameObject smoke;

    private bool showForgeUIWhenAnimatorIsIdle = false;
    
    private bool currentlyLightingForge = false;

    private int scrapRemaining = 0;
    private int metalForged = 0;
    public float timeBetweenUIOpeningAndMinigameStart = 1f;
    public float timeBetweenScrapDrops = 0.5f;

    public Material litForgeMaterial;

    public GameObject metalGO;
    public GameObject dirtGO;
    private List<GameObject> fallingObjects = new List<GameObject>();

    [Range(1, 100)]
    public int percentScrapIsMetal = 80;

    #region Inherited Functions

    public override void ProcessInteractAction() {
        if (!StaticVariables.interactScript.currentlyInteracting) {
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
            return true;
    }

    #endregion

    #region Show/Hide UI Functions

    private IEnumerator ShowForgeUI() {

        yield return StaticVariables.mainUI.HideUI2();
        background.SetActive(true);
        forgeUI.gameObject.SetActive(true);

        scrapRemaining = 20;
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
        scrapRemaining--;
        UpdateScrapRemainingText();
        if (scrapRemaining > 0)
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
        fallingObjects.Add(newItem);

        //set the transforms
        newItem.transform.SetParent(scrapMovingInterface);
        newItem.transform.localPosition = scrapSpawnArea.localPosition;
        newItem.layer = 5;
        foreach (Transform t in newItem.transform)
            t.gameObject.layer = 5;
        newItem.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 359));
        newItem.transform.localScale *= 1;

        //randomly determine if the scrap moves to the metal or dirt destination
        bool movesToMetal = Random.Range(0, 2) == 1;
        Transform dest = metalDestination;
        if (!movesToMetal)
            dest = dirtDestination;

        //start the object moving down the interface
        if (isMetal)
            newItem.transform.DOLocalMove(dest.localPosition, 3f).SetEase(Ease.Linear).OnComplete(MetalHitBottom);
        else
            newItem.transform.DOLocalMove(dest.localPosition, 3f).SetEase(Ease.Linear).OnComplete(DirtHitBottom);
    }

    private void MetalHitBottom() {
        if (IsObjectInMetalDestination(fallingObjects[0])) {
            metalForged++;
            UpdateMetalForgedText();
        }

        DestroyFirstFallingObject();
        CheckIfThereAreNoMoreFallingObjects();
    }

    private bool IsObjectInMetalDestination(GameObject go) {
        //get the object's local position
        Vector2 objPos = new Vector2(go.transform.localPosition.x, go.transform.localPosition.y);
        
        
        //get the recttransform's bounds
        RectTransform rt = metalDestination.GetComponent<RectTransform>();
        float leftSide = rt.anchoredPosition.x - rt.rect.width / 2;
        float rightSide = rt.anchoredPosition.x + rt.rect.width / 2;
        float topSide = rt.anchoredPosition.y + rt.rect.height / 2;
        float bottomSide = rt.anchoredPosition.y - rt.rect.height / 2;

        //see uf the object is inside the bounds
        if (objPos.x >= leftSide &&
            objPos.x <= rightSide &&
            objPos.y >= bottomSide &&
            objPos.y <= topSide)
                return true;
        
         return false;
    }

    private void DirtHitBottom() {
        if (IsObjectInMetalDestination(fallingObjects[0])) {
            if (metalForged > 0) {
                metalForged--;
                UpdateMetalForgedText();
            }
        }

        DestroyFirstFallingObject();
        CheckIfThereAreNoMoreFallingObjects();
    }

    private void CheckIfThereAreNoMoreFallingObjects() {
        if ((fallingObjects.Count == 0) && (scrapRemaining == 0))
            EndForging();
    }

    private void DestroyFirstFallingObject() {
        GameObject obj = fallingObjects[0];
        GameObject.Destroy(obj);
        fallingObjects.RemoveAt(0);
    }

    private void UpdateScrapRemainingText() {
        scrapRemainingText.text = scrapRemaining + " Scrap Remaining";
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
        metalDestination = scrapMovingInterface.Find("Metal Destination");
        dirtDestination = scrapMovingInterface.Find("Dirt Destination");
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

        //change the material of the forge
        Material[] newMaterials = forgeObject.GetComponent<MeshRenderer>().materials;
        newMaterials[1] = litForgeMaterial;
        forgeObject.GetComponent<MeshRenderer>().materials = newMaterials;
    }
}
