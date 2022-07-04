using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

[System.Serializable]
public class UIStackData
{ 
    public int ItemAmount;
    public int ItemTypeID;
    
    public Vector2Int TilePos;
}

public class CanvasDataType
{
    public GraphicRaycaster GraphicRaycaster;
    public RectTransform BoundingRect;
}

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UIStack : MonoBehaviour, IPointerClickHandler, IPoolable
{
    public UIStackData StackData { get; private set; }
    public CanvasDataType CanvasData { get; private set; }

    private ItemSO _itemSO;

    public Vector2Int Size
    {
        get
        {
            return _itemSO.Size;
        }
    }

    public void InitializeWithData(UIStackData stackData, CanvasDataType canvasData)
    {
        StackData = stackData;
        CanvasData = canvasData;

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


        _rect.SetParent(CanvasData.BoundingRect, false);

        gameObject.SetActive(true);
    }

    public void UpdateRect(Vector2 anchoredPos, Vector2 sizeDelta)
    {
        _rect.anchoredPosition = anchoredPos;
        _rect.sizeDelta = sizeDelta;
    }

    private RectTransform _rect;
    private Image _image;
    private TextMeshProUGUI _textMesh;
    private IEnumerator _followUpdateLoop;

    private enum State
    { 
        Idle,
        Following
    }
    private State _state;

    private const float DEPTH_POS = -1; // TODO: Load from SO

    private void Awake()
    {
        TryGetComponent(out _rect);
        TryGetComponent(out _image);
        if (!transform.GetChild(0).TryGetComponent(out _textMesh))
        {
            Debug.LogError("UIStack should have textMesh in its first child");
        }
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        { 
            return;
        }

        OnLeftClick();
    }

    private void OnLeftClick()
    {
        if (_state == State.Idle)
        {
            _state = State.Following;
            _followUpdateLoop = FollowCursorUpdateLoop();
            StartCoroutine(_followUpdateLoop);
        }
        else if (_state == State.Following)
        {
            if (CheckIfPlaceable())
            { 

            }
        }
    }

    private IEnumerator FollowCursorUpdateLoop()
    {
        while (true)
        { 
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                CanvasData.BoundingRect, Input.mousePosition, null, out Vector2 localPoint
            );

            _rect.anchoredPosition3D = new Vector3(localPoint.x, localPoint.y, DEPTH_POS);
            yield return null;
        }
    }

    private bool CheckIfPlaceable()
    {
        PointerEventData eventData = new PointerEventData(null);
        List<RaycastResult> results = new List<RaycastResult>();
        CanvasData.GraphicRaycaster.Raycast(eventData, results);
        foreach (RaycastResult res in results)
        {
            Debug.Log(res.gameObject + " " + res.sortingLayer);
        }

        return true;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}


// rect.anchorMin = new Vector2(0, 1);
// rect.anchorMax = new Vector2(0, 1);
// rect.pivot = new Vector2(0.5f, 0.5f);