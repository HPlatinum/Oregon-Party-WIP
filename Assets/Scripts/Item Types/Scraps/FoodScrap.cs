using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food Scrap", menuName = "Inventory System/Scrap/Food")]
public class FoodScrap : Scrap
{
    public void Awake() {
        name = "Food Scrap";
        type = ItemType.Scrap;
        scrapType = ScrapType.ScrapFood;
    }
    
}
