using UnityEngine;

[CreateAssetMenu(fileName = "IngredientsList", menuName = "ScriptableObjects/Crafting/IngredientsList", order = 1)]
public class ItemsListSO : ScriptableObject
{
    public ItemSO[] Items;

    public void AssignIDs()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].ID = i;
            Debug.Log("Assigned " + i + " to " + Items[i].name);
        }
    }
}