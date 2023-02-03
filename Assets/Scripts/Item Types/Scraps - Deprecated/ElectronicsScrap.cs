using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Electronics Scrap", menuName = "Inventory System/Scrap/Electronics")]
public class ElectronicsScrap : Scrap
{
    public void Awake() {
        name = "Electronics Scrap";
        type = ItemType.Scrap;
        scrapType = ScrapType.ScrapElectronics;
    }
    
}
