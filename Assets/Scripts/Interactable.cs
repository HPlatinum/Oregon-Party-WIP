using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum InteractTypes { Pickup, Fishing };
    public InteractTypes interactType;
    public Item item;
    public Item requiredItem; //only allow interaction if the required item is in the inventory

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
}
