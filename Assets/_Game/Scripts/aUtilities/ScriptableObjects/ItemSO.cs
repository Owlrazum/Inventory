using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient", menuName = "ScriptableObjects/Crafting/Ingredient", order = 1)]
public class ItemSO : ScriptableObject
{
    public int ID;

    public GameObject Sprite;
    public int SizeX;
    public int SizeY;

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