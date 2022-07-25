using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;
using Orazum.UI;

// All sizes are in screen space pixels;
public class UITilesWindow : MonoBehaviour, IPointerLocalPointHandler
{ 
    [SerializeField]
    protected Vector2Int _gridResolution;

    [SerializeField]
    protected RectTransform _itemsParent;

    [SerializeField]
    protected UITile[] _tiles;

    [SerializeField]
    private int _tileSizePixels;
    public int TileSizePixels { get { return _tileSizePixels; } }

    [SerializeField]
    private int _gapSize;

    [SerializeField]
    private Vector2Int _gridSize;

    [SerializeField]
    private Vector2Int _windowSize;

    protected HashSet<int> _tilesInstanceIDs;

    protected enum CursorLocationType
    {
        NotInitialized,
        OnTile,
        InsideWindow,
        OutsideWindow
    }
    protected CursorLocationType _cursorLocation;

    protected UITile _tileUnderPointer; 

    protected RectTransform _window;
    public RectTransform Rect { get { return _window; } }

    protected bool _shouldUpdateLocalPoint;
    public bool ShouldUpdateLocalPoint { get { return _shouldUpdateLocalPoint; } }

    public int InstanceID { get { return GetInstanceID(); } }

    private int _leftGridBorder;
    private int _rightGridBorder;
    private int _bottomGridBorder;
    private int _topGridBorder;

    private Vector2Int _localPoint;

    protected virtual void Awake()
    { 
#if UNITY_EDITOR
        if (_tiles == null)
        {
            Debug.LogError("You should generate an inventory through window InventoryGeneratorWindow");
            return;
        }
#endif

        TryGetComponent(out _window);

        _shouldUpdateLocalPoint = true;

        _leftGridBorder = -_gridSize.x / 2;
        _rightGridBorder = _gridSize.x / 2;
        _bottomGridBorder = -_gridSize.y / 2;
        _topGridBorder = _gridSize.y / 2;


        _tilesInstanceIDs = new HashSet<int>();
        for (int i = 0; i < _tiles.Length; i++)
        {
            _tilesInstanceIDs.Add(_tiles[i].GetInstanceID());
        }

        Subscribe();
    }

    protected virtual void Subscribe()
    { 
        CraftingDelegatesContainer.ReturnStackToItemsWindow += ReturnStackToPreviousPlace;
    }

    protected virtual void OnDestroy()
    { 
        CraftingDelegatesContainer.ReturnStackToItemsWindow -= ReturnStackToPreviousPlace;
    }

    protected virtual void Start()
    {
        var updater = UIDelegatesContainer.GetEventsUpdater();
        updater.AddPointerLocalPointHandler(this);
    }

    public void UpdateLocalPoint(in Vector2Int localPointArg)
    {
        _localPoint = localPointArg;

        _cursorLocation = CursorLocationType.NotInitialized;
        bool isSufficientCheck;

        CheckIfLocalPointOutsideGrid(out isSufficientCheck);
        if (isSufficientCheck)
        {
            return;
        }

        bool isPosValid;
        int row;
        int col;
        if (_gapSize != 0)
        {
            DetermineCursorLocationWithGapsBetweenTiles(out isPosValid, out row, out col);
            if (isPosValid)
            { 
                Assert.IsTrue(row >= 0 && row < _gridResolution.y);
                Assert.IsTrue(col >= 0 && col < _gridResolution.x);
            }
        }
        else
        {
            int adjustedLocalPoint = (-_localPoint.y + _gridSize.y / 2);
            if (adjustedLocalPoint == _tileSizePixels * _gridResolution.y)
            {
                adjustedLocalPoint--;
            }
            row = adjustedLocalPoint / _tileSizePixels;
            Assert.IsTrue(row >= 0 && row < _gridResolution.y);

            adjustedLocalPoint = (_localPoint.x + _gridSize.x / 2);
            if (adjustedLocalPoint == _tileSizePixels * _gridResolution.x)
            {
                adjustedLocalPoint--;
            }
            col = adjustedLocalPoint / _tileSizePixels;
            Assert.IsTrue(col >= 0 && col < _gridResolution.x);
            _cursorLocation = CursorLocationType.OnTile;
            isPosValid = true;
        }

        NotifyBasedOnCursorLocation(isPosValid, row, col);
    }

