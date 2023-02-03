using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Glass Shard Scrap", menuName = "Inventory System/Scrap/Glass Shard")]
public class GlassShardScrap : Scrap
{
    public void Awake() {
        name = "Glass Shard Scrap";
        type = ItemType.Scrap;
        scrapType = ScrapType.ScrapGlass;
    }
    
}
