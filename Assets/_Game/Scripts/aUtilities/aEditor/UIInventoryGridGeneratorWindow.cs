using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryGridGeneratorWindow : EditorWindow
{
    private Vector2Int _referenceScreenRes = new Vector2Int(1920, 1080);

    private int _deltaPos = 0;
    private int _tileSize = 100;
    private int _rowCount = 5;
    private int _colCount = 8;

    private GameObject _tilePrefab;
    private Transform _canvasParent;
    private Vector2Int _inventoryWindowBorderWidth = new Vector2Int(5, 10);

    [MenuItem("Window/Custom/GenerateInventoryWindow")]
    private static void Init()
    {
        UIInventoryGridGeneratorWindow window = 
            (UIInventoryGridGeneratorWindow)EditorWindow.GetWindow(
                typeof(UIInventoryGridGeneratorWindow));

        window.Show();
    }

    private void OnGUI()
    { 
        EditorGUILayout.LabelField("Generate inventory window gb and child it to UI Transform parent");
        _referenceScreenRes = EditorGUILayout.Vector2IntField("ScreenResolution", _referenceScreenRes);
        EditorGUILayout.Separator();

        _tileSize = EditorGUILayout.IntField("SquareSize", _tileSize);
        _deltaPos = EditorGUILayout.IntField("GapSize", _deltaPos);
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
        float scalarDeltaX = _tileSize + _deltaPos;
        float scalarDeltaZ = _tileSize + _deltaPos;
        Vector2 horizDisplacement = scalarDeltaX * Vector2.right;
        Vector2 verticalDisplacement = scalarDeltaZ * Vector2.down;

        Vector2 initTilePos = Vector2.zero;

        Vector2 rowStartTilePos = initTilePos;
        Vector2 tilePos = initTilePos;

        Vector2Int gridSize = new Vector2Int(_colCount * _tileSize, _rowCount *_tileSize);

        GameObject canvasGb = new GameObject("InventoryCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(UIInventoryCanvas));
        canvasGb.transform.SetParent(_canvasParent, false);

        canvasGb.TryGetComponent(out Canvas canvas);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGb.TryGetComponent(out CanvasScaler scaler);
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = _referenceScreenRes;

        GameObject inventoryWindow = new GameObject("InventoryWindow", typeof(RectTransform));
        inventoryWindow.transform.SetParent(canvasGb.transform, false);
        inventoryWindow.TryGetComponent(out RectTransform inventoryWindowRect);
        inventoryWindowRect.anchorMin = Vector2.one * 0.5f;
        inventoryWindowRect.anchorMax = Vector2.one * 0.5f;
        inventoryWindowRect.sizeDelta = gridSize + _inventoryWindowBorderWidth;
        
        RectTransform inventoryContainer = CreateInventoryContainer("Inventory", inventoryWindow.transform);
        RectTransform itemsParent = CreateItemsContainer("Items", inventoryWindow.transform);

        UITile[,] generatedTiles = new UITile[_colCount, _rowCount];
        for (int row = 0; row < _rowCount; row++)
        {
            for (int column = 0; column < _colCount; column++)
            {
                GameObject tileGb =
                    Instantiate(_tilePrefab, inventoryContainer);
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

        if (inventoryContainer.TryGetComponent(out UIInventory inventory))
        {
            inventory.AssignTiles(generatedTiles, itemsParent, _tileSize, gridSize, _inventoryWindowBorderWidth);
        }
    }

    private RectTransform CreateInventoryContainer(string name, Transform inventoryWindowTransform)
    { 
        GameObject gb = new GameObject(name, typeof(RectTransform), typeof(UIInventory));
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