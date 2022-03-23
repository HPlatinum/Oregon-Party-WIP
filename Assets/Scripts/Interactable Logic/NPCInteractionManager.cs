using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractionManager : MonoBehaviour {

    //nearby objects that the player can interact with
    public Interactable closestInteractable = null; //null means no object is within interact range
    [SerializeField] private List<Interactable> interactablesInRange = new List<Interactable>();

    //tracking the item in the player's hand
    public GameObject objectInHand;
    public Item itemInHand;
    public Item itemInHandBeforeInteraction = null;

    //interaction state trackers
    public bool currentlyInteracting = false;
    private bool interactAnimationStarted = false;

    //transforms for the player's hands, to add held items to
    public Transform rightHand;
    public Transform leftHand;

    //misc
    public Inventory inventory;
    private bool doNotPickupAfterAnimation = false;


    public GameObject dustPrefab;
    void Update() {
        if (!currentlyInteracting)
            UpdateClosestInteractable();
    }

    private void UpdateClosestInteractable() {
        Interactable closestObject = GetClosestInteractable();
        if ((closestObject != null) && (closestObject != closestInteractable)) {
            closestInteractable = closestObject;
        }
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

    public void SetVariablesOnInteractAnimationEnd() {
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
        Interactable i = closestInteractable;
        closestInteractable = null;
        interactablesInRange.Remove(i);
        if (i.destroyParentAlso)
            i.transform.parent.gameObject.AddComponent<AnimatedObjectRemoval>().dustPrefab = dustPrefab;
        else
            i.gameObject.AddComponent<AnimatedObjectRemoval>().dustPrefab = dustPrefab;
        Destroy(i);
    }

    public void DestroyInteractable(Transform newObj) {
        newObj.gameObject.AddComponent<AnimatedObjectRemoval>().dustPrefab = dustPrefab;
    }
    private void OnApplicationQuit() {
        inventory.inventorySlots.Clear();
        
    }
    
    private bool CanNPCInteractWithCurrentInteractable() {
        if (closestInteractable == null)
            return false;
        Interactable.InteractTypes type = closestInteractable.interactType;
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
            case (Interactable.InteractTypes.Forge):
                return StaticVariables.forgeHandler;
            case (Interactable.InteractTypes.SharpeningStation):
                return StaticVariables.sharpeningHandler;
            case (Interactable.InteractTypes.Deposit):
                return StaticVariables.depositHandler;
        }
        return null;
    }

    public InteractionHandler GetInteractionHandlerForClosestInteractable() {
        return GetInteractionHandlerForInteractable(closestInteractable);
    }

    public void PutItemInNPCHand(Item item) {
        //only allow one object in the hand at a time
        if (objectInHand != null)
            RemoveItemFromHand();

        //use the correct hand
        Transform hand = leftHand;
        if (item.useRightHand) hand = rightHand;
        GameObject newObj = InstantiatePrefabAsChild(item.model, hand, item);
        if(newObj.GetComponent<Rigidbody>() != null) {
            Destroy(newObj.GetComponent<Rigidbody>());
        }
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
        newObj.transform.localPosition = item.inBeaverHandPosition; // positions object in hand

        return newObj;
    }

    private void RotateObjectToFitInHand(GameObject go, Transform hand, Item item) {
        Vector3 newRotation;
        newRotation = item.inBeaverHandRotation;
        
        go.transform.localEulerAngles = newRotation;
    }

    public void RemoveItemFromHand() {
        if (objectInHand != null) {
            print("Removing Item From hand for some reason");
            DestroyImmediate(objectInHand);
            itemInHand = null;
        }
    }
} 
