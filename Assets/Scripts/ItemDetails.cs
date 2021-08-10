using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDetails : MonoBehaviour { 
    private Item item;
    private Transform itemModelParent;
    public float rotationSpeed = 50f;
    private Vector2 originalModelParentPos;
    
    void Start(){
        itemModelParent = transform.Find("Item Model").Find("Object Parent");
        originalModelParentPos = new Vector2(itemModelParent.localPosition.x, itemModelParent.localPosition.y);
        ShowContents(false);
    }

    private void Update() {
        itemModelParent.Rotate(0, (rotationSpeed * Time.deltaTime), 0);
    }

    private void ShowContents(bool show) {
        //if true, sets all child objects to active. if false, sets to inactive
        foreach (Transform t in transform) {
            t.gameObject.SetActive(show);
        }
    }

    public void DisplayItem(Item item) {
        //called when the player taps an item in the inventory
        //shows the item info and a close-up of the 3d model

        //set variables
        this.item = item;

        //create the 3d model instance and position it correctly
        GameObject newModel = GameObject.Instantiate(item.model, itemModelParent);
        newModel.transform.localPosition = Vector3.zero;
        SetLayerRecursively(newModel, 5); //assumes UI layer is #5
        newModel.transform.localScale = newModel.transform.localScale * item.modelScale;
        newModel.transform.Rotate(item.modelRotation);
        itemModelParent.localPosition = new Vector3 (originalModelParentPos.x + item.modelPosition.x, originalModelParentPos.y + item.modelPosition.y, itemModelParent.localPosition.z);

        //show the ItemDetails UI
        ShowContents(true);

    }

    public void Close() {
        //closes the window
        ShowContents(false);
        item = null;
        foreach (Transform t in itemModelParent) {
            GameObject.Destroy(t.gameObject);
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer) {
        //sets the object and all children to be in the specified layer
        if (obj == null) {
            return;
        }
        obj.layer = newLayer;
        foreach (Transform child in obj.transform) {
            if (child == null) {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

}
