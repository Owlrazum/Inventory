using System.Collections.Generic;

using Unity.Mathematics;
using Unity.Burst;

using UnityEngine;

public enum RecipeQualityType
{ 
    NoRecipe,
    Perfect,
    Good,
    Bad
}

[BurstCompile]
public class RecipeEvaluator : MonoBehaviour
{
    private Dictionary<int2, int> _recipeDictionary;

    private ItemSO _targetItem;
    private UITile[] _craftTiles;
    private int _columnCount;


    private void Awake()
    {
        _recipeDictionary = new Dictionary<int2, int>(10);

        CraftingDelegatesContainer.EvaluateRecipeQuality += Evaluate;
        GameDelegatesContainer.EventLevelStarted += OnLevelStarted;
    }

    private void OnDestroy()
    { 
        CraftingDelegatesContainer.EvaluateRecipeQuality -= Evaluate;
        GameDelegatesContainer.EventLevelStarted -= OnLevelStarted;
    }

    private void OnLevelStarted()
    {
        _targetItem = CraftingDelegatesContainer.GetTargetItem();
        _craftTiles = CraftingDelegatesContainer.GetCraftTiles();
        _columnCount = CraftingDelegatesContainer.GetCraftTilesColumnCount();
    }

    private RecipeQualityType Evaluate()
    {
        var result = EvaluateRecipeItemLocations(_targetItem.PerfectItemsData);
        if (result != RecipeQualityType.NoRecipe)
        {
            return result;
        }

        result = EvaluateRecipeItemLocations(_targetItem.FirstGoodItemsData);
        if (result != RecipeQualityType.NoRecipe)
        {
            return result;
        }

        result = EvaluateRecipeItemLocations(_targetItem.SecondGoodItemsData);
        if (result != RecipeQualityType.NoRecipe)
        {
            return result;
        }

        result = EvaluateRecipeItemLocations(_targetItem.FirstBadItemsData);
        if (result != RecipeQualityType.NoRecipe)
        {
            return result;
        }

        result = EvaluateRecipeItemLocations(_targetItem.SecondBadItemsData);
        if (result != RecipeQualityType.NoRecipe)
        {
            return result;
        }

        return RecipeQualityType.NoRecipe;
    }

    private RecipeQualityType EvaluateRecipeItemLocations(RecipeItemLocation[] recipeItemLocations)
    {
        _recipeDictionary.Clear();
        if (recipeItemLocations.Length > 0)
        { 
            foreach (RecipeItemLocation ril in recipeItemLocations)
            {
                _recipeDictionary.Add(ril.Pos, ril.ID);
            }
            if (IsRecipeFollowedExclusively())
            { 
                return RecipeQualityType.Perfect;    
            }
        }
        return RecipeQualityType.NoRecipe;
    }

    private bool IsRecipeFollowedExclusively()
    {
        for (int i = 0; i < _craftTiles.Length; i++)
        {
            int2 index = IndexUtilities.IndexToXy(i, _columnCount);
            if (_recipeDictionary.ContainsKey(index))
            {
                if (_craftTiles[i].PlacedStack.ItemType.ID != _recipeDictionary[index])
                {
                    return false;
                }
            }
            else if (_craftTiles[i].PlacedStack != null)
            {
                return false;
            }
        }

        return true;
    }
}