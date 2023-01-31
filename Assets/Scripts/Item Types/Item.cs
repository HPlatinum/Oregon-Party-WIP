using UnityEngine;

public enum ItemType { Ammunition, Clothing, Food, Instrument, Medicine, Tool, Weapon, Resource, RawFood, Scrap }
public enum WeaponType { Punch, OneHandRight, TwoHand }//OneHandLeft, Gun, TwoHandGun 

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Item")]
public class Item : ScriptableObject {
    // creates a scriptableObject "Item" and assigns values to it that can be modified in Unity i.e. Weight, stacklimit, etc.

    new public string name = "New Item"; 
    [HideInInspector]
    public ItemType type;
    public WeaponType weaponType = WeaponType.Punch;
    public bool defaultItem = false;
    [TextArea(15,20)]
    public string description;

    [Header("Scrap Returns")]
    public int stackLimit = 1;
    public int fabricScraps = 0;
    public int electronicScraps = 0;
    public int foodScrapReturn = 0;
    public int glassShardScraps = 0;
    public int metalScrapReturn  = 0;
    public int woodScrapReturn = 0;

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

    [Header("Custom Transform In Beaver Hand")]
    public Vector3 inBeaverHandPosition = Vector3.zero;
    public Vector3 inBeaverHandRotation = Vector3.zero;
    public float inBeaverHandScale = 1f;
    public bool useBeaverRightHand;

    [Header ("Universal Item Stats")]
    public int itemTier;
}