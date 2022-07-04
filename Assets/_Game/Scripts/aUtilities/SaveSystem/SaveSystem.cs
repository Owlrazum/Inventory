using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class InventoryData
{
    public List<int> items;
    public List<List<UIStackData>> stacks;
}

public class SaveSystem
{
    private const string INVENTORY_PATH = "inventory.txt";

    public static void SaveInventoryState(Dictionary<ItemSO, List<UIStack>> inventoryItems)
    {
        InventoryData data = new InventoryData();
        data.items = new List<int>(inventoryItems.Count);
        data.stacks = new List<List<UIStackData>>(inventoryItems.Count);
        int stackCounter = 0;
        foreach (var pair in inventoryItems)
        {
            data.items.Add(pair.Key.ID);
            data.stacks.Add(new List<UIStackData>());
            foreach (var uiStack in pair.Value)
            {
                data.stacks[stackCounter].Add(uiStack.StackData);
            }
            stackCounter++;
        }

        var jsonString = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + INVENTORY_PATH, jsonString);
    }

    public static InventoryData LoadInvenoryData()
    {
        if (!File.Exists(Application.persistentDataPath + INVENTORY_PATH))
        {
            return null;
        }

        var jsonString = File.ReadAllText(Application.persistentDataPath + INVENTORY_PATH);
        var data = JsonUtility.FromJson<InventoryData>(jsonString);

        return data;
    }
}