using UnityEngine;
using UnityEngine.UI;

public class InventorySpace : MonoBehaviour
{
    public Image icon; // need to make this the model GameObject maybe?

    Item item;

    // sets the item and icon for the inventoryUI space. Needs to be swapped to a 3d model.
    public void AddItem(Item newItem){
        item = newItem;
        icon.enabled = true;
    }

    // sets the item in the slot to null and clears the sprit/disables it.
    public void ClearSlot(){
        item = null;

        icon.sprite = null;
        icon.enabled = false;
    }
}
