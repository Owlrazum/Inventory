using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
public class UIInventory : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField]
    private Vector2Int _tilesSize;

    [SerializeField]
    private MultiDimArrayPackage<UITile>[] _tilesSerialized;

    [SerializeField]
    private DictionaryPackage<ItemSO, List<UIStack>>[] _stacksSerialized;

    [SerializeField]
    private RectTransform _tilesParent;
    [SerializeField]
    private RectTransform _itemsParent;

    private GraphicRaycaster _graphicRaycaster;

    private UITile[,] _tiles;
    private Dictionary<ItemSO, List<UIStack>> _itemStacks;
    
    #region Editor
#if UNITY_EDITOR
    public void AssignTiles(
        UITile[,] tilesArg, 
        RectTransform tilesParentArg,
        RectTransform itemsParentArg)
    {
        _tiles = tilesArg;
        _tilesSize = new Vector2Int(_tiles.GetLength(0), _tiles.GetLength(1));
        _tilesParent = tilesParentArg;
        _itemsParent = itemsParentArg;
    }

    public UITile[,] GetTiles()
    {
        OnAfterDeserialize();
        return _tiles;
    }
#endif
    #endregion

    #region Serialization
    public void OnBeforeSerialize()
    {
        if (_tiles == null)
        {
            Debug.Log("Not serializing tiles");
            return;
        }

        int tilesLength = _tilesSize.x * _tilesSize.y;
        _tilesSerialized = new MultiDimArrayPackage<UITile>[tilesLength];
        for (int j = 0; j < _tilesSize.y; j++)
        {
            for (int i = 0; i < _tilesSize.x; i++)
            {
                _tilesSerialized[j * _tilesSize.x + i] = 
                    (new MultiDimArrayPackage<UITile>(i, j, _tiles[i, j]));
            }
        }

        if (_itemStacks == null)
        {
            return;
        }

        _stacksSerialized = new DictionaryPackage<ItemSO, List<UIStack>>[_itemStacks.Count];
        int indexer = 0;
        foreach(var pair in _itemStacks)
        {
            _stacksSerialized[indexer++] = new DictionaryPackage<ItemSO, List<UIStack>>(pair.Key, pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        if (_tilesSerialized[0].Element == null)
        {
            Debug.Log("No Deserialize!");
            return;
        }

        _tiles = new UITile[_tilesSize.x, _tilesSize.y];
        foreach(var package in _tilesSerialized)
        {
            _tiles[package.ColumnIndex, package.RowIndex] = package.Element;
        }

        _itemStacks = new Dictionary<ItemSO, List<UIStack>>();
        if (_stacksSerialized == null)
        {
            return;
        }

        foreach(var element in _stacksSerialized)
        {
            _itemStacks.Add(element.Key, element.Value);
        }
    }
    #endregion

    private void Awake()
    {
        TryGetComponent(out _graphicRaycaster);
        gameObject.SetActive(false);
        if (_tiles == null)
        {
            Debug.LogError("You should generate an inventory through window InventoryGeneratorWindow");
            return;
        }

        if (_itemStacks == null)
        {
            _itemStacks = new Dictionary<ItemSO, List<UIStack>>();
        }

        CraftingDelegatesContainer.FuncNewItemsPlacementIfPossible += OnNewItemsPlacementIfPossible;
        //CraftingDelegatesContainer.EventInventoryUpdate += OnInventoryUpdate;

        InputDelegatesContainer.EventInventoryCommandTriggered += OnInventoryCommandTriggered;
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
        if (gameObject.activeSelf)
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
        gameObject.SetActive(false);
    }

    private void ShowInventory()
    { 
        gameObject.SetActive(true);
    }
    #endregion

    private bool OnNewItemsPlacementIfPossible(ItemSO itemType, int amount)
    {
        // TODO add and if no more possible, revert and return false
        int predictionAmount = amount;
        int stackMaxSize = itemType.StackCapacity;
        if (_itemStacks.ContainsKey(itemType))
        {
            foreach (UIStack stack in _itemStacks[itemType])
            {
                if (stack.ItemAmount < stackMaxSize)
                {
                    int delta = stackMaxSize - stack.ItemAmount;
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

        List<UIStack> addedStacks = new List<UIStack>();
        for (int j = 0; j < _tiles.GetLength(1); j++)
        {
            for (int i = 0; i < _tiles.GetLength(0); i++)
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
        if (_itemStacks.ContainsKey(itemType))
        {
            foreach (UIStack stack in _itemStacks[itemType])
            {
                if (stack.ItemAmount < stackMaxSize)
                {
                    int delta = stackMaxSize - stack.ItemAmount;
                    if (delta < amount)
                    {
                        amount -= delta;
                        stack.ItemAmount = stackMaxSize;
                    }
                    else
                    {
                        stack.ItemAmount += delta;
                        break;
                    }
                }
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
                if (tileIndex.x >= _tiles.GetLength(0) ||
                    tileIndex.y >= _tiles.GetLength(1))
                {
                    return false;
                }

                if (_tiles == null)
                {
                    Debug.LogError("Tiles are null");
                    return false;
                }

                if (_tiles[tileIndex.x, tileIndex.y] == null)
                {
                    Debug.LogError("tile is null");
                    return false;
                }

                if (_tiles[tileIndex.x, tileIndex.y].PlacedStack != null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private UIStack AddStackToInventory(ItemSO itemType, int itemAmount, Vector2Int tilePos)
    {
        UIStack uiStack = PoolingDelegatesContainer.FuncSpawnUIStack();
        uiStack.InitializeWithItemType(itemType, itemAmount);
        uiStack.AssignReferences(_itemsParent, _graphicRaycaster);

        FillTiles(uiStack, tilePos);

        RectTransform endTile = _tiles[tilePos.x + itemType.Size.x - 1, tilePos.y + itemType.Size.y - 1].Rect;
        Vector2 adjust = new Vector2(endTile.rect.width, -endTile.rect.height);

        Vector2 anchPos = 
            (_tiles[tilePos.x, tilePos.y].Rect.anchoredPosition +
            endTile.anchoredPosition + adjust) / 2;

        uiStack.UpdatePos(tilePos, anchPos);

        return uiStack;
    }

    private void FillTiles(UIStack stack, Vector2Int pos)
    { 
        for (int i = 0; i < stack.ItemType.Size.x; i++)
        {
            for (int j = 0; j < stack.ItemType.Size.y; j++)
            {
                _tiles[pos.x + i, pos.y + j].PlacedStack = stack;
            }
        }
    }

    private void RemoveStackFromInventory(UIStack stack)
    { 
        foreach (Vector2Int toFree in stack.GetFillState())
        {
            _tiles[toFree.x, toFree.y].PlacedStack = null;
        }
        stack.Despawn();
    }
}