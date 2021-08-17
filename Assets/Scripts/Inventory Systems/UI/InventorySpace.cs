using UnityEngine;
using UnityEngine.UI;

public class InventorySpace : MonoBehaviour
{
    public Image icon;

    Item item;

    public void AddItem(Item newItem){
        item = newItem;
        icon.enabled = true;
    }

    public void ClearSlot(){
        item = null;

        icon.sprite = null;
        icon.enabled = false;
    }
}
