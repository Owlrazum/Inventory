using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIWindowItems : UITilesWindow
{
    private Vector2Int _defaultStackSize;
    protected override void Awake()
    {
        base.Awake();
        _defaultStackSize = new Vector2Int(_tileSizePixels, _tileSizePixels);

        GameDelegatesContainer.StartLevel += OnPrepareLevel;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameDelegatesContainer.StartLevel -= OnPrepareLevel;
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
            stackData.TileInstanceID = _tiles[TileIndex(stackData.TilePos)].GetInstanceID();

            uiStack.InitializeWithData(stackData, _itemsParent);
            UpdateStackAnchPos(uiStack);
        }
    }

    protected override void UpdateStackAnchPos(UIStack stack)
    {
        Vector2Int tilePos = stack.Data.TilePos;
        Vector2 anchPos = _tiles[TileIndex(tilePos)].Rect.anchoredPosition;
        stack.UpdateRect(anchPos, _defaultStackSize, _itemsParent);
    }
}