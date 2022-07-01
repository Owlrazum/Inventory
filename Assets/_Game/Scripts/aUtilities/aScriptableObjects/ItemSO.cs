using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient", menuName = "ScriptableObjects/Crafting/Ingredient", order = 1)]
public class ItemSO : ScriptableObject
{
    public int ID;

    public Sprite Sprite;
    public Vector2Int Size = Vector2Int.one;
    public int StackCapacity = 1;

    [Serializable]
    public struct CraftRequirement
    {
        public int ID;
        public int Count;
    }

    public CraftRequirement[] CraftRequirements;

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