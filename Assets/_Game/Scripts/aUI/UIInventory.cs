using System.Collections.Generic;
using UnityEngine;

using SNG.UI;

public class UIInventory : MonoBehaviour, IPointerLocalPointHandler
{
    [SerializeField]
    private Vector2Int _gridResolution;

    [SerializeField]
    private int _tileSize;

    [SerializeField]
    private Vector2Int _gridSize;

    [SerializeField]
    private Vector2Int _windowSize;

    [SerializeField]
    private RectTransform _itemsParent;

    [SerializeField]
    private bool _shouldEmptyInventoryOnStart;

    [SerializeField]
    private UITile[] _tiles;

    private Dictionary<int, Dictionary<int, UIStack>> _itemStacks;

    private HashSet<int> _inventoryTiles;

    private enum CursorLocationType
    {
        OnTile,
        InsideWindow,
        OutsideWindow
    }
    private CursorLocationType _cursorLocation;

    private UITile _tileUnderPointer;
    private UIStack _pushedOutByPlacementStack;

    private RectTransform _inventoryWindow;
    public RectTransform Rect { get { return _inventoryWindow; } }

    private bool _shouldUpdateLocalPoint;
    public bool ShouldUpdateLocalPoint { get { return _shouldUpdateLocalPoint; } }

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

        _tileSize = tileSizeArg;
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

    private void Awake()
    {
#if UNITY_EDITOR
        if (_tiles == null)
        {
            Debug.LogError("You should generate an inventory through window InventoryGeneratorWindow");
            return;
        }

        if (_shouldEmptyInventoryOnStart)
        {
            SaveSystem.EmptyInventoryData();
        }
#endif

        _inventoryTiles = new HashSet<int>();
        for (int i = 0; i < _tiles.Length; i++)
        {
            _inventoryTiles.Add(_tiles[i].GetInstanceID());
        }

        TryGetComponent(out _inventoryWindow);

        _shouldUpdateLocalPoint = true;

        Subscribe();
    }

    private void Subscribe()
    { 
        CraftingDelegatesContainer.FuncNewItemsPlacementIfPossible += OnNewItemsPlacementIfPossible;
        
        CraftingDelegatesContainer.EventStackShouldHighlight += OnStackShouldHighlight;
        CraftingDelegatesContainer.EventStackShouldDefault   += OnStackShouldDefault;

        CraftingDelegatesContainer.FuncCheckSelectedStackFillStateValid += CheckIfSelectedStackFillStateValid;
        CraftingDelegatesContainer.FuncGetAndFreePushedOutByPlacementStack += GetAndFreePushedOutByPlacementStack;

        CraftingDelegatesContainer.EventTilesDeltaShouldHighLight += OnTilesDeltaShouldHighLight;
        CraftingDelegatesContainer.EventTilesDeltaShouldDefault   += OnTilesDeltaShouldDefault;

        CraftingDelegatesContainer.EventStackWasSelected += OnStackWasSelected;

        CraftingDelegatesContainer.EventStackPlacementUnderPointer += OnStackPlacementUnderPointer;
    }

    private void OnDestroy()
    { 
        CraftingDelegatesContainer.FuncNewItemsPlacementIfPossible -= OnNewItemsPlacementIfPossible;
        
        CraftingDelegatesContainer.EventStackShouldHighlight -= OnStackShouldHighlight;
        CraftingDelegatesContainer.EventStackShouldDefault   -= OnStackShouldDefault;

        CraftingDelegatesContainer.FuncCheckSelectedStackFillStateValid -= CheckIfSelectedStackFillStateValid;
        CraftingDelegatesContainer.FuncGetAndFreePushedOutByPlacementStack -= GetAndFreePushedOutByPlacementStack;    

        CraftingDelegatesContainer.EventTilesDeltaShouldHighLight -= OnTilesDeltaShouldHighLight;
        CraftingDelegatesContainer.EventTilesDeltaShouldDefault   -= OnTilesDeltaShouldDefault;

        CraftingDelegatesContainer.EventStackWasSelected -= OnStackWasSelected;

        CraftingDelegatesContainer.EventStackPlacementUnderPointer -= OnStackPlacementUnderPointer;
    }

    private void Start()
    {
        var updater = UIQueriesContainer.FuncGetUpdater();
        updater.AddPointerLocalPointHandler(this);

        _itemStacks = new Dictionary<int, Dictionary<int, UIStack>>();
        var data = SaveSystem.LoadInvenoryData();
        if (data == null)
        {
            return;
        }

        for (int i = 0; i < data.items.Count; i++)
        {
            Dictionary<int, UIStack> stacks = new Dictionary<int, UIStack>();
            for (int j = 0; j < data.stacksData[i].Count; j++)
            { 
                UIStack stack = AddStackToInventory(data.stacksData[i][j]);
                stacks.Add(stack.GetInstanceID(), stack);
            }
            _itemStacks.Add(data.items[i], stacks);
        }
    }

