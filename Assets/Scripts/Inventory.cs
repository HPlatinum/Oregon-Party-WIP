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

    // Event which we can subscribe different methods to -- Trigger calls all attached events
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public List<Item> items = new List<Item>();

    // Add's item to inventory list - need to add functionality for count
    public bool Add (Item item) {
        if(!item.defaultItem) {
            items.Add(item);
            
            // If something changes within inventory, we can trigger these functions 
            if (onItemChangedCallback != null){
                onItemChangedCallback.Invoke();
            }
        }
        else return false;
        
        return true;
    }

    // Removes's item - need to add functionality for count
    public void Remove (Item item, int count) {
        items.Remove(item);

        if(onItemChangedCallback != null){
            onItemChangedCallback.Invoke();
        }
    }

}
