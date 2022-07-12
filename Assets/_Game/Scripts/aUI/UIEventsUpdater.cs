using System.Collections.Generic;
using UnityEngine;

namespace SNG.UI 
{
    public class UIEventsUpdater : MonoBehaviour
    {
        private const float SQR_MAGNITUDE_POINTER_TOUCH_TOLERANCE = 10;

        [SerializeField]
        private float _offset;

        private List<IPointerTouchHandler> _touchHandlers;

        private List<IPointerEnterExitHandler> _enterExitHandlers;
        private List<bool> _enterStates;

        private List<IPointerLocalPointHandler> _localPointHandlers;

        private int _movingUICount = 0;
        private int _finishedMovingUICount = 0;
        private bool _isBeganTouchValid;

#if UNITY_EDITOR
        private Vector2 _pressMousePos;
        private Vector2 _currentMousePos;
#elif UNITY_ANDROID
        private Touch _currentTouch;
#endif

        private void Awake()
        {
            _touchHandlers = new List<IPointerTouchHandler>();
            
            _enterExitHandlers = new List<IPointerEnterExitHandler>();
            _enterStates = new List<bool>();
            
            _localPointHandlers = new List<IPointerLocalPointHandler>();

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

        public void AddPointerTouchHandler(IPointerTouchHandler handler)
        {
            _touchHandlers.Add(handler);
        }

        public void AddPointerEnterExitHandler(IPointerEnterExitHandler handler)
        {
            _enterExitHandlers.Add(handler);
            _enterStates.Add(false);
        }

        public void AddPointerLocalPointHandler(IPointerLocalPointHandler handler)
        {
            _localPointHandlers.Add(handler);
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
#if UNITY_EDITOR
#elif UNITY_ANDROID
            if (Input.touchCount == 0)
            {
                NotyfyManyPointerExitWithNoTouchPos();
                return;
            }
#endif

#if UNITY_EDITOR
            _currentMousePos = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
            {
                _isBeganTouchValid = true;
                _pressMousePos = Input.mousePosition;
                return;
            }
#elif UNITY_ANDROID
            _currentTouch = Input.GetTouch(0);
    
            if (_currentTouch.phase == TouchPhase.Began)
            {
                _isBeganTouchValid = true;
                return;
            }
#endif
            if (_isBeganTouchValid)
            {
#if UNITY_EDITOR
                if ((_currentMousePos - _pressMousePos).sqrMagnitude > SQR_MAGNITUDE_POINTER_TOUCH_TOLERANCE)
                {
                    _isBeganTouchValid = false;
                }
#elif UNITY_ANDROID
                if (_currentTouch.deltaPosition.sqrMagnitude > SQR_MAGNITUDE_POINTER_TOUCH_TOLERANCE)
                {
                    _isBeganTouchValid = false;
                }
#endif
            }

#if UNITY_EDITOR
            if (Input.GetMouseButtonUp(0))
            {
#elif UNITY_ANDROID
            if (_currentTouch.phase == TouchPhase.Ended || _currentTouch.phase == TouchPhase.Canceled)
            {
#endif
                if (_isBeganTouchValid)
                {
                    NotifyOnePointerTouchIfNeeded();
                }
                else
                { 
                    NotifyManyPointerExitIfNeeded();
                }
                return;
            }

            NotifyManyPointerExitIfNeeded();
            NotifyManyPointerEnterIfNeeded();
            NotifyLocalPointUpdateIfNeeded();
        }

        private void NotifyOnePointerTouchIfNeeded()
        { 
            foreach (var handler in _touchHandlers)
            {
#if UNITY_EDITOR
                if (RectTransformUtility.RectangleContainsScreenPoint(handler.Rect, _currentMousePos))
#elif UNITY_ANDROID
                if (RectTransformUtility.RectangleContainsScreenPoint(handler.Rect, _currentTouch.position))
#endif
                { 
                    handler.OnPointerTouch();
                    return;
                }
            }
        }

        private void NotifyManyPointerExitIfNeeded()
        {
            for (int i = 0; i < _enterExitHandlers.Count; i++)
            {
                IPointerEnterExitHandler handler = _enterExitHandlers[i];
#if UNITY_EDITOR
                if (!RectTransformUtility.RectangleContainsScreenPoint(handler.InteractionRect, 
                    _currentMousePos, null, Vector4.one * _offset))
#elif UNITY_ANDROID
                if (!RectTransformUtility.RectangleContainsScreenPoint(handler.InteractionRect, 
                    _currentTouch.position, null, Vector4.one * _offset))
#endif
                {
                    if (_enterStates[i])
                    {
                        _enterStates[i] = false;
                        handler.OnPointerExit();
                    }
                }
            }
        }

        private void NotyfyManyPointerExitWithNoTouchPos()
        { 
            for (int i = 0; i < _enterExitHandlers.Count; i++)
            {
                if (_enterStates[i])
                {
                    _enterStates[i] = false;
                    _enterExitHandlers[i].OnPointerExit();
                }
            }            
        }

        private void NotifyManyPointerEnterIfNeeded()
        { 
            for (int i = 0; i < _enterExitHandlers.Count; i++)
            {
                IPointerEnterExitHandler handler = _enterExitHandlers[i];
#if UNITY_EDITOR
                if (RectTransformUtility.RectangleContainsScreenPoint(handler.InteractionRect, 
                    _currentMousePos, null, Vector4.one * _offset))
#elif UNITY_ANDROID
                if (RectTransformUtility.RectangleContainsScreenPoint(handler.InteractionRect,
                     _currentTouch.position, null, Vector4.one * _offset))
#endif
                {
                    if (!_enterStates[i])
                    {
                        _enterStates[i] = true;
                        handler.OnPointerEnter();
                    }
                }
            }
        }

        private void NotifyLocalPointUpdateIfNeeded()
        { 
            for (int i = 0; i < _localPointHandlers.Count; i++)
            {
                IPointerLocalPointHandler handler = _localPointHandlers[i];
                if (handler.ShouldUpdateLocalPoint)
                {
#if UNITY_EDITOR
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(handler.Rect,
                        _currentMousePos, null, out Vector2 localPoint
                    );
#elif UNITY_ANDROID
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(handler.Rect,
                        _currentTouch.position, null, out Vector2 localPoint
                    );
#endif
                    handler.UpdateLocalPoint(new Vector2Int((int)localPoint.x, (int)localPoint.y));
                }
            }
        }

        private UIEventsUpdater GetUpdater()
        {
            return this;
        }
    }
}