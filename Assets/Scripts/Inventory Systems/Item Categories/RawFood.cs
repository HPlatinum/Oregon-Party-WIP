using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Raw Food Item", menuName = "Inventory System/Item/Raw Food")]
public class RawFood : Item {

    public Item cookedVariant;

    public void Awake(){
        type = ItemType.RawFood;
    }
}
