using System;
using UnityEngine;

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

    public static Func<Vector3, Vector3> FuncTransformDirectionFromCameraSpace;
    public static Vector3 QueryTransformDirectionFromCameraSpace(in Vector3 input)
    { 
#if UNITY_EDITOR
        if (FuncTransformDirectionFromCameraSpace.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncTransformDirectionFromCameraSpace.Invoke(input);
    }

    public static Func<Vector3, Vector3> FuncTransformScreenPosToWorldPos;
    public static Vector3 QueryTransformScreenPosToWorldPos(in Vector3 input)
    { 
#if UNITY_EDITOR
        if (FuncTransformScreenPosToWorldPos.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncTransformScreenPosToWorldPos.Invoke(input);
    }
}