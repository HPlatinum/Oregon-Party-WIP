using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodpileHandler : MonoBehaviour
{
    int woodPosition;
    bool listIsFilled;
    public Item item;
    public List<GameObject> woodpile;
    GameObject currentlogObject;
    // Start is called before the first frame update
    void Start()
    {
        listIsFilled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(StaticVariables.woodcuttingHandler.gameIsStarted) {
            if(!listIsFilled && StaticVariables.woodcuttingHandler.woodpile != null) {
                ResetLocalVariables();
            }
            if(listIsFilled) {
                if(CurrentlogObjectHasChanged()) {
                    SetCurrentlogObject();
                    AddInteractableScriptToCurrentObject();
                    SetCurrentObjectInteractableValues();
                }
                if(woodpile[woodPosition] == null) {
                    IncreaseWoodPosition();
                }
            }
        }
        
    }

    private void ResetLocalVariables() {
        woodpile = StaticVariables.woodcuttingHandler.woodpile;
        listIsFilled = true;
        woodPosition = 0;
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