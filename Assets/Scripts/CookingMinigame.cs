using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CookingMinigame : Minigame {



    #region Inherited Functions

    public override void ProcessInteractAction() {

    }

    public override void ProcessInteractAnimationEnding() {

    }

    #endregion

    void Start() {
        AssignLocalVariables();
        HideUI();
    }

    private void Update() {

    }


    private void AssignLocalVariables() {

    }

    public void ShowUI() {
        foreach (Transform t in transform) {
            t.gameObject.SetActive(true);
        }
    }

    public void HideUI() {
        foreach (Transform t in transform) {
            t.gameObject.SetActive(false);
        }
    }

}
