using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fabric Scrap", menuName = "Inventory System/Scrap/Fabric")]
public class FabricScrap : Scrap
{
    public void Awake() {
        name = "Fabric Scrap";
        type = ItemType.Scrap;
        scrapType = ScrapType.ScrapFabric;
    }
    
}
