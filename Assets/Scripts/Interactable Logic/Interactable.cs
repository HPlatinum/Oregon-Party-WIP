using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum InteractTypes { Pickup, Fishing, Chest, CookingTier1, CookingTier2, Woodcutting, Mining, Forge, SharpeningStation, Deposit, Log, Beaver };
    public InteractTypes interactType;
    public Item item;
    public Item requiredItem; //only allow interaction if the required item is in the inventory
    public Inventory inventory;
    // public GameObject interactableUI;
    public int storedItemCount;
    public bool destroyParentAlso = false;
    public bool resourceMined = false;

    public void Start() {

        //set up the outline script
        Outline outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        Color outlineColor;
        ColorUtility.TryParseHtmlString("#00FF41", out outlineColor);
        outline.OutlineColor = outlineColor;
        outline.OutlineWidth = 4;

        //by default, hide the outline
        outline.enabled = false;
    }

    public Item GetItem() {
        return item;
    }

    public void SetResourceMinedTrue() {
        resourceMined = true;
    }

    // can be used to toggle chest/trunk inventories. You may have a better way of doing this though
    // public bool inventoryState() {
    //     interactableUI.SetActive(!interactableUI.activeSelf);
    //     return(interactableUI.activeSelf);
    // }
}
