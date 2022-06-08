using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryGridGeneratorWindow : EditorWindow
{
    private Vector2Int _referenceScreenRes = new Vector2Int(1920, 1080);

    private float _deltaPos = 0;
    private float _squareSize = 100;
    private int _rowCount = 5;
    private int _colCount = 8;

    private GameObject _tilePrefab;
    private Transform _canvasParent;
    private Vector2 _inventoryWindowBorderWidth = new Vector2(5, 10);

    private RectTransform[,] _generatedTiles;

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

        _squareSize = EditorGUILayout.FloatField("SquareSize", _squareSize);
        _deltaPos = EditorGUILayout.FloatField("GapSize", _deltaPos);
        _rowCount = EditorGUILayout.IntField("RowCount", _rowCount);
        _colCount = EditorGUILayout.IntField("ColumnCount", _colCount);
        EditorGUILayout.Separator();
        
        _tilePrefab = (GameObject)EditorGUILayout.ObjectField("Tile", _tilePrefab, typeof(GameObject), true);
        _canvasParent = (Transform)EditorGUILayout.ObjectField("CanvasParent", _canvasParent, typeof(Transform), true);
        _inventoryWindowBorderWidth = EditorGUILayout.Vector2Field("BorderWidth" ,_inventoryWindowBorderWidth);

        if (GUILayout.Button("Generate"))
        {
            GenerateTiles();
        }
    }

    private void GenerateTiles()
    {
        float scalarDeltaX = _squareSize + _deltaPos;
        float scalarDeltaZ = _squareSize + _deltaPos;
        Vector2 horizDisplacement = scalarDeltaX * Vector2.right;
        Vector2 verticalDisplacement = scalarDeltaZ * Vector2.down;

        Vector2 initTilePos = Vector2.zero;

        Vector2 rowStartTilePos = initTilePos;
        Vector2 tilePos = initTilePos;

        Vector2 gridSizeDelta = new Vector2(_colCount * _squareSize, _rowCount *_squareSize);

        GameObject canvasGb = new GameObject("InventoryCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(UIInventory));
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
        inventoryWindowRect.sizeDelta = gridSizeDelta + _inventoryWindowBorderWidth;
        
        Transform tilesParent = CreateChildOfInventoryWindow("Tiles", inventoryWindow.transform);
        CreateChildOfInventoryWindow("Items", inventoryWindow.transform);

        _generatedTiles = new RectTransform[_colCount, _rowCount];
        for (int row = 0; row < _rowCount; row++)
        {
            for (int column = 0; column < _colCount; column++)
            {
                GameObject tileGb =
                    Instantiate(_tilePrefab, tilesParent);
                tileGb.TryGetComponent(out RectTransform rect);
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                rect.anchoredPosition = tilePos;

                _generatedTiles[column, row] = rect;

                tilePos += horizDisplacement;
            }
            tilePos = rowStartTilePos;
            tilePos += verticalDisplacement;
            rowStartTilePos = tilePos;
        }

        if (canvasGb.TryGetComponent(out UIInventory uiInventory))
        {
            uiInventory.AssignTiles(_generatedTiles);
        }
    }

    private Transform CreateChildOfInventoryWindow(string name, Transform inventoryWindowTransform)
    { 
        GameObject gb = new GameObject(name, typeof(RectTransform));
        gb.transform.SetParent(inventoryWindowTransform, false);
        gb.TryGetComponent(out RectTransform tilesParentRect);
        tilesParentRect.anchorMin = Vector2.zero;
        tilesParentRect.anchorMax = Vector2.one;
        tilesParentRect.sizeDelta = -_inventoryWindowBorderWidth;

        return gb.transform;
    }
}