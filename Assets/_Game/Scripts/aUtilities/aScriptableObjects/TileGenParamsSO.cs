using UnityEngine;

[CreateAssetMenu(fileName = "TileGenParamsSO", menuName = "Crafting/TileGenParamsSO", order = 1)]
public class TileGenParamsSO : ScriptableObject
{
    [SerializeField]
    private UITile _tilePrefab;
    public UITile TilePrefab { get { return _tilePrefab; } }

    [SerializeField]
    private Vector2Int _windowBorderWidth = new Vector2Int(5, 10);
    public Vector2 WindowBorderWidth { get { return _windowBorderWidth; } }

    [SerializeField]
    private Vector2Int _gridResolution;
    public Vector2Int GridResolution { get { return _gridResolution; } }

    [SerializeField]
    private int _tileSize;
    public int TileSize { get { return _tileSize; } }

    [SerializeField]
    private int _gapSize;
    public int GapSize { get { return _gapSize; } }
}