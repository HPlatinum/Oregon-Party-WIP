using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public GameObject inventoryUI;

    public Inventory playerInventory;
    public Inventory partyInventory;

    InventorySlots[] slots;
    // // Start is called before the first frame update
    void Start()
    {
        playerInventory.onItemChangedCallback += UpdateUI;
        slots = itemsParent.GetComponentsInChildren<InventorySlots>();
    }

    // // Update is called once per frame
    void Update()
    {
        inventoryState();
    }

    void UpdateUI() {
        for(int i = 0; i < slots.Length; i++) {
            if(i < playerInventory.inventorySlot.Count) {
                slots[i].AddItem(playerInventory.inventorySlot[i].item);
            }
            else {
                slots[i].ClearSlot();
            }
        }
        Debug.Log("Updating UI");
    }

    public bool inventoryState(){
        if(Input.GetButtonDown("Inventory")){
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            return(inventoryUI.activeSelf);
        }
        return(inventoryUI.activeSelf);
    }
}
