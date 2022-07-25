using System.Collections.Generic;
using UnityEngine;

public class UIWindowCraft : UITilesWindow
{
    private UIStack _pushedOutByPlacementStack;

    protected override void Subscribe()
    {
        base.Subscribe();

        CraftingDelegatesContainer.HighlightPlacedStack += OnStackShouldHighlight;
        CraftingDelegatesContainer.DefaultPlacedStack   += OnStackShouldDefault;

        CraftingDelegatesContainer.IsCurrentPlacementPosValid += CheckIfSelectedStackFillStateValid;

        CraftingDelegatesContainer.HighlightTilesUnderSelectedStack += HighlightTilesUnderSelectedStack;
        CraftingDelegatesContainer.DefaultLastTilesUnderSelectedStack   += OnTilesDeltaShouldDefault;

        // CraftingDelegatesContainer.EventStackWasSelected += OnStackWasSelected;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        CraftingDelegatesContainer.HighlightPlacedStack -= OnStackShouldHighlight;
        CraftingDelegatesContainer.DefaultPlacedStack   -= OnStackShouldDefault;

        CraftingDelegatesContainer.IsCurrentPlacementPosValid -= CheckIfSelectedStackFillStateValid;

        CraftingDelegatesContainer.HighlightTilesUnderSelectedStack -= HighlightTilesUnderSelectedStack;
        CraftingDelegatesContainer.DefaultLastTilesUnderSelectedStack   -= OnTilesDeltaShouldDefault;

        // CraftingDelegatesContainer.EventStackWasSelected -= OnStackWasSelected;
    }

    private void OnStackShouldHighlight(UIStack stack)
    {
        if (!_tilesInstanceIDs.Contains(stack.Data.TileInstanceID))
        {
            return;
        }

        // foreach (var index in stack.GetFillState())
        // {
        //     _tiles[TileIndex(index)].HighLightState();
        // }
    }

    private void OnStackShouldDefault(UIStack stack)
    { 
        if (!_tilesInstanceIDs.Contains(stack.Data.TileInstanceID))
        {
            return;
        }

        // foreach (var index in stack.GetFillState())
        // {
        //     _tiles[TileIndex(index)].DefaultState();
        // }
    }

    private bool CheckIfSelectedStackFillStateValid(
        UIStack stack, 
        Vector2Int stackSelectionLocalPos, 
        Vector2Int tilePos
    )
    {
        // UIStack currentStack = null;
        // _pushedOutByPlacementStack = null;
        // foreach (var pos in tilesDelta)
        // {
        //     Vector2Int tileIndex = tilePos + pos;
        //     if (tileIndex.x >= _gridResolution.x || 
        //         tileIndex.y >= _gridResolution.y ||
        //         tileIndex.x < 0 ||
        //         tileIndex.y < 0)
        //     {
        //         _pushedOutByPlacementStack = null;
        //         return false;
        //     }

        //     currentStack = _tiles[TileIndex(tileIndex)].PlacedStack;
        //     if (currentStack != null)
        //     {
        //         if (_pushedOutByPlacementStack == null)
        //         {
        //             _pushedOutByPlacementStack = currentStack;
        //         }
        //         else
        //         {
        //             if (_pushedOutByPlacementStack.GetInstanceID() != currentStack.GetInstanceID())
        //             {
        //                 _pushedOutByPlacementStack = null;
        //                 return false;
        //             }
        //         }
        //     }
        // }

        return true;
    }

    private UIStack GetAndFreePushedOutByPlacementStack()
    {
        if (_pushedOutByPlacementStack != null)
        {
            RemoveStackFromTilesReferences(_pushedOutByPlacementStack);
            CheckAndUpdateAnchPosBelowPointer(_pushedOutByPlacementStack);
        }
        UIStack toReturn = _pushedOutByPlacementStack;
        _pushedOutByPlacementStack = null;
        return toReturn;
    }

    private void HighlightTilesUnderSelectedStack(UIStack uiStack, Vector2Int tilePos)
    {
        // foreach (var pos in tilesDelta)
        // {
        //     _tiles[TileIndex(tilePos + pos)].HighLightState();
        // }
    }

    private void OnTilesDeltaShouldDefault(UIStack uiStack, Vector2Int tilePos)
    {
        // foreach (var pos in tilesDelta)
        // {
        //     _tiles[TileIndex(tilePos + pos)].DefaultState();
        // }
    }

    private void OnStackWasSelected(UIStack stack)
    {
        RemoveStackFromTilesReferences(stack);
    }

    private bool CheckIfSizeFits(Vector2Int size, Vector2Int pos)
    { 
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Vector2Int tileIndex = new Vector2Int(pos.x + i, pos.y + j);
                if (tileIndex.x >= _gridResolution.x ||
                    tileIndex.y >= _gridResolution.y)
                {
                    return false;
                }

                if (_tiles[TileIndex(tileIndex)].PlacedStack != null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void RemoveStackFromTilesReferences(UIStack stack)
    {
        // foreach (Vector2Int toFree in stack.GetFillState())
        // {
        //     _tiles[TileIndex(toFree.x, toFree.y)].PlacedStack = null;
        // }
    }

    private void CheckAndUpdateAnchPosBelowPointer(UIStack stack)
    {
        // var fillState = stack.GetFillState();
        // for (int i = 0; i < fillState.Length; i++)
        // {
        //     if (fillState[i] == _tileUnderPointer.Pos)
        //     {
        //         return;
        //     }
        // }

        // int minDistance = -1;
        // Vector2Int minDelta = Vector2Int.zero;
        // for (int i = 0; i < fillState.Length; i++)
        // {
        //     Vector2Int delta = _tileUnderPointer.Pos - fillState[i];
        //     int distance = Mathf.Abs(delta.x) + Mathf.Abs(delta.y);
        //     if (minDistance < 0 || distance < minDistance)
        //     {
        //         minDistance = distance;
        //         minDelta = delta;
        //     }
        // }

        // stack.Data.TilePos += minDelta;
        // UpdateStackAnchPos(stack);
    }
}