using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UIStack : MonoBehaviour, IPointerClickHandler, IPoolable
{
    public ItemSO ItemType { get; private set; }
    public int ItemAmount { get; set; }

    public void InitializeWithItemType(ItemSO itemTypeArg, int itemAmountArg)
    {
        ItemType = itemTypeArg;
        ItemAmount = itemAmountArg;

        _image.sprite = ItemType.Sprite;
        if (ItemAmount == 1)
        {
            _textMesh.text = "";
        }
        else
        {
            _textMesh.text = ItemAmount.ToString();
        }
    }

    public RectTransform BoundingRect { get; private set; }
    public GraphicRaycaster GraphicRaycaster { get; private set; }
    public void AssignReferences(
        RectTransform boundingRectArg,
        GraphicRaycaster graphicRaycasterArg
    )
    {
        BoundingRect = boundingRectArg;
        GraphicRaycaster = graphicRaycasterArg;

        _rect.SetParent(BoundingRect, false);
    }

    public Vector2Int TilePos { get; private set; }
    public void UpdatePos(Vector2Int tilePosArg, Vector2 anchoredPos)
    {
        TilePos = tilePosArg;
        _rect.anchoredPosition = anchoredPos;
    }

    public Action EventOnClick { get; set; }

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

// rect.anchorMin = new Vector2(0, 1);
// rect.anchorMax = new Vector2(0, 1);
// rect.pivot = new Vector2(0.5f, 0.5f);
    }

    /// <summary>
    /// Should be called after all public fields are set;
    /// </summary>
    public void OnSpawn(Vector2 anchoredPos, Vector2 sizeDeltaArg)
    {
        gameObject.SetActive(true);
        _image.sprite = ItemType.Sprite;
        _rect.anchoredPosition = anchoredPos;
        _rect.sizeDelta = sizeDeltaArg;
    }

    public Vector2Int[] GetFillState()
    {
        Vector2Int[] fillState = new Vector2Int[ItemType.Size.x * ItemType.Size.y];
        int indexer = 0;
        for (int j = TilePos.y; j < ItemType.Size.y; j++)
        {
            for (int i = TilePos.x; i < ItemType.Size.x; i++)
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
                BoundingRect, Input.mousePosition, null, out Vector2 localPoint
            );

            _rect.anchoredPosition3D = new Vector3(localPoint.x, localPoint.y, DEPTH_POS);
            yield return null;
        }
    }

    private bool CheckIfPlaceable()
    {
        PointerEventData eventData = new PointerEventData(null);
        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster.Raycast(eventData, results);
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