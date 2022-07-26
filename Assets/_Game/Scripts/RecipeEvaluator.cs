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
    private int2 _craftGridResolution;

    private int _maxRecipePosX;
    private int _maxRecipePosY;


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
        Vector2Int gridResolution = CraftingDelegatesContainer.GetCraftTilesGridResolution();
        _craftGridResolution = new int2(gridResolution.x, gridResolution.y);
    }

    private RecipeQualityType Evaluate()
    {
        var result = EvaluateRecipeItemLocations(_targetItem.PerfectItemsData, RecipeQualityType.Perfect);
        if (result != RecipeQualityType.NoRecipe)
        {
            return result;
        }

        result = EvaluateRecipeItemLocations(_targetItem.FirstGoodItemsData, RecipeQualityType.Good);
        if (result != RecipeQualityType.NoRecipe)
        {
            return result;
        }

        result = EvaluateRecipeItemLocations(_targetItem.SecondGoodItemsData, RecipeQualityType.Good);
        if (result != RecipeQualityType.NoRecipe)
        {
            return result;
        }

        result = EvaluateRecipeItemLocations(_targetItem.FirstBadItemsData, RecipeQualityType.Bad);
        if (result != RecipeQualityType.NoRecipe)
        {
            return result;
        }

        result = EvaluateRecipeItemLocations(_targetItem.SecondBadItemsData, RecipeQualityType.Bad);
        if (result != RecipeQualityType.NoRecipe)
        {
            return result;
        }

        return RecipeQualityType.NoRecipe;
    }

    private RecipeQualityType EvaluateRecipeItemLocations(
        RecipeItemLocation[] recipeItemLocations,
        RecipeQualityType correctRecipeQuality
    )
    {
        _recipeDictionary.Clear();
        if (recipeItemLocations.Length > 0)
        { 
            foreach (RecipeItemLocation ril in recipeItemLocations)
            {
                if (ril.Pos.x > _maxRecipePosX)
                {
                    _maxRecipePosX = ril.Pos.x;
                }
                if (ril.Pos.y > _maxRecipePosY)
                {
                    _maxRecipePosY = ril.Pos.y;
                }
                
                _recipeDictionary.Add(ril.Pos, ril.ID);
            }

            for (int y = 0; y < _craftGridResolution.y - _maxRecipePosY; y++)
            {
                for (int x = 0; x < _craftGridResolution.x - _maxRecipePosX; x++)
                {
                    if (IsRecipeFollowedExclusively(new int2(x, y)))
                    {
                        return correctRecipeQuality;
                    }
                }
            }
        }
        return RecipeQualityType.NoRecipe;
    }

    private bool IsRecipeFollowedExclusively(int2 startTilePos)
    {
        for (int y = 0; y < _maxRecipePosY; y++)
        {
            for (int x = 0; x < _maxRecipePosX; x++)
            { 
                int2 craftIndex2D = new int2(x + startTilePos.x, y + startTilePos.y);
                int  craftIndex = IndexUtilities.XyToIndex(craftIndex2D, _craftGridResolution.x);

                int2 recipeIndex = new int2(x, y);
                if (_recipeDictionary.ContainsKey(recipeIndex))
                {
                    if (_craftTiles[craftIndex].PlacedStack == null)
                    {
                        return false;
                    }
                    if (_craftTiles[craftIndex].PlacedStack.ItemType.ID != _recipeDictionary[recipeIndex])
                    {
                        return false;
                    }
                }
                else if (_craftTiles[craftIndex].PlacedStack != null)
                {
                    return false;
                }
            }
        }

        return true;
    }
}