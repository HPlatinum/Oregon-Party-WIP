using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Medicinal Item", menuName = "Inventory System/Item/Medicine")]
public class Medicine : Item {
    public void Awake(){
        type = ItemType.Medicine;
    }
}