using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wood Scrap", menuName = "Inventory System/Scrap/Wood")]
public class WoodScrap : Scrap
{
    public void Awake() {
        name = "Wood Scrap";
        scrapType = ScrapType.ScrapWood;
    }
    
}
