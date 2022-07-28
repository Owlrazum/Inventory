using UnityEngine.Assertions;
using UnityEngine;
using Orazum.UI;

// All sizes calcs are made in ints, because float point precision is not required.
[RequireComponent(typeof(RectTransform))]
public abstract class UITilesWindow : MonoBehaviour, IPointerLocalPointHandler
{
    // generation params are stored in abstract class and serve as properties.
    protected abstract WindowType InitializeWindowType();
    protected abstract void OnLocalPointUpdate(in UITile tileUnderPointer);
    public abstract void PlaceStack(UIStack stack, Vector2Int tilePos);

    protected WindowType _windowType;
    private TileGenParamsSO _tileGenParamsSO;

    protected Vector2Int GridResolution { get { return _tileGenParamsSO.GridResolution; } }
    protected int TileSize { get { return _tileGenParamsSO.TileSize; } }
    protected int GapSize { get { return _tileGenParamsSO.GapSize; } }
    public int GetTileSize()
    {
        return TileSize;
    }

    protected UITile[] _tiles;

    protected RectTransform _windowRect;
    private RectTransform _tileGridRect;
    private Vector2Int _tileGridSize;

    public RectTransform Rect { get { return _tileGridRect; } }
    public bool ShouldUpdateLocalPoint { get { return GameDelegatesContainer.GetGameState() == GameStateType.Crafting; } }
    public int InstanceID { get { return GetInstanceID(); } }

    protected virtual void Awake()
    {
        bool isFound = true;
        isFound &= TryGetComponent(out _tileGridRect);
        isFound &= TryGetComponent(out _windowRect);

        Assert.IsTrue(isFound);

        _windowType = InitializeWindowType();
        Subscribe();
    }
    protected virtual void Subscribe()
    {
        GameDelegatesContainer.StartLevel += OnStartLevel;
    }
    protected virtual void Start()
    {
        var updater = UIDelegatesContainer.GetEventsUpdater();
        updater.AddPointerLocalPointHandler(this);
    }
    protected virtual void OnDestroy()
    {
        GameDelegatesContainer.StartLevel -= OnStartLevel;

        if (UIDelegatesContainer.GetEventsUpdater != null)
        { 
            var updater = UIDelegatesContainer.GetEventsUpdater();
            updater.RemovePointerLocalPointHandler(this);
        }
    }

    protected virtual void OnStartLevel(LevelDescriptionSO levelDescriptionSO)
    {
        _tileGenParamsSO = levelDescriptionSO.GetTileGenParams(_windowType);
        _tiles = GenerateTiles(in _tileGenParamsSO, out _tileGridRect);
        _tileGridSize = Vector2Int.RoundToInt(_tileGridRect.rect.size);
    }

    private UITile[] GenerateTiles(in TileGenParamsSO generationParams, out RectTransform tileGridRect)
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

        UITile[] generatedTiles = new UITile[rowCount * colCount];
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

