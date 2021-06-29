using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Clothing Item", menuName = "Inventory System/Item/Clothing")]
public class Clothing : Item {
    public void Awake(){
        type = ItemType.Clothing;
    }
}

