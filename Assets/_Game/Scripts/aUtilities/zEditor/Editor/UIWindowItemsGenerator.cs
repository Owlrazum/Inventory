using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//LastPoint: complete.

public class UIWindowItemsGenerator : EditorWindow
{
    private Transform _windowParent;
    private Vector2Int _windowPos = new Vector2Int(0, 200);
    private Vector2Int _windowBorderWidth = new Vector2Int(5, 10);
    private int _gapSize = 20;
    private int _tileSize = 250;
    private int _rowCount = 5;
    private int _colCount = 3;

    private GameObject _tilePrefab;

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
        _windowPos = EditorGUILayout.Vector2IntField("WindowPosition", _windowPos);
        _windowBorderWidth = EditorGUILayout.Vector2IntField("BorderWidth" ,_windowBorderWidth);
        EditorGUILayout.Separator();

        _tileSize = EditorGUILayout.IntField("SquareSize", _tileSize);
        _gapSize = EditorGUILayout.IntField("WindowPosition", _gapSize);;
        _rowCount = EditorGUILayout.IntField("RowCount", _rowCount);
        _colCount = EditorGUILayout.IntField("ColumnCount", _colCount);
        EditorGUILayout.Separator();
        
        _tilePrefab = (GameObject)EditorGUILayout.ObjectField("Tile", _tilePrefab, typeof(GameObject), true);
        _windowParent = (Transform)EditorGUILayout.ObjectField("CanvasParent", _windowParent, typeof(Transform), true);

        if (GUILayout.Button("Generate"))
        {
            GenerateTiles();
        }
    }

    private void GenerateTiles()
    {
        Vector2Int gapSizeDelta = new Vector2Int(_rowCount - 1, _colCount - 1) * _gapSize;
        Vector2Int windowSize = new Vector2Int(_colCount * _tileSize, _rowCount *_tileSize) + gapSizeDelta;

        GameObject windowGb = new GameObject("ItemsWindow", typeof(RectTransform));
        windowGb.transform.SetParent(_windowParent.transform, false);
        windowGb.TryGetComponent(out RectTransform windowRect);
        windowRect.anchorMin = new Vector2(0.5f, 0);
        windowRect.anchorMax = new Vector2(0.5f, 0);
        windowRect.sizeDelta = windowSize + _windowBorderWidth;

        RectTransform tilesContainer = CreateTilesContainer("Tiles", windowGb.transform);

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
            uiWindowItems.AssignTiles(generatedTiles, _tileSize, _gapSize, windowSize, _windowBorderWidth);
        }
    }

    private RectTransform CreateTilesContainer(string name, Transform inventoryWindowTransform)
    { 
        GameObject gb = new GameObject(name, typeof(RectTransform), typeof(UIWindowItems));
        gb.transform.SetParent(inventoryWindowTransform, false);
        gb.TryGetComponent(out RectTransform tilesParentRect);
        tilesParentRect.anchorMin = Vector2.zero;
        tilesParentRect.anchorMax = Vector2.one;
        tilesParentRect.sizeDelta = -_windowBorderWidth;

        return gb.GetComponent<RectTransform>();
    }

    private RectTransform CreateItemsContainer(string name, Transform inventoryWindowTransform)
    { 
        GameObject gb = new GameObject(name, typeof(RectTransform));
        gb.transform.SetParent(inventoryWindowTransform, false);
        gb.TryGetComponent(out RectTransform tilesParentRect);
        tilesParentRect.anchorMin = Vector2.zero;
        tilesParentRect.anchorMax = Vector2.one;
        tilesParentRect.sizeDelta = -_windowBorderWidth;

        return gb.GetComponent<RectTransform>();
    }
}