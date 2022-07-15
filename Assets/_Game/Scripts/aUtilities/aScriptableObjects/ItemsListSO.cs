using UnityEngine;

[CreateAssetMenu(fileName = "GlobalItemList", menuName = "Crafting/GlobalItemList", order = 1)]
public class ItemsListSO : ScriptableObject
{
    [SerializeField]
    private ItemSO[] Items;

    public void AssignIDs()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].ID = i;
            Debug.Log("Assigned " + i + " to " + Items[i].name);
        }
    }

    public ItemSO GetItem(int id)
    {
        return Items[id];
    }
}