using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class UIInventory : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField]
    private int _rowCount = -1;

    [SerializeField]
    private int _columnCount = -1;

    [SerializeField]
    private MultiDimArrayPackage<RectTransform>[] _tilesSerialized;

    private RectTransform[,] _tiles = null;

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
        _tilesParent = transform.GetChild(0);
        _itemsParent = transform.GetChild(1);
        gameObject.SetActive(false);
        if (_rowCount < 0 || _columnCount < 0)
        {
            Debug.LogError(name + ": Assign valid values for dimensions");
            return;
        }

        _fillState = new bool[_rowCount, _columnCount];

        InputDelegatesContainer.EventInventoryCommandTriggered += OnInventoryCommandTriggered;
    }

    private void Start()
    {
        _currentInventory = CraftingDelegatesContainer.QueryInventoryInstance();
    }

    private void OnDestroy()
    { 
        InputDelegatesContainer.EventInventoryCommandTriggered -= OnInventoryCommandTriggered;
    }

    // Places items based on topLeftCorner
    private void OnInventoryCommandTriggered()
    {
        gameObject.SetActive(true);
        var items = _currentInventory.GetItems();
        foreach (KeyValuePair<ItemSO, int> pair in items)
        {
            for (int i = 0; i < _rowCount; i++)
            {
                for (int j = 0; j < _columnCount; j++)
                {
                    if (!_fillState[i, j])
                    {
                        if (CheckAndFillIfSizeFits(new Vector2Int(pair.Key.SizeX, pair.Key.SizeY), new Vector2Int(i, j)))
                        {
                            GameObject sprite = Instantiate(pair.Key.Sprite, _itemsParent);
                            sprite.TryGetComponent(out RectTransform rect);
                            rect.anchorMin = new Vector2(0, 1);
                            rect.anchorMax = new Vector2(0, 1);
                            rect.pivot = new Vector2(0, 1);
                            rect.anchoredPosition = _tiles[i, j].anchoredPosition;
                            // rect.sizeDelta = 
                        }
                    }
                }
            }
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