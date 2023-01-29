using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VehicleHandler : InteractionHandler {

    #region Inherited Functions

    public override void ProcessInteractAction() {
        //play some car entering animation?
        //fade the screen out
        //then jump to another scene
        print("you enter the car and leave");
        SceneManager.LoadScene("Mushroom-Picking Minigame");
    }

    public override void ProcessInteractAnimationEnding() {

    }

    public override bool CanPlayerInteractWithObject(Interactable interactable) {
        return true;
    }

    #endregion
}
