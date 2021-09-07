using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food Item", menuName = "Inventory System/Item/Food")]
public class RawFood : Item {
    public void Awake(){
        type = ItemType.RawFood;
    }
}