    private void CheckIfLocalPointOutsideGrid(out bool isSufficientCheck)
    {
        isSufficientCheck = false;
        if (_localPoint.x < _leftGridBorder || _localPoint.x > _rightGridBorder
            ||
            _localPoint.y < _bottomGridBorder || _localPoint.y > _topGridBorder)
        {
            if (_localPoint.x < -_windowSize.x / 2 || _localPoint.x > _windowSize.x / 2
                ||
                _localPoint.y < -_windowSize.y / 2 || _localPoint.y > _windowSize.y / 2)
            {
                _cursorLocation = CursorLocationType.OutsideWindow;
            }
            else
            {
                _cursorLocation = CursorLocationType.InsideWindow;
            }
            if (_tileUnderPointer != null)
            {
                CraftingDelegatesContainer.EventTileUnderPointerGone?.Invoke();
            }
            _tileUnderPointer = null;
            isSufficientCheck = true;
        }
    }
    
    private void DetermineCursorLocationWithGapsBetweenTiles(out bool isPosValid, out int row, out int col)
    {
        row = -1;
        col = -1;

        Vector2Int adjustedLocalPoint = _localPoint + _gridSize / 2;
        if (adjustedLocalPoint.x == _tileSizePixels * _gridResolution.x + _gapSize * (_gridResolution.x - 1))
        {
            adjustedLocalPoint.x--;
        }
        if (adjustedLocalPoint.y == _tileSizePixels * _gridResolution.y + _gapSize * (_gridResolution.y - 1))
        {
            adjustedLocalPoint.y--;
        }

        int leftBorder = _tileSizePixels + _gapSize / 2;
        int rightBorder = _gridSize.x - _tileSizePixels - _gapSize / 2;
        // adjustedLocalPoint.x -= leftBorder;

        int bottomBorder = leftBorder;
        int topBorder = _gridSize.y - _tileSizePixels - _gapSize / 2;
        // adjustedLocalPoint.y -= bottomBorder;

        bool isBeyondLeftBorder = adjustedLocalPoint.x < leftBorder;
        bool isBeyondRightBorder = adjustedLocalPoint.x > rightBorder;
        bool isBeyondBottomBorder = adjustedLocalPoint.y < bottomBorder;
        bool isBeyondTopBorder = adjustedLocalPoint.y > topBorder;

        if (isBeyondLeftBorder)
        { 
            col = 0;
        }
        else if (isBeyondRightBorder)
        {
            col = _gridResolution.x - 1;
        }
        else
        {
            int segmentWidth = _tileSizePixels;
            if (_gridResolution.x > 1)
            {
                segmentWidth += _gapSize;
            }

            col = (adjustedLocalPoint.x - leftBorder) / segmentWidth + 1;
            int segmentStart = (col - 1) * segmentWidth + leftBorder;
            int segmentPoint = adjustedLocalPoint.x - segmentStart;
            if (segmentPoint < _gapSize / 2 ||
                segmentPoint > _tileSizePixels + _gapSize / 2)
            {
                isPosValid = false;
                _cursorLocation = CursorLocationType.InsideWindow;
                return;
            }
        }

        if (isBeyondTopBorder)
        {
            row = 0;
        }
        else if (isBeyondBottomBorder)
        {
            row = _gridResolution.y - 1;
        }
        else
        {
            int segmentHeight = _tileSizePixels;
            if (_gridResolution.y > 1)
            {
                segmentHeight += _gapSize;
            }

            row = (adjustedLocalPoint.y - bottomBorder) / segmentHeight + 1;
            int segmentStart = (row - 1) * segmentHeight + bottomBorder;
            int segmentPoint = adjustedLocalPoint.y - segmentStart;
            if (segmentPoint < _gapSize / 2 ||
                segmentPoint > _tileSizePixels + _gapSize / 2)
            {
                isPosValid = false;
                _cursorLocation = CursorLocationType.InsideWindow;
                return;
            }

            row = _gridResolution.y - 1 - row;
        }

        isPosValid = true;
        _cursorLocation = CursorLocationType.OnTile;
    }

