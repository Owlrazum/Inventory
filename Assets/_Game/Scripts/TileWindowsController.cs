using UnityEngine;

public class TileWindowsController : MonoBehaviour
{
    [SerializeField]
    private UIWindowCraft _craftWindow;

    [SerializeField]
    private UIWindowItems _itemsWindow;

    [SerializeField]
    private float _stackReturnLerpSpeed = 1;

    private void Awake()
    {
        CraftingDelegatesContainer.GetTileSizeInCraftWindow += GetTileSizeInCraftWindow;
        CraftingDelegatesContainer.GetTileSizeInItemsWindow += GetTileSizeInItemsWindow;

        CraftingDelegatesContainer.GetCursorLocationCraftWindow += GetCursorLocationCraftWindow;
        CraftingDelegatesContainer.GetCursorLocationItemsWindow += GetCursorLocationItemsWindow;
        
        CraftingDelegatesContainer.EventStackWasSelected += OnStackWasSelected;
        CraftingDelegatesContainer.ReturnStack += ReturnStack;

        CraftingDelegatesContainer.IsPlacementPosValidInCraftWindow += IsPlacementPosValidInCraftWindow;
        CraftingDelegatesContainer.PlaceStack += PlaceStack;
    }

    private void OnDestroy()
    { 
        CraftingDelegatesContainer.GetTileSizeInCraftWindow -= GetTileSizeInCraftWindow;
        CraftingDelegatesContainer.GetTileSizeInItemsWindow -= GetTileSizeInItemsWindow;

        CraftingDelegatesContainer.GetCursorLocationCraftWindow -= GetCursorLocationCraftWindow;
        CraftingDelegatesContainer.GetCursorLocationItemsWindow -= GetCursorLocationItemsWindow;

        CraftingDelegatesContainer.EventStackWasSelected -= OnStackWasSelected;
        CraftingDelegatesContainer.ReturnStack -= ReturnStack;

        CraftingDelegatesContainer.IsPlacementPosValidInCraftWindow -= IsPlacementPosValidInCraftWindow;
        CraftingDelegatesContainer.PlaceStack -= PlaceStack;
    }

    private int GetTileSizeInCraftWindow()
    {
        return _craftWindow.TileSizePixels;
    }
    private int GetTileSizeInItemsWindow()
    {
        return _itemsWindow.TileSizePixels;
    }

    private CursorLocationType GetCursorLocationCraftWindow()
    {
        return _craftWindow.CursorLocation;
    }
    private CursorLocationType GetCursorLocationItemsWindow()
    { 
        return _itemsWindow.CursorLocation;
    }

    private void OnStackWasSelected(UIStack uiStack)
    {
        if (uiStack.RestingWindow == WindowType.CraftWindow)
        {
            print("Removing");
            _craftWindow.RemoveStackFromTilesReferences(uiStack);
        }
    }

    private void ReturnStack(UIStack stack)
    {
        Vector2 targetPos = _itemsWindow.GetItemToTilePos(stack.ItemType.ID);
        stack.ReturnToPosInItemsWindow(targetPos, _stackReturnLerpSpeed);
    }

    private bool IsPlacementPosValidInCraftWindow(Vector2Int stackSize, Vector2Int tilePos)
    {
        return _craftWindow.IsStackInsideGridResolution(stackSize, tilePos);
    }

    private void PlaceStack(UIStack toPlace, Vector2Int tilePos, WindowType toPlaceInto)
    {
        if (toPlaceInto == WindowType.CraftWindow)
        {
            _craftWindow.PlaceStack(toPlace, tilePos);
        }
        else if (toPlaceInto == WindowType.ItemsWindow)
        {
            _itemsWindow.PlaceStack(toPlace, tilePos);
        }
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