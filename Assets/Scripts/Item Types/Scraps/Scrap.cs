using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScrapType { ScrapElectronics, ScrapFabric, ScrapFood, ScrapGlass, ScrapMetal, ScrapWood }

[CreateAssetMenu(fileName = "New Scrap", menuName = "Inventory System/Scrap")]
public class Scrap : Item
{
    [HideInInspector]
    public ScrapType scrapType;
}
