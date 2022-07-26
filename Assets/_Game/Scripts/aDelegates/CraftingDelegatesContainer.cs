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
    public static Action<int> EventLastStackWithItemIDWasTaken;

    public static Action<UIStack, Vector2Int> HighlightTilesInCraftWindow; // red
    public static Action DefaultLastHighlightInCraftWindow;

    public static Func<Vector2Int, Vector2Int, bool> IsPlacementPosValidInCraftWindow;
    public static Action<UIStack, Vector2Int, WindowType> PlaceStack;

    public static Action<UIStack> ReturnStack; // Free tiles in window, and then move it
}