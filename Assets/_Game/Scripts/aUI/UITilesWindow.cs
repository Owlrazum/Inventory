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
        CraftingDelegatesContainer.PlaceStackUnderPointer += OnStackPlacementUnderPointer;

        CraftingDelegatesContainer.ReturnStackToPreviousPlace += ReturnStackToPreviousPlace;
    }

    protected virtual void OnDestroy()
    { 
        CraftingDelegatesContainer.PlaceStackUnderPointer -= OnStackPlacementUnderPointer;

        CraftingDelegatesContainer.ReturnStackToPreviousPlace -= ReturnStackToPreviousPlace;
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
            CheckIfLocalPointOnCorners(out isSufficientCheck, out isPosValid, out row, out col);
            if (!isSufficientCheck)
            {
                DetermineCursorLocationWithGapsBetweenTiles(out isPosValid, out row, out col);
            }
            Assert.IsTrue(row >= 0 && row < _gridResolution.y);
            Assert.IsTrue(col >= 0 && col < _gridResolution.x);
        }
        else
        { 
            row = (-_localPoint.y + _gridSize.y / 2) / _tileSizePixels;
            col = ( _localPoint.x + _gridSize.x / 2) / _tileSizePixels;
            Assert.IsTrue(row >= 0 && row < _gridResolution.y);
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
                OnTileUnderPointerGone();
            }
            _tileUnderPointer = null;
            isSufficientCheck = true;
        }
    }

    // Assumption is that localPoint is inside grid;
    private void CheckIfLocalPointOnCorners(   
        out bool isSufficientCheck, 
        out bool isPosValid, 
        out int row, 
        out int col
    )
    {
        isSufficientCheck = false;
        isPosValid = false;
        row = -1;
        col = -1;

        int localX = -1;
        int localY = -1;

        bool isLeftBorder = _localPoint.x <= _leftGridBorder + _tileSizePixels + _gapSize / 2;
        bool isRightBorder = _localPoint.x >= _rightGridBorder - _tileSizePixels - _gapSize / 2;
        bool isBotBorder = _localPoint.y <= _bottomGridBorder + _tileSizePixels + _gapSize / 2;
        bool isTopBorder = _localPoint.y >= _topGridBorder - _tileSizePixels - _gapSize / 2;

        if (isLeftBorder)
        {
            localX = _localPoint.x - _leftGridBorder;
            if (isTopBorder)
            {
                localY = _topGridBorder - _localPoint.y;
                row = 0;
                col = 0;
            }
            else if (isBotBorder)
            {
                localY = _localPoint.y - _bottomGridBorder;
                row = _gridResolution.y - 1;
                col = 0;
            }
        }
        else if (isRightBorder)
        {
            localX = _rightGridBorder - _localPoint.x;
            if (isTopBorder)
            {
                localY = _topGridBorder - _localPoint.y;
                row = 0;
                col = _gridResolution.x - 1;
            }
            else if (isBotBorder)
            {
                localY = _localPoint.y - _bottomGridBorder;
                row = _gridResolution.y - 1;
                col = _gridResolution.x - 1;
            }
        }

        bool isYBorder = isTopBorder || isBotBorder;
        if (isLeftBorder && isYBorder || isRightBorder && isYBorder)
        { 
            if (localX > _tileSizePixels || localY > _tileSizePixels)
            {
                _cursorLocation = CursorLocationType.InsideWindow;
            }
            else
            {
                _cursorLocation = CursorLocationType.OnTile;
                isPosValid = true;
            }

            isSufficientCheck = true;
        }
    }

    private void DetermineCursorLocationWithGapsBetweenTiles(out bool isPosValid, out int row, out int col)
    {
        row = -1;
        col = -1;

        Vector2Int adjustedLocalPoint = _localPoint + _gridSize / 2;

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
                OnTileUnderPointerGone();
                _tileUnderPointer = null;
                if (_cursorLocation == CursorLocationType.OnTile)
                { 
                    _tileUnderPointer = _tiles[TileIndex(col, row)];
                    OnTileUnderPointerCame();
                }
            }
        }
        else
        {
            if (_cursorLocation == CursorLocationType.OnTile)
            { 
                _tileUnderPointer = _tiles[TileIndex(col, row)];
                OnTileUnderPointerCame();
            }
        }
    }

    private void OnTileUnderPointerCame()
    {
        CraftingDelegatesContainer.EventTileUnderPointerCame?.Invoke(_tileUnderPointer);
        if (CraftingDelegatesContainer.IsStackSelected())
        {
            return;
        }

        _tileUnderPointer.HighLightState();
    }

    private void OnTileUnderPointerGone()
    {
        CraftingDelegatesContainer.EventTileUnderPointerGone?.Invoke();
        if (CraftingDelegatesContainer.IsStackSelected())
        {
            return;
        }

        _tileUnderPointer.DefaultState();
    }

    private void OnStackPlacementUnderPointer(UIStack stack, Vector2Int tilePosDelta)
    {
        if (_tileUnderPointer == null)
        {
            return;
        }

        stack.Data.TilePos = _tileUnderPointer.Pos + tilePosDelta;
        UpdateStackAnchPos(stack);
        AssignStackToTilesReferences(stack);
    }

    private void AddExistingStack(UIStack uiStack, Vector2Int tilePos)
    {
        uiStack.Data.TilePos = tilePos;
        UpdateStackAnchPos(uiStack);
        AssignStackToTilesReferences(uiStack);
    }

    private UIStack AddNewStack(UIStackData stackData)
    { 
        UIStack uiStack = PoolingDelegatesContainer.SpawnStack();
        uiStack.InitializeWithData(stackData, _itemsParent);
        Vector2Int stackSizeInt = CraftingDelegatesContainer.GetItemSO(stackData.ItemTypeID).Size;

        UpdateStackAnchPos(uiStack);
        AssignStackToTilesReferences(uiStack);
        
        return uiStack;
    }

    private UIStack AddNewStack(ItemSO itemType, int itemAmount, Vector2Int tilePos)
    {
        UIStack uiStack = PoolingDelegatesContainer.SpawnStack();

        UIStackData stackData = new UIStackData();
        stackData.ItemAmount = itemAmount;
        stackData.ItemTypeID = itemType.ID;
        stackData.TilePos = tilePos;

        uiStack.InitializeWithData(stackData, _itemsParent);
        UpdateStackAnchPos(uiStack);
        AssignStackToTilesReferences(uiStack);

        return uiStack;
    }

    protected void UpdateStackAnchPos(UIStack stack)
    {
        Vector2Int tilePos = stack.Data.TilePos;
        Vector2 anchPos = _tiles[TileIndex(tilePos)].Rect.anchoredPosition;
        print(tilePos);
        Vector2Int sizeInt = stack.ItemType.Size;
        Vector2 stackSize = new Vector2(_tileSizePixels * sizeInt.x, _tileSizePixels * sizeInt.y);
        stack.UpdateRect(anchPos, stackSize, _itemsParent);

        // RectTransform startTile = _tiles[TileIndex(tilePos)].Rect;
        // RectTransform endTile = _tiles[TileIndex(tilePos + sizeInt - Vector2Int.one)].Rect;
        // Vector2 adjust = new Vector2(endTile.rect.width, -endTile.rect.height);

        // Vector2 anchPos = (startTile.anchoredPosition + endTile.anchoredPosition + adjust) / 2;
    }

    private void AssignStackToTilesReferences(UIStack stack)
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