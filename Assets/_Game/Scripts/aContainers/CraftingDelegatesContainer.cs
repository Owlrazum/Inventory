using System;
using UnityEngine;

public static class CraftingDelegatesContainer
{
    public static Func<Inventory> FuncInventoryInstance;
    public static Inventory QueryInventoryInstance()
    {
#if UNITY_EDITOR
        if (FuncInventoryInstance.GetInvocationList().Length != 1)
        {
            Debug.LogError("There should be only one subscription");
        }
#endif

        return FuncInventoryInstance?.Invoke();
    }
}