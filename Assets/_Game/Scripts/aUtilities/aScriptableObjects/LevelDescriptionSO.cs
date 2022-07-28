using UnityEngine;

[CreateAssetMenu(fileName = "LevelDescription", menuName = "Crafting/LevelDescription", order = 1)]
public class LevelDescriptionSO : ScriptableObject
{
    public ItemSO TargetItem;

    public ItemSO[] ExistingItems;
    public int[] ExistingItemsAmount;

    [SerializeField]
    private TileGenParamsSO TileGenParamsItems;
    [SerializeField]
    private TileGenParamsSO TileGenParamsCraft;

    public TileGenParamsSO GetTileGenParams(WindowType windowType)
    {
        switch (windowType)
        { 
            case WindowType.CraftWindow:
                return TileGenParamsCraft;
            case WindowType.ItemsWindow:
                return TileGenParamsItems;
        }

        Debug.LogError("Unknown window type!!!");
        return null;
    }
}