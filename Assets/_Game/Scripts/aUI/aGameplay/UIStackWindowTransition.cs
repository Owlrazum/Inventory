using System.Collections;
using UnityEngine;

/// <summary>
/// It is used to change the size of UIStack so it fits correctly to different tile sizes of 
/// craftWindow and itemsWindow
/// </summary>
[RequireComponent(typeof(RectTransform))]
[DefaultExecutionOrder(-1)]
public class UIStackWindowTransition : MonoBehaviour
{
    private int _craftWindowBorder;
    private int _itemsWindowBorder;
    private int _borderRange;

    private int _craftWindowTileSize;
    private int _itemsWindowTileSize;

    private Vector2Int _craftWindowStackSize;
    private Vector2Int _itemsWindowStackSize;

    private RectTransform _rect;

    private UIStack _trackedStack;
    private IEnumerator _trackingCoroutine;

    private void Awake()
    {
        TryGetComponent(out _rect);
        
        _craftWindowBorder = (int)(_rect.anchoredPosition.y + _rect.rect.size.y / 2);
        _itemsWindowBorder = (int)(_rect.anchoredPosition.y - _rect.rect.size.y / 2);
        _borderRange = _craftWindowBorder - _itemsWindowBorder;


        CraftingDelegatesContainer.EventStackWasSelected += OnStackWasSelected;
        GameDelegatesContainer.StartLevel += OnStartLevel;
    }

    private void OnDestroy()
    {
        CraftingDelegatesContainer.EventStackWasSelected -= OnStackWasSelected;
        GameDelegatesContainer.StartLevel -= OnStartLevel;
    }

    private void OnStartLevel(LevelDescriptionSO levelDescriptionSO)
    { 
        _craftWindowTileSize = levelDescriptionSO.GetTileGenParams(WindowType.CraftWindow).TileSize;
        _itemsWindowTileSize = levelDescriptionSO.GetTileGenParams(WindowType.ItemsWindow).TileSize;
    }

    private void OnStackWasSelected(UIStack selectedStack)
    {
        _trackedStack = selectedStack;

        _craftWindowStackSize = _trackedStack.Size * _craftWindowTileSize;
        _itemsWindowStackSize = new Vector2Int(_itemsWindowTileSize, _itemsWindowTileSize);

        _trackingCoroutine = TrackingCoroutine();
        StartCoroutine(_trackingCoroutine);
    }

    private IEnumerator TrackingCoroutine()
    {
        while (true)
        {
            yield return null;
            
            Transform prevParent = _trackedStack.Rect.parent;
            _trackedStack.Rect.SetParent(_rect, true);
            int trackPos = (int)(_trackedStack.Rect.anchoredPosition.y + _trackedStack.Rect.rect.size.y / 2);
            _trackedStack.Rect.SetParent(prevParent, true);
            if (trackPos < 0)
            {
                _trackedStack.ChangeSizeDuringTransition(_itemsWindowStackSize);
                continue;
            }
            else if (trackPos > _borderRange)
            {
                _trackedStack.ChangeSizeDuringTransition(_craftWindowStackSize);
                continue;
            }

            float lerpParam = trackPos * 1.0f / _borderRange;
            Vector2 sizeFloat = Vector2.Lerp(_itemsWindowStackSize, _craftWindowStackSize, lerpParam);
            Vector2Int size = new Vector2Int((int)sizeFloat.x, (int)sizeFloat.y);
            _trackedStack.ChangeSizeDuringTransition(size);
        }
    }
}