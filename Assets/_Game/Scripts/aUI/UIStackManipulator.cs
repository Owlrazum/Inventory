using System.Collections;
using UnityEngine;

public class UIStackManipulator : MonoBehaviour
{
    private const float DEPTH_POS_DEFAULT = 0;
    private const float DEPTH_POS_SELECTED = 1;

    [SerializeField]
    private Vector2 _pickUpDelta = Vector2.up;

    private UIStack _selectedStack;
    private Vector2 _prevPos;

    private IEnumerator _stackFollowSequence;

    private void Awake()
    {
        CraftingDelegatesContainer.EventStackWasSelected += OnStackWasSelected;
        CraftingDelegatesContainer.FuncIsStackSelected += IsStackSelected;
    }

    private void OnDestroy()
    { 
        CraftingDelegatesContainer.EventStackWasSelected -= OnStackWasSelected;
        CraftingDelegatesContainer.FuncIsStackSelected -= IsStackSelected;
    }

    private void OnStackWasSelected(UIStack stack)
    {
        _selectedStack = stack;
        _stackFollowSequence = StackFollowSequence();
        StartCoroutine(_stackFollowSequence);
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
                if (CraftingDelegatesContainer.QueryIsStackPlaceableOnTileUnderPointer())
                {
                    CraftingDelegatesContainer.EventStackPlacementUnderPointer?.Invoke(_selectedStack);
                    yield break;
                }
            }

            Vector2 newPos = Input.mousePosition;
            _selectedStack.Rect.anchoredPosition += newPos - _prevPos;
            _prevPos = newPos;
            yield return null;
        }
    }
}