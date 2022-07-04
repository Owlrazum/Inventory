using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
public class UIInventory : MonoBehaviour
{
    [SerializeField]
    private Vector2Int _tilesResolution;

    [SerializeField]
    private float _tileSize;

    [SerializeField]
    private Vector2 _gridSizeDelta;

    [SerializeField]
    private RectTransform _tilesParent;
    [SerializeField]
    private RectTransform _itemsParent;

    [SerializeField]
    private UITile[] _tiles;

    private Dictionary<int, List<UIStack>> _itemStacks;
    
    private CanvasDataType _canvasData;
    private Canvas _canvas;
    
    
    #region Editor
#if UNITY_EDITOR
    public void AssignTiles(
        UITile[,] tilesArg, 
        RectTransform tilesParentArg,
        RectTransform itemsParentArg,
        float squareSizeArg,
        Vector2 gridSizeDeltaArg)
    {
        _tilesResolution = new Vector2Int(tilesArg.GetLength(0), tilesArg.GetLength(1));
        _tilesParent = tilesParentArg;
        _itemsParent = itemsParentArg;

        _tileSize = squareSizeArg;
        _gridSizeDelta = gridSizeDeltaArg;

        _tiles = new UITile[_tilesResolution.x * _tilesResolution.y];
        for (int j = 0; j < _tilesResolution.y; j++)
        {
            for (int i = 0; i < _tilesResolution.x; i++)
            {
                _tiles[j * _tilesResolution.x + i] = tilesArg[i, j];
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
#endif

        TryGetComponent(out _canvas);
        _canvas.enabled = false;

        _canvasData = new CanvasDataType();
        TryGetComponent(out _canvasData.GraphicRaycaster);
        _canvasData.BoundingRect = _itemsParent;

        CraftingDelegatesContainer.FuncNewItemsPlacementIfPossible += OnNewItemsPlacementIfPossible;
        //CraftingDelegatesContainer.EventInventoryUpdate += OnInventoryUpdate;

        InputDelegatesContainer.EventInventoryCommandTriggered += OnInventoryCommandTriggered;
    }

    private void Start()
    {
        _itemStacks = new Dictionary<int, List<UIStack>>();
        //TODO intiiaize
    }

    private void OnDestroy()
    { 
        CraftingDelegatesContainer.FuncNewItemsPlacementIfPossible -= OnNewItemsPlacementIfPossible;
        //CraftingDelegatesContainer.EventInventoryUpdate -= OnInventoryUpdate;

        InputDelegatesContainer.EventInventoryCommandTriggered -= OnInventoryCommandTriggered;
    }

    #region InvenotoryDisplay
    // Places items based on topLeftCorner
    private void OnInventoryCommandTriggered()
    {
        if (_canvas.enabled)
        {
            HideInventory();
        }
        else
        {
            ShowInventory();
        }
    }

    private void HideInventory()
    {
        _canvas.enabled = false;
    }

    private void ShowInventory()
    { 
        _canvas.enabled = true;
    }
    #endregion

    private bool OnNewItemsPlacementIfPossible(ItemSO itemType, int amount)
    {
        // TODO add and if no more possible, revert and return false
        int predictionAmount = amount;
        int stackMaxSize = itemType.StackCapacity;

        List<UIStack> addedStacks = new List<UIStack>();

        if (_itemStacks.ContainsKey(itemType.ID))
        {
            foreach (UIStack stack in _itemStacks[itemType.ID])
            {
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

        
        for (int j = 0; j < _tilesResolution.y; j++)
        {
            for (int i = 0; i < _tilesResolution.x; i++)
            {
                Vector2Int tilePos = new Vector2Int(i, j);
                if (CheckIfSizeFits(itemType.Size, tilePos))
                {
                    if (predictionAmount > itemType.StackCapacity)
                    {
                        predictionAmount -= itemType.StackCapacity;
                        var uiStack = AddStackToInventory(itemType, itemType.StackCapacity, tilePos);
                        addedStacks.Add(uiStack);
                    }
                    else
                    {
                        var uiStack = AddStackToInventory(itemType, predictionAmount, tilePos);
                        addedStacks.Add(uiStack);
                        goto Placement;
                    }
                }
            }
        }

        // We have not found a straight way to place item in inventory;
        foreach (var stack in addedStacks)
        {
            RemoveStackFromInventory(stack);
        }
        return false;

    Placement:
        if (_itemStacks.ContainsKey(itemType.ID))
        {
            foreach (UIStack stack in _itemStacks[itemType.ID])
            {
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
            _itemStacks[itemType.ID].AddRange(addedStacks);
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
                if (tileIndex.x >= _tilesResolution.x ||
                    tileIndex.y >= _tilesResolution.y)
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
        uiStack.InitializeWithData(stackData, _canvasData);
        Vector2Int stackSizeInt = CraftingDelegatesContainer.QueryGetItemSO(stackData.ItemTypeID).Size;

        UpdateStackPos(uiStack, stackData.TilePos, stackSizeInt);
        FillTiles(uiStack, stackData.TilePos);
        
        return uiStack;
    }

    private UIStack AddStackToInventory(ItemSO itemType, int itemAmount, Vector2Int tilePos)
    {
        UIStack uiStack = PoolingDelegatesContainer.FuncSpawnUIStack();

        UIStackData stackData = new UIStackData();
        stackData.ItemAmount = itemAmount;
        stackData.ItemTypeID = itemType.ID;
        stackData.TilePos = tilePos;

        uiStack.InitializeWithData(stackData, _canvasData);
        UpdateStackPos(uiStack, tilePos, itemType.Size);
        FillTiles(uiStack, tilePos);

        return uiStack;
    }

    private void FillTiles(UIStack stack, Vector2Int pos)
    { 
        for (int i = 0; i < stack.Size.x; i++)
        {
            for (int j = 0; j < stack.Size.y; j++)
            {
                _tiles[TileIndex(pos.x + i, pos.y + j)].PlacedStack = stack;
            }
        }
    }

    private void RemoveStackFromInventory(UIStack stack)
    { 
        foreach (Vector2Int toFree in stack.GetFillState())
        {
            _tiles[TileIndex(toFree.x, toFree.y)].PlacedStack = null;
        }
        stack.Despawn();
        _itemStacks.Remove(stack.StackData.ItemTypeID);
    }

    private void UpdateStackPos(UIStack uiStack, Vector2Int tilePos, Vector2Int stackSizeInt)
    { 
        RectTransform startTile = _tiles[TileIndex(tilePos)].Rect;
        RectTransform endTile = _tiles[TileIndex(tilePos + stackSizeInt - Vector2Int.one)].Rect;
        Vector2 adjust = new Vector2(endTile.rect.width, -endTile.rect.height);

        Vector2 anchPos = (startTile.anchoredPosition + endTile.anchoredPosition + adjust) / 2;
        Vector2 stackSize = new Vector2(_tileSize * stackSizeInt.x, _tileSize * stackSizeInt.y);
        uiStack.UpdateRect(anchPos, stackSize);
    }

    private int TileIndex(int x, int y)
    {
        return y * _tilesResolution.x + x;
    }

    private int TileIndex(Vector2Int xy)
    {
        return xy.y * _tilesResolution.x + xy.x;
    }
}