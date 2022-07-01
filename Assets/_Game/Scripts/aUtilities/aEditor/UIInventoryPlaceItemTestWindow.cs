using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryPlaceItemTestWindow : EditorWindow
{
    private Vector2Int _pos;
    private Vector2Int _size;
    private GameObject _itemSpritePrefab;
    private UIInventory _uiInventory;
    private Transform _itemsParent;

    [MenuItem("Window/Custom/TestItemPlacement")]
    private static void Init()
    {
        UIInventoryPlaceItemTestWindow window = 
            (UIInventoryPlaceItemTestWindow)EditorWindow.GetWindow(
                typeof(UIInventoryPlaceItemTestWindow));

        window.Show();
    }

    private void OnGUI()
    { 
        EditorGUILayout.LabelField("Place item with defined size and position to the inventory");
        _itemSpritePrefab = (GameObject)EditorGUILayout.ObjectField("Item", _itemSpritePrefab, typeof(GameObject), true);
        _itemsParent = (Transform)EditorGUILayout.ObjectField("ItemsParent", _itemsParent, typeof(Transform), true);
        _uiInventory = (UIInventory)EditorGUILayout.ObjectField("UIInventory", _uiInventory, typeof(UIInventory), true);
        _pos  = EditorGUILayout.Vector2IntField("Pos", _pos);
        _size = EditorGUILayout.Vector2IntField("Size", _size);

        if (GUILayout.Button("Place item"))
        {
            PlaceItem();
        }
    }

    private void PlaceItem()
    {
        var tiles = _uiInventory.GetTiles();
        if (_pos.x >= tiles.GetLength(0) || _pos.y >= tiles.GetLength(1))
        {
            Debug.LogError("Invalid pos");
            return;
        }

        if (_size.x >= tiles.GetLength(0) || _size.y >= tiles.GetLength(1))
        { 
            Debug.LogError("Invalid size");
            return;
        }

        if (_pos.x + _size.x > tiles.GetLength(0) || _pos.y + _size.y > tiles.GetLength(1))
        {
            Debug.LogError("Invalid pos with size");
            return;
        }

        GameObject tileGb =
            Instantiate(_itemSpritePrefab, _itemsParent);

        tileGb.TryGetComponent(out RectTransform rect);
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0.5f, 0.5f);

        RectTransform endTile = tiles[_pos.x + _size.x - 1, _pos.y + _size.y - 1].Rect;
        Vector2 adjust = new Vector2(endTile.rect.width, -endTile.rect.height);

        Vector2 anchPos = (tiles[_pos.x, _pos.y].Rect.anchoredPosition + 
            endTile.anchoredPosition + adjust) / 2;
        rect.anchoredPosition = anchPos;

        //_uiInventory.AssignTiles(_generatedTiles);
    }
}