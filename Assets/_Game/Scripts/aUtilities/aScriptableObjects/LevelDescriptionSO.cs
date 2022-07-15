using System;
using UnityEngine;
using Unity.Collections;

[CreateAssetMenu(fileName = "LevelDescription", menuName = "Crafting/LevelDescription", order = 1)]
public class LevelDescriptionSO : ScriptableObject
{
    public ItemSO TargetItem;

    public ItemSO[] ExistingItems;
    public int[] ExistingItemsAmount;
}