using UnityEngine;
using SNG.UI;

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
    private Vector2Int _gridSize;

    [SerializeField]
    private Vector2Int _windowSize;

    protected enum CursorLocationType
    {
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

        Subscribe();
    }

    protected virtual void Subscribe()
    { 
        CraftingDelegatesContainer.EventStackPlacementUnderPointer += OnStackPlacementUnderPointer;
    }

    protected virtual void OnDestroy()
    { 
        CraftingDelegatesContainer.EventStackPlacementUnderPointer -= OnStackPlacementUnderPointer;
    }

    protected virtual void Start()
    {
        var updater = UIQueriesContainer.FuncGetUpdater();
        updater.AddPointerLocalPointHandler(this);
    }

    public void UpdateLocalPoint(in Vector2Int localPoint)
    {
        if (localPoint.x < -_gridSize.x / 2 + 1 || localPoint.x > _gridSize.x / 2 - 1
            ||
            localPoint.y < -_gridSize.y / 2 + 1 || localPoint.y > _gridSize.y / 2 - 1)
        {
            if (localPoint.x < -_windowSize.x / 2 || localPoint.x > _windowSize.x / 2
                ||
                localPoint.y < -_windowSize.y / 2 || localPoint.y > _windowSize.y / 2)
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
            return;
        }

        _cursorLocation = CursorLocationType.OnTile;

        int col = (localPoint.x + _gridSize.x / 2) / _tileSizePixels;
        int row = (-localPoint.y + _gridSize.y / 2) / _tileSizePixels;

        if (_tileUnderPointer != null)
        {
            if (_tileUnderPointer.Pos.x != col || _tileUnderPointer.Pos.y != row)
            { 
                OnTileUnderPointerGone();
                _tileUnderPointer = _tiles[TileIndex(col, row)];
                OnTileUnderPointerCame();
            }
        }
        else
        {
            _tileUnderPointer = _tiles[TileIndex(col, row)];
            OnTileUnderPointerCame();
        }
    }

    private void OnTileUnderPointerCame()
    {
        CraftingDelegatesContainer.EventTileUnderPointerCame?.Invoke(_tileUnderPointer);
        if (CraftingDelegatesContainer.QueryIsStackSelected())
        {
            return;
        }

        _tileUnderPointer.HighLightState();
    }

    private void OnTileUnderPointerGone()
    {
        CraftingDelegatesContainer.EventTileUnderPointerGone?.Invoke();
        if (CraftingDelegatesContainer.QueryIsStackSelected())
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

        stack.StackData.TilePos = _tileUnderPointer.Pos + tilePosDelta;
        UpdateStackAnchPos(stack);
        AssignStackToTilesReferences(stack);
    }

    private void AddExistingStack(UIStack uiStack, Vector2Int tilePos)
    {
        uiStack.StackData.TilePos = tilePos;
        UpdateStackAnchPos(uiStack);
        AssignStackToTilesReferences(uiStack);
    }

    private UIStack AddNewStack(UIStackData stackData)
    { 
        UIStack uiStack = PoolingDelegatesContainer.FuncSpawnUIStack();
        uiStack.InitializeWithData(stackData, _itemsParent);
        Vector2Int stackSizeInt = CraftingDelegatesContainer.QueryGetItemSO(stackData.ItemTypeID).Size;

        UpdateStackAnchPos(uiStack);
        AssignStackToTilesReferences(uiStack);
        
        return uiStack;
    }

    private UIStack AddNewStack(ItemSO itemType, int itemAmount, Vector2Int tilePos)
    {
        UIStack uiStack = PoolingDelegatesContainer.FuncSpawnUIStack();

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
        Vector2Int tilePos = stack.StackData.TilePos;
        Vector2Int sizeInt = stack.ItemType.Size;
        RectTransform startTile = _tiles[TileIndex(tilePos)].Rect;
        RectTransform endTile = _tiles[TileIndex(tilePos + sizeInt - Vector2Int.one)].Rect;
        Vector2 adjust = new Vector2(endTile.rect.width, -endTile.rect.height);

        Vector2 anchPos = (startTile.anchoredPosition + endTile.anchoredPosition + adjust) / 2;
        Vector2 stackSize = new Vector2(_tileSizePixels * sizeInt.x, _tileSizePixels * sizeInt.y);
        stack.UpdateRect(anchPos, stackSize);
    }

    private void AssignStackToTilesReferences(UIStack stack)
    {
        Vector2Int pos = stack.StackData.TilePos;
        for (int i = 0; i < stack.Size.x; i++)
        {
            for (int j = 0; j < stack.Size.y; j++)
            {
                _tiles[TileIndex(pos.x + i, pos.y + j)].PlacedStack = stack;
            }
        }
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
        Vector2Int gridSize,
        Vector2Int borderWidthArg)
    {
        _gridResolution = new Vector2Int(tilesArg.GetLength(0), tilesArg.GetLength(1));
        _itemsParent = itemsParentArg;

        _tileSizePixels = tileSizeArg;
        _gridSize = gridSize;
        _windowSize = gridSize + borderWidthArg;

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