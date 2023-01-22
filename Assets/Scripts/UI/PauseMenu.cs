using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    private MainUI mainUI;
    private Text healthText;
    private Text sanityText;

    public void Start() {
        mainUI = FindObjectOfType<MainUI>();
        SetHealthAndSanityTextObjects();
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
        mainUI.ShowUIWithoutAnimation();
    }

    public void PauseGame() {
        ShowMenu(true);
        ShowHealthAndSanity();
        Time.timeScale = 0f;
    }

    public void ResumeGame() {
        ShowMenu(false);
        mainUI.ShowUIWithoutAnimation();
        Time.timeScale = 1f;
    }

    private void ShowHealthAndSanity(){

        healthText.text = StaticVariables.healthAndSanityTracker.currentHealth + "/" + StaticVariables.healthAndSanityTracker.maxHealth;
        sanityText.text = StaticVariables.healthAndSanityTracker.currentSanity + "/" + StaticVariables.healthAndSanityTracker.maxSanity;
    }

    private void SetHealthAndSanityTextObjects(){
        healthText = transform.Find("Health and Sanity").Find("Health Fraction").Find("Text").GetComponent<Text>();
        sanityText = transform.Find("Health and Sanity").Find("Sanity Fraction").Find("Text").GetComponent<Text>();
    }
}
