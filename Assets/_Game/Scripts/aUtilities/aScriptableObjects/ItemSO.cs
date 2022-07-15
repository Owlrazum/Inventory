using System;

using UnityEngine;

public enum RecipeQualityType
{ 
    Perfect,
    Good,
    Bad
}

[CreateAssetMenu(fileName = "Item", menuName = "Crafting/Item", order = 1)]
public class ItemSO : ScriptableObject
{
    public int ID;

    public Sprite Sprite;
    public Vector2Int Size = Vector2Int.one;

    // Craft requirement assumes that zero pos is top left corner

    public RecipeItemLocation[] PerfectItemsData;
    public RecipeItemLocation[] FirstGoodItemsData;
    public RecipeItemLocation[] SecondGoodItemsData;
    public RecipeItemLocation[] FirstBadItemsData;
    public RecipeItemLocation[] SecondBadItemsData;

    [Serializable]
    public class RecipeItemLocation
    { 
        public int ID;
        public Vector2Int Pos;
    }

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