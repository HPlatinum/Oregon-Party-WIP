using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainUI : MonoBehaviour {
    
    private PauseMenu pauseMenu;
    private GameObject interactionSymbolGeneral;
    private GameObject interactionSymbolLeave;
    private GameObject interactionDarkOverlay;
    private GameObject attackSymbolSword;
    private GameObject attackSymbolGun;
    private GameObject attackDarkOverlay;
    private Transform addItemBox;
    private Text addItemText;
    private Vector3 itemQuantityStartingPos;
    public float itemQuantitySlideTime = 1f;
    public float itemQuantityStayTime = 0.5f;
    public float itemQuantityFadeTime = 0.5f;
    private bool currentlyShowingItemQuantity = false;
    private List<string> upcomingItemQuantities = new List<string>();
    private GameObject pauseButton;

    private void Start() {
        pauseMenu = FindObjectOfType<PauseMenu>();
        interactionSymbolGeneral = transform.Find("Interact").Find("InteractImageGeneral").gameObject;
        interactionSymbolLeave = transform.Find("Interact").Find("InteractImageLeave").gameObject;
        interactionDarkOverlay = transform.Find("Interact").Find("Dark Overlay").gameObject;
        attackSymbolSword = transform.Find("Attack").Find("AttackImageSword").gameObject;
        attackSymbolGun = transform.Find("Attack").Find("AttackImageGun").gameObject;
        attackDarkOverlay = transform.Find("Attack").Find("Dark Overlay").gameObject;
        addItemBox = transform.Find("Add Item Mask").Find("Add Item");
        addItemText = addItemBox.Find("Text").GetComponent<Text>();
        itemQuantityStartingPos = addItemBox.localPosition;
        pauseButton = transform.Find("Pause").gameObject;

        //set the item quantity box position to be way offscreen, to hide it at the start of the scene
        Vector3 newPos = itemQuantityStartingPos + new Vector3(400, 0, 0);
        addItemBox.localPosition = newPos;

        HideInteractionSymbol();
        HideAttackSymbols();
    }

    public void Interact() {
        StaticVariables.interactScript.StartInteractionWithCurrentInteractable();
    }

    public void Attack() {
        print("you want to attack, huh?");
    }

    public void Pause() {
        pauseMenu.PauseGame();
        HideUI();
    }
    
    public void ShowUI() {
        foreach (Transform t in transform)
            t.gameObject.SetActive(true);
    }

    public void HideUI() {
        foreach (Transform t in transform)
            t.gameObject.SetActive(false);
    }

    //new function, used for animated UI Pop-In and Pop-Out
    public IEnumerator HideUI2() {
        if (transform.GetChild(0).gameObject.activeSelf)
            yield return StaticVariables.AnimateChildObjectsDisappearing(transform);
        yield return null;
    }

    //new function, used for animated UI Pop-In and Pop-Out
    public IEnumerator ShowUI2() {
        yield return StaticVariables.AnimateChildObjectsAppearing(transform);

        //if there is not an item currently shown and there are some waiting to be shown, show them
        if (!currentlyShowingItemQuantity && (upcomingItemQuantities.Count > 0)) {
            StartDisplayItemQuantity(upcomingItemQuantities[0]);
            upcomingItemQuantities.RemoveAt(0);
        }
        yield return null;
    }

    public void ShowInteractionSymbolGeneral() {
        interactionSymbolGeneral.SetActive(true);
        interactionSymbolLeave.SetActive(false);
        interactionDarkOverlay.SetActive(false);
    }
    public void ShowInteractionSymbolLeave() {
        interactionSymbolGeneral.SetActive(false);
        interactionSymbolLeave.SetActive(true);
        interactionDarkOverlay.SetActive(false);
    }

    public void HideInteractionSymbol() {
        interactionSymbolGeneral.SetActive(false);
        interactionSymbolLeave.SetActive(false);
        interactionDarkOverlay.SetActive(true);
    }

    public void ShowAttackSymbolSword() {
        attackSymbolGun.SetActive(false);
        attackSymbolSword.SetActive(true);
        attackDarkOverlay.SetActive(false);
    }
    public void ShowAttackSymbolGun() {
        attackSymbolGun.SetActive(true);
        attackSymbolSword.SetActive(false);
        attackDarkOverlay.SetActive(false);
    }

    public void HideAttackSymbols() {
        attackSymbolGun.SetActive(false);
        attackSymbolSword.SetActive(false);
        attackDarkOverlay.SetActive(true);
    }

    public void ShowItemQuantityChange(Item item, int quantity) {
        if (currentlyShowingItemQuantity || !IsPauseButtonShowing()) { //if there is an item currently showing, or the main ui is hidden, add the new item to the queue instead
            upcomingItemQuantities.Add(CreateStringFromItemAndQuantity(item, quantity));
            return;
        }
        StartDisplayItemQuantity(CreateStringFromItemAndQuantity(item, quantity));
    }

    private void StartDisplayItemQuantity(string displayString) {
        currentlyShowingItemQuantity = true;
        addItemText.text = displayString;
        LayoutRebuilder.ForceRebuildLayoutImmediate(addItemBox.GetComponent<RectTransform>());

        StartItemQuantityChangeSlideIn();
    }

    private string CreateStringFromItemAndQuantity(Item item, int quantity) {
        if (quantity > 0)
            return ("+" + quantity + " " + item.name);
        else
            return("-" + (quantity * -1f) + " " + item.name);
    }

    private void StartItemQuantityChangeSlideIn() {

        SetItemQuantityBoxStartingPosition();

        //move it to the new position
        addItemBox.DOLocalMove(itemQuantityStartingPos, itemQuantitySlideTime).OnComplete(ItemQuantityKeepShown);
    }

    private void ItemQuantityKeepShown() {
        StaticVariables.WaitTimeThenCallFunction(itemQuantityStayTime, StartItemQuantityChangeFadeOut);
    }

    private void SetItemQuantityBoxStartingPosition() {

        //the box should start sliding in from being completely hidden - its x coordinate should be shifted by its width
        Vector3 newPos = itemQuantityStartingPos + new Vector3(addItemBox.GetComponent<RectTransform>().rect.width, 0, 0);
        addItemBox.localPosition = newPos;
    }

    private void StartItemQuantityChangeFadeOut() {

        //image
        Image im = addItemBox.GetComponent<Image>();
        Color imageColor = im.color;
        imageColor.a = 0;
        im.DOColor(imageColor, itemQuantityFadeTime);

        //text
        Color textColor = addItemText.color;
        textColor.a = 0;
        addItemText.DOColor(textColor, itemQuantityFadeTime).OnComplete(EndItemQuantityChangeFadeOut);
    
    }

    private void EndItemQuantityChangeFadeOut() {
        Image im = addItemBox.GetComponent<Image>();
        Color imageColor = im.color;
        imageColor.a = 1;
        im.color = imageColor;

        //text
        Color textColor = addItemText.color;
        textColor.a = 1;
        addItemText.color = textColor;

        SetItemQuantityBoxStartingPosition();
        currentlyShowingItemQuantity = false;
        
        //if there are new items waiting to be displayed, start that process now
        if (upcomingItemQuantities.Count > 0) {
            StartDisplayItemQuantity(upcomingItemQuantities[0]);
            upcomingItemQuantities.RemoveAt(0);
        }
    }

    private bool IsPauseButtonShowing() {
        return pauseButton.activeSelf;
    }


}
