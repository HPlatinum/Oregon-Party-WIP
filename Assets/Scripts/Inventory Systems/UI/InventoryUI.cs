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
        //UpdateUI();
        inventoryUI.SetActive(false);
    }

    // // Update is called once per frame
    void Update()
    {
        //inventoryState();
    }

    // updates the UI (currently intensively goes through and recreates each item to simulate new items being added -- bug needs squashed)
    void UpdateUI() {
        for(int i = 0; i < slots.Length; i++) {
            if(i < inventory.inventorySlot.Count) {
                slots[i].AddItem(inventory.inventorySlot[i].item); // bug re-adds items each time the updateUI function is called
            }
            else
                slots[i].ClearSlot();
        }
        Debug.Log("Updating UI");
        //print(slots.Length);
    }
}