    public void UpdateWithLocalPointFromPointer(in Vector2Int localPoint)
    {
        if (CheckIfLocalPointOutsideGrid(in localPoint))
        {
            return;
        }

        UITile tileUnderPointer = DetermineTileFromLocalPoint(localPoint);
        OnLocalPointUpdate(tileUnderPointer);
    }
    private bool CheckIfLocalPointOutsideGrid(in Vector2Int localPoint)
    {
        if (localPoint.x < _tileGridRect.rect.xMin || localPoint.x > _tileGridRect.rect.xMax
            ||
            localPoint.y < _tileGridRect.rect.yMin || localPoint.y > _tileGridRect.rect.yMax)
        {
            return true;
        } 

        return false;
    }
    private UITile DetermineTileFromLocalPoint(in Vector2Int localPoint)
    {
        if (_tileGenParamsSO.GapSize > 0)
        {
            return GetTileWithGapsInGrid(localPoint);
        }
        else
        {
            return GetTileWithoutGapsInGrid(localPoint);
        }
    }
    private UITile GetTileWithGapsInGrid(in Vector2Int localPoint)
    {
        Vector2Int adjustedLocalPoint = localPoint + _tileGridSize / 2;
        if (adjustedLocalPoint.x == TileSize * GridResolution.x + GapSize * (GridResolution.x - 1))
        {
            adjustedLocalPoint.x--;
        }
        if (adjustedLocalPoint.y == TileSize * GridResolution.y + GapSize * (GridResolution.y - 1))
        {
            adjustedLocalPoint.y--;
        }

        int leftBorder = TileSize + GapSize / 2;
        int rightBorder = _tileGridSize.x - TileSize - GapSize / 2;
        // adjustedLocalPoint.x -= leftBorder;

        int bottomBorder = leftBorder;
        int topBorder = _tileGridSize.y - TileSize - GapSize / 2;
        // adjustedLocalPoint.y -= bottomBorder;

        bool isBeyondLeftBorder = adjustedLocalPoint.x < leftBorder;
        bool isBeyondRightBorder = adjustedLocalPoint.x > rightBorder;
        bool isBeyondBottomBorder = adjustedLocalPoint.y < bottomBorder;
        bool isBeyondTopBorder = adjustedLocalPoint.y > topBorder;

        int col = -1;
        if (isBeyondLeftBorder)
        { 
            col = 0;
        }
        else if (isBeyondRightBorder)
        {
            col = GridResolution.x - 1;
        }
        else
        {
            int segmentWidth = TileSize;
            if (GridResolution.x > 1)
            {
                segmentWidth += GapSize;
            }

            col = (adjustedLocalPoint.x - leftBorder) / segmentWidth + 1;
            int segmentStart = (col - 1) * segmentWidth + leftBorder;
            int segmentPoint = adjustedLocalPoint.x - segmentStart;
            if (segmentPoint < GapSize / 2 ||
                segmentPoint > TileSize + GapSize / 2)
            {
                return null;
            }
        }

        int row = -1;
        if (isBeyondTopBorder)
        {
            row = 0;
        }
        else if (isBeyondBottomBorder)
        {
            row = GridResolution.y - 1;
        }
        else
        {
            int segmentHeight = TileSize;
            if (GridResolution.y > 1)
            {
                segmentHeight += GapSize;
            }

            row = (adjustedLocalPoint.y - bottomBorder) / segmentHeight + 1;
            int segmentStart = (row - 1) * segmentHeight + bottomBorder;
            int segmentPoint = adjustedLocalPoint.y - segmentStart;
            if (segmentPoint < GapSize / 2 ||
                segmentPoint > TileSize + GapSize / 2)
            {
                return null;
            }

            row = GridResolution.y - 1 - row;
        }

        return _tiles[TileIndex(col, row)];
    }
    private UITile GetTileWithoutGapsInGrid(in Vector2Int LocalPoint)
    {
        int adjustedLocalPoint = (-LocalPoint.y + _tileGridSize.y / 2);
        if (adjustedLocalPoint == TileSize * GridResolution.y)
        {
            adjustedLocalPoint--;
        }
        int row = adjustedLocalPoint / TileSize;
        Assert.IsTrue(row >= 0 && row < GridResolution.y);

        adjustedLocalPoint = (LocalPoint.x + _tileGridSize.x / 2);
        if (adjustedLocalPoint == TileSize * GridResolution.x)
        {
            adjustedLocalPoint--;
        }
        int col = adjustedLocalPoint / TileSize;
        Assert.IsTrue(col >= 0 && col < GridResolution.x);

        return _tiles[TileIndex(col, row)];
    }

    protected int TileIndex(int x, int y)
    {
        return y * GridResolution.x + x;
    }
    protected int TileIndex(Vector2Int xy)
    {
        return xy.y * GridResolution.x + xy.x;
    }
    protected Vector2Int TileIndex(int index)
    {
        return new Vector2Int(index % GridResolution.x, index / GridResolution.x);
    }
}