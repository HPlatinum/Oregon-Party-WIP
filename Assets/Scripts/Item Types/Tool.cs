using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool Item", menuName = "Inventory System/Item/Tool")]
public class Tool : Item {
    [Header("Item Stats")]
    public double initialWear;
    public double currentWear;
    public enum ToolTypes { rod, axe, pickaxe, firelighter }
    public ToolTypes toolType;

    public void Awake(){
        type = ItemType.Tool;
        if(currentWear == 0) {
            currentWear = initialWear;
        }
    }
}