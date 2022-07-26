using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// TODO custom amount of tiles, related to amount of items in level

[RequireComponent(typeof(RectTransform))]
public class UIWindowItems : UITilesWindow
{
    private Dictionary<int, int> _itemsTilesRelation;
    private Vector2Int _defaultStackSize;
    protected override void Awake()
    {
        base.Awake();
        _defaultStackSize = new Vector2Int(_tileSizePixels, _tileSizePixels);
        _itemsTilesRelation = new Dictionary<int, int>(5);

        GameDelegatesContainer.StartLevel += OnPrepareLevel;

        CraftingDelegatesContainer.EventLastStackWithItemIDWasTaken += OnLastStackWasTaken;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameDelegatesContainer.StartLevel -= OnPrepareLevel;

        CraftingDelegatesContainer.EventLastStackWithItemIDWasTaken -= OnLastStackWasTaken;
    }

    private void OnPrepareLevel(LevelDescriptionSO levelDescription)
    {
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

            uiStack.InitializeWithData(stackData, _itemsParent);
            UpdateStackAnchPos(uiStack);
            
            uiStack.RestingWindow = WindowType.ItemsWindow;

            _tiles[TileIndex(stackData.TilePos)].PlacedStack = uiStack;
            _itemsTilesRelation.Add(itemType.ID, tileIndex);
        }
    }

    private void OnLastStackWasTaken(int itemID)
    {
        _tiles[_itemsTilesRelation[itemID]].PlacedStack = null;
    }

    public Vector2 GetItemToTilePos(int itemID)
    { 
        return _tiles[_itemsTilesRelation[itemID]].Rect.anchoredPosition;
    }

    public override void PlaceStack(UIStack stack, Vector2Int tilePos2D)
    {
        Assert.IsTrue(
            tilePos2D.x >= 0 && tilePos2D.x < _gridResolution.x &&
            tilePos2D.y >= 0 && tilePos2D.y < _gridResolution.y
        );

        int tilePos = TileIndex(tilePos2D);
        Assert.IsTrue(_itemsTilesRelation[stack.ItemType.ID] == tilePos);
        if (_tiles[tilePos].PlacedStack == null)
        { 
            base.PlaceStack(stack, tilePos2D);
        }
        else
        {
            _tiles[tilePos].PlacedStack.Data.ItemAmount++;
            PoolingDelegatesContainer.DespawnStack(stack);
        }
    }

    protected override void UpdateStackAnchPos(UIStack stack)
    {
        Vector2Int tilePos = stack.Data.TilePos;
        Vector2 anchPos = _tiles[TileIndex(tilePos)].Rect.anchoredPosition;
        stack.UpdateRect(anchPos, _defaultStackSize, _itemsParent);
    }

    protected override void AddStackToTilesReferences(UIStack stack)
    {
        Vector2Int pos = stack.Data.TilePos;
        _tiles[TileIndex(pos.x, pos.y)].PlacedStack = stack;
    }
}