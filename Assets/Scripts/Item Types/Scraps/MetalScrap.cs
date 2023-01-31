using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Metal Scrap", menuName = "Inventory System/Scrap/Metal")]
public class MetalScrap : Scrap
{
    public void Awake() {
        name = "Metal Scrap";
        type = ItemType.Scrap;
        scrapType = ScrapType.ScrapMetal;
    }
    
}
