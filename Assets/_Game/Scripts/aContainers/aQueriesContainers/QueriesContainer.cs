using System;

public static class QueriesContainer
{
    public static Func<int> FuncScenesCount;
    public static int QueryScenesCount()
    { 
#if UNITY_EDITOR
        if (FuncScenesCount.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncScenesCount.Invoke();
    }

    public static Func<BeizerSegment> FuncWallRunBeizerSegment;
    public static BeizerSegment QueryWallRunBeizerSegment()
    { 
#if UNITY_EDITOR
        if (FuncWallRunBeizerSegment.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncWallRunBeizerSegment.Invoke();  
    }

}