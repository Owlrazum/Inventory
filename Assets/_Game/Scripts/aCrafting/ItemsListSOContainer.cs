using UnityEngine;

public class ItemsListSOContainer : MonoBehaviour
{
    [SerializeField]
    private ItemsListSO _itemListSO;

    private void Awake()
    {
        CraftingDelegatesContainer.FuncGetItemSO += GetItemSO;
    }

    private void OnDestroy()
    { 
        CraftingDelegatesContainer.FuncGetItemSO -= GetItemSO;
    }

    private ItemSO GetItemSO(int id)
    {
        return _itemListSO.GetItem(id);
    }
}