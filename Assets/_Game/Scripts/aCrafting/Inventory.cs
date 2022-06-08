using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Inventory
{
    private Dictionary<ItemSO, int> _items;

    private void Awake()
    {
        _items = new Dictionary<ItemSO, int>();
    }

    public void AddItem(ItemSO item)
    {
        if (_items.ContainsKey(item))
        { 
            _items[item]++;
        }
        else
        {
            _items.Add(item, 0);
        }
    }

    public void RemoveItem(ItemSO item)
    {
        if (!_items.ContainsKey(item))
        {
            Debug.LogError("No item " + item + " to remove");
            return;
        }

        _items[item]--;
        if (_items[item] <= 0)
        {
            _items.Remove(item);
        }
    }

    public Dictionary<ItemSO, int>  GetItems()
    {
        return _items;
    }
}