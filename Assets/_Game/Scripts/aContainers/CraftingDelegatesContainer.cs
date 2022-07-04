using System;
using UnityEngine;

public static class CraftingDelegatesContainer
{
    public static Func<int, ItemSO> FuncGetItemSO;
    public static ItemSO QueryGetItemSO(int id)
    { 
#if UNITY_EDITOR
        if (FuncGetItemSO.GetInvocationList().Length != 1)
        {
            Debug.LogError("There should be only one subscription");
        }
#endif

        return FuncGetItemSO.Invoke(id);
    }

    public static Func<ItemSO, int, bool> FuncNewItemsPlacementIfPossible;
    public static bool QueryNewItemsPlacementIfPossible(ItemSO itemType, int amount)
    {
#if UNITY_EDITOR
        if (FuncNewItemsPlacementIfPossible.GetInvocationList().Length != 1)
        {
            Debug.LogError("There should be only one subscription");
        }
#endif

        return FuncNewItemsPlacementIfPossible.Invoke(itemType, amount);
    }

    public static Func<ItemSO, int, bool> FuncCheckIfItemsPresentInInventory;
    public static bool QueryCheckIfItemsPresentInInventory(ItemSO itemType, int amount)
    {
#if UNITY_EDITOR
        if (FuncCheckIfItemsPresentInInventory.GetInvocationList().Length != 1)
        {
            Debug.LogError("There should be only one subscription");
        }
#endif

        return FuncCheckIfItemsPresentInInventory.Invoke(itemType, amount);
    }
}