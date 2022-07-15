using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIWindowItems : UITilesWindow
{
    protected override void Awake()
    {
        base.Awake();
        EventsContainer.ShouldPrepareLevel += OnPrepareLevel;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventsContainer.ShouldPrepareLevel -= OnPrepareLevel;
    }

    private void OnPrepareLevel(LevelDescriptionSO levelDescription)
    {
        for (int i = 0; i < levelDescription.ExistingItems.Length; i++)
        {
            ItemSO itemType = levelDescription.ExistingItems[i];
            int itemAmount = levelDescription.ExistingItemsAmount[i];

            UIStack uiStack = PoolingDelegatesContainer.SpawnUIStackAndQueryIt();
            UIStackData stackData = new UIStackData();
            stackData.ItemTypeID = itemType.ID;
            stackData.ItemAmount = itemAmount;
            stackData.TilePos = new Vector2Int(i, 0);

            uiStack.InitializeWithData(stackData, _itemsParent);
        }
    }
}