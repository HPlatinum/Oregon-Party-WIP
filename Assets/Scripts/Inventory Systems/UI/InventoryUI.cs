using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public GameObject inventoryUI;

    public Inventory inventory;
    
    private InventorySlotUI[] slotsUI;
    // // Start is called before the first frame update
    void Start()
    {
        // when an item is added or removed, the updateUI function is also called (adds updateUI to onItemChangedCallback function)
        inventory.onItemChangedCallback += UpdateUI;
        slotsUI = itemsParent.GetComponentsInChildren<InventorySlotUI>();
        SetupAllInventorySlotsUI();
        UpdateUI();
        inventoryUI.SetActive(false);
    }

    // updates the UI
    void UpdateUI() {
        for(int i = 0; i < slotsUI.Length; i++) {
            InventorySlotUI slotUI = slotsUI[i];
            if( i < inventory.inventorySlot.Count) {
                InventorySlot slot = inventory.inventorySlot[i];
                if (ShouldUpdateSlotUIItem(slotUI, slot)){
                    slotUI.ClearSlot(); // clear item and remove old game object
                    slotUI.AddItemToInventorySlot(slot.item); // add new item
                    slotUI.DisplayItemQuantity(slot.quantity);
                }
                if(ShouldUpdateSlotUIQuantity(slotUI, slot)) {
                    slotUI.DisplayItemQuantity(slot.quantity);
                }
            }
            else if(slotUI.item != null) { // if the slot item isn't null but the slot.count is grater than inventory slot count
                slotUI.ClearSlot(); // clear item and remove old game object
            }
        }
        Debug.Log("Updating UI");
        
    }
    

    private bool ShouldUpdateSlotUIQuantity(InventorySlotUI ui, InventorySlot slot) {
        if (slot.quantity != ui.quantity)
            return true;
        return false;
    }

    private bool ShouldUpdateSlotUIItem(InventorySlotUI ui, InventorySlot slot) {
        if (slot.item != ui.item)
            return true;
        return false;
    }

    private void SetupAllInventorySlotsUI() {
        foreach (InventorySlotUI slotUI in slotsUI) {
            slotUI.Setup();
        }
    }
}
