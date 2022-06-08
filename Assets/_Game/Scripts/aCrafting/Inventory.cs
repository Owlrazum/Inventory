using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class Inventory : ISerializationCallbackReceiver
{
    [SerializeField]
    private MultiDimArrayPackage<ItemSO>[] _itemsSerialized;

    private Dictionary<ItemSO, int> _items;

    public Inventory()
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
            Debug.Log("Adding item " + item);
            _items.Add(item, 1);
            Debug.Log("Dictionary count " + _items.Count);
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

    public void OnBeforeSerialize()
    {
        if (_items == null)
        {
            return;
        }

        // Convert our unserializable array into a serializable list
        _itemsSerialized = new MultiDimArrayPackage<ItemSO>[_items.Count];
        int i = 0;
        foreach(var pair in _items)
        {
            _itemsSerialized[i++] =
                (new MultiDimArrayPackage<ItemSO>(pair.Value, -1, pair.Key));
        }
    }

    public void OnAfterDeserialize()
    {
        if (_itemsSerialized == null)
        {
            return;
        }
        _items = new Dictionary<ItemSO, int>();
        foreach(var package in _itemsSerialized)
        {
            _items.Add(package.Element, package.ColumnIndex);
        }
    }
}