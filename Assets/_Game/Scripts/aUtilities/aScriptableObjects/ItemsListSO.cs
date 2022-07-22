using UnityEngine;

[CreateAssetMenu(fileName = "GlobalItemList", menuName = "Crafting/GlobalItemList", order = 1)]
public class ItemsListSO : ScriptableObject
{
    public ItemSO[] Data;

    public void AssignIDs()
    {
        for (int i = 0; i < Data.Length; i++)
        {
            Data[i].ID = i;
            Debug.Log("Assigned " + i + " to " + Data[i].name);
        }
    }

    public ItemSO GetItem(int id)
    {
        return Data[id];
    }
}