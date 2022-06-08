using UnityEngine;

public class InventoryHolder : MonoBehaviour
{
    [SerializeField]
    private Inventory _inventory;

    private void Awake()
    { 
        CraftingDelegatesContainer.FuncInventoryInstance += GetInventoryInstance;
    }

    private void OnDestroy()
    { 
        CraftingDelegatesContainer.FuncInventoryInstance -= GetInventoryInstance;
    }

    private Inventory GetInventoryInstance()
    {
        return _inventory;
    }

    public void AddItem(ItemSO item)
    {
        _inventory.AddItem(item);
    }

    public void RemoveItem(ItemSO item)
    {
        _inventory.RemoveItem(item);
    }
}