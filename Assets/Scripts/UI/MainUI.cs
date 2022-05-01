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

    private void Start() {
        pauseMenu = FindObjectOfType<PauseMenu>();
        interactionSymbolGeneral = transform.Find("Interact").Find("InteractImageGeneral").gameObject;
        interactionSymbolLeave = transform.Find("Interact").Find("InteractImageLeave").gameObject;
        interactionDarkOverlay = transform.Find("Interact").Find("Dark Overlay").gameObject;
        attackSymbolSword = transform.Find("Attack").Find("AttackImageSword").gameObject;
        attackSymbolGun = transform.Find("Attack").Find("AttackImageGun").gameObject;
        attackDarkOverlay = transform.Find("Attack").Find("Dark Overlay").gameObject;
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

}
