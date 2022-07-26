using System.Collections.Generic;
using UnityEngine;

public class UIWindowCraft : UITilesWindow
{
    private HashSet<int> _highlightedTilesIndices;
    private HashSet<int> _highlightedStacks;

    protected override void Awake()
    {
        base.Awake();

        _highlightedTilesIndices = new HashSet<int>(_gridResolution.x * _gridResolution.y);
        _highlightedStacks = new HashSet<int>(10);
    }

    protected override void Subscribe()
    {
        base.Subscribe();

        CraftingDelegatesContainer.GetCraftTiles += GetTiles;
        CraftingDelegatesContainer.GetCraftTilesColumnCount += GetCraftTilesColumnCount;

        CraftingDelegatesContainer.HighlightTilesInCraftWindow += HighlightTiles;
        CraftingDelegatesContainer.DefaultLastHighlightInCraftWindow += DefaultLastHighlightedTiles;

        CraftingDelegatesContainer.EventLastStackIDWasTakenFromItemsWindow += OnLastStackWithItemIDWasTaken;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        CraftingDelegatesContainer.GetCraftTiles -= GetTiles;
        CraftingDelegatesContainer.GetCraftTilesColumnCount -= GetCraftTilesColumnCount;

        CraftingDelegatesContainer.HighlightTilesInCraftWindow -= HighlightTiles;
        CraftingDelegatesContainer.DefaultLastHighlightInCraftWindow -= DefaultLastHighlightedTiles;

        CraftingDelegatesContainer.EventLastStackIDWasTakenFromItemsWindow -= OnLastStackWithItemIDWasTaken;
    }

    public UITile[] GetTiles()
    {
        return _tiles;
    }

    public int GetCraftTilesColumnCount()
    {
        return _gridResolution.x;
    }

    public override void PlaceStack(UIStack uiStack, Vector2Int tilePos)
    {
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
        if (tilePos.y + stackSize.y - 1 >= _gridResolution.y ||
            tilePos.x + stackSize.x - 1 >= _gridResolution.x)
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

    private void OnLastStackWithItemIDWasTaken(UIStack lastStack)
    {
        lastStack.Rect.SetParent(_window, true);
    }
}