using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SNG.UI;

[System.Serializable]
public class UIStackData
{ 
    public int ItemAmount;
    public int ItemTypeID;
    
    public Vector2Int TilePos;
}

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UIStack : MonoBehaviour, IPoolable, IPointerClickHandler
{
    public UIStackData StackData { get; private set; }
    public RectTransform BoundingRect { get; private set; }

    private ItemSO _itemSO;

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

    public void UpdateRect(Vector2 anchoredPos, Vector2 sizeDelta)
    {
        _rect.anchoredPosition = anchoredPos;
        _rect.sizeDelta = sizeDelta;
    }

    private RectTransform _rect;
    public RectTransform Rect { get { return _rect; } }

    private Image _image;
    private TextMeshProUGUI _textMesh;

    private void Awake()
    {
        TryGetComponent(out _rect);
        TryGetComponent(out _image);
        if (!transform.GetChild(0).TryGetComponent(out _textMesh))
        {
            Debug.LogError("UIStack should have textMesh in its first child");
        }
    }

    private void Start()
    {
        UIQueriesContainer.QueryGetUpdater().AddPointerClickHandler(this);
    }

    public Vector2Int[] GetFillState()
    {
        Vector2Int[] fillState = new Vector2Int[_itemSO.Size.x * _itemSO.Size.y];
        int indexer = 0;
        for (int j = StackData.TilePos.y; j < _itemSO.Size.y; j++)
        {
            for (int i = StackData.TilePos.x; i < _itemSO.Size.x; i++)
            {
                fillState[indexer++] = new Vector2Int(i, j);
            }
        }

        return fillState;
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
        PoolingDelegatesContainer.EventDespawnUIStack(this);
    }

    public void OnPointerClick(MouseButtonType pressedButton)
    {
        if (CraftingDelegatesContainer.QueryIsStackSelected())
        {
            return;
        }

        CraftingDelegatesContainer.EventStackWasSelected(this);

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

    public Transform GetTransform()
    {
        return transform;
    }
}


// rect.anchorMin = new Vector2(0, 1);
// rect.anchorMax = new Vector2(0, 1);
// rect.pivot = new Vector2(0.5f, 0.5f);