    private void NotifyBasedOnCursorLocation(bool isPosValid, int row, int col)
    {
        if (_tileUnderPointer != null)
        {
            if (_tileUnderPointer.Pos.x != col || _tileUnderPointer.Pos.y != row)
            { 
                CraftingDelegatesContainer.EventTileUnderPointerGone?.Invoke();
                _tileUnderPointer = null;
                if (_cursorLocation == CursorLocationType.OnTile)
                { 
                    _tileUnderPointer = _tiles[TileIndex(col, row)];
                    CraftingDelegatesContainer.EventTileUnderPointerCame?.Invoke(_tileUnderPointer);
                }
            }
        }
        else
        {
            if (_cursorLocation == CursorLocationType.OnTile)
            { 
                _tileUnderPointer = _tiles[TileIndex(col, row)];
                CraftingDelegatesContainer.EventTileUnderPointerCame?.Invoke(_tileUnderPointer);
            }
        }
    }

    public virtual void PlaceStack(UIStack stack, Vector2Int placeDelta, out UIStack pushedOutStack)
    {
        Assert.IsTrue(_tileUnderPointer == null);
        pushedOutStack = null;

        stack.Data.TilePos = _tileUnderPointer.Pos + placeDelta;
        UpdateStackAnchPos(stack);
        AssignStackToTilesReferences(stack);
    }

    protected void UpdateStackAnchPos(UIStack stack)
    {
        Vector2Int tilePos = stack.Data.TilePos;
        Vector2 anchPos = _tiles[TileIndex(tilePos)].Rect.anchoredPosition;
        Vector2Int sizeInt = stack.ItemType.Size;
        Vector2 stackSize = new Vector2(_tileSizePixels * sizeInt.x, _tileSizePixels * sizeInt.y);
        stack.UpdateRect(anchPos, stackSize, _itemsParent);

        // RectTransform startTile = _tiles[TileIndex(tilePos)].Rect;
        // RectTransform endTile = _tiles[TileIndex(tilePos + sizeInt - Vector2Int.one)].Rect;
        // Vector2 adjust = new Vector2(endTile.rect.width, -endTile.rect.height);

        // Vector2 anchPos = (startTile.anchoredPosition + endTile.anchoredPosition + adjust) / 2;
    }

    protected void AssignStackToTilesReferences(UIStack stack)
    {
        Vector2Int pos = stack.Data.TilePos;
        for (int i = 0; i < stack.Size.x; i++)
        {
            for (int j = 0; j < stack.Size.y; j++)
            {
                _tiles[TileIndex(pos.x + i, pos.y + j)].PlacedStack = stack;
            }
        }
    }

    private void ReturnStackToPreviousPlace(UIStack stack)
    {
        if (!_tilesInstanceIDs.Contains(stack.Data.TileInstanceID))
        {
            return;
        }

        stack.ReturnToItsPlace(_tiles[TileIndex(stack.Data.TilePos)].Rect.anchoredPosition);
    }
    

    protected int TileIndex(int x, int y)
    {
        return y * _gridResolution.x + x;
    }

    protected int TileIndex(Vector2Int xy)
    {
        return xy.y * _gridResolution.x + xy.x;
    }

    #region Editor
#if UNITY_EDITOR
    public void AssignTiles(
        UITile[,] tilesArg, 
        RectTransform itemsParentArg,
        int tileSizeArg,
        int gapSizeArg,
        Vector2Int gridSize,
        Vector2Int borderWidthArg
    )
    {
        _gridResolution = new Vector2Int(tilesArg.GetLength(0), tilesArg.GetLength(1));
        _itemsParent = itemsParentArg;

        _tileSizePixels = tileSizeArg;
        _gridSize = gridSize;
        _windowSize = gridSize + borderWidthArg;
        _gapSize = gapSizeArg;

        _tiles = new UITile[_gridResolution.x * _gridResolution.y];
        for (int j = 0; j < _gridResolution.y; j++)
        {
            for (int i = 0; i < _gridResolution.x; i++)
            {
                _tiles[j * _gridResolution.x + i] = tilesArg[i, j];
            }
        }
    }
#endif
    #endregion
}