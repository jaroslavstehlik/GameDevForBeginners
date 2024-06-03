using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Inventory", menuName = "GMD/Inventory/Inventory", order = 1)]
public class Inventory : ScriptableObject
{
    public UnityEvent<Inventory> onChanged;
    public UnityEvent<Inventory, int> onItemAdded;
    public UnityEvent<Inventory, int> onItemRemoved;
    
    [SerializeField] private string key;
    [SerializeField] private InventoryCollection _inventoryCollection;
    [SerializeField] private InventoryItem[] _items;

    public InventoryItem GetInventoryItem(string key)
    {
        if (string.IsNullOrEmpty(key))
            return null;
        
        return _inventoryCollection.Get(key);
    }
    
    public string Get(int index)
    {
        if (_items[index] == null)
            return null;
        
        return _items[index].key;
    }

    int EmptySlot
    {
        get
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == null)
                    return i;
            }

            return -1;
        }
    }
    
    public bool Set(string key)
    {
        int emptySlot = EmptySlot;
        if (emptySlot == -1)
            return false;
        return Set(emptySlot, key);
    }
    
    public bool Set(int index, string key)
    {
        // Clear item if key is null or empty
        if (string.IsNullOrEmpty(key))
        {
            if (_items[index] != null)
            {
                _items[index] = null;
                
                if (onItemRemoved != null)
                    onItemRemoved.Invoke(this, index);
                
                if(onChanged != null)
                    onChanged.Invoke(this);
            }

            return true;
        }
        
        // Check if key is in collection
        if (!_inventoryCollection.Contains(key))
        {
            Debug.LogError($"Inventory collection does not contain: {key}");
            return false;
        }

        _items[index] = _inventoryCollection.Get(key);
        
        if (onItemAdded != null)
            onItemAdded.Invoke(this, index);

        if(onChanged != null)
            onChanged.Invoke(this);

        return true;
    }

    // Make it possible to set or get the item with [] accessor
    public string this[int index] {
        get
        {
            return Get(index);   
        }
        set
        {
            Set(index, value);
        }
    }

    public int At(string key)
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] == null)
                continue;
            
            if (_items[i].key == key)
                return i;
        }

        return -1;
    }

    public bool Swap(int a, int b)
    {
        // check if a equals b therefore now swap is needed
        if (a == b)
            return false;
        
        // check if a or b is out of bounds
        if(a < 0 || b < 0 || a >= Count || b >= Count)
            return false;

        // swap items
        (_items[a], _items[b]) = (_items[b], _items[a]);
        
        if(onChanged != null)
            onChanged.Invoke(this);
        
        return true;
    }

    public int Count
    {
        get
        {
            return _items.Length;
        }
    }

    public bool Load()
    {
        if (!PlayerPrefs.HasKey(key))
            return false;
        
        string input = PlayerPrefs.GetString(key);
        if (string.IsNullOrEmpty(input))
            return false;

        string[] keys = input.Split(',');
        for (int i = 0; i < _items.Length; i++)
        {
            if(i < keys.Length)
                _items[i] = _inventoryCollection.Get(keys[i]);
        }
        
        return true;
    }

    public void Save()
    {
        string output = "";
        foreach (var item in _items)
        {
            if (item != null)
            {
                output += item.key;
            }
            output += ",";
        }
        PlayerPrefs.SetString(key, output);
    }

    public void Reset()
    {
        for (int i = 0; i < _items.Length; i++)
        {
            _items[i] = null;
        }
    }
}