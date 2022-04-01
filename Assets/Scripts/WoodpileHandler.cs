using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodpileHandler : MonoBehaviour
{
    int woodPosition;
    bool listIsFilled;
    public Item item;
    List<GameObject> woodpile;
    GameObject currentlogObject;
    // Start is called before the first frame update
    void Start()
    {
        ResetLocalVariables();
    }

    // Update is called once per frame
    void Update()
    {
        if(listIsFilled && CurrentlogObjectHasChanged()) {
            SetCurrentlogObject();
            AddInteractableScriptToCurrentObject();
            SetCurrentObjectInteractableValues();
        }

        if(woodpile[woodPosition] == null) {
            IncreaseWoodPosition();
        }
    }

    private void ResetLocalVariables() {
        listIsFilled = false;
        CreateWoodPileList();
        FillWoodPileList();
    }

    private void CreateWoodPileList() {
        woodpile = new List<GameObject>();
    }

    private void FillWoodPileList() {
        GameObject logObject = null;
        for(int i = 0; i < 14; i++) {
            if(i == 0) {
                logObject = (GameObject.Find("Log"));

            }
            else {
                logObject = GameObject.Find("Log (" + i + ")");
            }

            woodpile.Add(logObject);
        }
        listIsFilled = true;
    }
    private void SetCurrentlogObject() {
        currentlogObject = woodpile[woodPosition];
    }

    private void IncreaseWoodPosition() {
        if(woodPosition < woodpile.Count - 1) {
            woodPosition ++;
        }
        
    }

    private bool CurrentlogObjectHasChanged() {
        return currentlogObject != woodpile[woodPosition];
    }

    private void SetlogObjectInteractableAndOutlineInactive(GameObject logObject) {
        logObject.GetComponent<Interactable>().enabled = false;
    }
    
    private void AddInteractableScriptToCurrentObject() {
        Interactable currentInteractable = woodpile[woodPosition].transform.Find("Log").gameObject.AddComponent<Interactable>() as Interactable;
    }

    private void SetCurrentObjectInteractableValues() {
        woodpile[woodPosition].transform.Find("Log").gameObject.GetComponent<Interactable>().interactType = Interactable.InteractTypes.Log;
        woodpile[woodPosition].transform.Find("Log").gameObject.GetComponent<Interactable>().item = item;
        woodpile[woodPosition].transform.Find("Log").gameObject.GetComponent<Interactable>().storedItemCount = 1;
        woodpile[woodPosition].transform.Find("Log").gameObject.GetComponent<Interactable>().destroyParentAlso = true;
    }
}