using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public GameObject inventoryUI;

    public Inventory inventory; 

    InventorySlots[] slots;
    // // Start is called before the first frame update
    void Start()
    {
        // when an item is added or removed, the updateUI function is also called (adds updateUI to onItemChangedCallback function)
        inventory.onItemChangedCallback += UpdateUI;
        slots = itemsParent.GetComponentsInChildren<InventorySlots>();
        UpdateUI();
        inventoryUI.SetActive(false);
    }

    // // Update is called once per frame
    void Update()
    {
    }

    // updates the UI
    void UpdateUI() {
        for(int i = 0; i < slots.Length; i++) {
            if( i < inventory.inventorySlot.Count) {
                if(slots[i].GetItem() != inventory.inventorySlot[i].item){ // if it doesn't match
                    slots[i].ClearSlot(); // clear item and remove old game object
                    slots[i].AddItemToInventorySlot(inventory.inventorySlot[i].item); // add new item
                }
                if(inventory.inventorySlot[i].quantity > 1 && slots[i].HasItemQuantityChanged(i)) { // this will refresh every time. I can add a get quantity function to check it maybe?
                    slots[i].DisplayItemQuantity(i);
                }
            }
            else if(slots[i].GetItem() != null) { // if the slot item isn't null but the slot.count is grater than inventory slot count
                slots[i].ClearSlot(); // clear item and remove old game object
            }
        }
        Debug.Log("Updating UI");
        
    }
}
