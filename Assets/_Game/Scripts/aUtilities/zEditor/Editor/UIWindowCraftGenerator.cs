using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowCraftGenerator : EditorWindow
{
    private Vector2Int _referenceScreenRes = new Vector2Int(1080, 1920);

    private int _gapSize = 0;
    private int _tileSize = 100;
    private int _rowCount = 5;
    private int _colCount = 3;

    private GameObject _tilePrefab;
    private Transform _canvasParent;
    private Vector2Int _inventoryWindowBorderWidth = new Vector2Int(5, 10);

    [MenuItem("Window/Custom/GenerateCraftWindow")]
    private static void Init()
    {
        UIWindowCraftGenerator window = 
            (UIWindowCraftGenerator)EditorWindow.GetWindow(
                typeof(UIWindowCraftGenerator));

        window.Show();
    }

    private void OnGUI()
    { 
        EditorGUILayout.LabelField("Generate craft window gb and child it to UI Transform parent");
        _referenceScreenRes = EditorGUILayout.Vector2IntField("ScreenResolution", _referenceScreenRes);
        EditorGUILayout.Separator();

        _tileSize = EditorGUILayout.IntField("SquareSize", _tileSize);
        _gapSize = EditorGUILayout.IntField("GapSize", _gapSize);
        _rowCount = EditorGUILayout.IntField("RowCount", _rowCount);
        _colCount = EditorGUILayout.IntField("ColumnCount", _colCount);
        EditorGUILayout.Separator();
        
        _tilePrefab = (GameObject)EditorGUILayout.ObjectField("Tile", _tilePrefab, typeof(GameObject), true);
        _canvasParent = (Transform)EditorGUILayout.ObjectField("CanvasParent", _canvasParent, typeof(Transform), true);
        _inventoryWindowBorderWidth = EditorGUILayout.Vector2IntField("BorderWidth" ,_inventoryWindowBorderWidth);

        if (GUILayout.Button("Generate"))
        {
            GenerateTiles();
        }
    }

    private void GenerateTiles()
    {
        float scalarDeltaX = _tileSize + _gapSize;
        float scalarDeltaZ = _tileSize + _gapSize;
        Vector2 horizDisplacement = scalarDeltaX * Vector2.right;
        Vector2 verticalDisplacement = scalarDeltaZ * Vector2.down;

        Vector2 initTilePos = Vector2.zero;

        Vector2 rowStartTilePos = initTilePos;
        Vector2 tilePos = initTilePos;

        Vector2Int gridSize = new Vector2Int(_colCount * _tileSize, _rowCount *_tileSize);

        GameObject canvasGb = new GameObject("InventoryCanvas", typeof(Canvas), typeof(CanvasScaler));
        canvasGb.transform.SetParent(_canvasParent, false);

        canvasGb.TryGetComponent(out Canvas canvas);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGb.TryGetComponent(out CanvasScaler scaler);
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = _referenceScreenRes;

        GameObject itemsTotalWindowGB = new GameObject("itemsWindow", typeof(RectTransform));
        itemsTotalWindowGB.transform.SetParent(canvasGb.transform, false);
        itemsTotalWindowGB.TryGetComponent(out RectTransform itemsTotalWindowRect);
        itemsTotalWindowRect.anchorMin = Vector2.one * 0.5f;
        itemsTotalWindowRect.anchorMax = Vector2.one * 0.5f;
        itemsTotalWindowRect.sizeDelta = gridSize + _inventoryWindowBorderWidth;
        
        RectTransform tilesContainer = CreateTilesContainer("Tiles", itemsTotalWindowGB.transform);
        RectTransform itemsParent = CreateItemsContainer("Items", itemsTotalWindowGB.transform);

        UITile[,] generatedTiles = new UITile[_colCount, _rowCount];
        for (int row = 0; row < _rowCount; row++)
        {
            for (int column = 0; column < _colCount; column++)
            {
                GameObject tileGb =
                    Instantiate(_tilePrefab, tilesContainer);
                tileGb.TryGetComponent(out RectTransform rect);
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                rect.anchoredPosition = tilePos;

                if (!tileGb.TryGetComponent(out UITile tile))
                {
                    Debug.LogError("Tile prefab does not contain UITile");
                    return;
                }

                // tile.Rect = rect;
                generatedTiles[column, row] = tile;
                tile.GenerationInitialize(new Vector2Int(column, row));

                tilePos += horizDisplacement;
            }
            tilePos = rowStartTilePos;
            tilePos += verticalDisplacement;
            rowStartTilePos = tilePos;
        }
    }

    private RectTransform CreateTilesContainer(string name, Transform totalWindowTransform)
    { 
        GameObject gb = new GameObject(name, typeof(RectTransform), typeof(UIWindowCraft));
        gb.transform.SetParent(totalWindowTransform, false);
        gb.TryGetComponent(out RectTransform tilesParentRect);
        tilesParentRect.anchorMin = Vector2.zero;
        tilesParentRect.anchorMax = Vector2.one;
        tilesParentRect.sizeDelta = -_inventoryWindowBorderWidth;

        return gb.GetComponent<RectTransform>();
    }

    private RectTransform CreateItemsContainer(string name, Transform totalWindowTransform)
    { 
        GameObject gb = new GameObject(name, typeof(RectTransform));
        gb.transform.SetParent(totalWindowTransform, false);
        gb.TryGetComponent(out RectTransform tilesParentRect);
        tilesParentRect.anchorMin = Vector2.zero;
        tilesParentRect.anchorMax = Vector2.one;
        tilesParentRect.sizeDelta = -_inventoryWindowBorderWidth;

        return gb.GetComponent<RectTransform>();
    }
}