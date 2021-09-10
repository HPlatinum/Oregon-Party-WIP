using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Essentials : MonoBehaviour{

    // Start is called before the first frame update
    void Start() {
        Transform canvas = transform.Find("Canvas");

        //misc stuff
        StaticVariables.essentials = this;
        StaticVariables.playerAnimator = FindObjectOfType<Invector.vCharacterController.vThirdPersonController>().GetComponent<Animator>();
        StaticVariables.controller = FindObjectOfType<Invector.vCharacterController.vThirdPersonController>();
        StaticVariables.interactScript = FindObjectOfType<InteractionManager>();
        StaticVariables.currentMinigame = null;
        StaticVariables.tweenDummy = transform.Find("Empty Tween Dummy - For Delaying Function Calls");

        //UI elements        
        StaticVariables.itemDetails = canvas.Find("Item Details").GetComponent<ItemDetails>();
        StaticVariables.mainUI = canvas.Find("Main UI").GetComponent<MainUI>(); StaticVariables.interactButtonText = StaticVariables.mainUI.transform.Find("Interact").Find("Text").GetComponent<Text>();


        //minigames
        StaticVariables.cookingMinigame = canvas.Find("Cooking Interface").GetComponent<CookingMinigame>();
        StaticVariables.fishingMinigame = canvas.Find("Fishing Popup").GetComponent<FishingMinigame>();

        //turn on all UI elements
        //they usually hide themselves on startup, after setting local variables
        foreach (Transform t in transform.Find("Canvas")) {
            t.gameObject.SetActive(true);
        }
    }

}
