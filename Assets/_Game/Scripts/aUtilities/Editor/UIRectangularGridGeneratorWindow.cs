using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

public class UIRectangularGridGeneratorWindow : EditorWindow
{
    private int _screenWidth;
    private int _screenHeight;

    private float _deltaPos;
    private float _squareSize;
    private int _rowCount;
    private int _colCount;

    private GameObject _tilePrefab;
    private Transform _parent;

    private List<List<GameObject>> _generatedTiles;

    [MenuItem("Window/Custom/GenerateGBsInRectangularGrid")]
    private static void Init()
    {
        UIRectangularGridGeneratorWindow window = 
            (UIRectangularGridGeneratorWindow)EditorWindow.GetWindow(
                typeof(UIRectangularGridGeneratorWindow));

        window.Show();
    }

    private void OnGUI()
    { 
        EditorGUILayout.LabelField("Generate inventory tiles using supplied delta");
        _screenWidth = EditorGUILayout.IntField("ScreenWidth", _screenWidth);
        _screenHeight = EditorGUILayout.IntField("ScreenHeight", _screenHeight);
        EditorGUILayout.Separator();

        _squareSize = EditorGUILayout.FloatField("SquareSize", _squareSize);
        _deltaPos = EditorGUILayout.FloatField("GapSize", _deltaPos);
        _rowCount = EditorGUILayout.IntField("RowCount", _rowCount);
        _colCount = EditorGUILayout.IntField("ColumnCount", _colCount);
        EditorGUILayout.Separator();
        
        _tilePrefab = (GameObject)EditorGUILayout.ObjectField("Tile", _tilePrefab, typeof(GameObject), true);
        _parent = (Transform)EditorGUILayout.ObjectField("Parent", _parent, typeof(Transform), true);

        if (GUILayout.Button("Generate"))
        {
            GenerateTiles();
        }

        if (_generatedTiles != null)
        { 
            if (GUILayout.Button("Destroy generated"))
            {
                DeleteTiles();
            }
        }
    }

    private void GenerateTiles()
    {
        float scalarDeltaX = _squareSize + _deltaPos;
        float scalarDeltaZ = _squareSize + _deltaPos;
        Vector3 horizDisplacement = scalarDeltaX * Vector3.right;
        Vector3 verticalDisplacement = scalarDeltaZ * Vector3.up;

        Vector3 initialHorizDisp = scalarDeltaX / 2 * -Vector3.right;
        Vector3 initialVertDisp = scalarDeltaZ / 2 * -Vector3.up;
        if (_rowCount % 2 == 1)
        {
            initialHorizDisp = -horizDisplacement;
            initialVertDisp = -verticalDisplacement;
        }

        Vector3 centerAdjust = new Vector3(_screenWidth / 2, _screenHeight / 2, 0);
        Vector3 initTilePos =
            -(_rowCount / 2 - 1) * horizDisplacement + initialHorizDisp +
            -(_colCount / 2 - 1) * verticalDisplacement + initialVertDisp  + centerAdjust;

        Vector3 rowStartTilePos = initTilePos;
        Vector3 tilePos = initTilePos;


        Quaternion tileRot = Quaternion.identity;

        _generatedTiles = new List<List<GameObject>>();
        for (int row = 0; row < _rowCount; row++)
        {
            _generatedTiles.Add(new List<GameObject>());
            for (int column = 0; column < _colCount; column++)
            {
                GameObject tileGb =
                    Instantiate(_tilePrefab, _parent);
                tileGb.TryGetComponent(out RectTransform rect);
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.zero;
                rect.anchoredPosition = tilePos;

                _generatedTiles[row].Add(tileGb);

                tilePos += horizDisplacement;
            }
            tilePos = rowStartTilePos;
            tilePos += verticalDisplacement;
            rowStartTilePos = tilePos;
        }
    }

    private void DeleteTiles()
    {
        for (int i = 0; i < _generatedTiles.Count; i++)
        {
            for (int j = 0; j < _generatedTiles[i].Count; j++)
            {
                DestroyImmediate(_generatedTiles[i][j]);
            }
        }
        _generatedTiles.Clear();
        _generatedTiles = null;
    }
}