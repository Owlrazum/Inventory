using System;
using UnityEngine;

public static class CraftingDelegatesContainer
{
    public static Func<int, ItemSO> GetItemSO;
    public static Func<int> GetTileSizeInCraftWindow;
    public static Func<int> GetTileSizeInItemsWindow;

    public static Func<CursorLocationType> GetCursorLocationCraftWindow;
    public static Func<CursorLocationType> GetCursorLocationItemsWindow;

    public static Action<UITile> EventTileUnderPointerCame;
    public static Action EventTileUnderPointerGone;

    public static Action<UIStack> EventStackWasSelected;
    public static Func<bool> IsStackSelected;

    public static Action<UIStack> HighlightPlacedStack;
    public static Action<UIStack> DefaultPlacedStack;
    public static Action<UIStack, Vector2Int> HighlightTilesUnderSelectedStack;
    public static Action<UIStack, Vector2Int> DefaultLastTilesUnderSelectedStack;

    public static Func<UIStack, Vector2Int, Vector2Int, bool> IsCurrentPlacementPosValid;

    public delegate void PlacementDelegate(
        UIStack toPlace, 
        Vector2Int stackSelectionLocalPos, 
        out UIStack pushedOutStack
    );
    public static PlacementDelegate PlaceStackUnderPointer;

    public static Action<UIStack> ReturnStackToItemsWindow; // no impl
}