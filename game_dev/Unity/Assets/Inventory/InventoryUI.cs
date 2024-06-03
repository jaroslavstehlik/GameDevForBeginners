using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private RectTransform _container;

    private void OnEnable()
    {
        _inventory.onChanged.AddListener(OnInventoryChanged);
        OnInventoryChanged(_inventory);
    }
    
    private void OnDisable()
    {
        _inventory.onChanged.RemoveListener(OnInventoryChanged);
    }

    void OnInventoryChanged(Inventory inventory)
    {
        Debug.Log($"OnInventoryChanged: {inventory.name}");
        // Clear all children
        while (_container.childCount > 0)
        {
            Transform child = _container.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
        
        // Instantiate children
        for (int i = 0; i < inventory.Count; i++)
        {
            InventoryItem inventoryItem = inventory.GetInventoryItem(inventory[i]);
            if(inventoryItem == null)
                continue;
            
            Debug.Log("inventoryItem");
            Instantiate(inventoryItem.uiPrefab, _container);
        }
    }
}
