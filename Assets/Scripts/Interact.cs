using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{

    public Interactable interactSubject;
    public Inventory inventory;

    // Start is called before the first frame update
    
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnTriggerEnter(Collider obj) {

        //check if the object is interactable
        Interactable interactable = obj.GetComponent<Interactable>();
        if (interactable == null) return;

        //if it is, remove the previous interactable object
        RemoveInteractable();
        
        //then add the new interactable object
        interactSubject = interactable;
        //update the UI interact description


        //add an outline to the interactable object
        interactable.GetComponent<Outline>().enabled = true;


    }

    private void OnTriggerExit(Collider obj) {
        Interactable interactable = obj.GetComponent<Interactable>();
        if (interactable == null) return;

        if (interactable == interactSubject) {
            RemoveInteractable();
        }
    }

    private void RemoveInteractable() {
        //clears the interactable object
        //used when exiting interact range, or when entering interact range with a new object
        if (interactSubject == null) return;
        interactSubject.GetComponent<Outline>().enabled = false;
        interactSubject = null;

        //update the UI interact description
    }

    public void StartInteract() {
        //called by vThirdPersonInput.CheckInteractState
        //any code that should be run as soon as the interaction animation starts playing

    }

    public void EndInteract() {
        //called by vThirdPersonInput.CheckInteractState
        //any code that should be run as soon as the interaction animation stops playing
        if (interactSubject.interactType == Interactable.InteractTypes.Pickup) {
            Pickup();
        }
    }

    public void Pickup() {
        Item item = interactSubject.item;
        int yield = 1; //need better logic for how quantity is yielded. right now set to 1

        // validates we can pick up item and adds it to the inventory
        bool attemptPickup = inventory.AddItem(item, yield, inventory);
        // If able Destroy object
        if (attemptPickup) {
            Destroy(interactSubject.gameObject);
        }
    }

    // clears inventory and inventory weight on application quit
    private void OnApplicationQuit() {
        inventory.inventorySlot.Clear();
        inventory.currentWeight = 0;
}
} 
