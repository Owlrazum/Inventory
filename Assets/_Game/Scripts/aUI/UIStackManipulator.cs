using System.Collections;
using UnityEngine;

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

        _isCurrentPlacementPosValid = CraftingDelegatesContainer.IsCurrentPlacementPosValid(
            _selectedStack, 
            _stackSelectionLocalPos, 
            _tileUnderPointer.Pos
        );

        if (!_isCurrentPlacementPosValid)
        {
            return;
        }

        CraftingDelegatesContainer.HighlightTilesUnderSelectedStack?
            .Invoke(_selectedStack, _tileUnderPointer.Pos);
    }

    private void OnTileUnderPointerGone()
    {
        _tileUnderPointer.UndoDebugColor();
        if (_selectedStack == null)
        {
            _isCurrentPlacementPosValid = false;
            _tileUnderPointer = null;
            return;
        }

        if (!_isCurrentPlacementPosValid)
        {
            _tileUnderPointer = null;
            return;
        }

        CraftingDelegatesContainer.DefaultLastTilesUnderSelectedStack?
            .Invoke(_selectedStack, _tileUnderPointer.Pos);
        
        _isCurrentPlacementPosValid = false;
        _tileUnderPointer = null;
    }


    private void OnSelectStackCommand(UIStack stack, Vector2Int stackSelectionLocalPosArg)
    {
        _stackSelectionLocalPos = stackSelectionLocalPosArg;

        if (stack.Data.ItemAmount > 1)
        {
            _selectedStack = PoolingDelegatesContainer.SpawnStack();
            UIStackData stackData = UIStackData.TakeOne(stack.Data);
            _selectedStack.InitializeWithData(stackData, stack.BoundingRect);
            _selectedStack.Rect.anchoredPosition = stack.Rect.anchoredPosition;
            _selectedStack.Rect.sizeDelta = stack.Rect.sizeDelta;
        }
        else
        { 
            _selectedStack = stack;
        }
        
        CraftingDelegatesContainer.EventStackWasSelected?.Invoke(_selectedStack);

        _isCurrentPlacementPosValid = CraftingDelegatesContainer.IsCurrentPlacementPosValid(
            _selectedStack, 
            _stackSelectionLocalPos,
            _tileUnderPointer.Pos
        );
        CraftingDelegatesContainer.HighlightTilesUnderSelectedStack?.Invoke(_selectedStack, _tileUnderPointer.Pos);

        _stackFollowSequence = StackFollowSequence();
        StartCoroutine(_stackFollowSequence);
    }

    private IEnumerator StackFollowSequence()
    {
        _pointerEventsUpdater.RegisterMovingUI();
#if UNITY_EDITOR
        _prevPos = Input.mousePosition;
        _prevPos -= _pickUpDelta;
        yield return null;

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_isCurrentPlacementPosValid)
                {
                    UIStack pushedOutStack = null;
                    CraftingDelegatesContainer.PlaceStackUnderPointer?.Invoke(
                        _selectedStack, 
                        _stackSelectionLocalPos, 
                        out pushedOutStack
                    );

                    if (pushedOutStack == null)
                    {
                        _selectedStack = null;
                        _pointerEventsUpdater.UnregisterMovingUI();
                        _stackFollowSequence = null;
                        yield break;
                    }

                    CheckPushedOutStackSelection();

                    _prevPos = Input.mousePosition;
                    _prevPos -= _pickUpDelta;
                }
                else
                { 
                    CraftingDelegatesContainer.ReturnStackToItemsWindow(_selectedStack);
                }
            }

            Vector2 newPos = Input.mousePosition;
            _selectedStack.Rect.anchoredPosition += newPos - _prevPos;
            _prevPos = newPos;
            _pointerEventsUpdater.NotifyFinishedMove();
            yield return null;
        }
#endif
    }

    // _selectedStack is already initialized by pushedOut
    private void CheckPushedOutStackSelection()
    {
        if (_selectedStack == null)
        {
            return;
        }
        _isCurrentPlacementPosValid = CraftingDelegatesContainer.IsCurrentPlacementPosValid(
            _selectedStack,
            _stackSelectionLocalPos,
            _tileUnderPointer.Pos
        );
    }
}

// Android implementations
/*
    #elif UNITY_ANDROID
        Touch touch = Input.GetTouch(0);
        _prevPos = touch.position;
        _prevPos -= _pickUpDelta;
        yield return null;

        while (true)
        {
            if (Input.touchCount != 1)
            { 
                if (_isCurrentPlacementPosValid)
                {
                    UIStack toPlace = _selectedStack;
                    _selectedStack = CraftingDelegatesContainer.QueryPushedOutByPlacementStack();

                    CraftingDelegatesContainer.EventStackPlacementUnderPointer?.Invoke(toPlace, _selectedStackFillState[0]);
                    CheckPushedOutStackSelection();
                    if (_selectedStack == null)
                    { 
                        UIEventsContainer.EventUnregisterMovingUI?.Invoke();
                        _stackFollowSequence = null;
                        yield break;
                    }

                    _prevPos = Input.mousePosition;
                    _prevPos -= _pickUpDelta;
                }
                else
                {
                    CraftingDelegatesContainer.ActionReturnStackToItsPlace(_selectedStack);
                }
            }

            touch = Input.GetTouch(0);
            Vector2 newPos = touch.position;
            _selectedStack.Rect.anchoredPosition += newPos - _prevPos;
            _prevPos = newPos;
            UIEventsContainer.EventMovingUIFinishedMove?.Invoke();
            yield return null;
        }
*/