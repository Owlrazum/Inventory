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
            _image.color = Color.red;
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

    private enum StateType
    { 
        Default,
        Highlighted
    }
    private StateType _state;

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
        if (_state == StateType.Highlighted || _isStateControlledOutside)
        {
            return;
        }

        _image.sprite = _activeSprite;
        _state = StateType.Highlighted;
        CraftingDelegatesContainer.EventTileUnderPointerCame(this);
    }

    public void OnPointerExit()
    {
        if (_state == StateType.Default || _isStateControlledOutside)
        {
            return;
        }

        _image.sprite = _defaultSprite;
        _state = StateType.Default;
        CraftingDelegatesContainer.EventTileUnderPointerGone();
    }

    public void HighLightState()
    {
        _isStateControlledOutside = true;
        _image.sprite = _activeSprite;
        _state = StateType.Highlighted;
    }

    public void DefaultState()
    { 
        _isStateControlledOutside = false;
        _image.sprite = _defaultSprite;
        _state = StateType.Default;
    }
}