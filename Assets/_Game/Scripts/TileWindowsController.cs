using UnityEngine;

public class TileWindowsController : MonoBehaviour
{
    [SerializeField]
    private UIWindowCraft _craftWindow;

    [SerializeField]
    private UIWindowItems _itemsWindow;


    private UITilesWindow _currentActiveWindow;

    private void Awake()
    {
        CraftingDelegatesContainer.GetTileSizeInCraftWindow += GetTileSizeInCraftWindow;
        CraftingDelegatesContainer.GetTileSizeInItemsWindow += GetTileSizeInItemsWindow;
        CraftingDelegatesContainer.PlaceStackUnderPointer += OnStackPlacement;
    }

    private void OnDestroy()
    { 
        CraftingDelegatesContainer.GetTileSizeInCraftWindow -= GetTileSizeInCraftWindow;
        CraftingDelegatesContainer.GetTileSizeInItemsWindow -= GetTileSizeInItemsWindow;
        CraftingDelegatesContainer.PlaceStackUnderPointer -= OnStackPlacement;
    }

    private int GetTileSizeInCraftWindow()
    {
        return _craftWindow.TileSizePixels;
    }

    private int GetTileSizeInItemsWindow()
    {
        return _itemsWindow.TileSizePixels;
    }

    private void OnStackPlacement(UIStack toPlace, Vector2Int pos, out UIStack pushedOutStack)
    {
        pushedOutStack = null;
    }
}



/*
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
*/