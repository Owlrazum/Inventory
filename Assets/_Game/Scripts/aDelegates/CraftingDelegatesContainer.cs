using System;
using UnityEngine;

public static class CraftingDelegatesContainer
{
    public static Func<int, ItemSO> GetItemSO;

    public static Action<UITile> EventTileUnderPointerCame;
    public static Action EventTileUnderPointerGone;

    public static Action<UIStack> EventStackWasSelected;
    public static Func<bool> IsStackSelected;

    public static Action<UIStack> HighlightStack;
    public static Action<UIStack> DefaultStack;
    public static Action<Vector2Int[], Vector2Int> HighlightTilesDelta;
    public static Action<Vector2Int[], Vector2Int> DefaultTilesDelta;

    // delta relative to tileUnderPointer
    public static Func<Vector2Int[], Vector2Int, bool> CheckSelectedStackFillStateValid;
    public static Func<UIStack> GetPushedOutByPlacementStack;

    public static Action<UIStack, Vector2Int> PlaceStackUnderPointer;
    public static Func<ItemSO, int, bool> CheckIfItemsPresentInInventory;

    public static Action<UIStack> ReturnStackToPreviousPlace; // no impl
}