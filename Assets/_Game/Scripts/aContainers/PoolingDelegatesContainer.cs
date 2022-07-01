using System;
using UnityEngine;

public static class PoolingDelegatesContainer
{
    // public static Action<Vector3, Vector3, float> EventSpawnProjectile;
    // public static Action<CannonProjectile> EventDespawnProjectile;

    #region UIStack
    public static Func<UIStack> FuncSpawnUIStack;
    public static UIStack SpawnUIStackAndQueryIt()
    {
#if UNITY_EDITOR
        if (FuncSpawnUIStack.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncSpawnUIStack.Invoke();  
    }
    public static Action<UIStack> EventDespawnUIStack;
    #endregion//PuzzleItem
}