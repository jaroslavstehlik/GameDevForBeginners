using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This field tells UnityEditor to create an asset menu
// which creates a new scriptable object in project.
[CreateAssetMenu(fileName = "InventoryItem", menuName = "GMD/Inventory/Inventory Item", order = 1)]
public class InventoryItem : ScriptableObject
{
    public string key;
    public string itemName;
    public string description;
    public GameObject scenePrefab;
    public GameObject uiPrefab;
}
