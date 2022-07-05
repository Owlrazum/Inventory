using System.Collections.Generic;
using UnityEngine;
using SNG.UI;

public enum MouseButtonType
{ 
    Left,
    Right,
    Middle
}

public class UIEventsUpdater : MonoBehaviour
{
    private List<IPointerClickHandler> _clickHandlers;

    private List<IPointerEnterExitHandler> _enterExitHandlers;
    private List<bool> _enterStates;

    private int _movingUICount = 0;
    private int _finishedMovingUICount = 0;

    private void Awake()
    {
        _clickHandlers = new List<IPointerClickHandler>();
        _enterExitHandlers = new List<IPointerEnterExitHandler>();
        _enterStates = new List<bool>();

        UIEventsContainer.EventRegisterMovingUI += RegisterMovingUI;
        UIEventsContainer.EventUnregisterMovingUI += UnregisterMovingUI;
        UIEventsContainer.EventMovingUIFinishedMove += OnMovingUIFinishedMove;

        UIQueriesContainer.FuncGetUpdater += GetUpdater;
    }

    private void OnDestroy()
    { 
        UIEventsContainer.EventRegisterMovingUI -= RegisterMovingUI;
        UIEventsContainer.EventUnregisterMovingUI -= UnregisterMovingUI;
        UIEventsContainer.EventMovingUIFinishedMove -= OnMovingUIFinishedMove;

        UIQueriesContainer.FuncGetUpdater -= GetUpdater;
    }

    public void AddPointerClickHandler(IPointerClickHandler handler)
    {
        _clickHandlers.Add(handler);
    }

    public void AddPointerEnterExitHandler(IPointerEnterExitHandler handler)
    {
        _enterExitHandlers.Add(handler);
        _enterStates.Add(false);
    }

    private void RegisterMovingUI()
    {
        _movingUICount++;
    }

    private void UnregisterMovingUI()
    {
        _movingUICount--;
    }

    private void OnMovingUIFinishedMove()
    {
        _finishedMovingUICount++;
        if (_finishedMovingUICount == _movingUICount)
        {
            UpdatePointerHandlers();
            _finishedMovingUICount = 0;
        }
    }

    private void Update()
    {
        if (_movingUICount == 0)
        {
            UpdatePointerHandlers();
            _finishedMovingUICount = 0;
        }
    }

    private void UpdatePointerHandlers()
    {
        Vector2 mousePos = Input.mousePosition;
        foreach (var handler in _clickHandlers)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(handler.Rect, mousePos))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    handler.OnPointerClick(MouseButtonType.Left);
                    continue;
                }

                if (Input.GetMouseButtonDown(1))
                {
                    handler.OnPointerClick(MouseButtonType.Right);
                    continue;
                }
            }
        }

        for (int i = 0; i < _enterExitHandlers.Count; i++)
        {
            IPointerEnterExitHandler handler = _enterExitHandlers[i];
            if (!RectTransformUtility.RectangleContainsScreenPoint(handler.InteractionRect, mousePos, null))
            {
                if (_enterStates[i])
                {
                    _enterStates[i] = false;
                    handler.OnPointerExit();
                }
            }
        }

        for (int i = 0; i < _enterExitHandlers.Count; i++)
        {
            IPointerEnterExitHandler handler = _enterExitHandlers[i];
            if (RectTransformUtility.RectangleContainsScreenPoint(handler.InteractionRect, mousePos, null))
            {
                if (!_enterStates[i])
                {
                    _enterStates[i] = true;
                    handler.OnPointerEnter();
                }
            }
        }
    }

    private UIEventsUpdater GetUpdater()
    {
        return this;
    }
}