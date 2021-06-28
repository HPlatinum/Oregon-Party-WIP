using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{

    public GameObject interactSubject;

    // Start is called before the first frame update
    
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnTriggerEnter(Collider obj) {
        //if (obj.gameObject.tag == "Interact (Lifting)") {
        if (obj.gameObject.tag.Contains("Interact")) {
            interactSubject = obj.gameObject;

            //update the UI interact description
        }
    }

    private void OnTriggerExit(Collider obj) {
        if (obj.gameObject == interactSubject) {
            interactSubject = null;

            //update the UI interact description
        }
    }

    public void StartInteract() {

    }

    public void EndInteract() {
        if (interactSubject.tag.Contains("Lifting"))
        {
            pickup();
        }
    }

    public void pickup()
    {
        Item item = interactSubject.GetComponent<Interactable>().item;
        // validates we can pick up item and adds it to the inventory
        bool attemptPickup = Inventory.instance.Add(item);

        // If able Destroy object
        if (attemptPickup) {
            Destroy(interactSubject);
        }
    }
}
