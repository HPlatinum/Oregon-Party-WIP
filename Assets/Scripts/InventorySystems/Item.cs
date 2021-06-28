using UnityEngine;

public enum ItemType { Ammunition, Clothing, Food, Instrument, Medicine, Tool, Weapon }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Item")]
public class Item : ScriptableObject {

    new public string name = "New Item"; 
    public ItemType type;
    public Sprite icon = null; 
    public bool defaultItem = false;  
    [TextArea(15,20)]
    public string description;
    public int stackLimit;
}
