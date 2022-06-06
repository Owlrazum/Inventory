using UnityEngine;

public class Inventory : MonoBehaviour
{
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
        return this;
    }
}