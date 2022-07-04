using System;

using UnityEngine;
using SNG.UI;

[RequireComponent(typeof(RectTransform))]
public class UIButton : MonoBehaviour, IPointerClickHandler
{
    private RectTransform _rect;
    public RectTransform Rect { get { return _rect; } }
    
    private void Awake()
    {
        TryGetComponent(out _rect);
    }

    private void Start()
    {
        UIQueriesContainer.QueryGetUpdater().AddPointerClickHandler(this);
    }

    public virtual void OnPointerClick(MouseButtonType pressedButton)
    {
        EventOnClick?.Invoke();
    }

    public Action EventOnClick { get; set; }
}