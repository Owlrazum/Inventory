using System;
using UnityEngine;

public static class PlayerQueriesContainer
{
    #region States
    public static Func<Transform> FuncTransform;
    public static Transform QueryTransform()
    { 
#if UNITY_EDITOR
        if (FuncTransform.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncTransform.Invoke();       
    }

    public static Func<bool> FuncIsGrounded;
    public static bool QueryIsGrounded()
    { 
#if UNITY_EDITOR
        if (FuncIsGrounded.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncIsGrounded.Invoke();
    }

    public static Func<float> FuncSlopeAngleBelowRaycast;
    public static float QuerySlopeAngleBelowRaycast()
    { 
#if UNITY_EDITOR
        if (FuncSlopeAngleBelowRaycast.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncSlopeAngleBelowRaycast.Invoke();
    }

//     public static Func<PlayerMovementStateType> FuncPreviousStateType;
//     public static PlayerMovementStateType QueryPreviousStateType()
//     {
// #if UNITY_EDITOR
//         if (FuncPreviousStateType.GetInvocationList().Length != 1)
//         {
//             throw new NotSupportedException("There should be only one subscription");
//         }
// #endif

//         return FuncPreviousStateType.Invoke();
//     }

    #endregion

    #region ClassInstances
    public static Func<PlayerCharacter> FuncPlayerCharacterInstance;
    public static PlayerCharacter QueryPlayerCharacterInstance()
    {
#if UNITY_EDITOR
        if (FuncPlayerCharacterInstance.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncPlayerCharacterInstance.Invoke();
    }

    public static Func<PlayerController> FuncPlayerControllerInstance;
    public static PlayerController QueryPlayerControllerInstance()
    {
#if UNITY_EDITOR
        if (FuncPlayerControllerInstance.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncPlayerControllerInstance.Invoke();
    }

//     public static Func<PlayerMovementState> FuncInitialMovementState;
//     public static PlayerMovementState QueryInitialMovementState()
//     {
// #if UNITY_EDITOR
//         if (FuncInitialMovementState == null)
//         { 
//             throw new NotSupportedException("null query");
//         }
//         if (FuncInitialMovementState.GetInvocationList().Length != 1)
//         {
//             throw new NotSupportedException("There should be only one subscription");
//         }
// #endif

//         return FuncInitialMovementState.Invoke();
//     }
    #endregion//ClassInstances

    #region Parameters
    public static Func<float> FuncFallingGravityAmount;
    public static float QueryFallingGravityAmount()
    {
#if UNITY_EDITOR
        if (FuncFallingGravityAmount.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncFallingGravityAmount.Invoke();
    }

    public static Func<float> FuncSlopeAngleMinThreshold;
    public static float QuerySlopeAngleMinThreshold()
    {
#if UNITY_EDITOR
        if (FuncSlopeAngleMinThreshold.GetInvocationList().Length != 1)
        { 
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncSlopeAngleMinThreshold.Invoke();
    }

    public static Func<float> FuncSlopeAngleMaxThreshold;
    public static float QuerySlopeAngleMaxThreshold()
    { 
#if UNITY_EDITOR
        if (FuncSlopeAngleMaxThreshold.GetInvocationList().Length != 1)
        { 
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncSlopeAngleMaxThreshold.Invoke();
    }
    #endregion//Parameters

    #region MovementStatesStates
    public static Func<float> FuncFallSpeed;
    public static float QueryFallSpeed()
    { 
#if UNITY_EDITOR
        if (FuncFallSpeed.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncFallSpeed.Invoke();
    }
    #endregion//MovementStatesStates

    #region MovementStatesTriggering
    public static Func<bool> FuncIsDashTriggering;
    public static bool QueryIsDashTriggering()
    { 
#if UNITY_EDITOR
        if (FuncIsDashTriggering.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncIsDashTriggering.Invoke();
    }
    
    public static Func<bool> FuncIsGlidingTriggering;
    public static bool QueryIsGlidingTriggering()
    { 
#if UNITY_EDITOR
        if (FuncIsGlidingTriggering.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncIsGlidingTriggering.Invoke();
    }

//     public static Func<bool> FuncIsWallRunTriggering;
//     public static bool QueryIsWallRunTriggering()
//     { 
// #if UNITY_EDITOR
//         if (FuncIsWallRunTriggering.GetInvocationList().Length != 1)
//         {
//             throw new NotSupportedException("There should be only one subscription");
//         }
// #endif

//         return FuncIsWallRunTriggering.Invoke();
//     }

    public static Func<bool> FuncIsSlidingTriggering;
    public static bool QueryIsSlidingTriggering()
    { 
#if UNITY_EDITOR
        if (FuncIsSlidingTriggering.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncIsSlidingTriggering.Invoke();
    }

    public static Func<bool> FuncIsGroundJumpTrigering;
    public static bool QueryIsGroundJumpTrigering()
    { 
#if UNITY_EDITOR
        if (FuncIsGroundJumpTrigering.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncIsGroundJumpTrigering.Invoke();
    }

    public static Func<bool> FuncIsCoyoteGroundJumpTriggering;
    public static bool QueryIsCoyoteGroundJumpTriggering()
    { 
#if UNITY_EDITOR
        if (FuncIsCoyoteGroundJumpTriggering.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncIsCoyoteGroundJumpTriggering.Invoke();
    }

    public static Func<bool> FuncIsBufferGroundJumpTriggering;
    public static bool QueryIsBufferGroundJumpTriggering()
    { 
#if UNITY_EDITOR
        if (FuncIsBufferGroundJumpTriggering.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncIsBufferGroundJumpTriggering.Invoke();        
    }

    public static Func<bool> FuncIsDoubleJumpTrigering;
    public static bool QueryIsDoubleJumpTrigering()
    { 
#if UNITY_EDITOR
        if (FuncIsDoubleJumpTrigering.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncIsDoubleJumpTrigering.Invoke();
    }

    public static Func<bool> FuncIsJumpableJumpTriggering;
    public static bool QueryIsJumpableJumpTriggering()
    { 
#if UNITY_EDITOR
        if (FuncIsJumpableJumpTriggering.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncIsJumpableJumpTriggering.Invoke(); 
    }

    public static Func<bool> FuncIsBufferJumpableJumpTriggering;
    public static bool QueryIsBufferJumpableJumpTriggering()
    { 
#if UNITY_EDITOR
        if (FuncIsBufferJumpableJumpTriggering.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncIsBufferJumpableJumpTriggering.Invoke();        
    }

    public static Func<bool> FuncIsGravityPullTriggering;
    public static bool QueryIsGravityPullTriggering()
    { 
#if UNITY_EDITOR
        if (FuncIsGravityPullTriggering.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncIsGravityPullTriggering.Invoke();
    }

    public static Func<bool> FuncIsSpikeDamagedMovementTriggering;
    public static bool QueryIsSpikeDamagedMovementTriggering()
    { 
#if UNITY_EDITOR
        if (FuncIsSpikeDamagedMovementTriggering.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncIsSpikeDamagedMovementTriggering.Invoke();
    }
    #endregion//MovementStatesTriggering
}