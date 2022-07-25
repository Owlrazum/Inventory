// using System;
// using System.Collections.Generic;
// using System.IO;
// using UnityEngine;

// [Serializable]
// public class InventoryData
// {
//     public List<int> items;
//     public List<StackDataListWrapper> stacksData;
// }

// [Serializable]
// public class StackDataListWrapper
// {
//     public List<UIStackData> stacksData;
//     public StackDataListWrapper(List<UIStackData> stacksDataArg)
//     {
//         stacksData = stacksDataArg;
//     }

//     public int Count
//     {
//         get { return stacksData.Count; }
//     }

//     public UIStackData this[int key]
//     {
//         get
//         {
//             return stacksData[key];
//         }
//         set
//         {
//             stacksData[key] = value;
//         }
//     }

//     public void Add(UIStackData data)
//     {
//         stacksData.Add(data);
//     }
// }

// public class SaveSystem
// {
//     private const string INVENTORY_PATH = "inventory.txt";

//     public static void SaveInventoryState(Dictionary<int, Dictionary<int, UIStack>> inventoryItems)
//     {
//         InventoryData data = new InventoryData();
//         data.items = new List<int>(inventoryItems.Count);
//         data.stacksData = new List<StackDataListWrapper>(inventoryItems.Count);
//         int stackCounter = 0;
//         foreach (var itemStackPair in inventoryItems)
//         {
//             data.items.Add(itemStackPair.Key);
//             data.stacksData.Add(new StackDataListWrapper(new List<UIStackData>()));
//             foreach (var uiStackPair in itemStackPair.Value)
//             {
//                 data.stacksData[stackCounter].Add(uiStackPair.Value.Data);
//             }
//             stackCounter++;
//         }

//         var jsonString = JsonUtility.ToJson(data, true);
//         File.WriteAllText(Application.persistentDataPath + INVENTORY_PATH, jsonString);
//     }

//     public static InventoryData LoadInvenoryData()
//     {
//         if (!File.Exists(Application.persistentDataPath + INVENTORY_PATH))
//         {
//             return null;
//         }

//         var jsonString = File.ReadAllText(Application.persistentDataPath + INVENTORY_PATH);
//         var data = JsonUtility.FromJson<InventoryData>(jsonString);

//         return data;
//     }

//     public static void EmptyInventoryData()
//     { 
//         File.WriteAllText(Application.persistentDataPath + INVENTORY_PATH, "");
//     }
// }