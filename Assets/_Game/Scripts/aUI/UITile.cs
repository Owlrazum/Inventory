using UnityEngine;
using UnityEngine.UI;

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

    public void HighLightState()
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
}