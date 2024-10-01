using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public Inventory inventory;

    public void PickUp(InventoryItem inventoryItem)
    {
        inventory.Set(inventoryItem.key);
    }

    public void Drop(InventoryItem inventoryItem)
    {
        int slotIndex = inventory.At(inventoryItem.key);
        
        // Found slot index
        if (slotIndex != -1)
        {
            // Clear slot index
            inventory.Set(slotIndex, null);
        }
    }

    public bool Swap(int a, int b)
    {
        return inventory.Swap(a, b);
    }
}
