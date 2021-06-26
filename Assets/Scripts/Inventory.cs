using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Singleton to check inventory instance for errors
    #region Singleton
    public static Inventory instance;
    
    void Awake () {
        if(instance != null) {
            Debug.LogWarning("More than one instance of inventory found!");
            return;
        }

        instance = this;
    }

    #endregion

    public List<Item> items = new List<Item>();

    // Add's Count * item
    public void Add (Item item, int count) {
        if(!item.defaultItem) {
            items.Add(item);
        }
        
    }

    // Removes's Count * item
    public void Remove (Item item, int count) {
        items.Remove(item);
    }

}
