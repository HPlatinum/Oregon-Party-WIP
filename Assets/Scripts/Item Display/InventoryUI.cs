using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public GameObject inventoryUI;

    public Inventory inventory;
    
    private InventorySlotUI[] slotsUI;

    public Text bulletQuantity;

    public Text totalScrapQuantity;

    public PauseMenu pauseMenu;

    public GameObject scrapListInterface;

    public Text electronicScrapQuantity;
    public Text fabricScrapQuantity;
    public Text foodScrapQuantity;
    public Text glassScrapQuantity;
    public Text metalScrapQuantity;
    public Text woodScrapQuantity;

    void Start()
    {
        // when an item is added or removed, the updateUI function is also called (adds updateUI to onItemChangedCallback function)
        inventory.onItemChangedCallback += UpdateUI;
        slotsUI = itemsParent.GetComponentsInChildren<InventorySlotUI>();
        SetupAllInventorySlotsUI();
        UpdateUI();
        inventoryUI.SetActive(false);
        scrapListInterface.SetActive(false);
    }

    // updates the UI
    void UpdateUI() {
        for(int i = 0; i < slotsUI.Length; i++) {
            InventorySlotUI slotUI = slotsUI[i];
            if( i < inventory.inventorySlots.Count) {
                InventorySlot slot = inventory.inventorySlots[i];
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
        ShowBulletQuantity();
        ShowTotalScrapQuantity();
    }

    private void ShowBulletQuantity(){
        bulletQuantity.text = (inventory.bulletCount + "");
    }

    private void ShowTotalScrapQuantity(){
        totalScrapQuantity.text = (inventory.GetTotalScrapCount() + "");
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

    public void PushBulletButton(){
        print("you have " + inventory.bulletCount + " bullets");
    }

    public void PushScrapButton(){
        ShowScrapListInterface();
    }

    private void ShowScrapListInterface(){
        pauseMenu.ShowMenu(false);
        scrapListInterface.SetActive(true);
        ShowElectronicScrapQuantity();
        ShowFabricScrapQuantity();
        ShowFoodScrapQuantity();
        ShowGlassScrapQuantity();
        ShowMetalScrapQuantity();
        ShowWoodScrapQuantity();
    }



    //scrap list interface functions
    private void ShowElectronicScrapQuantity(){
        electronicScrapQuantity.text = (inventory.electronicScrap + "");
    }

    public void PushElectronicScrapButton(){
        print("you have " + inventory.electronicScrap + " electronic scrap");
    }    
    
    private void ShowFabricScrapQuantity(){
        fabricScrapQuantity.text = (inventory.fabricScrap + "");
    }

    public void PushFabricScrapButton(){
        print("you have " + inventory.fabricScrap + " fabric scrap");
    }
    
    private void ShowFoodScrapQuantity(){
        foodScrapQuantity.text = (inventory.foodScrap + "");
    }

    public void PushFoodScrapButton(){
        print("you have " + inventory.foodScrap + " food scrap");
    }
    
    private void ShowGlassScrapQuantity(){
        glassScrapQuantity.text = (inventory.glassScrap + "");
    }

    public void PushGlassScrapButton(){
        print("you have " + inventory.glassScrap + " glass scrap");
    }
    
    private void ShowMetalScrapQuantity(){
        metalScrapQuantity.text = (inventory.metalScrap + "");
    }

    public void PushMetalScrapButton(){
        print("you have " + inventory.metalScrap + " metal scrap");
    }
    
    private void ShowWoodScrapQuantity(){
        woodScrapQuantity.text = (inventory.woodScrap + "");
    }

    public void PushWoodScrapButton(){
        print("you have " + inventory.woodScrap + " wood scrap");
    }


    public void CloseScrapListInterface(){
        scrapListInterface.SetActive(false);
        pauseMenu.ShowMenu(true);
    }
}
