using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum InteractTypes { Pickup };
    public InteractTypes interactType;
    public Item item;

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
