using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CommonUI : MonoBehaviour {
    
    private GameObject addItemDisplay;
    private Text addItemDisplayText;
    private bool isItemAddedPopupBeingDisplayed = false;

    private void Start() {
        addItemDisplay = transform.Find("Add Item Display").gameObject;
        addItemDisplayText = addItemDisplay.transform.Find("Text").GetComponent<Text>();


        HideItemBeingAddedPopup();
    }
    

    public void ShowItemQuantityChange(Item item, int quantity) {
        if (quantity > 0)
            addItemDisplayText.text = "+" + quantity + " " + item.name;
        else
            addItemDisplayText.text = "-" + (quantity * -1f) + " " + item.name;

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
