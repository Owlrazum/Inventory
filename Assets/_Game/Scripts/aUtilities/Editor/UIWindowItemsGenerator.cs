using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowItemsGenerator : EditorWindow
{
    private Vector2Int _referenceScreenRes = new Vector2Int(1080, 1920);

    private int _gapSize = 0;
    private int _tileSize = 100;
    private int _rowCount = 5;
    private int _colCount = 3;

    private GameObject _tilePrefab;
    private Transform _canvasParent;
    private Vector2Int _inventoryWindowBorderWidth = new Vector2Int(5, 10);

    [MenuItem("Window/Custom/GenerateItemsWindow")]
    private static void Init()
    {
        UIWindowItemsGenerator window = 
            (UIWindowItemsGenerator)EditorWindow.GetWindow(
                typeof(UIWindowItemsGenerator));

        window.Show();
    }

    private void OnGUI()
    { 
        EditorGUILayout.LabelField("Generate items window gb and child it to UI Transform parent");
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
        Vector2Int gapSizeDelta = new Vector2Int(_rowCount - 1, _colCount - 1) * _gapSize;
        Vector2Int gridSize = new Vector2Int(_colCount * _tileSize, _rowCount *_tileSize) + gapSizeDelta;

        GameObject canvasGb = new GameObject("ItemsWindowCanvas", typeof(Canvas), typeof(CanvasScaler));
        canvasGb.transform.SetParent(_canvasParent, false);

        canvasGb.TryGetComponent(out Canvas canvas);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGb.TryGetComponent(out CanvasScaler scaler);
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = _referenceScreenRes;

        GameObject inventoryWindow = new GameObject("ItemsTotalWindow", typeof(RectTransform));
        inventoryWindow.transform.SetParent(canvasGb.transform, false);
        inventoryWindow.TryGetComponent(out RectTransform inventoryWindowRect);
        inventoryWindowRect.anchorMin = Vector2.one * 0.5f;
        inventoryWindowRect.anchorMax = Vector2.one * 0.5f;
        inventoryWindowRect.sizeDelta = gridSize + _inventoryWindowBorderWidth;

        RectTransform tilesContainer = CreateInventoryContainer("Tiles", inventoryWindow.transform);
        RectTransform itemsParent = CreateItemsContainer("Items", inventoryWindow.transform);

        float scalarDeltaX = _tileSize + _gapSize;
        float scalarDeltaZ = _tileSize + _gapSize;
        Vector2 initTilePos = Vector2.zero;
        Vector2 rowStartTilePos = initTilePos;
        Vector2 horizDisplacement = scalarDeltaX * Vector2.right;
        Vector2 verticalDisplacement = scalarDeltaZ * Vector2.down;

        Vector2 tilePos = initTilePos;

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
                tile.AssignWindowTypeOnGeneration(WindowType.ItemsWindow);
                generatedTiles[column, row] = tile;
                tile.GenerationInitialize(new Vector2Int(column, row));

                tilePos += horizDisplacement;
            }
            tilePos = rowStartTilePos;
            tilePos += verticalDisplacement;
            rowStartTilePos = tilePos;
        }

        if (tilesContainer.TryGetComponent(out UIWindowItems uiWindowItems))
        {
            uiWindowItems.AssignTiles(generatedTiles, itemsParent, _tileSize, _gapSize, gridSize, _inventoryWindowBorderWidth);
        }
    }

    private RectTransform CreateInventoryContainer(string name, Transform inventoryWindowTransform)
    { 
        GameObject gb = new GameObject(name, typeof(RectTransform), typeof(UIWindowItems));
        gb.transform.SetParent(inventoryWindowTransform, false);
        gb.TryGetComponent(out RectTransform tilesParentRect);
        tilesParentRect.anchorMin = Vector2.zero;
        tilesParentRect.anchorMax = Vector2.one;
        tilesParentRect.sizeDelta = -_inventoryWindowBorderWidth;

        return gb.GetComponent<RectTransform>();
    }

    private RectTransform CreateItemsContainer(string name, Transform inventoryWindowTransform)
    { 
        GameObject gb = new GameObject(name, typeof(RectTransform));
        gb.transform.SetParent(inventoryWindowTransform, false);
        gb.TryGetComponent(out RectTransform tilesParentRect);
        tilesParentRect.anchorMin = Vector2.zero;
        tilesParentRect.anchorMax = Vector2.one;
        tilesParentRect.sizeDelta = -_inventoryWindowBorderWidth;

        return gb.GetComponent<RectTransform>();
    }
}