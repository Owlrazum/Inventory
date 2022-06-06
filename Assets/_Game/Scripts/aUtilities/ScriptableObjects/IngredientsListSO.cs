using UnityEngine;

[CreateAssetMenu(fileName = "IngredientsList", menuName = "ScriptableObjects/Crafting/IngredientsList", order = 1)]
public class IngredientsListSO : ScriptableObject
{
    public Ingredient[] Ingredients;

    public void AssignIDs()
    {
        for (int i = 0; i < Ingredients.Length; i++)
        {
            Ingredients[i].ID = i;
            Debug.Log("Assigned " + i + " to " + Ingredients[i].name);
        }
    }
}