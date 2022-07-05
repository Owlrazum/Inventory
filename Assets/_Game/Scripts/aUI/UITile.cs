using UnityEngine;
using UnityEngine.UI;
using SNG.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UITile : MonoBehaviour, IPointerEnterExitHandler
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

    private RectTransform _interactionRect;
    public RectTransform InteractionRect 
    {
        get { return _interactionRect; }
    }

    private Image _image;

    private bool _isStateControlledOutside;

    public void GenerationInitialize(Vector2Int posArg)
    {
        _pos = posArg;
    }

    private void Awake()
    {
        TryGetComponent(out _rect);
        transform.GetChild(0).TryGetComponent(out _interactionRect);
        TryGetComponent(out _image);

        _placedStack = null;
    }

    private void Start()
    {
        var updater = UIQueriesContainer.QueryGetUpdater();
        updater.AddPointerEnterExitHandler(this);
    }

    public void OnPointerEnter()
    {
        CraftingDelegatesContainer.EventTileUnderPointerCame(this);
    }

    public void OnPointerExit()
    {
        CraftingDelegatesContainer.EventTileUnderPointerGone();
    }

    public void HighLightState()
    {
        _isStateControlledOutside = true;
        _image.sprite = _activeSprite;
    }

    public void DefaultState()
    { 
        _isStateControlledOutside = false;
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