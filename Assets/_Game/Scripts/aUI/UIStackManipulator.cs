using System.Collections;
using UnityEngine;

public class UIStackManipulator : MonoBehaviour
{
    private const float DEPTH_POS_DEFAULT = 0;
    private const float DEPTH_POS_SELECTED = 1;

    [SerializeField]
    private Vector2 _pickUpDelta = Vector2.up;

    private UIStack _selectedStack;
    private Vector2Int[] _selectedStackFillState;
    private UITile _tileUnderPointer;
    private bool _isCurrentPlacementPosValid;

    private IEnumerator _stackFollowSequence;
    private Vector2 _prevPos;

    private void Awake()
    {
        CraftingDelegatesContainer.EventTileUnderPointerCame += OnTileUnderPointerCame;
        CraftingDelegatesContainer.EventTileUnderPointerGone += OnTileUnderPointerGone;

        CraftingDelegatesContainer.EventStackWasSelected += OnStackWasSelected;
        // CraftingDelegatesContainer.FuncSelectedStack += GetSelectedStack;

        CraftingDelegatesContainer.FuncIsStackSelected += IsStackSelected;
    }

    private void OnDestroy()
    { 
        CraftingDelegatesContainer.EventTileUnderPointerCame -= OnTileUnderPointerCame;
        CraftingDelegatesContainer.EventTileUnderPointerGone -= OnTileUnderPointerGone;

        CraftingDelegatesContainer.EventStackWasSelected -= OnStackWasSelected;
        // CraftingDelegatesContainer.FuncSelectedStack -= GetSelectedStack;

        CraftingDelegatesContainer.FuncIsStackSelected -= IsStackSelected;
    }

    private void OnTileUnderPointerCame(UITile tile)
    {
        _tileUnderPointer = tile;
        _tileUnderPointer.DebugColor();

        if (_selectedStack == null)
        {
            return;
        }

        _isCurrentPlacementPosValid =
            CraftingDelegatesContainer.QueryCheckSelectedStackFillStateValid(_selectedStackFillState, _tileUnderPointer.Pos);
        if (!_isCurrentPlacementPosValid)
        {
            return;
        }

        CraftingDelegatesContainer.EventTilesDeltaShouldHighLight?.Invoke(_selectedStackFillState, _tileUnderPointer.Pos);
    }

    private void OnTileUnderPointerGone()
    {
        _tileUnderPointer.UndoDebugColor();
        if (_selectedStack == null)
        {
            // _tileUnderPointer = null;
            return;
        }

        if (!_isCurrentPlacementPosValid)
        {
            return;
        }

        CraftingDelegatesContainer.EventTilesDeltaShouldDefault?.Invoke(_selectedStackFillState, _tileUnderPointer.Pos);
        // _tileUnderPointer = null;
    }


    private void OnStackWasSelected(UIStack stack)
    {
        _selectedStack = stack;

        InitializeSelectedFillState(stack);

        _isCurrentPlacementPosValid = true;
        CraftingDelegatesContainer.EventTilesDeltaShouldHighLight?.Invoke(_selectedStackFillState, _tileUnderPointer.Pos);

        _stackFollowSequence = StackFollowSequence();
        StartCoroutine(_stackFollowSequence);
    }

    private UIStack GetSelectedStack()
    {
        return _selectedStack;
    }

    private bool IsStackSelected()
    {
        return _selectedStack != null;
    }

    private IEnumerator StackFollowSequence()
    {
        _prevPos = Input.mousePosition;
        _prevPos -= _pickUpDelta;
        yield return null;

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_isCurrentPlacementPosValid)
                {
                    UIStack toPlace = _selectedStack;
                    _selectedStack = CraftingDelegatesContainer.QueryPushedOutByPlacementStack();
                    CraftingDelegatesContainer.EventStackPlacementUnderPointer?.Invoke(toPlace);
                    if (_selectedStack == null)
                    { 
                        yield break;
                    }

                    _prevPos = Input.mousePosition;
                    _prevPos -= _pickUpDelta;
                }
            }

            Vector2 newPos = Input.mousePosition;
            _selectedStack.Rect.anchoredPosition += newPos - _prevPos;
            _prevPos = newPos;
            yield return null;
        }
    }

    private void InitializeSelectedFillState(UIStack stack)
    { 
        var fillState = stack.GetFillState();
        _selectedStackFillState = new Vector2Int[fillState.Length];
        fillState.CopyTo(_selectedStackFillState, 0);
        Vector2Int selectedTileIndex = _tileUnderPointer.Pos;
        for (int i = 0; i < _selectedStackFillState.Length; i++)
        {
            _selectedStackFillState[i] -= selectedTileIndex;
        }
    }
}