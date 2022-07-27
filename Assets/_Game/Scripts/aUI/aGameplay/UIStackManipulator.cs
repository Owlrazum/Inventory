using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

using Orazum.UI;

public class UIStackManipulator : MonoBehaviour
{
    private const float DEPTH_POS_DEFAULT = 0;
    private const float DEPTH_POS_SELECTED = 1;

    [SerializeField]
    private Vector2 _pickUpDelta = Vector2.up;

    private UIStack _selectedStack;
    private Vector2Int _stackSelectionLocalPos;

    private UITile _tileUnderPointer;
    private bool _isCurrentPlacementPosValid;

    private IEnumerator _stackFollowSequence;
    private Vector2 _prevPos;

    private UIPointerEventsUpdater _pointerEventsUpdater;

    private void Awake()
    {
        InputDelegatesContainer.SelectStackCommand += OnSelectStackCommand;
        // CraftingDelegatesContainer.FuncSelectedStack += GetSelectedStack;

        CraftingDelegatesContainer.EventTileUnderPointerCame += OnTileUnderPointerCame;
        CraftingDelegatesContainer.EventTileUnderPointerGone += OnTileUnderPointerGone;

        CraftingDelegatesContainer.IsStackSelected += IsStackSelected;
    }

    private void OnDestroy()
    { 
        InputDelegatesContainer.SelectStackCommand -= OnSelectStackCommand;
        // CraftingDelegatesContainer.FuncSelectedStack -= GetSelectedStack;

        CraftingDelegatesContainer.EventTileUnderPointerCame -= OnTileUnderPointerCame;
        CraftingDelegatesContainer.EventTileUnderPointerGone -= OnTileUnderPointerGone;

        CraftingDelegatesContainer.IsStackSelected -= IsStackSelected;
    }

    private void Start()
    {
        _pointerEventsUpdater = UIDelegatesContainer.GetEventsUpdater();
    }

    private UIStack GetSelectedStack()
    {
        return _selectedStack;
    }

    private bool IsStackSelected()
    {
        return _selectedStack != null;
    }

    private void OnTileUnderPointerCame(UITile tile)
    {
        _tileUnderPointer = tile;
        _tileUnderPointer.DebugColor();

        if (_selectedStack == null)
        {
            return;
        }

        switch (_tileUnderPointer.RestingWindow)
        {
            case WindowType.ItemsWindow:
                _isCurrentPlacementPosValid = true;
                break;
            case WindowType.CraftWindow:
                _isCurrentPlacementPosValid = CraftingDelegatesContainer.IsPlacementPosValidInCraftWindow(
                    _selectedStack.Size,
                    _tileUnderPointer.Pos - _stackSelectionLocalPos
                );

                if (_isCurrentPlacementPosValid)
                {
                    CraftingDelegatesContainer.HighlightTilesInCraftWindow(
                        _selectedStack,
                        _tileUnderPointer.Pos - _stackSelectionLocalPos
                    );
                }
                break;
        }
    }

    private void OnTileUnderPointerGone()
    {
        if (_tileUnderPointer != null)
        { 
            // Perhaps the bug is here
            _tileUnderPointer.UndoDebugColor();
        }

        if (_selectedStack != null && _tileUnderPointer.RestingWindow == WindowType.CraftWindow)
        {
            CraftingDelegatesContainer.DefaultLastHighlightInCraftWindow?.Invoke();   
        }
        
        _isCurrentPlacementPosValid = false;
        _tileUnderPointer = null;
    }


    private void OnSelectStackCommand(UIStack stack, Vector2Int stackSelectionLocalPosArg)
    {
        _stackSelectionLocalPos = stackSelectionLocalPosArg;
        _selectedStack = SelectStack(stack);

        CraftingDelegatesContainer.EventStackWasSelected?.Invoke(_selectedStack);
        _selectedStack.RestingWindow = WindowType.NoWindow; // Others need info on RestingWindow

        _isCurrentPlacementPosValid = true;

        if (_tileUnderPointer.RestingWindow == WindowType.CraftWindow)
        {
            CraftingDelegatesContainer.HighlightTilesInCraftWindow(
                _selectedStack, 
                _tileUnderPointer.Pos - _stackSelectionLocalPos
            );
        }

        _stackFollowSequence = StackFollowSequence();
        StartCoroutine(_stackFollowSequence);
    }

    private UIStack SelectStack(UIStack stack)
    {
        UIStack toReturn;
        if (stack.Data.ItemAmount > 1)
        {
            toReturn = PoolingDelegatesContainer.SpawnStack();
            UIStackData stackData = UIStackData.TakeOne(stack.Data);
            toReturn.InitializeWithData(stackData, stack.BoundingRect);
            toReturn.Rect.anchoredPosition = stack.Rect.anchoredPosition;
            toReturn.Rect.sizeDelta = stack.Rect.sizeDelta;
        }
        else
        { 
            toReturn = stack;
            if (_tileUnderPointer.RestingWindow == WindowType.ItemsWindow)
            { 
                CraftingDelegatesContainer.EventLastStackIDWasTakenFromItemsWindow(toReturn);
            }
        }

        return toReturn;
    }

    private IEnumerator StackFollowSequence()
    {
        _pointerEventsUpdater.RegisterMovingUI();
        _prevPos = InputDelegatesContainer.GetPointerPosition();
        _prevPos -= _pickUpDelta;
        yield return null;

        while (true)
        {
            if (_selectedStack.IsPointerUp)
            {
                if (_isCurrentPlacementPosValid)
                {
                    CraftingDelegatesContainer.PlaceStack(
                        _selectedStack, 
                        _tileUnderPointer.Pos - _stackSelectionLocalPos,
                        _tileUnderPointer.RestingWindow
                    );
                }
                else
                {
                    CraftingDelegatesContainer.ReturnStack(_selectedStack);
                }

                _selectedStack = null;
                _pointerEventsUpdater.UnregisterMovingUI();
                _stackFollowSequence = null;

                yield break;
            }

            Vector2 newPos = InputDelegatesContainer.GetPointerPosition();
            _selectedStack.Rect.anchoredPosition += newPos - _prevPos;
            _prevPos = newPos;
            _pointerEventsUpdater.NotifyFinishedMove();
            yield return null;
        }
    }
}
