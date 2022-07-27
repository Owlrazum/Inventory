using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Orazum.UI;

public enum WindowTransitionStateType
{
    ItemsWindow,
    Transition,
    CraftWindow
}

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
        }
    }

    [NonSerialized]
    public int TileInstanceID;

    // [NonSerialized]
    // public Action EventTilePosChanged;

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

// LastPoint: Returning to its place
// TODO: make PointerDown and PointerUp handlers

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UIStack : MonoBehaviour, IPointerTouchHandler, IPointerDownUpHandler
{
    public UIStackData Data { get; private set; }
    public RectTransform BoundingRect { get; private set; }

    private ItemSO _itemSO;
    public ItemSO ItemType {
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

    public Vector2Int Pos
    {
        get
        {
            return Data.TilePos;
        }
    }

    public int ItemAmount
    {
        get
        {
            return Data.ItemAmount;
        }
    }

    public WindowType RestingWindow
    { 
        get; 
        set; 
    }

    public void InitializeWithData(UIStackData stackData, RectTransform boundingRect)
    {
        Data = stackData;
        BoundingRect = boundingRect;

        _itemSO = CraftingDelegatesContainer.GetItemSO(stackData.ItemTypeID);
        _fillState = new Vector2Int[_itemSO.Size.x * _itemSO.Size.y];
        _shouldUpdateFillState = true;
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
        Data.EventItemAmountChanged -= OnItemAmountChanged;
        Data = null;
        PoolingDelegatesContainer.DespawnStack(this);
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
        
        if (newParent != null)
        { 
            _rect.SetParent(newParent, false);
        }
    }

    private RectTransform _rect;
    public RectTransform Rect { get { return _rect; } }

    private RectTransform _interactionRect;
    public RectTransform InteractionRect { get { return _interactionRect; } }
    public int InstanceID { get { return GetInstanceID(); } }

    private bool _enterState;
    public bool EnterState { get { return _enterState; } set { _enterState = value; } }

    public bool IsPointerDown { get; set; }
    public bool IsPointerUp { get; set; }

    public bool ShouldInvokePointerTouchEvent { get { return GameDelegatesContainer.GetGameState() == GameStateType.Crafting; } }

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
    }

    private void Start()
    {
        var uiEventsUpdater = UIDelegatesContainer.GetEventsUpdater();
        uiEventsUpdater.AddPointerTouchHandler(this);
        uiEventsUpdater.AddPointerDownUpHandler(this);
    }

    private void OnDestroy()
    {
        if (Data != null)
        { 
            Data.EventItemAmountChanged -= OnItemAmountChanged;
        }
    }

    public void ReturnToPosInItemsWindow(Vector2 targetPos, Vector2 targetSize, float lerpSpeed, Action<UIStack> OnLerpComplete)
    {
        RestingWindow = WindowType.NoWindow;
        StartCoroutine(ReturnalToItemsWindowSequence(targetPos, targetSize, lerpSpeed, OnLerpComplete));
    }

    private IEnumerator ReturnalToItemsWindowSequence(Vector2 targetPos, Vector2 targetSize, float lerpSpeed, Action<UIStack> OnLerpComplete)
    {
        float lerpParam = 0;
        Vector2 initialPos = _rect.anchoredPosition;
        Vector2 initialSize = _rect.sizeDelta;
        while (lerpParam < 1)
        {
            lerpParam += lerpSpeed * Time.deltaTime;
            _rect.anchoredPosition = Vector2.Lerp(initialPos, targetPos, lerpParam);
            _rect.sizeDelta = Vector2.Lerp(initialSize, targetSize, lerpParam);
            yield return null;
        }

        RestingWindow = WindowType.ItemsWindow;
        OnLerpComplete(this);
        // TODO Place stack in itemsWindow.
    }

    public void OnPointerTouch()
    {
        if (CraftingDelegatesContainer.IsStackSelected())
        {
            return;
        }


        if (RestingWindow == WindowType.ItemsWindow)
        {
            InputDelegatesContainer.SelectStackCommand?.Invoke(this, Vector2Int.zero);
        }
        else if (RestingWindow == WindowType.CraftWindow)
        {
            Vector2Int localPoint = UIDelegatesContainer.GetEventsUpdater().GetLocalPoint(Rect, out bool isValid);
            // it returns as a negative with current sceen setup;
            // SceneChange: Hope that it will not change.
            localPoint.y = -localPoint.y; 
            if (!isValid)
            {
                return;
            }

            int tileSize = CraftingDelegatesContainer.GetTileSizeInCraftWindow();
            int col = localPoint.x / tileSize;
            int row = localPoint.y / tileSize;

            InputDelegatesContainer.SelectStackCommand?.Invoke(this, new Vector2Int(col, row));
        }
    }

    public void ChangeSizeDuringTransition(Vector2 newSize)
    {
        _rect.sizeDelta = newSize;
    }
}

// rect.anchorMin = new Vector2(0, 1);
// rect.anchorMax = new Vector2(0, 1);
// rect.pivot = new Vector2(0.5f, 0.5f);