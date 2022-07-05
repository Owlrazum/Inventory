using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SNG.UI;

[System.Serializable]
public class UIStackData
{ 
    public int ItemAmount;
    public int ItemTypeID;
    
    [SerializeField]
    private Vector2Int _tilePos;

    public Vector2Int TilePos
    {
        get { return _tilePos; }
        set
        {
            _tilePos = value;
            TilePosEventChanged?.Invoke();
        }
    }

    [NonSerialized]
    public Action TilePosEventChanged;
}

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UIStack : MonoBehaviour, IPoolable, IPointerClickHandler, IPointerEnterExitHandler
{
    public UIStackData StackData { get; private set; }
    public RectTransform BoundingRect { get; private set; }

    private ItemSO _itemSO;
    public ItemSO ItemType
    {
        get
        {
            return _itemSO;
        }
    }

    public Vector2Int Size
    {
        get
        {
            return _itemSO.Size;
        }
    }

    public void InitializeWithData(UIStackData stackData, RectTransform boundingRect)
    {
        StackData = stackData;
        BoundingRect = boundingRect;

        _itemSO = CraftingDelegatesContainer.FuncGetItemSO(stackData.ItemTypeID);
        _fillState = new Vector2Int[_itemSO.Size.x * _itemSO.Size.y];
        _shouldUpdateFillState = true;
        StackData.TilePosEventChanged += OnTilePosChanged;

        _image.sprite = _itemSO.Sprite;
        if (StackData.ItemAmount == 1)
        {
            _textMesh.text = "";
        }
        else
        {
            _textMesh.text = StackData.ItemAmount.ToString();
        }


        _rect.SetParent(BoundingRect, false);

        gameObject.SetActive(true);
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
        StackData.TilePosEventChanged -= OnTilePosChanged;
        StackData = null;
        PoolingDelegatesContainer.EventDespawnUIStack(this);
    }

    private void OnTilePosChanged()
    {
        _shouldUpdateFillState = true;
    }

    public void UpdateRect(Vector2 anchoredPos, Vector2 sizeDelta)
    {
        _rect.anchoredPosition = anchoredPos;
        _rect.sizeDelta = sizeDelta;
    }

    private RectTransform _rect;
    public RectTransform Rect { get { return _rect; } }

    private RectTransform _interactionRect;
    public RectTransform InteractionRect { get { return _interactionRect; } }

    private Image _image;
    private TextMeshProUGUI _textMesh;

    private Vector2Int[] _fillState;
    private bool _shouldUpdateFillState;

    private void Awake()
    {
        TryGetComponent(out _rect);
        TryGetComponent(out _image);
        if (!transform.GetChild(0).TryGetComponent(out _textMesh))
        {
            Debug.LogError("UIStack should have textMesh in its first child");
        }
        if (!transform.GetChild(1).TryGetComponent(out _interactionRect))
        { 
            Debug.LogError("UIStack should have interactionRect as its second child");
        }

        Subscribe();
    }

    private void Start()
    {
        UIQueriesContainer.QueryGetUpdater().AddPointerClickHandler(this);
        UIQueriesContainer.QueryGetUpdater().AddPointerEnterExitHandler(this);
    }

    private void Subscribe()
    {
    }

    private void OnDestroy()
    {
        if (StackData != null)
        { 
            StackData.TilePosEventChanged -= OnTilePosChanged;
        }
    }

    public Vector2Int[] GetFillState()
    {
        if (_shouldUpdateFillState)
        { 
            int indexer = 0;
            for (int j = StackData.TilePos.y; j < StackData.TilePos.y + _itemSO.Size.y; j++)
            {
                for (int i = StackData.TilePos.x; i < StackData.TilePos.x + _itemSO.Size.x; i++)
                {
                    _fillState[indexer++] = new Vector2Int(i, j);
                }
            }
            _shouldUpdateFillState = false;
        }

        return _fillState;
    }

    public void OnPointerClick(MouseButtonType pressedButton)
    {
        if (CraftingDelegatesContainer.QueryIsStackSelected())
        {
            return;
        }

        CraftingDelegatesContainer.EventStackWasSelected?.Invoke(this);

        switch(pressedButton)
        {
            case MouseButtonType.Left:
                OnLeftClick();
                break;
            case MouseButtonType.Right:
                OnRightClick();
                break;
        }
    }

    private void OnLeftClick()
    {
    }

    private void OnRightClick()
    { 

    }

    public void OnPointerEnter()
    {
        CraftingDelegatesContainer.EventStackShouldHighlight?.Invoke(this);
    }

    public void OnPointerExit()
    { 
        CraftingDelegatesContainer.EventStackShouldDefault?.Invoke(this);
    }

    public Transform GetTransform()
    {
        return transform;
    }
}


// rect.anchorMin = new Vector2(0, 1);
// rect.anchorMax = new Vector2(0, 1);
// rect.pivot = new Vector2(0.5f, 0.5f);