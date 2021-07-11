using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour {

    public Interactable interactSubject; //which object is closest to the player. null means no object is within interact range
    public Inventory inventory;
    public List<Interactable> objectsInRange = new List<Interactable>(); //all of the objects that are within interact range of the player
    private bool currentlyInteracting = false;

    // Start is called before the first frame update
    
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (!currentlyInteracting) {
            //update the current interact subject
            Interactable closestObject = FindClosestObject();
            if (closestObject != interactSubject) {
                RemoveInteractable();
                interactSubject = closestObject;
                interactSubject.GetComponent<Outline>().enabled = true;
            }
        }

    }

    private void OnTriggerEnter(Collider obj) {

        //check if the object is interactable
        Interactable interactable = obj.GetComponent<Interactable>();
        if (interactable != null) {
            //add the object to the list of interactables within range
            objectsInRange.Add(interactable);

            //choose the closest object to be the interact subject
            Interactable closestObject = FindClosestObject();
            if (closestObject != interactSubject) {
                RemoveInteractable();
                interactSubject = closestObject;
                interactSubject.GetComponent<Outline>().enabled = true;
            }
        }
    }

    private void OnTriggerExit(Collider obj) {

        //check if the object is interactable
        Interactable interactable = obj.GetComponent<Interactable>();
        if (interactable != null) {
            //if the object you just left was the interact subject, unhighlight it
            if (interactable == interactSubject) {
                RemoveInteractable();
            }

            //remove the object from the list of interactables in range
            objectsInRange.Remove(interactable);

            //set a new closest object
            interactSubject = FindClosestObject();
            if (interactSubject != null) {
                interactSubject.GetComponent<Outline>().enabled = true;
            }
        }
    }

    private Interactable FindClosestObject() {
        //returns the interactable out of the list that is closest to the player

        //if no objects are in interact range, return null
        if (objectsInRange.Count == 0)
            return null;

        //if there is one object in range, it has to be the closest
        if (objectsInRange.Count == 1) {
            return objectsInRange[0];
        }

        //if there are multiple objects in range, find the closest one
        Interactable closestObject = null;
        float closestDistance = 99f;
        foreach(Interactable interactable in objectsInRange) {
            float dist = FindDistanceToPlayer(interactable);
            if (dist < closestDistance) {
                closestDistance = dist;
                closestObject = interactable;
            }
        }
        return closestObject;
    }

    private float FindDistanceToPlayer(Interactable interactable) {
        //finds the distance from the object to the player, based on the center point of each on the x-z plane
        float xpos = interactable.transform.position.x;
        float zpos = interactable.transform.position.z;

        Vector2 objPos = new Vector2(xpos, zpos);
        Vector2 playerPos = new Vector2(transform.position.x, transform.position.z);

        return Vector2.Distance(objPos, playerPos);
    }

    private void RemoveInteractable() {
        //clears the interactable object
        //used when exiting interact range, or when entering interact range with a new object
        if (interactSubject == null) return;
        interactSubject.GetComponent<Outline>().enabled = false;
        interactSubject = null;

        //update the UI interact description?
    }

    public void StartInteract() {
        //called by vThirdPersonInput.CheckInteractState
        //any code that should be run as soon as the interaction animation starts playing
        currentlyInteracting = true;
    }

    public void EndInteract() {
        //called by vThirdPersonInput.CheckInteractState
        //any code that should be run as soon as the interaction animation stops playing

        currentlyInteracting = false;

        if (interactSubject.interactType == Interactable.InteractTypes.Pickup) {
            //print("about to pickup");
            Pickup();
        }

        else if (interactSubject.interactType == Interactable.InteractTypes.Fishing) {
            //print("about to pickup");
            Pickup();
        }

        else if (interactSubject.interactType == Interactable.InteractTypes.Chest) {
            //print("about to pickup");
            Open();
        }

        #region Discard Items Test Application
        // The discard function works, however, I have no method attached for selecting and deleting. 
        // Leaving the functionality here for now. I assume this will be attached to UI buttons.

        // if (interactSubject.interactType == Interactable.InteractTypes.Discard) {
        //     Debug.Log("We are here.");
        //     int yield = 1; 
        //     bool attemptDiscard = inventory.RemoveItem(itemTest, yield, inventory);
        // }
        #endregion
    }

    public void Pickup() {
        Item item = interactSubject.item;
        int yield = 1; //need better logic for how quantity is yielded. right now set to 1
        // validates we can pick up item and adds it to the inventory
        bool attemptPickup = inventory.AddItem(item, yield, inventory);
        //print(attemptPickup);
        // If able Destroy object
        if (attemptPickup) {
            //print("picked up");
            objectsInRange.Remove(interactSubject);
            Destroy(interactSubject.gameObject);
        }
    }

    public void Open() {
        if(!interactSubject.inventoryState())
            interactSubject.inventoryState();
    }

    // clears inventory and inventory weight on application quit
    private void OnApplicationQuit() {
        inventory.inventorySlot.Clear();
        inventory.currentWeight = 0;
    }
    
    public bool IsInteractAllowed() {
        //returns if interaction with the current interactSubject is allowed
        if (interactSubject.interactType == Interactable.InteractTypes.Pickup) {
            if (inventory.CanAdd(inventory, interactSubject.item)) { //check if the player can carry the new item
                return true;
            }
        }
        if (interactSubject.interactType == Interactable.InteractTypes.Fishing) {
            if (inventory.ItemQuantity(interactSubject.requiredItem) > 0) { //check for required item (probably will be fishing rod)
                if (inventory.CanAdd(inventory, interactSubject.item)) { //check if the player can carry the new item
                    return true;
                }
            }
        }
        if (interactSubject.interactType == Interactable.InteractTypes.Chest) {
                return true; // return true since it's a chest
        }
        return false;
    }
} 
