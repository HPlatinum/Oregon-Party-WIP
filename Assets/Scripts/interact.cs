using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{

    public Interactable interactSubject;

    // Start is called before the first frame update
    
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnTriggerEnter(Collider obj) {

        Interactable interactable = obj.GetComponent<Interactable>();
        if (interactable == null) return;
        interactSubject = interactable;
        //update the UI interact description


    }

    private void OnTriggerExit(Collider obj) {
        Interactable interactable = obj.GetComponent<Interactable>();

        if (interactable == interactSubject) {
            interactSubject = null;

            //update the UI interact description
        }
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
        // validates we can pick up item and adds it to the inventory
        bool attemptPickup = Inventory.instance.Add(item);

        // If able Destroy object
        if (attemptPickup) {
            Destroy(interactSubject.gameObject);
        }
    }
}
