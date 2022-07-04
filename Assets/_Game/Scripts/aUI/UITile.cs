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
        set { _placedStack = value; }
    }

    [SerializeField]
    private RectTransform _rect;
    public RectTransform Rect 
    {
        get { return _rect; }
    }

    [SerializeField]
    private Sprite _activeSprite;

    [SerializeField]
    private Sprite _defaultSprite;

    private Image _image;

    private void Awake()
    {
        TryGetComponent(out _rect);
        TryGetComponent(out _image);
    }

    private void Start()
    {
        var updater = UIQueriesContainer.QueryGetUpdater();
        updater.AddPointerEnterExitHandler(this);
    }

    public void OnPointerEnter()
    {
        if (_placedStack == null)
        {
            return;
        }

        _image.sprite = _activeSprite;
    }

    public void OnPointerExit()
    {
        if (_placedStack == null)
        {
            return;
        }

        _image.sprite = _defaultSprite;
    }
}