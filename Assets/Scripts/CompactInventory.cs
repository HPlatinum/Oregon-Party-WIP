using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CompactInventory : MonoBehaviour {
    
    public GameObject inventorySpacePrefab;
    private Transform inventoryParent;
    private InventorySlotUI.OnClickEffect onClick;
    private Text titleText;

    void Start() {
        AssignLocalVariables();
    }
    
    private void AssignLocalVariables() {
        inventoryParent = transform.Find("ItemsParent");
        titleText = transform.Find("Title").Find("Text").GetComponent<Text>();
    }

    public void SetupValues(InventorySlotUI.OnClickEffect onClick, string inventoryTitle) {
        this.onClick = onClick;
        titleText.text = inventoryTitle;
    }

    public void DisplayAllItemsOfTypeFromInventory(ItemType itemType, Inventory inventory) {
        List<(Item, int)> itemData = inventory.GetListOfItemsWithType(itemType);
        //PrintTupleListContents(itemData, itemType);

        foreach ((Item, int) tuple in itemData) {
            Item item = tuple.Item1;
            int quantity = tuple.Item2;
            
            GameObject newSlot = Instantiate(inventorySpacePrefab);
            SetupInstanceTransform(newSlot);

            InventorySlotUI s = newSlot.GetComponent<InventorySlotUI>();
            s.Setup();
            s.AddItemToInventorySlot(item);
            s.DisplayItemQuantity(quantity);
            s.clickEffect = onClick;
        }
    }

    private void SetupInstanceTransform(GameObject obj) {
        obj.transform.SetParent(inventoryParent);
        obj.transform.localScale = new Vector3(1, 1, 1);
    } 
    
    private void PrintTupleListContents(List<(Item, int)> list, ItemType itemType) {
        foreach ((Item, int) tuple in list) 
            print(itemType.ToString()+ " found: " + tuple.Item1 + ", with quantity " + tuple.Item2);
        if (list.Count == 0) 
            print("No " + itemType.ToString() + " found in inventory");
    }
}
