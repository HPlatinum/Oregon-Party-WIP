using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Inventory System/Item/Weapon")]
public class Weapon : Item {
    public int damage;
    public void Awake(){
        type = ItemType.Weapon;
    }
}
