// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Item/Database")]
// public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
// {
//     // item database that allows you to assign an item using an ID rather than assign it by grabbing the Item itself. It should
//     // streamline the inventory UI system by allowing us to just say item = item.Id
//     public Item[] Items;
//     public Dictionary<Item, int> GetId = new Dictionary<Item, int>();
//     public Dictionary<int, Item> GetItem = new Dictionary<int, Item>();

//     // adds the items to the database and gives them an ID based on their location within the database.
//     public void OnAfterDeserialize() {
//         GetId = new Dictionary<Item, int>();
//         GetItem = new Dictionary<int, Item>();
//         for(int i = 0; i <Items.Length; i++) {
//             GetId.Add(Items[i], i);
//             GetItem.Add(i, Items[i]);
//         }
//     }

//     public void OnBeforeSerialize() {
//     }
// }
