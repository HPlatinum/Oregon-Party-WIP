using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ammunition Item", menuName = "Inventory System/Item/Ammunition")]
public class Ammunition : Item {
    public void Awake(){
        type = ItemType.Ammunition;
    }
}