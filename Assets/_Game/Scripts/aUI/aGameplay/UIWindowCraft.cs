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

    protected override UITile[] GenerateTiles(in TileGenParamsSO generationParams, out RectTransform tileGridRect)
    {
        int rowCount = GridResolution.y;
        int colCount = GridResolution.x;

        Vector2Int gapSizeDelta = new Vector2Int(colCount - 1, rowCount - 1) * GapSize;
        Vector2Int windowSize = new Vector2Int(colCount * TileSize, rowCount * TileSize) + gapSizeDelta;

        _windowRect.sizeDelta = windowSize + generationParams.WindowBorderWidth;

        GameObject tileGridGb = new GameObject("TileGrid", typeof(RectTransform));
        tileGridRect = tileGridGb.GetComponent<RectTransform>();
        tileGridRect.SetParent(_windowRect, false);

        tileGridRect.anchorMin = Vector2.zero;
        tileGridRect.anchorMax = Vector2.one;
        tileGridRect.sizeDelta = -generationParams.WindowBorderWidth;

        float scalarDeltaX = TileSize + GapSize;
        float scalarDeltaY = TileSize + GapSize;
        Vector2 initTilePos = Vector2.zero;
        Vector2 rowStartTilePos = initTilePos;
        Vector2 horizDisplacement = scalarDeltaX * Vector2.right;
        Vector2 verticalDisplacement = scalarDeltaY * Vector2.down;

        Vector2 tilePos = initTilePos;

        UITile[] generatedTiles = new UITile[rowCount * GridResolution.y + colCount];
        for (int row = 0; row < rowCount; row++)
        {
            for (int column = 0; column < colCount; column++)
            {
                UITile tile =
                    Instantiate(generationParams.TilePrefab);
                RectTransform tileRect = tile.GetComponent<RectTransform>();
                tileRect.SetParent(tileGridRect);
                tileRect.anchorMin = new Vector2(0, 1);
                tileRect.anchorMax = new Vector2(0, 1);
                tileRect.pivot = new Vector2(0, 1);
                tileRect.anchoredPosition = tilePos;

                tile.AssignWindowTypeOnGeneration(_windowType);
                generatedTiles[TileIndex(column, row)] = tile;
                tile.GenerationInitialize(new Vector2Int(column, row));

                tilePos += horizDisplacement;
            }
            tilePos = rowStartTilePos;
            tilePos += verticalDisplacement;
            rowStartTilePos = tilePos;
        }

        return generatedTiles;
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
        anchPos.y = -UIDelegatesContainer.GetReferenceScreenResolution().y + anchPos.y;
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