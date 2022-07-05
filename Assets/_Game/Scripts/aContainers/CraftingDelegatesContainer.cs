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

    // delta relative to tileUnderPointer
    public static Func<Vector2Int[], Vector2Int, bool> FuncCheckSelectedStackFillStateValid;
    public static bool QueryCheckSelectedStackFillStateValid(Vector2Int[] fillState, Vector2Int pos)
    { 
#if UNITY_EDITOR
        if (FuncCheckSelectedStackFillStateValid.GetInvocationList().Length != 1) { Debug.LogError("There should be only one subscription"); }
#endif

        return FuncCheckSelectedStackFillStateValid.Invoke(fillState, pos);
    }

    public static Func<UIStack> FuncGetPushedOutByPlacementStack;
    public static UIStack QueryPushedOutByPlacementStack()
    { 
#if UNITY_EDITOR 
        if (FuncGetPushedOutByPlacementStack.GetInvocationList().Length != 1) { Debug.LogError("There should be only one subscription"); } 
#endif

        return FuncGetPushedOutByPlacementStack.Invoke();
    }

    public static Action<Vector2Int[], Vector2Int> EventTilesDeltaShouldHighLight;
    public static Action<Vector2Int[], Vector2Int> EventTilesDeltaShouldDefault;

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

    public static Action<UIStack, Vector2Int> EventStackPlacementUnderPointer;

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