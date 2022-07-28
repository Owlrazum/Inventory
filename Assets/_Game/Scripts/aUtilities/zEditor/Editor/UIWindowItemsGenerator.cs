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
    private int _rowCount = 1;
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
        _gapSize  = EditorGUILayout.IntField("WindowPosition", _gapSize);;
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

        GameObject windowBordersGb = new GameObject("ItemsWindowWithBorders", typeof(RectTransform));
        RectTransform windowBordersRect = windowBordersGb.GetComponent<RectTransform>();
        windowBordersRect.SetParent(_windowParent.transform, false);
        
        windowBordersRect.anchorMin = new Vector2(0.5f, 0.5f);
        windowBordersRect.anchorMax = new Vector2(0.5f, 0.5f);
        windowBordersRect.sizeDelta = windowSize + _windowBorderWidth;
        windowBordersRect.anchoredPosition = _windowPos;

        GameObject windowGb = new GameObject(name, typeof(RectTransform), typeof(UIWindowItems));
        RectTransform windowRect = windowGb.GetComponent<RectTransform>();
        windowRect.SetParent(windowBordersRect, false);

        windowRect.anchorMin = Vector2.zero;
        windowRect.anchorMax = Vector2.one;
        windowRect.sizeDelta = -_windowBorderWidth;

        float scalarDeltaX = _tileSize + _gapSize;
        float scalarDeltaY = _tileSize + _gapSize;
        Vector2 initTilePos = Vector2.zero;
        Vector2 rowStartTilePos = initTilePos;
        Vector2 horizDisplacement = scalarDeltaX * Vector2.right;
        Vector2 verticalDisplacement = scalarDeltaY * Vector2.down;

        Vector2 tilePos = initTilePos;

        UITile[,] generatedTiles = new UITile[_colCount, _rowCount];
        for (int row = 0; row < _rowCount; row++)
        {
            for (int column = 0; column < _colCount; column++)
            {
                GameObject tileGb =
                    Instantiate(_tilePrefab);
                RectTransform tileRect = tileGb.GetComponent<RectTransform>();
                tileRect.anchorMin = new Vector2(0, 1);
                tileRect.anchorMax = new Vector2(0, 1);
                tileRect.pivot = new Vector2(0, 1);
                tileRect.anchoredPosition = tilePos;

                if (!tileGb.TryGetComponent(out UITile tile))
                {
                    Debug.LogError("Tile prefab does not contain UITile");
                    return;
                }

                tile.AssignWindowTypeOnGeneration(WindowType.ItemsWindow);
                generatedTiles[column, row] = tile;
                tile.GenerationInitialize(new Vector2Int(column, row));

                tilePos += horizDisplacement;
            }
            tilePos = rowStartTilePos;
            tilePos += verticalDisplacement;
            rowStartTilePos = tilePos;
        }

        UIWindowItems uiWindowItems = windowGb.GetComponent<UIWindowItems>();
    }
}