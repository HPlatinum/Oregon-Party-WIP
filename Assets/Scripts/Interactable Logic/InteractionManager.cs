using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour {

    //nearby objects that the player can interact with
    public Interactable closestInteractable = null; //null means no object is within interact range
    [SerializeField] private List<Interactable> interactablesInRange = new List<Interactable>();

    //tracking the item in the player's hand
    public GameObject objectInHand;
    public Item itemInHand;

    //interaction state trackers
    public bool currentlyInteracting = false;
    private bool interactAnimationStarted = false;

    //transforms for the player's hands, to add held items to
    public Transform rightHand;
    public Transform leftHand;

    //holdable items
    [Header("Tools")]
    public Item fishingRod;
    public Item axe;
    public Item pickaxe;

    //misc
    public Inventory inventory;
    public bool removeItemWhenFinishedWithInteraction = true;
    private bool doNotPickupAfterAnimation = false;

    void Start() {
        StaticVariables.playerInventory = inventory;
    }

    void Update() {
        if (!currentlyInteracting)
            UpdateClosestInteractable();

        bool justStarted = CheckIfInteractAnimationJustStarted();
        if (justStarted)
            ProcessInteractAnimationStarting();

        bool justEnded = CheckIfInteractAnimationJustEnded();
        if (justEnded)
            ProcessInteractAnimationEnding();
    }

    private void UpdateClosestInteractable() {
        Interactable closestObject = GetClosestInteractable();
        if ((closestObject != null) && (closestObject != closestInteractable)) {
            UnhighlightInteractable(closestInteractable);
            closestInteractable = closestObject;
            HighlightClosestInteractable();
        }
    }

    private void HighlightClosestInteractable() {
        if (closestInteractable != null)
            closestInteractable.GetComponent<Outline>().enabled = true;
    }

    private void UnhighlightInteractable(Interactable interactable) {
        if (interactable != null)
            interactable.GetComponent<Outline>().enabled = false;
    }

    private void UnhighlightInteractable(GameObject go) {
        UnhighlightInteractable(go.GetComponent<Interactable>());
    }

    private void OnTriggerEnter(Collider obj) {
        if (!currentlyInteracting) {
            AddObjectToInteractablesListIfInteractable(obj.gameObject);
        }
    }

    private void OnTriggerExit(Collider obj) {
        GameObject go = obj.gameObject;
        if (!currentlyInteracting) {
            RemoveObjectFromInteractablesList(go);
            UnhighlightInteractable(go);
            if (closestInteractable == obj.GetComponent<Interactable>()) {
                closestInteractable = null;
            }
        }
    }

    private void AddObjectToInteractablesListIfInteractable(GameObject go) {
        Interactable interactable = go.GetComponent<Interactable>();
        if (interactable != null) 
            interactablesInRange.Add(interactable);
    }

    private void RemoveObjectFromInteractablesList(GameObject go) {
        Interactable interactable = go.GetComponent<Interactable>();
        if ((interactable != null) && interactablesInRange.Contains(interactable))
            interactablesInRange.Remove(interactable);
    }

    public Interactable GetClosestInteractable() {
        if (interactablesInRange.Count == 0)
            return null;
        if (interactablesInRange.Count == 1)
            return interactablesInRange[0];

        //if there are multiple objects in range, find the closest one
        Interactable closest = null;
        float closestDist = 99f;
        foreach(Interactable interactable in interactablesInRange) {
            float dist = FindDistanceToPlayer(interactable);
            if (dist < closestDist) {
                closestDist = dist;
                closest = interactable;
            }
        }
        return closest;
    }

    private float FindDistanceToPlayer(Interactable interactable) {
        float xpos = interactable.transform.position.x;
        float zpos = interactable.transform.position.z;
        Vector2 objPos = new Vector2(xpos, zpos);
        Vector2 playerPos = new Vector2(transform.position.x, transform.position.z);
        return Vector2.Distance(objPos, playerPos); //distance is based on the center point of the object and the player, on the x-z plane
    }

    private void ProcessInteractAnimationEnding() {
        SetVariablesOnInteractAnimationEnd();
        if (doNotPickupAfterAnimation) {
            doNotPickupAfterAnimation = false;
            return;
        }
            
        //pass along the function call to the current minigame
        if (StaticVariables.currentInteractionHandler != null)
            StaticVariables.currentInteractionHandler.ProcessInteractAnimationEnding();

        //else if (closestInteractable.interactType == Interactable.InteractTypes.Chest)
        //    OpenChest();
    }

    private void SetVariablesOnInteractAnimationEnd() {
        interactAnimationStarted = false;
        currentlyInteracting = false;
        StaticVariables.controller.lockMovement = false;
        StaticVariables.controller.lockRotation = false;
    }

    public void AddCurrentInteractableItemToInventory() {
        Item item = closestInteractable.item;
        int amount = 1; //need better logic for how quantity is determined
        inventory.AddItemToInventory(item, amount);
    }

    public void DestroyCurrentInteractable() {
        interactablesInRange.Remove(closestInteractable);
        if (closestInteractable.destroyParentAlso)
            Destroy(closestInteractable.transform.parent.gameObject);
        else
            Destroy(closestInteractable.gameObject);
    }

    /*
    public void OpenChest() {
        // if(!interactSubject.inventoryState())
        //     interactSubject.inventoryState();
        print("Opening chest...");
    }
    */

    // clears inventory and inventory weight on application quit
    private void OnApplicationQuit() {
        inventory.inventorySlots.Clear();
        
    }
    
    private bool CanPlayerInteractWithCurrentInteractable() {
        Interactable.InteractTypes type = closestInteractable.interactType;
        
        Item item = closestInteractable.item;
        //after all of the different handlers process their own interact conditions, we can just do
        return GetInteractionHandlerForClosestInteractable().CanPlayerInteractWithObject(closestInteractable);
    }
    
    private InteractionHandler GetInteractionHandlerForInteractable(Interactable interactable) {
        switch (interactable.interactType){
            case (Interactable.InteractTypes.Fishing):
                    return StaticVariables.fishingHandler;
            case (Interactable.InteractTypes.CookingTier1):
                return StaticVariables.cookingHandler;
            case (Interactable.InteractTypes.CookingTier2):
                return StaticVariables.cookingHandler;
            case (Interactable.InteractTypes.Pickup):
                return StaticVariables.pickupHandler;
            case (Interactable.InteractTypes.Woodcutting):
                return StaticVariables.woodcuttingHandler;
            case (Interactable.InteractTypes.Mining):
                return StaticVariables.miningHandler;
        }
        return null;
    }

    public InteractionHandler GetInteractionHandlerForClosestInteractable() {
        return GetInteractionHandlerForInteractable(closestInteractable);
    }

    public void PutItemInPlayerHand(Item item, bool useRightHand) {
        //only allow one object in the hand at a time
        if (objectInHand != null)
            RemoveItemFromHand();

        //use the correct hand
        Transform hand = leftHand;
        if (useRightHand) hand = rightHand;

        GameObject newObj = InstantiatePrefabAsChild(item.model, hand, item);
        RotateObjectToFitInHand(newObj, hand, item);
        TurnOffCollidersOnObject(newObj);

        //assumes we only have one object in the hand at a time
        objectInHand = newObj;
        itemInHand = item;
    }

    private void TurnOffCollidersOnObject(GameObject go) {
        //move to StaticVariables?
        CapsuleCollider cc = go.GetComponent<CapsuleCollider>();
        SphereCollider sc = go.GetComponent<SphereCollider>();
        BoxCollider bc = go.GetComponent<BoxCollider>();
        if (cc != null) Destroy(cc);
        if (sc != null) Destroy(sc);
        if (bc != null) Destroy(bc);
    }

    private GameObject InstantiatePrefabAsChild(GameObject prefab, Transform parent, Item item) {
        GameObject newObj = Instantiate(prefab);
        newObj.transform.SetParent(parent);
        newObj.transform.localPosition = item.inHandPosition; // positions object in hand
        return newObj;
    }

    private void RotateObjectToFitInHand(GameObject go, Transform hand, Item item) {
        Vector3 newRotation;
        newRotation = item.inHandRotation;

        print("Game Object " + go);
        print(go.transform.position);
        
        go.transform.localEulerAngles = newRotation;
    }

    public void RemoveItemFromHand() {
        if (objectInHand != null) {
            DestroyImmediate(objectInHand);
            itemInHand = null;
        }
    }

    public void StartInteractionWithCurrentInteractable() {
        if (StaticVariables.currentInteractionHandler != null) {
            PassInteractionToCurrentHandler();
            return;
        }
        else if (closestInteractable == null)
            return;
        else if (!CanPlayerInteractWithCurrentInteractable()) {
            print("you cannot perform the " + closestInteractable.interactType.ToString() + " action");
            StaticVariables.SetupPlayerInteractionWithHighlightedObject();
            doNotPickupAfterAnimation = true;
            StaticVariables.PlayAnimation("Shrugging");
            return;
        }
        //after all the different handlers process their own setup conditions, we can just do
        StaticVariables.currentInteractionHandler = GetInteractionHandlerForClosestInteractable();
        StaticVariables.currentInteractionHandler.ProcessInteractAction();
    }

    private void PassInteractionToCurrentHandler() {
        if (StaticVariables.currentInteractionHandler != null) 
            StaticVariables.currentInteractionHandler.ProcessInteractAction();
    }

    private void PreparePlayerForInteraction() {
        currentlyInteracting = true;
        interactAnimationStarted = false;
        StaticVariables.controller.lockMovement = true;
        StaticVariables.controller.lockRotation = true;
        StaticVariables.controller.HaltPlayerVelocity();
    }

    public void SetupPlayerInteractionWithClosestInteractable(float transitionDuration = 0.2f) {
        PreparePlayerForInteraction();
        StaticVariables.controller.FaceTo(StaticVariables.interactScript.closestInteractable.gameObject, transitionDuration);
    }

    private bool CheckIfInteractAnimationJustStarted() {
        if (currentlyInteracting) {
            if (!interactAnimationStarted) {
                if (StaticVariables.DoesPlayerAnimatorStateHaveInteractTag()) {
                    return true;
                }
            }
        }
        return false;
    }

    private void ProcessInteractAnimationStarting() {
        interactAnimationStarted = true;
    }

    private bool CheckIfInteractAnimationJustEnded() {
        if (currentlyInteracting) {
            if (interactAnimationStarted) {
                if (!StaticVariables.DoesPlayerAnimatorStateHaveInteractTag()) {
                    return true;
                }
            }
        }
        return false;
    }

} 
