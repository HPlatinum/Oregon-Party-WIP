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
        inventory.onItemChangedCallback += UpdateInventoryUI;
        slots = itemsParent.GetComponentsInChildren<InventorySlots>();
        inventoryUI.SetActive(false);
    }

    // // Update is called once per frame
    void Update()
    {
    }

    // updates the UI (currently intensively goes through and recreates each item to simulate new items being added -- bug needs squashed)
    void UpdateInventoryUI() {
        for(int i = 0; i < slots.Length; i++) {
            if(i < inventory.inventorySlot.Count) {
                slots[i].DisplayItem(inventory.inventorySlot[i].item); // bug re-adds items each time the updateUI function is called
            }
            else
                slots[i].ClearSlot();
        }
        Debug.Log("Updating UI");
    }
}
