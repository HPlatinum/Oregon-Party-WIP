using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainUI : MonoBehaviour {
    
    private PauseMenu pauseMenu;
    private GameObject addItemDisplay;
    private Text addItemDisplayText;
    private bool isItemAddedPopupBeingDisplayed = false;

    private void Start() {
        pauseMenu = FindObjectOfType<PauseMenu>();
        addItemDisplay = transform.Find("Add Item Display").gameObject;
        addItemDisplayText = addItemDisplay.transform.Find("Text").GetComponent<Text>();


        HideItemBeingAddedPopup();
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
        if (!isItemAddedPopupBeingDisplayed)
            addItemDisplay.SetActive(false);
    }

    private void HideUI() {
        foreach (Transform t in transform)
            t.gameObject.SetActive(false);
    }

    public void ShowItemBeingAdded(Item item, int quantity) {
        addItemDisplayText.text = "+" + quantity + " " + item.name;

        //fade out the text
        Color transparent = Color.black;
        transparent.a = 0;
        addItemDisplayText.DOColor(transparent, 1).OnComplete(HideItemBeingAddedPopup);

        addItemDisplay.SetActive(true);

        isItemAddedPopupBeingDisplayed = true;
    }

    private void HideItemBeingAddedPopup() {
        addItemDisplayText.DOColor(Color.black, 0);
        addItemDisplay.SetActive(false);
        isItemAddedPopupBeingDisplayed = false;
    }
}
