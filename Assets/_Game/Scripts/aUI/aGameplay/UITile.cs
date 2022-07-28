using UnityEngine;
using UnityEngine.UI;

public enum WindowType
{ 
    NoWindow,
    CraftWindow,
    ItemsWindow
}

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UITile : MonoBehaviour
{
    [SerializeField]
    private UIStack _placedStack;
    public UIStack PlacedStack
    {
        get { return _placedStack; }
        set
        {
            _placedStack = value;
        }
    }

    [SerializeField]
    private Vector2Int _pos;
    public Vector2Int Pos
    {
        get { return _pos; }
    }

    [SerializeField]
    private Sprite _activeSprite;

    [SerializeField]
    private Sprite _defaultSprite;

    [SerializeField]
    private WindowType _restingWindow;
    public WindowType RestingWindow
    {
        get { return _restingWindow; }
    }

    private RectTransform _rect;
    public RectTransform Rect 
    {
        get { return _rect; }
    }

    public RectTransform InteractionRect 
    {
        get { return _rect; }
    }

    private Image _image;

    public void GenerationInitialize(Vector2Int posArg)
    {
        _pos = posArg;
    }

    private void Awake()
    {
        TryGetComponent(out _rect);
        TryGetComponent(out _image);

        _placedStack = null;
    }

    public void HighLightFreeState()
    {
        _image.sprite = _activeSprite;
    }

    public void HighlightFilledState()
    {
        _image.sprite = _activeSprite;
    }

    public void DefaultState()
    { 
        _image.sprite = _defaultSprite;
    }

    public void DebugColor()
    {
        _image.color = Color.red;
    }

    public void UndoDebugColor()
    { 
        _image.color = Color.white;
    }

    public void AssignWindowTypeOnGeneration(WindowType windowTypeArg)
    {
        _restingWindow = windowTypeArg;
    }
}