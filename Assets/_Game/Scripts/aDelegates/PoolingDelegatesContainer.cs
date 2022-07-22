using System;

public static class PoolingDelegatesContainer
{
    public static Func<UIStack> SpawnStack;
    public static Action<UIStack> DespawnStack;
}