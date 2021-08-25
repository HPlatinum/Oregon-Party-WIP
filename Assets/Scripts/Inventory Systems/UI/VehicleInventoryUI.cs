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
    // allows you to add an inventory to an object and gives functionality for updating a UI. This should probably just be merged with the
    // other inventory UI script.
    void Start()
    {
        inventory.onItemChangedCallback += UpdateInventoryUI;
        slots = itemsParent.GetComponentsInChildren<InventorySlots>();
        UpdateInventoryUI();
        inventoryUI.SetActive(false);
    }

    // // Update is called once per frame
    void Update()
    {
        
    }

    // updates the inventory UI -- same code as updateUI -- same bug
    void UpdateInventoryUI() {
        for(int i = 0; i < slots.Length; i++) {
            if(i < inventory.inventorySlot.Count) {
                slots[i].DisplayItem(inventory.inventorySlot[i].item); // bug
            }
            else {
                slots[i].ClearSlot();
            }
        }
        Debug.Log("Updating UI");
    }

    // enables inventory showing
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

    // function to transfer items from player inventory to stash, store, etc.-- needs to be updated to play with Item.Id's instead.
    public void TransferTo() {
        Debug.Log("attempting to p inven below");
        Debug.Log(playerInventory.inventorySlot.Count);
        if(playerInventory.inventorySlot.Count > 0) {
            print(playerInventory.inventorySlot[0].item + " item");
            print(playerInventory.inventorySlot[0].quantity + " quantity");
            inventory.AddItem(playerInventory.inventorySlot[0].item,playerInventory.inventorySlot[0].quantity, inventory);

            playerInventory.RemoveItem(playerInventory.inventorySlot[0].item,playerInventory.inventorySlot[0].quantity, playerInventory);
        }
        else
            print("Transfer inventory empty");
    }

    // function to transfer items from one inventory to player inventory -- needs to be updated to play with Item.Id's instead.
    public void TransferFrom() {
        Debug.Log("attempting from");
        if(inventory.inventorySlot.Count > 0) {
            playerInventory.AddItem(inventory.inventorySlot[0].item,inventory.inventorySlot[0].quantity, playerInventory);
            inventory.RemoveItem(inventory.inventorySlot[0].item,inventory.inventorySlot[0].quantity, inventory);
        }
        else
            print("Transfer inventory empty");
    }
}
