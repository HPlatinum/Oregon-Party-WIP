using UnityEngine;

public enum ItemType { Ammunition, Clothing, Food, Instrument, Medicine, Tool, Weapon }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Item")]
public class Item : ScriptableObject {

    new public string name = "New Item"; 
    public ItemType type;
    public bool defaultItem = false;
    [TextArea(15,20)]
    public string description;
    public int stackLimit = 1;
    public float weight = 1; // by kg

    [Header("Item Details Model")]
    public GameObject model = null;
    public float modelScale = 1f;
    public Vector3 modelRotation = Vector3.zero;
    public Vector2 modelPosition = Vector2.zero;
}
