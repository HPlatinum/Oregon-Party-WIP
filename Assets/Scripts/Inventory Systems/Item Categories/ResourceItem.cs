using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Item", menuName = "Inventory System/Item/Resource")]
public class ResourceItem : Item {
    public int damage;
    public void Awake(){
        type = ItemType.Resource;
    }
}
