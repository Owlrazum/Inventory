using System;
using UnityEngine;

public static class PoolingDelegatesContainer
{
    // public static Action<Vector3, Vector3, float> EventSpawnProjectile;
    // public static Action<CannonProjectile> EventDespawnProjectile;

    #region PuzzleItem
    public static Func<int, Vector3, PuzzleItem> FuncSpawnPuzzleItemIndexed;
    public static PuzzleItem SpawnPuzzleItemIndexedAndQueryIt(int index, Vector3 pos)
    {
#if UNITY_EDITOR
        if (FuncSpawnPuzzleItemIndexed.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncSpawnPuzzleItemIndexed.Invoke(index, pos);  
    }
    public static Action<int, PuzzleItem> EventDespawnPuzzleItemIndexed;
    #endregion//PuzzleItem
}