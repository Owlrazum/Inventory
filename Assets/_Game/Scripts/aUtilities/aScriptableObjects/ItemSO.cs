using System;

using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class RecipeItemLocation
{ 
    public int ID;
    public int2 Pos;
}

[CreateAssetMenu(fileName = "Item", menuName = "Crafting/Item", order = 1)]
public class ItemSO : ScriptableObject
{
    public int ID;

    public Sprite Sprite;
    public Vector2Int Size = Vector2Int.one;

    // Craft requirement assumes that zero pos is top left corner

    // public string RecipeRequirements
    [Help("Imagine a box with certain size, the top left corner has position (0, 0), " +
          "\nbottomLeft has (0, boxSize), \ntopRight (boxSize,  0), \nbottomRight(boxSize, boxSize)")]
    [SerializeField]
    private bool _justToShowHelpBox;

    public RecipeItemLocation[] PerfectItemsData;
    public RecipeItemLocation[] FirstGoodItemsData;
    public RecipeItemLocation[] SecondGoodItemsData;
    public RecipeItemLocation[] FirstBadItemsData;
    public RecipeItemLocation[] SecondBadItemsData;

    public override bool Equals(object other)
    {
        ItemSO otherIngredientSO = other as ItemSO;
        if (otherIngredientSO == null)
        {
            return false;
        }

        return ID.Equals(otherIngredientSO.ID);
    }

    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }
}