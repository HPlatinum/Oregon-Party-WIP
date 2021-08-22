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
        inventory.onItemChangedCallback += UpdateUI;
        slots = itemsParent.GetComponentsInChildren<InventorySlots>();
        //UpdateUI();
        inventoryUI.SetActive(false);
    }

    // // Update is called once per frame
    void Update()
    {
        inventoryState();
    }

    void UpdateUI() {
        for(int i = 0; i < slots.Length; i++) {
            print(inventory.inventorySlot.Count + " " + i);
            if (i == inventory.inventorySlot.Count) {
                print("here");
                slots[i-1].AddItem(inventory.inventorySlot[i-1].item);
                print("here2");
            }
            if(i > inventory.inventorySlot.Count) {
                slots[i].ClearSlot();
            }
        }
        Debug.Log("Updating UI");
        //print(slots.Length);
    }

    public bool inventoryState() {
        if(Input.GetButtonDown("Inventory")){
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            return(inventoryUI.activeSelf);
        }
        return(inventoryUI.activeSelf);
    }

}
