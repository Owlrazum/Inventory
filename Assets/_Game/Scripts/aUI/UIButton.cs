using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButton : Selectable, IPointerClickHandler
{
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        { 
            return;
        }

        OnClick();
    }

    protected virtual void OnClick()
    {
        EventOnClick?.Invoke();
    }

    public Action EventOnClick { get; set; }
}