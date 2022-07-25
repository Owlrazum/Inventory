using System.Collections.Generic;
using UnityEngine;

namespace Orazum.UI 
{
    public class UIPointerEventsUpdater : MonoBehaviour
    {
        private const float SQR_MAGNITUDE_POINTER_TOUCH_TOLERANCE = 10;

        [SerializeField]
        private float _offset = 1;

        private Dictionary<int, IPointerTouchHandler> _touchHandlers;
        private Dictionary<int, IPointerEnterExitHandler> _enterExitHandlers;
        private Dictionary<int, IPointerLocalPointHandler> _localPointHandlers;

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
            _touchHandlers      = new Dictionary<int, IPointerTouchHandler>();
            _enterExitHandlers  = new Dictionary<int, IPointerEnterExitHandler>();
            _localPointHandlers = new Dictionary<int, IPointerLocalPointHandler>();

            UIDelegatesContainer.GetEventsUpdater += GetUpdater;
        }

        private void OnDestroy()
        { 
            UIDelegatesContainer.GetEventsUpdater -= GetUpdater;
        }

        public void AddPointerTouchHandler(IPointerTouchHandler handler)
        {
            _touchHandlers.Add(handler.InstanceID, handler);
        }

        public void AddPointerEnterExitHandler(IPointerEnterExitHandler handler)
        {
            handler.EnterState = false;
            _enterExitHandlers.Add(handler.InstanceID, handler);
        }

        public void AddPointerLocalPointHandler(IPointerLocalPointHandler handler)
        {
            _localPointHandlers.Add(handler.InstanceID, handler);
        }

        public void RemovePointerTouchHandler(IPointerTouchHandler handler)
        {
            _touchHandlers.Remove(handler.InstanceID);
        }

        public void RemovePointerEnterExitHandler(IPointerEnterExitHandler handler)
        {
            handler.EnterState = false;
            _enterExitHandlers.Remove(handler.InstanceID);
        }

        public void RemovePointerLocalPointHandler(IPointerLocalPointHandler handler)
        {
            _localPointHandlers.Remove(handler.InstanceID);
        }

        public void RegisterMovingUI()
        {
            _movingUICount++;
        }

        public void UnregisterMovingUI()
        {
            _movingUICount--;
        }

        public void NotifyFinishedMove()
        {
            _finishedMovingUICount++;
            if (_finishedMovingUICount == _movingUICount)
            {
                UpdatePointerHandlers();
                _finishedMovingUICount = 0;
            }
        }

        public Vector2Int GetLocalPoint(RectTransform rect, out bool isValid)
        {
#if UNITY_EDITOR
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,
                Input.mousePosition, null, out Vector2 localPoint
            );
            isValid = true;
#elif UNITY_ANDROID
            if (Input.touchCount != 1)
            {
                isValid = false;
                return Vector2Int.zero;
            }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,
                _currentTouch.position, null, out Vector2 localPoint
            );
#endif
            return new Vector2Int((int)localPoint.x, (int)localPoint.y);
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
                    _isBeganTouchValid = false;
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
            foreach (var pair in _touchHandlers)
            {
                var handler = pair.Value;
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
            foreach (var pair in _enterExitHandlers)
            {
                var handler = pair.Value;
#if UNITY_EDITOR
                if (!RectTransformUtility.RectangleContainsScreenPoint(handler.InteractionRect, 
                    _currentMousePos, null, Vector4.one * _offset))
#elif UNITY_ANDROID
                if (!RectTransformUtility.RectangleContainsScreenPoint(handler.InteractionRect, 
                    _currentTouch.position, null, Vector4.one * _offset))
#endif
                {
                    if (handler.EnterState)
                    {
                        handler.EnterState = false;
                        handler.OnPointerExit();
                    }
                }
            }
        }

        private void NotyfyManyPointerExitWithNoTouchPos()
        { 
            foreach (var pair in _enterExitHandlers)
            {
                var handler = pair.Value;
                if (handler.EnterState)
                {
                    handler.EnterState = false;
                    handler.OnPointerExit();
                }
            }            
        }

        private void NotifyManyPointerEnterIfNeeded()
        { 
            foreach (var pair in _enterExitHandlers)
            {
                var handler = pair.Value;
#if UNITY_EDITOR
                if (RectTransformUtility.RectangleContainsScreenPoint(handler.InteractionRect, 
                    _currentMousePos, null, Vector4.one * _offset))
#elif UNITY_ANDROID
                if (RectTransformUtility.RectangleContainsScreenPoint(handler.InteractionRect,
                     _currentTouch.position, null, Vector4.one * _offset))
#endif
                {
                    if (!handler.EnterState)
                    {
                        handler.EnterState = true;
                        handler.OnPointerEnter();
                    }
                }
            }
        }

        private void NotifyLocalPointUpdateIfNeeded()
        { 
            foreach (var pair in _localPointHandlers)
            {
                var handler = pair.Value;
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

        private UIPointerEventsUpdater GetUpdater()
        {
            return this;
        }
    }
}