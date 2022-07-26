using UnityEngine;

public class UIWindowCraft : UITilesWindow
{
    private int[] _highlightedTilesIndices;
    private int _highlightCount;

    protected override void Awake()
    {
        base.Awake();

        _highlightedTilesIndices = new int[_gridResolution.x * _gridResolution.y];
    }

    protected override void Subscribe()
    {
        base.Subscribe();

        CraftingDelegatesContainer.HighlightTilesInCraftWindow += HighlightTiles;
        CraftingDelegatesContainer.DefaultLastHighlightInCraftWindow += DefaultLastHighlightedTiles;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        CraftingDelegatesContainer.HighlightTilesInCraftWindow -= HighlightTiles;
        CraftingDelegatesContainer.DefaultLastHighlightInCraftWindow -= DefaultLastHighlightedTiles;
    }

    public override void PlaceStack(UIStack uiStack, Vector2Int tilePos)
    {
        for (int y = 0; y < uiStack.Size.y; y++)
        {
            for (int x = 0; x < uiStack.Size.x; x++)
            {
                Vector2Int delta = new Vector2Int(x, y);
                int tileIndex = TileIndex(tilePos + delta);
                UIStack prevPlacedStack = _tiles[tileIndex].PlacedStack;
                if (prevPlacedStack != null)
                {
                    RemoveStackFromTilesReferences(_tiles[tileIndex].PlacedStack);
                    CraftingDelegatesContainer.ReturnStack(prevPlacedStack);
                }
            }
        }

        uiStack.Data.TilePos = tilePos;
        AddStackToTilesReferences(uiStack);
        UpdateStackAnchPos(uiStack);
    }

    public void RemoveStackFromTilesReferences(UIStack stack)
    {
        for (int y = stack.Pos.y; y < stack.Size.y; y++)
        {
            for (int x = stack.Pos.x; x < stack.Size.x; x++)
            { 
                _tiles[TileIndex(x, y)].PlacedStack = null;
            }
        }
    }

    public bool IsStackInsideGridResolution(Vector2Int stackSize, Vector2Int tilePos)
    {
        if (tilePos.y + stackSize.y > _gridResolution.y ||
            tilePos.x + stackSize.x > _gridResolution.x)
        {
            return false;
        }

        return true;
    }

    public void HighlightTiles(UIStack uiStack, Vector2Int tilePos)
    {
        _highlightCount = 0;
        for (int y = 0; y < uiStack.Size.y; y++)
        {
            for (int x = 0; x < uiStack.Size.x; x++)
            {
                Vector2Int delta = new Vector2Int(x, y);
                int hightlightIndex = TileIndex(tilePos + delta);
                if (_tiles[hightlightIndex].PlacedStack != null)
                {
                    UIStack toHighlight = _tiles[hightlightIndex].PlacedStack;
                    for (int y2 = 0; y2 < toHighlight.Size.y; y2++)
                    {
                        for (int x2 = 0; x2 < toHighlight.Size.x; x2++)
                        {
                            Vector2Int delta2 = new Vector2Int(x2, y2);
                            int hightlightIndex2 = TileIndex(toHighlight.Pos + delta2);
                            _highlightedTilesIndices[_highlightCount] = hightlightIndex2;
                            _highlightCount++;
                            _tiles[hightlightIndex2].HighlightFilledState();
                        }
                    }
                }
                else
                { 
                    _tiles[hightlightIndex].HighLightFreeState();
                }
                _highlightedTilesIndices[_highlightCount] = hightlightIndex;
                _highlightCount++;
            }
        }
    }

    public void DefaultLastHighlightedTiles()
    {
        for (int h = 0; h < _highlightCount; h++)
        {
            _tiles[_highlightedTilesIndices[h]].DefaultState();
        }
    }
}