using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    private MainUI mainUI;

    public void Start() {
        mainUI = FindObjectOfType<MainUI>();
        ShowMenu(false);

        //keep the inventory UI on, so it can run its start function
        FindObjectOfType<InventoryUI>().gameObject.SetActive(true);
    }
    

    public void ShowMenu(bool show) {
        //if true, sets all child objects to active. if false, sets to inactive
        foreach (Transform t in transform) {
            t.gameObject.SetActive(show);
        }
    }

    public void Close() {
        //open the pause menu, with the inventory inside it
        ShowMenu(false);
        mainUI.ShowUI(true);
    }

    public void PauseGame() {
        ShowMenu(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame() {
        ShowMenu(false);
        mainUI.ShowUI(true);
        Time.timeScale = 1f;
    }
}
