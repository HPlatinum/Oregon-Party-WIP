using UnityEngine;
using UnityEngine.UI;

public class VehicleInventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public GameObject inventoryUI;
    public Inventory playerInventory;

    public Inventory inventory;
    public Button uiActivate;
    public Button transferTo;
    public Button transferFrom;

    InventorySlots[] slots;
    // // Start is called before the first frame update
    void Start()
    {
        inventory.onItemChangedCallback += UpdateInventoryUI;
        slots = itemsParent.GetComponentsInChildren<InventorySlots>();
        UpdateInventoryUI();
    }

    // // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateInventoryUI() {
        for(int i = 0; i < slots.Length; i++) {
            if(i < inventory.inventorySlot.Count) {
                slots[i].AddItem(inventory.inventorySlot[i].item);
            }
            else {
                slots[i].ClearSlot();
            }
        }
        Debug.Log("Updating UI");
    }

    public bool InventoryState() {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
        return(inventoryUI.activeSelf);
    }
    
    public void OnVIOButton() {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }

    public void CloseInventory() {
        InventoryState();
    }

    public void TransferTo() {
        if(playerInventory.inventorySlot.Count > 0) {
            inventory.AddItem(playerInventory.inventorySlot[0].item,playerInventory.inventorySlot[0].quantity, inventory);
            playerInventory.RemoveItem(playerInventory.inventorySlot[0].item,playerInventory.inventorySlot[0].quantity, playerInventory);
        }
        else
            print("Transfer inventory empty");
    }
    public void TransferFrom() {
        if(inventory.inventorySlot.Count > 0) {
            playerInventory.AddItem(inventory.inventorySlot[0].item,inventory.inventorySlot[0].quantity, playerInventory);
            inventory.RemoveItem(inventory.inventorySlot[0].item,inventory.inventorySlot[0].quantity, inventory);
        }
        else
            print("Transfer inventory empty");
    }
}
