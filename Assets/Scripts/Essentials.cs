using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Essentials : MonoBehaviour{

    // Start is called before the first frame update
    void Start() {
        StaticVariables.playerAnimator = FindObjectOfType<Invector.vCharacterController.vThirdPersonController>().GetComponent<Animator>();
        StaticVariables.controller = FindObjectOfType<Invector.vCharacterController.vThirdPersonController>();
        StaticVariables.fishingMinigame = transform.Find("Canvas").Find("Fishing Popup").GetComponent<FishingMinigame>();
        StaticVariables.essentials = this;
        StaticVariables.interactScript = FindObjectOfType<InteractionManager>();
        StaticVariables.currentMinigame = null;
        StaticVariables.interactButtonText = FindObjectOfType<MainUI>().transform.Find("Interact").Find("Text").GetComponent<Text>();
        StaticVariables.tweenDummy = transform.Find("Empty Tween Dummy - For Delaying Function Calls");
        StaticVariables.mainUI = transform.Find("Canvas").Find("Main UI").GetComponent<MainUI>();
        StaticVariables.cookingMinigame = transform.Find("Canvas").Find("Cooking Interface").GetComponent<CookingMinigame>();

        foreach (Transform t in transform.Find("Canvas")) {
            t.gameObject.SetActive(true);
        }
    }

}
