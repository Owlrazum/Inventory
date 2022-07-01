using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class UITile : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private int cc;

    [SerializeField]
    private UIStack _placedStack;
    public UIStack PlacedStack
    {
        get { return _placedStack; }
        set { _placedStack = value; }
    }

    [SerializeField]
    private RectTransform _rect;
    public RectTransform Rect 
    {
        get { return _rect; }
    }

    private void Awake()
    {
        TryGetComponent(out _rect);
    }

    public void OnPointerClick(PointerEventData eventData)
    { 

    }
}