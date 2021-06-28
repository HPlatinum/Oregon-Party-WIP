using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool Item", menuName = "Inventory System/Item/Tool")]
public class Tool : Item {
    public void Awake(){
        type = ItemType.Food;
    }
}
