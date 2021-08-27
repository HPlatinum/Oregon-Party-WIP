using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour {

    //private Invector.vCharacterController.vThirdPersonController characterController;
    private PauseMenu pauseMenu;

    private void Start() {
        //characterController = FindObjectOfType<Invector.vCharacterController.vThirdPersonController>();
        pauseMenu = FindObjectOfType<PauseMenu>();
    }

    public void Interact() {
        //when the player pushes the interact button

        StaticVariables.interactScript.StartInteractionWithCurrentInteractable();
    }

    public void Attack() {
        print("you want to attack, huh?");
    }

    public void Pause() {
        //open the pause menu, with the inventory inside it
        pauseMenu.PauseGame();
        ShowUI(false);
    }

    public void ShowUI(bool show) {
        //if true, sets all child objects to active. if false, sets to inactive
        foreach (Transform t in transform) {
            t.gameObject.SetActive(show);
        }
    }
}
