using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class Inventory : ScriptableObject, ISerializationCallbackReceiver
{
    public string savePath;
    public ItemDatabaseObject database;
    // Event which we can subscribe different methods to -- Trigger calls all attached events
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;
    
    public float maxWeight; // max inventory weight
    public float currentWeight; // current weight

    public List<InventorySlot> inventorySlot = new List<InventorySlot>();

    // Adds an item to the List<InventorySlot>
    public bool AddItem(Item _item, int _quantity, Inventory _inventory) {
        bool containsItem = false;
        // checks to see if it's possible to add the item, then adds it if it is and returns True or False.
        if(CanAdd(_inventory, _item)) {
            for(int i = 0; i < inventorySlot.Count; i++) {
                // if the item exists in that inventory and the quantity is not greater than the stack limit, add quantity of item and return true.
                if(inventorySlot[i].item == _item && inventorySlot[i].quantity < _item.stackLimit) {
                    containsItem = true;
                    inventorySlot[i].AddQuantity(_quantity);
                    AddWeight(_item, _quantity);
                if(onItemChangedCallback != null)
                    onItemChangedCallback.Invoke();
                }
            }
                // if the item is not contained within the inventory, adds it to the inventory
            if(!containsItem) {
                inventorySlot.Add(new InventorySlot(database.GetId[_item], _item, _quantity));
                AddWeight(_item, _quantity);
                containsItem = true;
                if(onItemChangedCallback != null)
                    onItemChangedCallback.Invoke();
            }
        }
        return containsItem;
    }

    //Adds the ability to save the inventory state to the given path
    public void Save() {
        string saveData = JsonUtility.ToJson(this, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        bf.Serialize(file, saveData);
        file.Close();
    }

    //Adds the ability to load the inventory state from the given path
    public void Load() {
        if(File.Exists(string.Concat(Application.persistentDataPath, savePath))){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            file.Close();
            onItemChangedCallback.Invoke();
        }
    }

    public void OnAfterDeserialize() {
        for(int i = 0; i < inventorySlot.Count; i++) {
            inventorySlot[i].item = database.GetItem[inventorySlot[i].ID];
        }
    }

    public void OnBeforeSerialize() {
    }

    // function for removing an item from an inventory
    public bool RemoveItem(Item _item, int _quantity, Inventory _inventory) {
        bool containsItem = false;
        Debug.Log(_item);
        Debug.Log(_quantity);
        for(int i = 0; i < inventorySlot.Count; i++) {
            if(inventorySlot[i].item = _item) {
                containsItem = true;
                if(_quantity > inventorySlot[i].quantity) {
                    return containsItem;
                }
                else if(_quantity == inventorySlot[i].quantity) {
                    SubtractWeight(_item, inventorySlot[i].quantity);
                    inventorySlot.RemoveAt(i);
                    if(onItemChangedCallback != null)
                        onItemChangedCallback.Invoke();
                    return containsItem;
                }
                else {
                    SubtractWeight(_item, inventorySlot[i].quantity);
                    inventorySlot[i].RemoveQuantity(_quantity);
                    if(onItemChangedCallback != null)
                        onItemChangedCallback.Invoke();
                }
            }
        }
        return containsItem;
    }

    // Checks to see if the current weight plus the new item's weight is within the 
    // inventory scope
    // may be obsolete with our new design change (slots fill vs weight)
    public bool CanAdd(Inventory _inventory, Item _item) { 
        if(_inventory.currentWeight + _item.weight <= _inventory.maxWeight) {
            return true;
        }
        Debug.Log("This weighs too much for you to lift. Weaksauce.");
        return false;
    }

    // adds to inventory weight total
    public void AddWeight(Item _item, int _quantity) {
        currentWeight += _item.weight * _quantity;
    }

    // subtracts from inventory weight total
    public void SubtractWeight(Item _item, int quantity) {
        currentWeight -= _item.weight * quantity;
    }

    public int ItemQuantity(Item item) {
        //returns the amount of a specified item held in the inventory
        foreach (InventorySlot i in inventorySlot) {
            if (i.item == item) {
                return i.quantity;
            }
        }
        
        return 0;
    }
}

// Allows us to add or remove items to our inventory.
[System.Serializable]
public class InventorySlot {
    public int ID;
    public Item item;
    public int quantity;
    public InventorySlot(int _id, Item _item, int _quantity) {
        ID = _id;
        item = _item;
        quantity = _quantity;
    }

    public void AddQuantity(int value) {
        quantity += value;
    }

    public void RemoveQuantity(int value) {
        quantity -= value;
    }
}