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

    public static Action<UITile> EventTileUnderPointerCame;
    public static Action EventTileUnderPointerGone;

    public static Action<UIStack> EventStackWasSelected;
    public static Func<bool> FuncIsStackSelected;
    public static bool QueryIsStackSelected()
    { 
#if UNITY_EDITOR
        if (FuncIsStackSelected.GetInvocationList().Length != 1)
        {
            Debug.LogError("There should be only one subscription");
        }
#endif

        return FuncIsStackSelected.Invoke();
    }

    public static Action<UIStack> EventStackShouldHighlight;
    public static Action<UIStack> EventStackShouldDefault;

    //     public static Func<UIStack> FuncSelectedStack;
    //     public static UIStack QuerySelectedStack()
    //     { 
    // #if UNITY_EDITOR
    //         if (FuncSelectedStack.GetInvocationList().Length != 1)
    //         {
    //             Debug.LogError("There should be only one subscription");
    //         }
    // #endif

    //         return FuncSelectedStack.Invoke();
    //     }

    public delegate bool PlaceUnderPointerDelegate(UIStack toPlace, out UIStack pushedOutStack);
    public static PlaceUnderPointerDelegate FuncIsStackPlaceableOnTileUnderPointer;
    public static bool QueryIsStackPlaceableOnTileUnderPointer(UIStack stack, out UIStack pushedOutStack)
    {
#if UNITY_EDITOR
        if (FuncIsStackPlaceableOnTileUnderPointer.GetInvocationList().Length != 1)
        {
            Debug.LogError("There should be only one subscription");
        }
#endif

        return FuncIsStackPlaceableOnTileUnderPointer.Invoke(stack, out pushedOutStack);
    }

    public static Action<UIStack> EventStackPlacementUnderPointer;

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