    public void UpdateLocalPoint(in Vector2Int localPoint)
    {
        // Debug.Log(localPoint + " // " + _gridSize + " // " + _tileSize);

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

        int col = (localPoint.x + _gridSize.x / 2) / _tileSize;
        int row = (-localPoint.y + _gridSize.y / 2) / _tileSize;

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

    private void OnStackShouldHighlight(UIStack stack)
    {
        foreach (var index in stack.GetFillState())
        {
            _tiles[TileIndex(index)].HighLightState();
        }
    }

    private void OnStackShouldDefault(UIStack stack)
    { 
        foreach (var index in stack.GetFillState())
        {
            _tiles[TileIndex(index)].DefaultState();
        }
    }

    private bool CheckIfSelectedStackFillStateValid(Vector2Int[] tilesDelta, Vector2Int tilePos)
    {
        UIStack currentStack = null;
        _pushedOutByPlacementStack = null;
        foreach (var pos in tilesDelta)
        {
            Vector2Int tileIndex = tilePos + pos;
            if (tileIndex.x >= _gridResolution.x || 
                tileIndex.y >= _gridResolution.y ||
                tileIndex.x < 0 ||
                tileIndex.y < 0)
            {
                _pushedOutByPlacementStack = null;
                return false;
            }

            currentStack = _tiles[TileIndex(tileIndex)].PlacedStack;
            if (currentStack != null)
            {
                if (_pushedOutByPlacementStack == null)
                {
                    _pushedOutByPlacementStack = currentStack;
                }
                else
                {
                    if (_pushedOutByPlacementStack.GetInstanceID() != currentStack.GetInstanceID())
                    {
                        _pushedOutByPlacementStack = null;
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private UIStack GetAndFreePushedOutByPlacementStack()
    {
        if (_pushedOutByPlacementStack != null)
        {
            FreeTiles(_pushedOutByPlacementStack);
            CheckAndUpdateAnchPosBelowPointer(_pushedOutByPlacementStack);
        }
        UIStack toReturn = _pushedOutByPlacementStack;
        _pushedOutByPlacementStack = null;
        return toReturn;
    }

    private void OnTilesDeltaShouldHighLight(Vector2Int[] tilesDelta, Vector2Int tilePos)
    {
        foreach (var pos in tilesDelta)
        {
            _tiles[TileIndex(tilePos + pos)].HighLightState();
        }
    }

    private void OnTilesDeltaShouldDefault(Vector2Int[] tilesDelta, Vector2Int tilePos)
    {
        foreach (var pos in tilesDelta)
        {
            _tiles[TileIndex(tilePos + pos)].DefaultState();
        }
    }

    private void OnStackWasSelected(UIStack stack)
    {
        FreeTiles(stack);
    }

    private void OnStackPlacementUnderPointer(UIStack stack, Vector2Int tilePosDelta)
    {
        if (_tileUnderPointer == null)
        {
            return;
        }

        stack.StackData.TilePos = _tileUnderPointer.Pos + tilePosDelta;
        UpdateStackAnchPos(stack);
        FillTiles(stack);
    }

    private bool OnNewItemsPlacementIfPossible(ItemSO itemType, int amount)
    {
        int predictionAmount = amount;
        int stackMaxSize = itemType.StackCapacity;

        Dictionary<int, UIStack> addedStacks = new Dictionary<int, UIStack>();

        if (_itemStacks.ContainsKey(itemType.ID))
        {
            foreach (var pair in _itemStacks[itemType.ID])
            {
                UIStack stack = pair.Value;
                if (stack.StackData.ItemAmount < stackMaxSize)
                {
                    int delta = stackMaxSize - stack.StackData.ItemAmount;
                    if (delta < predictionAmount)
                    {
                        predictionAmount -= delta;
                    }
                    else
                    {
                        goto Placement;
                    }
                }
            }
        }

        
        for (int j = 0; j < _gridResolution.y; j++)
        {
            for (int i = 0; i < _gridResolution.x; i++)
            {
                Vector2Int tilePos = new Vector2Int(i, j);
                if (CheckIfSizeFits(itemType.Size, tilePos))
                {
                    if (predictionAmount > itemType.StackCapacity)
                    {
                        predictionAmount -= itemType.StackCapacity;
                        var uiStack = AddStackToInventory(itemType, itemType.StackCapacity, tilePos);
                        addedStacks.Add(uiStack.GetInstanceID(), uiStack);
                    }
                    else
                    {
                        var uiStack = AddStackToInventory(itemType, predictionAmount, tilePos);
                        addedStacks.Add(uiStack.GetInstanceID(), uiStack);
                        goto Placement;
                    }
                }
            }
        }

        // We have not found a straight way to place item in inventory;
        foreach (var pair in addedStacks)
        {
            DeleteAddedStack(pair.Value);
        }
        return false;

    Placement:
        if (_itemStacks.ContainsKey(itemType.ID))
        {
            foreach (var pair in _itemStacks[itemType.ID])
            {
                UIStack stack = pair.Value;
                if (stack.StackData.ItemAmount < stackMaxSize)
                {
                    int delta = stackMaxSize - stack.StackData.ItemAmount;
                    if (delta < amount)
                    {
                        amount -= delta;
                        stack.StackData.ItemAmount = stackMaxSize;
                    }
                    else
                    {
                        stack.StackData.ItemAmount += delta;
                        break;
                    }
                }
            }
        }

        if (amount <= 0)
        {
            return true;
        }

        if (!_itemStacks.ContainsKey(itemType.ID))
        {
            _itemStacks.Add(itemType.ID, addedStacks);
        }
        else
        {
            foreach (var pair in addedStacks)
            {
                _itemStacks[itemType.ID].Add(pair.Key, pair.Value);
            }
        }

        return true;
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

    private UIStack AddStackToInventory(UIStackData stackData)
    { 
        UIStack uiStack = PoolingDelegatesContainer.FuncSpawnUIStack();
        uiStack.InitializeWithData(stackData, _itemsParent);
        Vector2Int stackSizeInt = CraftingDelegatesContainer.QueryGetItemSO(stackData.ItemTypeID).Size;

        UpdateStackAnchPos(uiStack);
        FillTiles(uiStack);
        
        return uiStack;
    }

    private UIStack AddStackToInventory(ItemSO itemType, int itemAmount, Vector2Int tilePos)
    {
        UIStack uiStack = PoolingDelegatesContainer.FuncSpawnUIStack();

        UIStackData stackData = new UIStackData();
        stackData.ItemAmount = itemAmount;
        stackData.ItemTypeID = itemType.ID;
        stackData.TilePos = tilePos;

        uiStack.InitializeWithData(stackData, _itemsParent);
        UpdateStackAnchPos(uiStack);
        FillTiles(uiStack);

        return uiStack;
    }

    private void FillTiles(UIStack stack)
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

    private void FreeTiles(UIStack stack)
    { 
        foreach (Vector2Int toFree in stack.GetFillState())
        {
            _tiles[TileIndex(toFree.x, toFree.y)].PlacedStack = null;
        }
    }

    private void DeleteAddedStack(UIStack stack)
    { 
        foreach (Vector2Int toFree in stack.GetFillState())
        {
            _tiles[TileIndex(toFree.x, toFree.y)].PlacedStack = null;
        }
        stack.Despawn();
    }

    private void RemoveStackFromInventory(UIStack stack)
    {
        FreeTiles(stack);

        int key = stack.StackData.ItemTypeID;
        if (_itemStacks[key].Count > 1)
        {
            _itemStacks[key].Remove(stack.GetInstanceID());
        }
        else
        { 
            _itemStacks.Remove(stack.StackData.ItemTypeID);
        }
    }

    private void UpdateStackAnchPos(UIStack stack)
    {
        Vector2Int tilePos = stack.StackData.TilePos;
        Vector2Int sizeInt = stack.ItemType.Size;
        RectTransform startTile = _tiles[TileIndex(tilePos)].Rect;
        RectTransform endTile = _tiles[TileIndex(tilePos + sizeInt - Vector2Int.one)].Rect;
        Vector2 adjust = new Vector2(endTile.rect.width, -endTile.rect.height);

        Vector2 anchPos = (startTile.anchoredPosition + endTile.anchoredPosition + adjust) / 2;
        Vector2 stackSize = new Vector2(_tileSize * sizeInt.x, _tileSize * sizeInt.y);
        stack.UpdateRect(anchPos, stackSize);
    }

    private int TileIndex(int x, int y)
    {
        return y * _gridResolution.x + x;
    }

    private int TileIndex(Vector2Int xy)
    {
        return xy.y * _gridResolution.x + xy.x;
    }

    private bool CheckIfTileInInventory(UITile tile)
    {
        if (!_inventoryTiles.Contains(tile.GetInstanceID()))
        {
            return false;
        }
        return true;
    }

    private void CheckAndUpdateAnchPosBelowPointer(UIStack stack)
    {
        var fillState = stack.GetFillState();
        for (int i = 0; i < fillState.Length; i++)
        {
            if (fillState[i] == _tileUnderPointer.Pos)
            {
                return;
            }
        }

        int minDistance = -1;
        Vector2Int minDelta = Vector2Int.zero;
        for (int i = 0; i < fillState.Length; i++)
        {
            Vector2Int delta = _tileUnderPointer.Pos - fillState[i];
            int distance = Mathf.Abs(delta.x) + Mathf.Abs(delta.y);
            if (minDistance < 0 || distance < minDistance)
            {
                minDistance = distance;
                minDelta = delta;
            }
        }

        stack.StackData.TilePos += minDelta;
        UpdateStackAnchPos(stack);
    }

    private void OnApplicationQuit()
    {
        SaveSystem.SaveInventoryState(_itemStacks);
    }
}