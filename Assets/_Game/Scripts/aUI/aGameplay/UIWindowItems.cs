using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// TODO custom amount of tiles, related to amount of items in level

public class UIWindowItems : UITilesWindow
{
    protected override WindowType InitializeWindowType()
    {
        return WindowType.ItemsWindow;
    }

    [SerializeField]
    private RectTransform _itemStacksParent;

    private Dictionary<int, int> _itemsTilesRelation;

    protected override void Awake()
    {
        base.Awake();
        _itemsTilesRelation = new Dictionary<int, int>(5);
    }

    protected override void Subscribe()
    {
        base.Subscribe();

        CraftingDelegatesContainer.EventLastStackIDWasTakenFromItemsWindow += OnLastStackWasTaken;
        CraftingDelegatesContainer.IsPlacementPosValidInItemsWindow += IsPlacementPosValidInItemsWindow;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        CraftingDelegatesContainer.EventLastStackIDWasTakenFromItemsWindow -= OnLastStackWasTaken;
        CraftingDelegatesContainer.IsPlacementPosValidInItemsWindow -= IsPlacementPosValidInItemsWindow;
    }

    protected override void OnStartLevel(LevelDescriptionSO levelDescription)
    {
        base.OnStartLevel(levelDescription);

        for (int i = 0; i < levelDescription.ExistingItems.Length; i++)
        {
            ItemSO itemType = levelDescription.ExistingItems[i];
            int itemAmount = levelDescription.ExistingItemsAmount[i];

            UIStack uiStack = PoolingDelegatesContainer.SpawnStack();
            UIStackData stackData = new UIStackData();
            stackData.ItemTypeID = itemType.ID;
            stackData.ItemAmount = itemAmount;
            stackData.TilePos = new Vector2Int(i, 0);
            int tileIndex = TileIndex(stackData.TilePos);
            stackData.TileInstanceID = _tiles[tileIndex].GetInstanceID();

            uiStack.InitializeWithData(stackData, _itemStacksParent);
            UpdateStackAnchPos(uiStack);
            
            uiStack.RestingWindow = WindowType.ItemsWindow;
            uiStack.Rect.SetParent(_itemStacksParent, true);

            _tiles[TileIndex(stackData.TilePos)].PlacedStack = uiStack;
            _itemsTilesRelation.Add(itemType.ID, tileIndex);
        }
    }

    protected override void OnLocalPointUpdate(in UITile tileUnderPointer)
    {
        CraftingDelegatesContainer.EventTileUnderPointerGone();
        if (tileUnderPointer != null)
        { 
            CraftingDelegatesContainer.EventTileUnderPointerCame(tileUnderPointer);
        }
    }

    private void OnLastStackWasTaken(UIStack uiStack)
    {
        int itemID = uiStack.ItemType.ID;
        _tiles[_itemsTilesRelation[itemID]].PlacedStack = null;
    }

    private bool IsPlacementPosValidInItemsWindow(Vector2Int tilePos, int itemID)
    {
        int tileIndex = TileIndex(tilePos);
        if (_itemsTilesRelation[tileIndex] == itemID)
        {
            return true;
        }

        return false;
    }

    public Vector2 GetItemToTileLocalAnchPos(int itemID)
    {
        Vector3 anchPos = _tiles[_itemsTilesRelation[itemID]].Rect.anchoredPosition;
        return anchPos;
    }

    public Vector2Int GetItemToTilePos(int itemID)
    {
        int index = _itemsTilesRelation[itemID];
        return TileIndex(index);
    }

    public override void PlaceStack(UIStack stack, Vector2Int tilePos2D)
    {
        Assert.IsTrue(
            tilePos2D.x >= 0 && tilePos2D.x < GridResolution.x &&
            tilePos2D.y >= 0 && tilePos2D.y < GridResolution.y
        );

        int tilePos = TileIndex(tilePos2D);
        Assert.IsTrue(_itemsTilesRelation[stack.ItemType.ID] == tilePos);
        if (_tiles[tilePos].PlacedStack == null)
        {
            stack.Data.TilePos = tilePos2D;
            UpdateStackAnchPos(stack);
            AddStackToTilesReferences(stack);
        }
        else
        {
            _tiles[tilePos].PlacedStack.Data.ItemAmount++;
            PoolingDelegatesContainer.DespawnStack(stack);
        }
    }

    private void UpdateStackAnchPos(UIStack stack)
    {
        Vector2Int tilePos = stack.Data.TilePos;
        Vector2 anchPos = _tiles[TileIndex(tilePos)].Rect.position;
        // anchPos.y = -Screen.currentResolution.height + anchPos.y;
        stack.UpdateRect(anchPos, new Vector2Int(TileSize, TileSize));
    }

    private void AddStackToTilesReferences(UIStack stack)
    {
        Vector2Int pos = stack.Data.TilePos;
        _tiles[TileIndex(pos.x, pos.y)].PlacedStack = stack;
    }
}