using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool Item", menuName = "Inventory System/Item/Tool")]
public class Tool : Item {
    [Header("Item Stats")]
    public double wear;
    public enum ToolTypes { rod, axe, pickaxe, firelighter }
    public enum ToolTier {Tier1, Tier2, Tier3}
    public ToolTypes toolType;
    public ToolTier toolTier;
    public void Awake(){
        type = ItemType.Tool;

    }
}
