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

        CraftingDelegatesContainer.EventStackWasSelected += OnStackWasSelected;
        CraftingDelegatesContainer.ReturnStack += ReturnStack;

        CraftingDelegatesContainer.IsPlacementPosValidInCraftWindow += IsPlacementPosValidInCraftWindow;
        CraftingDelegatesContainer.PlaceStack += PlaceStack;
    }

    private void OnDestroy()
    { 
        CraftingDelegatesContainer.GetTileSizeInCraftWindow -= GetTileSizeInCraftWindow;
        CraftingDelegatesContainer.GetTileSizeInItemsWindow -= GetTileSizeInItemsWindow;

        CraftingDelegatesContainer.EventStackWasSelected -= OnStackWasSelected;
        CraftingDelegatesContainer.ReturnStack -= ReturnStack;

        CraftingDelegatesContainer.IsPlacementPosValidInCraftWindow -= IsPlacementPosValidInCraftWindow;
        CraftingDelegatesContainer.PlaceStack -= PlaceStack;
    }

    private int GetTileSizeInCraftWindow()
    {
        return _craftWindow.GetTileSize();
    }
    private int GetTileSizeInItemsWindow()
    {
        return _itemsWindow.GetTileSize();
    }

    private void OnStackWasSelected(UIStack uiStack)
    {
        if (uiStack.RestingWindow == WindowType.CraftWindow)
        {
            _craftWindow.RemoveStackFromTilesReferences(uiStack);
        }
    }

    private void ReturnStack(UIStack stack)
    {
        Vector2 targetPos = _itemsWindow.GetItemToTileWorldPos(stack.ItemType.ID);
        Vector2 targetSize = Vector2.one * GetTileSizeInItemsWindow();
        stack.ReturnToPosInItemsWindow(targetPos, targetSize, _stackReturnLerpSpeed, OnReturningComplete);
    }

    private void OnReturningComplete(UIStack uiStack)
    {
        Vector2Int tilePos = _itemsWindow.GetItemToTilePos(uiStack.ItemType.ID);
        _itemsWindow.PlaceStack(uiStack, tilePos);
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
