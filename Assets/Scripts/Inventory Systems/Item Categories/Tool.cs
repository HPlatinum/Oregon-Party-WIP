using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool Item", menuName = "Inventory System/Item/Tool")]
public class Tool : Item {
    [Header("Item Stats")]
    public double wear;
    public void Awake(){
        type = ItemType.Tool;
    }
}
