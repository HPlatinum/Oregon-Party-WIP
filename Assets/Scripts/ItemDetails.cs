using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetails : MonoBehaviour { 
    private Item item;
    public float rotationSpeed = 50f;
    private Vector2 originalModelParentPos;

    //gameobject references
    private Transform itemModelParent;
    private Text descriptionText;
    private Text nameText;

    void Start(){
        //declare the gameobject references
        itemModelParent = transform.Find("Item Model").Find("Object Parent");
        descriptionText = transform.Find("Description").Find("Text").GetComponent<Text>();
        nameText = transform.Find("Name").Find("Text").GetComponent<Text>();

        //set the starting position of the 3d model UI element
        originalModelParentPos = new Vector2(itemModelParent.localPosition.x, itemModelParent.localPosition.y);

        //hide the item details popup
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

        //add the description text
        descriptionText.text = item.description;

        //add the item name
        nameText.text = item.name;

        //show the ItemDetails UI
        ShowContents(true);

    }

    public void Close() {
        //closes the window
        ShowContents(false);

        //clear the UI elements
        foreach (Transform t in itemModelParent)
            GameObject.Destroy(t.gameObject);
        descriptionText.text = "";
        nameText.text = "";

        //clear the item reference

        item = null;
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

    public  void Trash() {
        //prompt the user if they want to destroy the item
        print("trash item");
    }

    public void BreakDown() {
        //prompt the user if they want to break down the item for components
        print("break down item");
    }

    public void MainAction() {
        //use the item's main action
        print("main action");
    }

}
