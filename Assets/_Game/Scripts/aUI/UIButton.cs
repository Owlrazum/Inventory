using System;

using UnityEngine;
using SNG.UI;

[RequireComponent(typeof(RectTransform))]
public class UIButton : MonoBehaviour, IPointerTouchHandler
{
    private RectTransform _rect;
    public RectTransform Rect { get { return _rect; } }
    
    private void Awake()
    {
        TryGetComponent(out _rect);
    }

    private void Start()
    {
        UIQueriesContainer.QueryGetUpdater().AddPointerTouchHandler(this);
    }

    public virtual void OnPointerTouch()
    {
        EventOnTouch?.Invoke();
    }

    public Action EventOnTouch { get; set; }
}