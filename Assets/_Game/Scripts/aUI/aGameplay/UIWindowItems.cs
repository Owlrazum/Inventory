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

    protected override UITile[] GenerateTiles(in TileGenParamsSO generationParams, out RectTransform tileGridRect)
    {
        int rowCount = GridResolution.y;
        int colCount = GridResolution.x;

        Vector2Int gapSizeDelta = new Vector2Int(colCount - 1, rowCount - 1) * GapSize;
        // print("gapSizeDelta: " + gapSizeDelta);
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

        UITile[] generatedTiles = new UITile[rowCount * GridResolution.y + colCount];
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
        anchPos.y = -UIDelegatesContainer.GetReferenceScreenResolution().y + anchPos.y;
        stack.UpdateRect(anchPos, new Vector2Int(TileSize, TileSize));
    }

    private void AddStackToTilesReferences(UIStack stack)
    {
        Vector2Int pos = stack.Data.TilePos;
        _tiles[TileIndex(pos.x, pos.y)].PlacedStack = stack;
    }
}