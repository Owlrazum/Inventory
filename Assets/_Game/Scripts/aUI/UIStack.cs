using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Orazum.UI;

[System.Serializable]
public class UIStackData
{
    [SerializeField]
    private int _itemAmount;
    public int ItemAmount
    {
        get { return _itemAmount; }
        set
        {
            _itemAmount = value;
            EventItemAmountChanged?.Invoke();
        }
    }
    public int ItemTypeID;
    
    [SerializeField]
    private Vector2Int _tilePos;
    public Vector2Int TilePos
    {
        get { return _tilePos; }
        set
        {
            _tilePos = value;
            EventTilePosChanged?.Invoke();
        }
    }

    [NonSerialized]
    public int TileInstanceID;

    [NonSerialized]
    public Action EventTilePosChanged;

    [NonSerialized]
    public Action EventItemAmountChanged;


    public static UIStackData TakeOne(UIStackData copyFrom)
    {
        copyFrom.ItemAmount--;
        UIStackData stackData = new UIStackData();

        stackData.ItemAmount = 1;
        stackData.ItemTypeID = copyFrom.ItemTypeID;
        stackData._tilePos = copyFrom.TilePos;
        stackData.TileInstanceID = copyFrom.TileInstanceID;
        return stackData;
    }
}

// LastPoint: Returning to its place, ItemType get private set

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UIStack : MonoBehaviour, IPointerTouchHandler, IPointerEnterExitHandler
{
    public UIStackData Data { get; private set; }
    public RectTransform BoundingRect { get; private set; }

    private ItemSO _itemSO;
    public ItemSO ItemType {
        get{
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
        Data = stackData;
        BoundingRect = boundingRect;

        _itemSO = CraftingDelegatesContainer.GetItemSO(stackData.ItemTypeID);
        _fillState = new Vector2Int[_itemSO.Size.x * _itemSO.Size.y];
        _shouldUpdateFillState = true;
        Data.EventTilePosChanged += OnTilePosChanged;
        Data.EventItemAmountChanged += OnItemAmountChanged;

        _image.sprite = _itemSO.Sprite;
        if (Data.ItemAmount == 1)
        {
            _textMesh.text = "";
        }
        else
        {
            _textMesh.text = Data.ItemAmount.ToString();
        }


        _rect.SetParent(BoundingRect, false);

        gameObject.SetActive(true);
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
        Data.EventTilePosChanged -= OnTilePosChanged;
        Data.EventItemAmountChanged -= OnItemAmountChanged;
        Data = null;
        PoolingDelegatesContainer.DespawnStack(this);
    }

    private void OnTilePosChanged()
    {
        _shouldUpdateFillState = true;
    }

    private void OnItemAmountChanged()
    { 
        if (Data.ItemAmount == 1)
        {
            _textMesh.text = "";
        }
        else
        {
            _textMesh.text = Data.ItemAmount.ToString();
        }
    }

    public void UpdateRect(Vector2 anchoredPos, Vector2 sizeDelta, RectTransform newParent = null)
    {
        _rect.anchoredPosition = anchoredPos;
        _rect.sizeDelta = sizeDelta;
        _rect.SetParent(newParent, false);
    }

    private RectTransform _rect;
    public RectTransform Rect { get { return _rect; } }

    private RectTransform _interactionRect;
    public RectTransform InteractionRect { get { return _interactionRect; } }
    public int InstanceID { get { return GetInstanceID(); } }

    private bool _enterState;
    public bool EnterState { get { return _enterState; } set { _enterState = value; } }

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
        UIDelegatesContainer.GetEventsUpdater().AddPointerTouchHandler(this);
        UIDelegatesContainer.GetEventsUpdater().AddPointerEnterExitHandler(this);
    }

    private void Subscribe()
    {
    }

    private void OnDestroy()
    {
        if (Data != null)
        { 
            Data.EventTilePosChanged -= OnTilePosChanged;
            Data.EventItemAmountChanged -= OnItemAmountChanged;
        }
    }

    public void ReturnToItsPlace(Vector2 anchPos)
    {
        StartCoroutine(ReturnalSequence(anchPos));
    }

    private IEnumerator ReturnalSequence(Vector2 anchPos)
    {
        yield return null;
    }

    public Vector2Int[] GetFillState()
    {
        if (_shouldUpdateFillState)
        { 
            int indexer = 0;
            for (int j = Data.TilePos.y; j < Data.TilePos.y + _itemSO.Size.y; j++)
            {
                for (int i = Data.TilePos.x; i < Data.TilePos.x + _itemSO.Size.x; i++)
                {
                    _fillState[indexer++] = new Vector2Int(i, j);
                }
            }
            _shouldUpdateFillState = false;
        }

        return _fillState;
    }

    public void OnPointerTouch()
    {
        if (CraftingDelegatesContainer.IsStackSelected())
        {
            return;
        }

        CraftingDelegatesContainer.EventStackWasSelected?.Invoke(this);
    }

    public void OnPointerEnter()
    {
        CraftingDelegatesContainer.HighlightStack?.Invoke(this);
    }

    public void OnPointerExit()
    { 
        CraftingDelegatesContainer.DefaultStack?.Invoke(this);
    }

    public Transform GetTransform()
    {
        return transform;
    }
}


// rect.anchorMin = new Vector2(0, 1);
// rect.anchorMax = new Vector2(0, 1);
// rect.pivot = new Vector2(0.5f, 0.5f);