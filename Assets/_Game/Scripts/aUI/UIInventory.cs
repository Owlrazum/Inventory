using System.Collections.Generic;
using UnityEngine;

using TMPro;

[RequireComponent(typeof(Canvas))]
public class UIInventory : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField]
    private int _rowCount = -1;

    [SerializeField]
    private int _columnCount = -1;

    [SerializeField]
    private MultiDimArrayPackage<RectTransform>[] _tilesSerialized;

    [SerializeField]
    private Dictionary<ItemSO, TextMeshProUGUI> _placedInCanvasItems;

    private RectTransform[,] _tiles = null;

    [SerializeField]
    private Inventory _currentInventory;

    private Transform _tilesParent;
    private Transform _itemsParent;

    private bool[,] _fillState;

#if UNITY_EDITOR
    public void AssignTiles(RectTransform[,] tilesArg)
    {
        _tiles = tilesArg;
        _columnCount = _tiles.GetLength(0);
        _rowCount = _tiles.GetLength(1);
    }

    public RectTransform[,] GetTiles()
    {
        OnAfterDeserialize();
        return _tiles;
    }
#endif

    public void OnBeforeSerialize()
    {
        if (_tiles == null)
        {
            return;
        }

        // Convert our unserializable array into a serializable list
        _tilesSerialized = new MultiDimArrayPackage<RectTransform>[_tiles.GetLength(0) * _tiles.GetLength(1)];
        for (int j = 0; j < _tiles.GetLength(1); j++)
        {
            for (int i = 0; i < _tiles.GetLength(0); i++)
            {
                _tilesSerialized[j * _tiles.GetLength(0) + i] = 
                    (new MultiDimArrayPackage<RectTransform>(i, j, _tiles[i, j]));
            }
        }
    }

    public void OnAfterDeserialize()
    {
        if (_tilesSerialized == null)
        {
            return;
        }
        _tiles = new RectTransform[_columnCount, _rowCount];
        foreach(var package in _tilesSerialized)
        {
            _tiles[package.ColumnIndex, package.RowIndex] = package.Element;
        }
    }


    private void Awake()
    {
        _tilesParent = transform.GetChild(0).GetChild(0);
        _itemsParent = transform.GetChild(0).GetChild(1);

        gameObject.SetActive(false);
        if (_rowCount < 0 || _columnCount < 0)
        {
            Debug.LogError(name + ": Assign valid values for dimensions");
            return;
        }

        _placedInCanvasItems = new Dictionary<ItemSO, TextMeshProUGUI>();
        _fillState = new bool[_columnCount, _rowCount];

        InputDelegatesContainer.EventInventoryCommandTriggered += OnInventoryCommandTriggered;
    }

    private void OnDestroy()
    { 
        InputDelegatesContainer.EventInventoryCommandTriggered -= OnInventoryCommandTriggered;
    }

    // Places items based on topLeftCorner
    private void OnInventoryCommandTriggered()
    {
        if (gameObject.activeSelf)
        {
            HideInventory();
        }
        else
        {
            ShowInventory();
        }
    }

    private void HideInventory()
    {
        gameObject.SetActive(false);
    }

    private void ShowInventory()
    { 
        gameObject.SetActive(true);
        _currentInventory = CraftingDelegatesContainer.QueryInventoryInstance();
        var items = _currentInventory.GetItems();
        print(items.Count);
        foreach (KeyValuePair<ItemSO, int> pair in items)
        {
            if (_placedInCanvasItems.ContainsKey(pair.Key))
            {
                _placedInCanvasItems[pair.Key].text = pair.Value.ToString();
                goto outerLoop;
            }
            for (int j = 0; j < _rowCount; j++)
            {
                for (int i = 0; i < _columnCount; i++)
                {
                    print(i + " " + j + " " + _fillState[i, j]);
                    if (!_fillState[i, j])
                    {
                        ItemSO item = pair.Key;
                        if (CheckAndFillIfSizeFits(new Vector2Int(item.SizeX, item.SizeY), new Vector2Int(i, j)))
                        {
                            GameObject sprite = Instantiate(item.Sprite, _itemsParent);
                            sprite.TryGetComponent(out RectTransform rect);
                            rect.anchorMin = new Vector2(0, 1);
                            rect.anchorMax = new Vector2(0, 1);
                            rect.pivot = new Vector2(0.5f, 0.5f);
                            RectTransform endTile = _tiles[i + item.SizeX - 1, j + item.SizeY - 1];
                            print((i + item.SizeX - 1) + " " + (j + item.SizeY - 1));
                            Vector2 adjust = new Vector2(endTile.rect.width, -endTile.rect.height);

                            Vector2 anchPos = (_tiles[i, j].anchoredPosition +
                                endTile.anchoredPosition + adjust) / 2;
                            rect.anchoredPosition = anchPos;

                            bool wasTextFound = sprite.transform.GetChild(0)
                                .TryGetComponent(out TextMeshProUGUI textMeshProUGUI);

                            if (!wasTextFound)
                            {
                                Debug.LogError("The sprite for item " + pair.Key + " is missing TextMeshProUGUI in its first child");
                            }

                            textMeshProUGUI.text = pair.Value.ToString();

                            _placedInCanvasItems.Add(pair.Key, textMeshProUGUI);

                            goto outerLoop;
                        }
                    }
                }
            }
            outerLoop:;
        }
    }

    /// <returns>Whether placed</returns>
    private bool CheckAndFillIfSizeFits(Vector2Int size, Vector2Int pos)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Vector2Int tile = new Vector2Int(pos.x + i, pos.y + j);
                if (tile.x >= _fillState.GetLength(0) ||
                    tile.y >= _fillState.GetLength(1))
                {
                    return false;
                }

                if (_fillState[tile.x, tile.y])
                {
                    return false;
                }
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                _fillState[pos.x + i, pos.y + j] = true;
            }
        }

        return true;
    }
}