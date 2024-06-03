using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This field tells UnityEditor to create an asset menu
// which creates a new scriptable object in project.
[CreateAssetMenu(fileName = "InventoryCollection", menuName = "GMD/Inventory/Inventory Collection", order = 1)]
public class InventoryCollection : ScriptableObject
{
    [SerializeField] InventoryItem[] items;
    private Dictionary<string, InventoryItem> map = new Dictionary<string, InventoryItem>();
    
    void OnEnable()
    {
        map.Clear();
        if (items != null)
        {
            // Add inventory items in to map
            foreach (var inventoryItem in items)
            {
                // Check if item exists
                if (inventoryItem == null)
                {
                    // Show error when it does not exist
                    Debug.LogError("Inventory Item is null!");
                    // skip item
                    continue;
                }
                
                // Make sure each key is unique
                if (map.ContainsKey(inventoryItem.key))
                {
                    // Show error when we found and item with already existing key
                    Debug.LogError($"Key: {inventoryItem.key} already exists!");
                    // skip item
                    continue;
                }

                // add the item to our map
                map.Add(inventoryItem.key, inventoryItem);
            }
        }
    }

    private void OnDisable()
    {
        map.Clear();
    }

    public bool Contains(string key)
    {
        return map.ContainsKey(key);
    }
    public InventoryItem Get(string key)
    {
        if (!map.ContainsKey(key))
            return null;
        
        return map[key];
    }
}
