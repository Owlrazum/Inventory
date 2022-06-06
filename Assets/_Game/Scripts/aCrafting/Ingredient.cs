using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient", menuName = "ScriptableObjects/Crafting/Ingredient", order = 1)]
public class Ingredient : ScriptableObject
{
    public int ID;

    [Serializable]
    public struct CraftRequirement
    {
        public int ID;
        public int Count;
    }

    public CraftRequirement[] CraftRequirements;
}