using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Instrument Item", menuName = "Inventory System/Item/Instrument")]
public class Instrument : Item {
    public void Awake(){
        type = ItemType.Instrument;
    }
}