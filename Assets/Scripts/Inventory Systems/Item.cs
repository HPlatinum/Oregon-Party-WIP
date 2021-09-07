using UnityEngine;

public enum ItemType { Ammunition, Clothing, Food, Instrument, Medicine, Tool, Weapon, Resource, RawFood }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Item")]
public class Item : ScriptableObject {
    // creates a scriptableObject "Item" and assigns values to it that can be modified in Unity i.e. Weight, stacklimit, etc.

    new public string name = "New Item"; 
    public ItemType type;
    public bool defaultItem = false;
    [TextArea(15,20)]
    public string description;
    public int stackLimit = 1;

    [Header("Item Details Model")]
    public GameObject model = null;
    public float modelScale = 1f;
    public Vector3 modelRotation = Vector3.zero;
    public Vector2 modelPosition = Vector2.zero;

    [Header("Custom Transform In Player Hand")]
    public Vector3 inHandPosition = Vector3.zero;
    public Vector3 inHandRotation = Vector3.zero;
    public float inHandScale = 1f;
    public bool useRightHand;
}
