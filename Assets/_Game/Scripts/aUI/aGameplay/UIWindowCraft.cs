using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UIWindowCraft : UITilesWindow
{
    protected override WindowType InitializeWindowType()
    {
        return WindowType.CraftWindow;
    }

    private HashSet<int> _highlightedTilesIndices;
    private HashSet<int> _highlightedStacks;

    protected override void Subscribe()
    {
        base.Subscribe();

        CraftingDelegatesContainer.GetCraftTiles += GetTiles;
        CraftingDelegatesContainer.GetCraftTilesGridResolution += GetCraftTilesGridResolution;

        CraftingDelegatesContainer.HighlightTilesInCraftWindow += HighlightTiles;
        CraftingDelegatesContainer.DefaultLastHighlightInCraftWindow += DefaultLastHighlightedTiles;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();

        CraftingDelegatesContainer.GetCraftTiles -= GetTiles;
        CraftingDelegatesContainer.GetCraftTilesGridResolution -= GetCraftTilesGridResolution;

        CraftingDelegatesContainer.HighlightTilesInCraftWindow -= HighlightTiles;
        CraftingDelegatesContainer.DefaultLastHighlightInCraftWindow -= DefaultLastHighlightedTiles;
    }

    protected override void OnStartLevel(LevelDescriptionSO levelDescriptionSO)
    {
        base.OnStartLevel(levelDescriptionSO);
        _highlightedTilesIndices = new HashSet<int>(GridResolution.x * GridResolution.y);
        _highlightedStacks = new HashSet<int>(10);
    }

    protected override void OnLocalPointUpdate(in UITile tileUnderPointer)
    {
        CraftingDelegatesContainer.EventTileUnderPointerGone();
        if (tileUnderPointer != null)
        { 
            CraftingDelegatesContainer.EventTileUnderPointerCame(tileUnderPointer);
        }
    }

    public UITile[] GetTiles()
    {
        return _tiles;
    }
    public Vector2Int GetCraftTilesGridResolution()
    {
        return GridResolution;
    }

    public override void PlaceStack(UIStack uiStack, Vector2Int tilePos)
    {
        Assert.IsTrue(
            tilePos.x >= 0 && tilePos.x < GridResolution.x &&
            tilePos.y >= 0 && tilePos.y < GridResolution.y
        );

        for (int y = tilePos.y; y < uiStack.Size.y + tilePos.y; y++)
        {
            for (int x = tilePos.x; x < uiStack.Size.x + tilePos.x; x++)
            {
                int tileIndex = TileIndex(x, y);
                UIStack prevPlacedStack = _tiles[tileIndex].PlacedStack;
                if (prevPlacedStack != null)
                {
                    RemoveStackFromTilesReferences(_tiles[tileIndex].PlacedStack);
                    CraftingDelegatesContainer.ReturnStack(prevPlacedStack);
                }
            }
        }

        uiStack.Data.TilePos = tilePos;
        uiStack.RestingWindow = WindowType.CraftWindow;
        AddStackToTilesReferences(uiStack);
        UpdateStackAnchPos(uiStack);

        CraftingDelegatesContainer.DefaultLastHighlightInCraftWindow();
    }

    private void UpdateStackAnchPos(UIStack stack)
    {
        Vector2Int tilePos = stack.Data.TilePos;
        Vector2 anchPos = _tiles[TileIndex(tilePos)].Rect.position;
        Vector2Int sizeInt = stack.ItemType.Size;
        Vector2 stackSize = new Vector2(TileSize * sizeInt.x, TileSize * sizeInt.y);
        stack.UpdateRect(anchPos, stackSize);
    }

    private void AddStackToTilesReferences(UIStack stack)
    { 
        for (int y = stack.Pos.y; y < stack.Size.y + stack.Pos.y; y++)
        {
            for (int x = stack.Pos.x; x < stack.Size.x + stack.Pos.x; x++)
            { 
                _tiles[TileIndex(x, y)].PlacedStack = stack;
            }
        }
    }

    public void RemoveStackFromTilesReferences(UIStack stack)
    {
        for (int y = stack.Pos.y; y < stack.Size.y + stack.Pos.y; y++)
        {
            for (int x = stack.Pos.x; x < stack.Size.x + stack.Pos.x; x++)
            { 
                _tiles[TileIndex(x, y)].PlacedStack = null;
            }
        }
    }

    public bool IsStackInsideGridResolution(Vector2Int stackSize, Vector2Int tilePos)
    {
        if (tilePos.y < 0 || tilePos.x < 0)
        {
            return false;
        }
        if (tilePos.y + stackSize.y - 1 >= GridResolution.y ||
            tilePos.x + stackSize.x - 1 >= GridResolution.x)
        {
            return false;
        }

        return true;
    }

    public void HighlightTiles(UIStack uiStack, Vector2Int tilePos)
    {
        _highlightedTilesIndices.Clear();
        _highlightedStacks.Clear();

        for (int y = tilePos.y; y < uiStack.Size.y + tilePos.y; y++)
        {
            for (int x = tilePos.x; x < uiStack.Size.x + tilePos.x; x++)
            {
                int hightlightIndex = TileIndex(x, y);
                if (_tiles[hightlightIndex].PlacedStack != null)
                {
                    UIStack toHighlight = _tiles[hightlightIndex].PlacedStack;
                    if (!_highlightedStacks.Contains(toHighlight.InstanceID))
                    {
                        _highlightedStacks.Add(toHighlight.InstanceID);

                        for (int y2 = toHighlight.Pos.y; y2 < toHighlight.Size.y + toHighlight.Pos.y; y2++)
                        {
                            for (int x2 = toHighlight.Pos.x; x2 < toHighlight.Size.x + toHighlight.Pos.x; x2++)
                            {
                                int hightlightIndex2 = TileIndex(x2, y2);
                                if (!_highlightedTilesIndices.Contains(hightlightIndex2))
                                { 
                                    _highlightedTilesIndices.Add(hightlightIndex2);
                                }
                                _tiles[hightlightIndex2].HighlightFilledState();
                            }
                        }
                    }
                }
                else
                { 
                    _tiles[hightlightIndex].HighLightFreeState();
                }

                _highlightedTilesIndices.Add(hightlightIndex);
            }
        }
    }
    public void DefaultLastHighlightedTiles()
    {
        foreach (int highlightedTileIndex in _highlightedTilesIndices)
        {
            _tiles[highlightedTileIndex].DefaultState();
        }
    }
}